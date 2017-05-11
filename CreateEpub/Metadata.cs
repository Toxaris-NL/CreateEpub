using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub {
    internal class Metadata {
        private readonly List<DcItem> _dcItems = new List<DcItem>();
        private readonly List<MetaItem> _metaItems = new List<MetaItem>();
        private readonly List<Item> _items = new List<Item>();

        internal void AddAuthor(string name, string sort) {
            AddCreator(name, "aut", sort);
        }

        internal void AddTranslator(string name, string sort) {
            AddCreator(name, "trl", sort);
        }

        internal void AddSubject(string subject) {
            DcItem item = new DcItem("subject", subject);
            _dcItems.Add(item);
        }

        internal void AddDescription(string description) {
            DcItem item = new DcItem("description", description);
            _dcItems.Add(item);
        }

        internal void AddType(string type) {
            DcItem item = new DcItem("type", type);
            _dcItems.Add(item);
        }

        internal void AddFormat(string format) {
            DcItem item = new DcItem("format", format);
            _dcItems.Add(item);
        }

        internal void AddLanguage(string language) {
            DcItem item = new DcItem("language", language);
            _dcItems.Add(item);
        }

        internal void AddRelation(string relation) {
            DcItem item = new DcItem("relation", relation);
            _dcItems.Add(item);
        }

        internal void AddRights(string rights) {
            DcItem item = new DcItem("rights", rights);
            _dcItems.Add(item);
        }

        internal void AddCreator(string name, string role, string sort = "") {
            DcItem item = new DcItem("creator", name);
            if (Globals.Version == 2) {
                item.SetOpfAttribute("role", role);
                if (!string.IsNullOrEmpty(sort)) {
                    item.SetOpfAttribute("file-as", sort);
                }
                _dcItems.Add(item);
            } else if(Globals.Version == 3) {
                MetaItem metaitem = new MetaItem(role);
                string id;
                if (role == "aut") {
                    id = role + Globals.AuthorCount;
                } else {
                    id = role + Globals.TranslatorCount;
                }
                item.SetAttribute("id", id);
                _dcItems.Add(item);
                metaitem.SetAttribute("property", "role");
                metaitem.SetAttribute("refines", "#" + id);
                metaitem.SetAttribute("scheme", "marc:relators");
                _metaItems.Add(metaitem);
            }
        }

        internal void AddContributor(string name, string role) {
            DcItem item = new DcItem("contributor", name);
            item.SetOpfAttribute("role", role);
            _dcItems.Add(item);
        }

        internal void AddTitle(string title) {
            DcItem item = new DcItem("title", title);
            if(Globals.Version == 2) {
                _dcItems.Add(item);
            } else if (Globals.Version == 3) {
                MetaItem metaitem = new MetaItem("main");
                string id = "title";
                item.SetAttribute("id", id);
                _dcItems.Add(item);
                metaitem.SetAttribute("property", "title-type");
                metaitem.SetAttribute("refines", "#" + id);
                _metaItems.Add(metaitem);
            }
        }

        internal void AddSeries(string serie, string number) {
             if (Globals.Version == 3) {
                DcItem item = new DcItem("title", serie);
                MetaItem metaitem = new MetaItem("collection");
                string id = "series";
                item.SetAttribute("id", id);
                _dcItems.Add(item);
                metaitem.SetAttribute("property", "title-type");
                metaitem.SetAttribute("refines", "#" + id);
                _metaItems.Add(metaitem);
                metaitem = new MetaItem(number);
                metaitem.SetAttribute("property", "group-position");
                metaitem.SetAttribute("refines", "#" + id);
                _metaItems.Add(metaitem);
            }
        }

        internal void AddModification(DateTime time) {
            string TimeString = time.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
            DcItem item = new DcItem("date", TimeString);
            if (Globals.Version == 2) {
                item.SetOpfAttribute("event", "creation");
                _dcItems.Add(item);
            } else if (Globals.Version == 3) {
                _dcItems.Add(item);
                MetaItem metaitem = new MetaItem(TimeString);
                metaitem.SetAttribute("property", "dcterms:modified");
                _metaItems.Add(metaitem);
            }
        }

        internal void AddBookIdentifier(string id, string uuid) {
            AddBookIdentifier(id, uuid, string.Empty);
        }

        internal void AddBookIdentifier(string id, string uuid, string scheme) {
            DcItem item = new DcItem("identifier", uuid);
            item.SetAttribute("id", id);
            if(!string.IsNullOrEmpty(scheme)) {
                item.SetOpfAttribute("scheme", scheme);
            }
            _dcItems.Add(item);
        }

        internal void AddItem(string name, string content) {
            Item item = new Item(name, content);
            _items.Add(item);
        }

        internal XElement ToElement() {
            XElement element;

            if (Globals.Version == 2) {
                element = new XElement("metadata", new XAttribute(XNamespace.Xmlns + "dc", Globals.DcNs), new XAttribute(XNamespace.Xmlns + "opf", Globals.OpfNs));

            } else if (Globals.Version == 3) {
                element = new XElement("metadata", new XAttribute(XNamespace.Xmlns + "dcterms", Globals.DcTerms), new XAttribute(XNamespace.Xmlns + "dc", Globals.DcNs), new XAttribute(XNamespace.Xmlns + "opf", Globals.OpfNs));
            } else {
                element = new XElement("metadata");
            }

            foreach(DcItem item in _dcItems) {
                XElement itemElement = item.ToElement();
                element.Add(itemElement);
            }

            foreach(MetaItem item in _metaItems) {
                XElement itemElement = item.ToElement();
                element.Add(itemElement);
            }

            foreach(Item item in _items) {
                XElement itemElement = item.ToElement();
                element.Add(itemElement);
            }

            return element;
        }
    }
}
