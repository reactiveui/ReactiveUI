using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using ICSharpCode.NRefactory.CSharp;

namespace RxUIViewModelGenerator
{
    public class ScaffoldRenderer
    {
        public Tuple<string, string> RenderGeneratedViewModel(string interfaceCode, Dictionary<string, object> options = null, string templateOverride = null)
        {
            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.ViewModelGeneratedTemplate.mustache");
            var dict = createRenderInfo(interfaceCode, options);

            return Tuple.Create(
                ((string)dict["filename"]).Replace(".cs", ".generated.cs"), 
                renderTemplate(templateOverride ?? new StreamReader(res).ReadToEnd(), dict));
        }

        public Tuple<string, string> RenderUserViewModel(string interfaceCode, Dictionary<string, object> options = null, string templateOverride = null)
        {
            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.ViewModelTemplate.mustache");

            var dict = createRenderInfo(interfaceCode, options);
            return Tuple.Create(
                (string)dict["filename"], 
                renderTemplate(templateOverride ?? new StreamReader(res).ReadToEnd(), dict));
        }

        public IEnumerable<Tuple<string, string>> RenderUserControlXaml(string interfaceCode, Dictionary<string, object> options = null, string templateOverride = null)
        {
            var ri = createRenderInfo(interfaceCode, options);

            var renderInfos = (IEnumerable<InterfaceRenderInformation>)ri["interfaces"];

            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.XamlViewTemplate.mustache");
            var templ = templateOverride ?? new StreamReader(res).ReadToEnd();

            return renderInfos.Select(renderInfo => {
                var dict = ri
                    .Where(x => x.Key != "interfaces")
                    .ToDictionary(k => k.Key, v => v.Value);

                if (!dict.ContainsKey("control")) dict["control"] = "UserControl";
                dict["implClassName"] = renderInfo.implClassName.Replace("ViewModel", "View");

                return Tuple.Create(
                    renderInfo.implClassName.Replace("ViewModel", "View") + ".xaml",
                    Nustache.Core.Render.StringToString(templ, dict));
            }).ToArray();
        }

        public IEnumerable<Tuple<string, string>> RenderUserControlCodeBehind(string interfaceCode, Dictionary<string, object> options = null, string templateOverride = null)
        {
            var ri = createRenderInfo(interfaceCode, options);

            var renderInfos = (IEnumerable<InterfaceRenderInformation>)ri["interfaces"];

            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.XamlViewCodebehindTemplate.mustache");
            var templ = templateOverride ?? new StreamReader(res).ReadToEnd();

            return renderInfos.Select(renderInfo => {
                var dict = ri
                    .Where(x => x.Key != "interfaces")
                    .ToDictionary(k => k.Key, v => v.Value);

                if (!dict.ContainsKey("control")) dict["control"] = "UserControl";
                dict["implClassName"] = renderInfo.implClassName.Replace("ViewModel", "View");

                return Tuple.Create(
                    renderInfo.implClassName.Replace("ViewModel", "View") + ".xaml.cs",
                    renderTemplate(templ, dict));
            }).ToArray();
        }

        private string renderTemplate(string template, Dictionary<string, object> dict)
        {
            var ret = Nustache.Core.Render.StringToString(template, dict);

            var root = (new CSharpParser()).Parse(ret);
            var style = FormattingOptionsFactory.CreateKRStyle();

            return cleanUpSpacing(root.ToString());
        }

        Dictionary<string, object> createRenderInfo(string interfaceCode, Dictionary<string, object> options)
        {
            var parser = new CSharpParser();
            var root = parser.Parse(interfaceCode);

            if (!root.Children.Any()) {
                throw new ArgumentException("Compilation failed or code is badly formatted");
            }

            if (!root.Children.All(x => x.NodeType == NodeType.TypeDeclaration || x.NodeType == NodeType.Whitespace)) {
                throw new ArgumentException("Code must be one ore more interfaces");
            }

            var typeDecls = root.Children.OfType<TypeDeclaration>().ToArray();
            if (typeDecls.Any(x => x.FirstChild.ToString() != "interface") || !typeDecls.Any()) {
                throw new ArgumentException("Code must be one ore more interfaces");
            }

            var renderInfo = typeDecls.Select(renderInterface).ToArray();

            var filename = renderInfo.Length == 1 ? renderInfo[0].implClassName + ".cs" : "ViewModels.cs";
            var dict = new Dictionary<string, object> {
                { "interfaces", renderInfo },
                { "filename", filename },
            };

            if (options != null) {
                foreach (var kvp in options) dict.Add(kvp.Key, kvp.Value);
            }

            if (!dict.ContainsKey("namespace")) dict["namespace"] = "TODO";

            return dict;
        }

