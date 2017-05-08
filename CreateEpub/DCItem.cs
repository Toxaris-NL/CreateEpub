using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class DcItem {
        private readonly string _name;
        private readonly string _value;
        private readonly IDictionary<string, string> _attributes;
        private readonly IDictionary<string, string> _opfAttributes;

        internal DcItem(string name, string value) {
            this._name = name;
            this._value = value;
            this._attributes = new Dictionary<string, string>();
            this._opfAttributes = new Dictionary<string, string>();
        }

        internal void SetAttribute(string name, string value) {
            this._attributes.Add(name, value);
        }

        internal void SetOpfAttribute(string name, string value) {
            this._opfAttributes.Add(name, value);
        }

        internal XElement ToElement() {
            XElement Element = new XElement(Document.DcNs + this._name, this._value);
            foreach(string key in this._opfAttributes.Keys) {
                string value = this._opfAttributes[key];
                Element.SetAttributeValue(Document.OpfNs + key, value);
            }
            foreach (string key in this._attributes.Keys) {
                string value = this._attributes[key];
                Element.SetAttributeValue(key, value);
            }

            return Element;
        }
    }
}
