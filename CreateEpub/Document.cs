using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Epub
{
    public class Document
    {
        private readonly Metadata _metadata;
        private readonly Manifest _manifest;
        private readonly Spine _spine;
        private readonly Guide _guide;
        private readonly Ncx _ncx;
        private readonly Container _container;
        private readonly Dictionary<string, int> _ids;
        private string _tempDirectory;
        private string _opfDirectory;
        private string _metainfDirectory;
        private bool _apple = false;
        private int _version;

        /// <summary>
        /// Creates a new ePUB document instance
        /// </summary>
        /// <param name="version">Set version of document. Default is version 2. It can be adapted later.</param>
        public Document(int version = 2)
        {
            _metadata = new Metadata();
            _manifest = new Manifest();
            _spine = new Spine();
            _guide = new Guide();
            _ncx = new Ncx();
            _container = new Container();
            _ids = new Dictionary<string, int>();
            Globals.AuthorCount = 0;
            Globals.TranslatorCount = 0;
            Globals.Version = version;
            _version = Globals.Version;
            if (_version != 2 && _version != 3) { _version = 2; }

            // setup mandatory TOC file
            _manifest.AddItem("ncx", "toc.ncx", "application/x-dtbncx+xml");
            _spine.SetToc("ncx");
            _container.AddRootFile("OEBPS/content.opf", "application/oebps-package+xml");
            Guid guid = Guid.NewGuid();
            string uuid = "urn:uuid" + guid.ToString();
            _ncx.SetUid(uuid);
            _metadata.AddBookIdentifier("BookId", uuid);
        }

        /// <summary>
        /// Set the version of the ePUB document.
        /// </summary>
        /// <param name="version">Only version 2 and 3 are allowed. If another value is entered, the version will be set to 2.</param>
        public void SetVersion(int version)
        {
            if (version != 2 && version != 3) { Globals.Version = 2; }
            _version = Globals.Version;
        }

        /// <summary>
        /// Indicates if the XML file for iBooks must be added to allow custom fonts.
        /// </summary>
        /// <param name="ibooks">Boolean parameter to specify if the XML must be added.</param>
        public void AddIbooksXml(bool ibooks)
        {
            _apple = ibooks;
        }

        /// <summary>
        /// Destroys instance. Performs temporary directory clean-up if one has been created.
        /// </summary>
        ~Document()
        {
            if (!string.IsNullOrEmpty(_tempDirectory) && Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        private string GetTempDirectory()
        {
            if (string.IsNullOrEmpty(_tempDirectory))
            {
                _tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(_tempDirectory);
            }

            return _tempDirectory;
        }

        private string GetOpfDirectory()
        {
            if (string.IsNullOrEmpty(_opfDirectory))
            {
                string tempDirectory = GetTempDirectory();
                _opfDirectory = Path.Combine(tempDirectory, "OEBPS");
                Directory.CreateDirectory(_opfDirectory);
            }

            return _opfDirectory;
        }

        private string GetMetaInfDirectory()
        {
            if (string.IsNullOrEmpty(_metainfDirectory))
            {
                string tempDirectory = GetTempDirectory();
                _metainfDirectory = Path.Combine(tempDirectory, "META-INF");
                Directory.CreateDirectory(_metainfDirectory);
            }

            return _metainfDirectory;
        }

        private string GetNextID(string kind)
        {
            string id;
            if (_ids.Keys.Contains(kind))
            {
                _ids[kind]++;
                id = kind + _ids[kind].ToString();
            }
            else
            {
                id = kind + "1";
                _ids[kind] = 1;
            }

            return id;
        }

        /// <summary>
        /// Add an author of the document.
        /// </summary>
        /// <param name="author">Human-readable full name</param>
        /// <param name="author_sort">Sortable arrangement of the full name</param>
        public void AddAuthor(string author, string author_sort = "")
        {
            Globals.AuthorCount++;
            _metadata.AddAuthor(author, author_sort);
            _ncx.AddAuthor(author);
        }

        /// <summary>
        /// Add title of the document
        /// </summary>
        /// <param name="title">Document's title</param>
        public void AddTitle(string title)
        {
            _metadata.AddTitle(title);
            _ncx.AddTitle(title);
        }

        /// <summary>
        /// Add document translator
        /// </summary>
        /// <param name="name">Human-readable full name</param>
        /// <param name="name_sort">Sortable arrangement of the full name</param>
        public void AddTranslator(string name, string name_sort = "")
        {
            Globals.TranslatorCount++;
            _metadata.AddTranslator(name, name_sort);
        }

        /// <summary>
        /// Add collection name and number
        /// </summary>
        /// <param name="serie">Collection/series name</param>
        /// <param name="number">Number in the collection/series. Number is a string!</param>
        public void AddSeries(string serie, string number)
        {
            _metadata.AddSeries(serie, number);
        }

        /// <summary>
        /// Add subject of document. It can be a phrase or a list of keywords.
        /// </summary>
        /// <param name="subject">Document's subject</param>
        public void AddSubject(string subject)
        {
            _metadata.AddSubject(subject);
        }

        /// <summary>
        /// Add description of the document's content
        /// </summary>
        /// <param name="description">Document's description</param>
        public void AddDescription(string description)
        {
            _metadata.AddDescription(description);
        }

        /// <summary>
        /// Add terms describing general categories, functions, genres, or aggregation levels for content.
        /// The advised best practice is to select a value from a controlled vocabulary.
        /// </summary>
        /// <param name="type">Document's type</param>
        public void AddType(string type)
        {
            _metadata.AddType(type);
        }

        /// <summary>
        /// Add media type or dimensions of the resource. Best practice is to use a value from a controlled vocabulary (e.g. MIME media types).
        /// </summary>
        /// <param name="format">Document's format</param>
        public void AddFormat(string format)
        {
            _metadata.AddFormat(format);
        }

        /// <summary>
        /// Add language of the documet's content
        /// </summary>
        /// <param name="language">RFC3066-complient two-letter language code e.g. "en", "es", "it"</param>
        public void AddLanguage(string language)
        {
            _metadata.AddLanguage(language);
        }

        /// <summary>
        /// Add an identifier of an auxiliary resource and its relationship to the publication.
        /// </summary>
        /// <param name="relation">Document's relation</param>
        public void AddRelation(string relation)
        {
            _metadata.AddRelation(relation);
        }

        /// <summary>
        /// Add a statement about rights, or a reference to one.
        /// </summary>
        /// <param name="rights">A statement about rights, or a reference to one</param>
        public void AddRights(string rights)
        {
            _metadata.AddRights(rights);
        }

        /// <summary>
        /// Add book identifier
        /// </summary>
        /// <param name="id">A string or number used to uniquely identify the resource</param>
        public void AddBookIdentifier(string id)
        {
            AddBookIdentifier(id, string.Empty);
        }

        /// <summary>
        /// Add book identifier
        /// </summary>
        /// <param name="id">A string or number used to uniquely identify the resource</param>
        /// <param name="scheme">System or authority that generated or assigned the id parameter, for example "ISBN" or "DOI"</param>
        public void AddBookIdentifier(string id, string scheme)
        {
            _metadata.AddBookIdentifier(GetNextID("id"), id, scheme);
        }

        /// <summary>
        /// Add generic metadata
        /// </summary>
        /// <param name="name">meta element name</param>
        /// <param name="content">meta element content</param>
        public void AddMetaItem(string name, string content)
        {
            _metadata.AddItem(name, content);
        }

        /// <summary>
        /// Add modification metadata
        /// </summary>
        /// <param name="content">Date/time of modification</param>
        public void AddModification(DateTime content)
        {
            _metadata.AddModification(content);
        }

        /// <summary>
        /// Add guide reference
        /// </summary>
        /// <param name="href">meta element href</param>
        /// <param name="type">meta element type</param>
        /// <param name="title">meta element title</param>
        public void AddReference(string href, string type, string title)
        {
            _guide.AddReference(href, type, title);
        }

        /// <summary>
        /// Create ePUB document and save to specified filename
        /// </summary>
        /// <param name="epubFile"></param>
        public void Create(string epubFile)
        {
            WriteOpf("content.opf");
            WriteNcx("toc.ncx");
            WriteContainer();

            using (ZipStorer zip = ZipStorer.Create(epubFile, string.Empty))
            {
                zip.EncodeUTF8 = true;
                using (MemoryStream mimetype = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("application/epub+zip")))
                {
                    zip.AddStream(ZipStorer.Compression.Store, "mimetype", mimetype, DateTime.Now, "");
                }

                if (_apple)
                {
                    AppleIbooksOptions appledisplay = new AppleIbooksOptions();

                    using (MemoryStream apple = new MemoryStream())
                    {
                        XDocument applexml = new XDocument(appledisplay.ToElement());
                        applexml.Save(apple);
                        zip.AddStream(ZipStorer.Compression.Deflate, @"META-INF\com.apple.ibooks.display-options.xml", apple, DateTime.Now, "");
                    }
                }

                zip.AddDirectory(ZipStorer.Compression.Deflate, GetTempDirectory() + @"\OEBPS", string.Empty, string.Empty);
                zip.AddDirectory(ZipStorer.Compression.Deflate, GetTempDirectory() + @"\META-INF", string.Empty, string.Empty);
            }
        }

        private string AddEntry(string path, string type)
        {
            string id = GetNextID("id");
            _manifest.AddItem(id, path, type);
            return id;
        }

        private string AddStylesheetEntry(string path)
        {
            string id = Path.GetFileName(path);
            _manifest.AddItem(id, path, "text/css");
            return id;
        }

        private string AddXhtmlEntry(string path, string properties = "")
        {
            return AddXhtmlEntry(path, true, properties);
        }

        private string AddXhtmlEntry(string path, bool linear, string properties)
        {
            string id = Path.GetFileName(path);
            _manifest.AddItem(id, path, "application/xhtml+xml", properties);
            _spine.AddItemRef(id, linear);
            return id;
        }

        private string AddImageEntry(string path, string properties = "")
        {
            string id = Path.GetFileName(path);
            string contentType = string.Empty;
            string lower = path.ToLower();
            if (lower.EndsWith(".jpg", StringComparison.Ordinal) || lower.EndsWith(".jpeg", StringComparison.Ordinal))
            {
                contentType = "image/jpeg";
            }
            else if (lower.EndsWith(".png", StringComparison.Ordinal))
            {
                contentType = "image/png";
            }
            else if (lower.EndsWith(".gif", StringComparison.Ordinal))
            {
                contentType = "image/gif";
            }
            else if (lower.EndsWith(".svg", StringComparison.Ordinal))
            {
                contentType = "image/svg+xml";
            }
            _manifest.AddItem(id, path, contentType, properties);

            return id;
        }

        private void CopyFile(string path, string epubpath)
        {
            string fullPath = Path.Combine(GetOpfDirectory(), epubpath);
            EnsureDirectoryExists(fullPath);
            File.Copy(path, fullPath, true);
        }

        private void EnsureDirectoryExists(string path)
        {
            // ToDo: Ensure epubpath contains no '..\..\'
            string destinationDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
        }

        private void WriteFile(string epubpath, byte[] content)
        {
            string fullpath = Path.Combine(GetOpfDirectory(), epubpath);
            EnsureDirectoryExists(fullpath);
            File.WriteAllBytes(fullpath, content);
        }

        private void WriteFile(string epubpath, string content)
        {
            string fullpath = Path.Combine(GetOpfDirectory(), epubpath);
            EnsureDirectoryExists(fullpath);
            File.WriteAllText(fullpath, content, Encoding.UTF8);
        }

        /// <summary>
        /// Add image to ePUB document
        /// </summary>
        /// <param name="path">Path to source image file</param>
        /// <param name="epubpath">Path to image file in ePUB document</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>id of newly created element</returns>
        public string AddImageFile(string path, string epubpath, string properties = "")
        {
            CopyFile(path, epubpath);
            return AddImageEntry(epubpath, properties);
        }

        /// <summary>
        /// Add stylesheet to ePUB document
        /// </summary>
        /// <param name="path">Path to source stylesheet file</param>
        /// <param name="epubpath">Path to stylesheet file in ePUB Document</param>
        /// <returns>id of newly created element</returns>
        public string AddStylesheetFile(string path, string epubpath)
        {
            CopyFile(path, epubpath);
            return AddStylesheetEntry(epubpath);
        }

        /// <summary>
        /// Add primary XHTML file to ePUB document
        /// </summary>
        /// <param name="path">Path to source XHTML file</param>
        /// <param name="epubpath">Path to XHTML file in ePUB Document</param>
        /// <returns>id of newly created element</returns>
        public string AddXhtmlFile(string path, string epubpath)
        {
            return AddXhtmlFile(path, epubpath, true);
        }

        /// <summary>
        /// Add primary or auxiliary XHTML files to ePUB document
        /// </summary>
        /// <param name="path">Path to source XHTML file</param>
        /// <param name="epubpath">Path to XHTML file in ePUB Document</param>
        /// <param name="primary">true for primary, false for auxiliary</param>
        /// <returns>id of newly created element</returns>
        public string AddXhtmlFile(string path, string epubpath, bool primary)
        {
            CopyFile(path, epubpath);
            return AddXhtmlEntry(epubpath, primary.ToString());
        }

        /// <summary>
        /// Add generic file to ePUB Document
        /// </summary>
        /// <param name="path">source file path</param>
        /// <param name="epubpath">path in ePUB document</param>
        /// <param name="mediatype">MIME media-type, e.g. "application/octet-stream"</param>
        /// <returns>id of newly added file</returns>
        public string AddFile(string path, string epubpath, string mediatype)
        {
            CopyFile(path, epubpath);
            return AddEntry(epubpath, mediatype);
        }

        /// <summary>
        /// Add image file to document with specified content. Image type will be derived by extension.
        /// </summary>
        /// <param name="epubpath">path in ePUB</param>
        /// <param name="content">file content</param>
        /// <returns>id of newly added file</returns>
        public string AddImageData(string epubpath, byte[] content)
        {
            WriteFile(epubpath, content);
            return AddImageEntry(epubpath);
        }

        /// <summary>
        /// Add Stylesheet file (CSS) to the ePUB document with specified content
        /// </summary>
        /// <param name="epubpath">path in ePUB</param>
        /// <param name="content">file content</param>
        /// <returns>id of newly added file</returns>
        public string AddStylesheetData(string epubpath, string content)
        {
            WriteFile(epubpath, content);
            return AddStylesheetEntry(epubpath);
        }

        /// <summary>
        /// Add primary or auxiliary XHTML file to ePUB document with specified content.
        /// </summary>
        /// <param name="epubpath">path in ePUB</param>
        /// <param name="content">file content</param>
        /// <param name="primary">true if file is primary, false if auxiliary</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public string AddXhtmlData(string epubpath, string content, bool primary, string properties = "")
        {
            WriteFile(epubpath, content);
            return AddXhtmlEntry(epubpath, primary, properties);
        }

        /// <summary>
        /// Add primary XHTML file to ePUB document with specified content
        /// </summary>
        /// <param name="epubpath">path in ePUB</param>
        /// <param name="content">file content</param>
        /// <param name="properties"></param>
        /// <returns>identifier of added file</returns>
        public string AddXhtmlData(string epubpath, string content, string properties = "")
        {
            return AddXhtmlData(epubpath, content, true, properties);
        }

        /// <summary>
        /// Add generic file to ePUB document with specified content
        /// </summary>
        /// <param name="epubpath">path in ePUB</param>
        /// <param name="content">file content</param>
        /// <param name="mediatype">MIME media-type, e.g. "application/octet-stream"</param>
        /// <returns>identifier of added file</returns>
        public string AddData(string epubpath, byte[] content, string mediatype)
        {
            WriteFile(epubpath, content);
            return AddEntry(epubpath, mediatype);
        }

        private void WriteOpf(string opffilepath)
        {
            string fullpath = Path.Combine(GetOpfDirectory(), opffilepath);
            XElement element;

            if (_version == 2)
            {
                element = new XElement(Globals.OpfNs + "package", new XAttribute("version", "2.0"), new XAttribute("unique-identifier", "BookId"));
            }
            else
            {
                element = new XElement(Globals.OpfNs + "package", new XAttribute("version", "3.0"), new XAttribute("unique-identifier", "BookId"), new XAttribute("prefix", "rendition: http://www.idpf.org/vocab/rendition/#"));
            }

            element.Add(_metadata.ToElement());
            element.Add(_manifest.ToElement());
            element.Add(_spine.ToElement());
            element.Add(_guide.ToElement());

            string xmlstring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r" + element.ToString();
            xmlstring = xmlstring.Replace("xmlns=\"\"", "");
            File.WriteAllText(fullpath, xmlstring);
        }

        private void WriteNcx(string ncxfilepath)
        {
            string fullpath = Path.Combine(GetOpfDirectory(), ncxfilepath);
            XDocument ncx = _ncx.ToXmlDocument();
            SaveXDocument(fullpath, ncx);
        }

        private void WriteContainer()
        {
            string fullpath = Path.Combine(GetMetaInfDirectory(), "container.xml");
            XElement element = _container.ToElement();
            SaveXElement(fullpath, element);
        }

        /// <summary>
        /// Add navigation point to top-level Table of Contents.
        /// </summary>
        /// <param name="label">Text of TOC entry</param>
        /// <param name="content">Link to TOC entry</param>
        /// <param name="playorder">play order counter</param>
        /// <returns>newly created NavPoint</returns>
        public NavPoint AddNavPoint(string label, string content, int playorder)
        {
            string id = GetNextID("navid");
            return _ncx.AddNavPoint(label, id, content, playorder);
        }

        private void SaveXDocument(string fullpath, XDocument xmlFile)
        {
            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
                using (XmlWriter xw = XmlWriter.Create(fs, settings))
                {
                    xmlFile.Save(xw);
                }
            }
        }

        private void SaveXElement(string fullpath, XElement element)
        {
            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
                using (XmlWriter xw = XmlWriter.Create(fs, settings))
                {
                    element.Save(xw);
                }
            }
        }
    }
}