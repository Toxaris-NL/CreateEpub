using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
   internal static class Globals {
        internal static readonly XNamespace OpfNs = "http://www.idpf.org/2007/opf";
        internal static readonly XNamespace DcNs = "http://purl.org/dc/elements/1.1/";
        internal static readonly XNamespace DcTerms = "http://purl.org/dc/terms/";
   
        internal static int Version { get; set; } = 2;
        internal static int AuthorCount { get; set; }
        internal static int TranslatorCount { get; set; }
    
    }
}
