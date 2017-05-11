using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Manifest {
        private readonly XElement _element;

        internal Manifest() {
            this._element = new XElement(Globals.OpfNs + "manifest");
        }

        internal void AddItem(string id, string href, string type, string properties = "") {
            XElement item = new XElement(Globals.OpfNs + "item");
            item.SetAttributeValue("id", id);
            item.SetAttributeValue("href", href);
            item.SetAttributeValue("media-type", type);
            if (!string.IsNullOrEmpty(properties)) {
                item.SetAttributeValue("properties", properties);
            }
            this._element.Add(item);
        }

        internal XElement ToElement() {
            return this._element;
        }
    }
}
