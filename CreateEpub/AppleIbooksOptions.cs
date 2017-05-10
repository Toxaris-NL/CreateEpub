using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class AppleIbooksOptions {
        
        internal XElement ToElement() {
            XElement element = new XElement("display_options", new XElement("platform", new XAttribute("name", "*"), new XElement("option", new XAttribute("name", "specified-fonts"), true)));

            return element;
        }
    }
}
