using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Item {
        private readonly string _name;
        private readonly string _value;

        internal Item(string name, string value) {
            this._name = name;
            this._value = value;
        }

        internal XElement ToElement() {
            XElement element = new XElement("meta");
            element.SetAttributeValue("name", this._name);
            element.SetAttributeValue("content", this._value);
            return element;
        }
    }
}
