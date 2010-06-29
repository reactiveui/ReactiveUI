using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.Text.RegularExpressions;

namespace ReactiveXamlSample.Tests
{
    // HACK: gblock would probably mock me openly for this
    public class HijackedPartCatalog : ComposablePartCatalog
    {
        readonly ComposablePartCatalog innerCatalog;
        public HijackedPartCatalog(ComposablePartCatalog InnerCatalog, params string[] NamesToHide)
        {
            innerCatalog = InnerCatalog;
            HiddenPartNames = NamesToHide.Select(x => new Regex(x)).ToList();
        }

        public readonly List<Regex> HiddenPartNames = new List<Regex>();

        public override IQueryable<ComposablePartDefinition> Parts {
            get {
                // HACK: Yes, this is ugly, but it means we don't have to cast 
                // Parts to an IEnumerable then back to an IQueryable
                var ret = innerCatalog.Parts.Where(x => !(x is ICompositionElement) ||
                    (HiddenPartNames.All(regex => !regex.IsMatch(((ICompositionElement)x).DisplayName))));
                return ret;
            }
        }
    }
}
