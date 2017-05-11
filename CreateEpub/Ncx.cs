using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Ncx {
        private string _title;
        private readonly List<string> _authors;
        private string _uid;
        private readonly List<NavPoint> _navpoints;
        internal static readonly XNamespace NcxNs = "http://www.daisy.org/z3986/2005/ncx/";

        internal Ncx() {
            this._navpoints = new List<NavPoint>();
            this._authors = new List<string>();
            this._title = string.Empty;
    }

        internal void SetUid(string uid) {
            this._uid = uid;
        }

        internal void AddAuthor(string author) {
            this._authors.Add(author);
        }

        internal void AddTitle(string title) {
            this._title = this._title + " " + title;
        }

        internal void SetTitle(string title) {
            if(string.IsNullOrEmpty(title)) {
                this._title = string.Empty;
            } else {
                this._title = title;
            }
        }

        internal XDocument ToXmlDocument() {
            XDocument ncxDocument;

            if(Globals.Version == 2) {
                ncxDocument = new XDocument(new object[] { new XDocumentType("ncx", "-//NISO/DTD ncx 2005-1//EN", "http://www.daisy.org/z3986/2005/ncx-2005.1.dtd", null) });
            } else {
                ncxDocument = new XDocument();
            }

            XElement ncx = new XElement(Ncx.NcxNs + "ncx");
            ncx.SetAttributeValue("verion", "2005-1");
            ncx.Add(this.CreateHeadElement());
            ncx.Add(new XElement(Ncx.NcxNs + "docTitle", new XElement(Ncx.NcxNs + "text", this._title)));

            foreach(string author in this._authors) {
                ncx.Add(new XElement(Ncx.NcxNs + "docAuthor", new XElement(Ncx.NcxNs + "text", author)));
            }

            XElement navMap = new XElement(Ncx.NcxNs + "navMap");


            foreach (NavPoint point in this._navpoints) {
                navMap.Add(point.ToElement());
            }

            ncx.Add(navMap);
            ncxDocument.Add(ncx);
            return ncxDocument;
        }

        internal NavPoint AddNavPoint(string label, string id, string content, int playOrder) {
            NavPoint point = new NavPoint(label, id, content, playOrder);
            this._navpoints.Add(point);
            return point;
        }

        private XElement CreateHeadElement() {
            XElement head = new XElement(Ncx.NcxNs + "head");
            head.Add(new XElement(Ncx.NcxNs + "meta", new object[] { new XAttribute("name", "dtb:uid"), new XAttribute("content", this._uid) }));
            head.Add(new XElement(Ncx.NcxNs + "meta", new object[] { new XAttribute("name", "dtb:depth"), new XAttribute("content", "1") }));
            head.Add(new XElement(Ncx.NcxNs + "meta", new object[] { new XAttribute("name", "dtb:totalPageCount"), new XAttribute("content", "0") }));
            head.Add(new XElement(Ncx.NcxNs + "meta", new object[] { new XAttribute("name", "dtb:maxPageNumber"), new XAttribute("content", "0") }));
            return head;
        }
    }
}
