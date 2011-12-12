using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReactiveUI.CompileTime
{
    public class XibReferencedClass 
    {
        public string Name { get; set; }
        public string[] Actions { get; set; }
        
        public static string[] FindAllXibFiles(string root)
        {
            var output = new List<string>();
            var di = new DirectoryInfo(root);
            foreach(var f in di.GetFiles("*.xib")) {
                output.Add(f.FullName);
            }
            
            foreach(var d in di.GetDirectories()) {
                output.AddRange(FindAllXibFiles(d.FullName));
            }
            return output.ToArray();
        }
        
        public static XibReferencedClass[] ClassesInXib(string xibPath)
        {
            var doc = XDocument.Load(new StreamReader(xibPath, Encoding.UTF8));
            var classDesc = doc.Root.Descendants().Where(x => x.Name == "object" && x.Attribute("class").Value == "IBPartialClassDescription");
            
            if (classDesc == null) {
                Console.WriteLine("Xib '{0}' has no class descriptions, perhaps it is post-compiled?", xibPath);
                return new XibReferencedClass[0];
            }
            
            Console.WriteLine("Class Count: {0}", classDesc.Count());
            
            var ret = classDesc.Select(root => {
                var obj = new XibReferencedClass();
                obj.Name = root.Elements().Where(x => x.Name == "string" && x.Attribute("key").Value == "className").Single().Value;
                obj.Actions = root
                   .XPathSelectElements("//object[@class='IBActionInfo']/string[@key='name']")
                   .Select(x => x.Value).ToArray();
                return obj;
            }).ToArray();
            
            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
