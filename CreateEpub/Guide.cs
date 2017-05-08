using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Guide {
        private readonly XElement _element;

        internal Guide() {
            this._element = new XElement(Document.OpfNs + "guide");
        }

        internal void AddReference(string href, string type) {
            this.AddReference(href, type, string.Empty);
        }

        internal void AddReference(string href, string type, string title) {
            XElement itemref = new XElement(Document.OpfNs + "reference", new object[] { new XAttribute("href", href), new XAttribute("type", type), new XAttribute("title", title) });
            if (!string.IsNullOrEmpty(title)) {
                itemref.SetAttributeValue("title", title);
            }
            this._element.Add(itemref);
        }

        internal XElement ToElement() {
            return this._element;
        }
    }
}
