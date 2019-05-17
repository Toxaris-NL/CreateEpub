using System.Collections.Generic;
using System.Xml.Linq;

namespace Epub
{
    public class NavPoint
    {
        private readonly string _label;
        private readonly string _id;
        private readonly string _content;
        private readonly string _class;
        private readonly int _playOrder;
        private readonly List<NavPoint> _navpoints;

        internal NavPoint(string label, string id, string content, int playOrder, string @class)
        {
            _label = label;
            _id = id;
            _content = content;
            _playOrder = playOrder;
            _class = @class;
            _navpoints = new List<NavPoint>();
        }

        internal NavPoint(string label, string id, string content, int playOrder) : this(label, id, content, playOrder, string.Empty)
        {
        }

        public NavPoint AddNavPoint(string label, string content, int playOrder)
        {
            string id = _id + "x" + (_navpoints.Count + 1).ToString();
            NavPoint point = new NavPoint(label, id, content, playOrder);
            _navpoints.Add(point);
            return point;
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(Ncx.NcxNs + "navPoint", new object[] { new XAttribute("id", _id), new XAttribute("playOrder", _playOrder) });
            if (!string.IsNullOrEmpty(_class))
            {
                element.Add(new XAttribute("class", _class));
            }
            element.Add(new XElement(Ncx.NcxNs + "navLabel", new XElement(Ncx.NcxNs + "text", _label)));
            element.Add(new XElement(Ncx.NcxNs + "content", new XAttribute("src", _content)));
            foreach (NavPoint point in _navpoints)
            {
                element.Add(point.ToElement());
            }

            return element;
        }
    }
}