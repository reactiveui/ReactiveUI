using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxUIViewModelGenerator
{
    enum TemplateType {
        GeneratedViewModel,
        ViewModel,
        XamlControl,
        XamlCodeBehind,
    };

    class Program
    {
        static int Main(string[] args)
        {
            var opts = default(OptionSet);
            var type = default(TemplateType);
            var input = Console.In;
            var template = default(string);

            opts = new OptionSet() {
                { "t=|type=", "Type of template, one of ViewModel, GeneratedViewModel, XamlControl, XamlCodeBehind", 
                    (string x) => Enum.TryParse<TemplateType>(x, out type) },
                { "i=|input=", "The input interface file, defaults to stdin", 
                    (string x) => input = new StreamReader(File.OpenRead(x), Encoding.UTF8) },
                { "template-override=", "The input interface file, defaults to stdin", 
                    (string x) => template = File.ReadAllText(x, Encoding.UTF8) },
                { "h|help", "Displays Help", 
                    _ => opts.WriteOptionDescriptions(Console.Out) },
            };

            var rest = default(List<string>);
            try {
                rest = opts.Parse(args);
            } catch (OptionException ex) {
                Console.Error.WriteLine(ex.Message);
                opts.WriteOptionDescriptions(Console.Error);
            }

            var dict = default(Dictionary<string, object>);
            try {
                dict = rest.Select(x => x.Split('=')).ToDictionary(k => k[0], v => (object)v[1]);
            } catch {
                Console.Error.WriteLine("Extra parameters are specified as theKey=theValue");
                return -1;
            }

            var content = input.ReadToEnd();

            var renderer = new ScaffoldRenderer();
            switch (type) {
            case TemplateType.GeneratedViewModel:
                Console.WriteLine(renderer.RenderGeneratedViewModel(content, dict, template));
                break;
            case TemplateType.ViewModel:
                Console.WriteLine(renderer.RenderUserViewModel(content, dict, template));
                break;
            case TemplateType.XamlCodeBehind:
                Console.WriteLine(renderer.RenderUserViewModel(content, dict, template));
                break;
            }

            return 0;
        }
    }
}
