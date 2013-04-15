using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;

namespace RxUIViewModelGenerator
{
    public class ScaffoldRenderer : IEnableLogger
    {
        public string RenderGeneratedViewModel(string interfaceCode, Dictionary<string, object> options = null)
        {
            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.ViewModelGeneratedTemplate.mustache");
            return Render(interfaceCode, new StreamReader(res).ReadToEnd(), options);
        }

        public string RenderUserViewModel(string interfaceCode, Dictionary<string, object> options = null)
        {
            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.ViewModelTemplate.mustache");
            return Render(interfaceCode, new StreamReader(res).ReadToEnd(), options);
        }

        public IEnumerable<string> RenderUserControlXaml(string interfaceCode, Dictionary<string, object> options = null)
        {
            var ri = createRenderInfo(interfaceCode, options);

            var renderInfos = (IEnumerable<InterfaceRenderInformation>)ri["interfaces"];

            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.XamlViewTemplate.mustache");
            var templ = new StreamReader(res).ReadToEnd();

            return renderInfos.Select(renderInfo => {
                var dict = ri
                    .Where(x => x.Key != "interfaces")
                    .ToDictionary(k => k.Key, v => v.Value);

                if (!dict.ContainsKey("control")) dict["control"] = "UserControl";
                dict["implClassName"] = renderInfo.implClassName.Replace("ViewModel", "View");
                return Nustache.Core.Render.StringToString(templ, dict);
            }).ToArray();
        }

        public IEnumerable<string> RenderUserControlCodeBehind(string interfaceCode, Dictionary<string, object> options = null)
        {
            var ri = createRenderInfo(interfaceCode, options);

            var renderInfos = (IEnumerable<InterfaceRenderInformation>)ri["interfaces"];

            var res = this.GetType().Assembly.GetManifestResourceStream("RxUIViewModelGenerator.Resources.XamlViewCodebehindTemplate.mustache");
            var templ = new StreamReader(res).ReadToEnd();

            return renderInfos.Select(renderInfo => {
                var dict = ri
                    .Where(x => x.Key != "interfaces")
                    .ToDictionary(k => k.Key, v => v.Value);

                if (!dict.ContainsKey("control")) dict["control"] = "UserControl";
                dict["implClassName"] = renderInfo.implClassName.Replace("ViewModel", "View");
                return renderTemplate(templ, dict);
            }).ToArray();
        }

        public string Render(string interfaceCode, string template, Dictionary<string, object> options = null)
        {
            var dict = createRenderInfo(interfaceCode, options);
            return renderTemplate(template, dict);
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

            var dict = new Dictionary<string, object> {
                { "interfaces", renderInfo },
            };

            if (options != null) {
                foreach (var kvp in options) dict.Add(kvp.Key, kvp.Value);
            }

            if (dict.ContainsKey("namespace")) dict["namespace"] = "TODO";

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
