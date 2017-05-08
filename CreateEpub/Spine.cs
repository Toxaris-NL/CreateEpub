using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Spine {
        private struct ItemRef {
            public string id;
            public bool linear;
        }

        private string _toc;
        private readonly List<ItemRef> _itemRefs;

        internal Spine() {
            this._itemRefs = new List<ItemRef>();
        }

        internal void SetToc(string toc) {
            this._toc = toc;
        }

        internal void AddItemRef(string id, bool linear) {
            ItemRef reference;
            reference.id = id;
            reference.linear = linear;
            this._itemRefs.Add(reference);
        }

        internal XElement ToElement() {
            XElement element = new XElement(Document.OpfNs + "spine");
            if (!string.IsNullOrEmpty(this._toc)) {
                element.Add(new XAttribute("toc", this._toc));
            }
            foreach(ItemRef reference in this._itemRefs) {
                XElement item = new XElement(Document.OpfNs + "itemref", new XAttribute("idref", reference.id));
                if (!reference.linear) {
                    item.SetAttributeValue("linear", "no");
                }
                element.Add(item);
            }
            return element;
        }
    }
}
