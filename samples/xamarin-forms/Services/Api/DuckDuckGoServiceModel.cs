// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reactive;

namespace Services.Api
{
    /// <summary>
    /// Icon for related topic(s) or external site(s)
    /// </summary>
    public class DuckDuckGoIcon
    {
        /// <summary>
        /// URL of icon
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Height of icon (px)
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// Width of icon (px)
        /// </summary>
        public string Width { get; set; }
    }

    /// <summary>
    /// Individual result returned from the query
    /// </summary>
    public class DuckDuckGoQueryResult
    {
        /// <summary>
        /// HTML link(s) to related topic(s) or external site(s)
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Icon associated with related topic(s) or FirstUrl
        /// </summary>
        public DuckDuckGoIcon Icon { get; set; }

        /// <summary>
        /// First URL in Result
        /// </summary>
        public string FirstUrl { get; set; }

        /// <summary>
        /// Text from first URL
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Overal results from query 
    /// </summary>
    public class DuckDuckGoSearchResult 
    {
        /// <summary>
        /// Topic summary containing HTML
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Topic summary containing no HTML
        /// </summary>
        public string AbstractText { get; set; }

        /// <summary>
        /// Type of Answer, e.g. calc, color, digest, info, ip, iploc, phone, pw, rand, regexp, unicode, upc, or zip (see goodies & tech pages for examples).
        /// </summary>
        public string AnswerType { get; set; }

        /// <summary>
        /// Name of Abstract Source
        /// </summary>
        public string AbstractSource { get; set; }

        /// <summary>
        /// Dictionary definition (may differ from Abstract)
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Name of Definition source
        /// </summary>
        public string DefinitionSource { get; set; }

        /// <summary>
        /// Name of topic that goes with Abstract
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Link to image that goes with Abstract
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Array of internal links to related topics associated with Abstract
        /// </summary>
        public List<DuckDuckGoQueryResult> RelatedTopics { get; set; }

        /// <summary>
        /// Response category, i.e. A (article), D (disambiguation), C (category), N (name), E (exclusive), or nothing.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// !bang redirect URL
        /// </summary>
        public string Redirect { get; set; }

        /// <summary>
        /// Deep link to expanded definition page in DefinitionSource
        /// </summary>
        public string DefinitionUrl { get; set; }

        /// <summary>
        /// Instant answer
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Array of external links associated with Abstract
        /// </summary>
        public List<DuckDuckGoQueryResult> Results { get; set; }

        /// <summary>
        /// Deep link to the expanded topic page in AbstractSource
        /// </summary>
        public string AbstractUrl { get; set; }
    }
}
