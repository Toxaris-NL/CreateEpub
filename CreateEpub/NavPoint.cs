using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    public class NavPoint {
        private readonly string _label;
        private readonly string _id;
        private readonly string _content;
        private readonly string _class;
        private readonly int _playOrder;
        private readonly List<NavPoint> _navpoints;

        internal NavPoint(string label, string id, string content, int playOrder, string @class){
        this._label = label;
            this._id = id;
            this._content = content;
            this._playOrder = playOrder;
            this._class = @class;
            this._navpoints = new List<NavPoint>();
        }

        internal NavPoint(string label, string id, string content, int playOrder):this(label, id, content, playOrder, string.Empty) { }
        
        internal NavPoint AddNavPoint(string label, string content, int playOrder) {
            string id = this._id + "x" + (this._navpoints.Count + 1).ToString();
            NavPoint point = new NavPoint(label, id, content, playOrder);
            this._navpoints.Add(point);
            return point;
        }

        internal XElement ToElement() {
            XElement element = new XElement(Ncx.NcxNs + "navPoint", new object[] { new XAttribute("id", this._id), new XAttribute("playOrder", this._playOrder) });
            if(!string.IsNullOrEmpty(this._class)) {
                element.Add(new XAttribute("class", this._class));
            }
            element.Add(new XElement(Ncx.NcxNs + "navLabel", new XElement(Ncx.NcxNs + "text", this._label)));
            element.Add(new XElement(Ncx.NcxNs + "content", new XAttribute("src", this._content)));
            foreach(NavPoint point in this._navpoints) {
                element.Add(point.ToElement());
            }

            return element;
        }

    }
}
