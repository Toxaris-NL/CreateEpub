using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epub
{
    internal class Container
    {
        private struct RootFile {
            public string File;
            public string MediaType;
        }

        private readonly List<Container.RootFile> _rootFiles;

        internal Container() {
            this._rootFiles = new List<RootFile>();
        }

        internal void AddRootFile(string file, string mediaType) {
            Container.RootFile rFile;
            rFile.File = file;
            rFile.MediaType = mediaType;
            this._rootFiles.Add(rFile);
        }

        internal XElement ToElement() {
            XNamespace NameSpace = "urn:oasis:names:tc:opendocument:xmlns:container";
            XElement Element = new XElement(NameSpace + "container", new XAttribute("version", "1.0"));
            XElement FilesElement = new XElement(NameSpace + "rootfiles");
            foreach(Container.RootFile rFile in this._rootFiles) {
                XElement FileElement = new XElement(NameSpace + "rootfile", new object[] { new XAttribute("full-path", rFile.File), new XAttribute("media-type", rFile.MediaType) });
                FilesElement.Add(FileElement);
            }
            Element.Add(FilesElement);
            return Element;
        }
    }
}