        InterfaceRenderInformation renderInterface(TypeDeclaration interfaceDecl)
        {
            var ret = new InterfaceRenderInformation();

            ret.isRoutableViewModel = interfaceDecl.BaseTypes
                .OfType<SimpleType>()
                .Any(x => x.Identifier == "IRoutableViewModel") ? ret : null;

            ret.definition = "public " + chompedString(interfaceDecl.ToString().Replace("[Once]", ""));
            ret.interfaceName = chompedString(interfaceDecl.Name);
            ret.implClassName = ret.interfaceName.Substring(1); // Skip the 'I'

            ret.properties = interfaceDecl.Children
                .Where(x => x is PropertyDeclaration || x is MethodDeclaration)
                .Select(renderPropertyDeclaration)
                .ToArray();

            ret.onceProperties = ret.properties
                .Where(x => x.onceProp != null)
                .Select(x => x.onceProp)
                .ToArray();

            return ret;
        }

        PropertyRenderInformation renderPropertyDeclaration(AstNode node)
        {
            var propDecl = node as PropertyDeclaration;

            if (propDecl == null) {
                return new PropertyRenderInformation() {
                    anythingElse = new NameAndTypeRenderInformation() { name = chompedString(node.ToString()) },
                };
            }

            var nameAndType = new NameAndTypeRenderInformation() {
                name = chompedString(propDecl.Name), 
                type = chompedString(propDecl.ReturnType.ToString()),
            };

            var commands = new[] {
                "ReactiveCommand",
                "ReactiveAsyncCommand",
            };

            if (propDecl.Attributes.Any(x => x.ToString().StartsWith("[Once]") || commands.Contains(propDecl.ReturnType.ToString()))) {
                return new PropertyRenderInformation() { onceProp = nameAndType, };
            }

            if (!propDecl.Setter.IsNull) {
                return new PropertyRenderInformation() { readWriteProp = nameAndType, };
            } else {
                return new PropertyRenderInformation() { outputProp = nameAndType, };
            }
        }

        string chompedString(string code)
        {
            if (!code.Contains("\n")) {
                return code.TrimEnd(' ', '\t');
            }

            var lines = code.Split('\n')
                .Select(x => x.TrimEnd(' ', '\t'))
                .Where(x => !(String.IsNullOrWhiteSpace(x) && x.Length > 2));

            return String.Join("\n", lines);
        }

        string cleanUpSpacing(string code)
        {
            var sb = new StringBuilder(code);

            var fixups = new[] {
                new { re = new Regex(@"^    }$"), val = "    }\n" },
                new { re = new Regex(@"namespace "), val = "\n\nnamespace " },
            };

            // NB: Yes, I know this is massively inefficient.
            var lines = code.Replace("\r\n", "\n").Replace("\t", "    ").Split('\n');
            return lines
                .Select(line => fixups.Aggregate(line, (acc, x) => x.re.Replace(acc, x.val)))
                .Aggregate(new StringBuilder(), (acc, x) => acc.AppendLine(x))
                .ToString();
        }
    }

    public class InterfaceRenderInformation
    {
        public string definition { get; set; }
        public string implClassName { get; set; }
        public string interfaceName { get; set; }
        public object isRoutableViewModel { get; set; } // 'this' if true, null if false, Mustacheism

        public IEnumerable<PropertyRenderInformation> properties { get; set; }
        public IEnumerable<NameAndTypeRenderInformation> onceProperties { get; set; }
    }

    public class PropertyRenderInformation
    {
        // NB: Only *one* of these should be non-null, Mustacheism
        public NameAndTypeRenderInformation outputProp { get; set; }
        public NameAndTypeRenderInformation onceProp { get; set; }
        public NameAndTypeRenderInformation readWriteProp { get; set; }
        public NameAndTypeRenderInformation anythingElse { get; set; }
    }

    public class NameAndTypeRenderInformation
    {
        public string name { get; set; }
        public string type { get; set; }
    }
}
