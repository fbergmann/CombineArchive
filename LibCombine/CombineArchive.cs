using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System;

namespace LibCombine
{

    /// <summary>
    /// Main entry point of LibCombine. The CombineArchive, can create
    /// modify and store COMBINE archives.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// var newArchive = new CombineArchive
    ///         {
    ///             BaseDir = @"C:\Users\fbergmann\Documents\SBML Models\",
    ///             Descriptions = new List< OmexDescription > { 
    ///                 new OmexDescription
    ///            {
    ///                About = "./BorisEJB.xml",
    ///                Description = "original JDesigner model for Kholodenko2000 - MAPK feedback",
    ///                Creators =
    ///                    new List< VCard > 
    ///                    { 
    ///                        new VCard 
    ///                        { 
    ///                            FamilyName = "Bergmann", 
    ///                            GivenName = "Frank", 
    ///                            Email = "fbergman@caltech.edu", 
    ///                            Organization = "California Institute of Technology" 
    ///                        } 
    ///                    },
    ///                Created = DateTime.Parse("2013-04-04 16:00+1")
    ///            }
    ///             },
    ///             Entries = new List< Entry > { 
    ///                 new Entry { Location = "./BorisEJB.xml", Format = Entry.KnownFormats["sbml"] }, 
    ///                 new Entry { Location = "./paper/Kholodenko2000.pdf", Format = Entry.KnownFormats["pdf"] }, 
    ///                 new Entry { Location = "http://www.ebi.ac.uk/biomodels-main/BIOMD0000000010", Format = Entry.KnownFormats["sbml"] }, 
    ///             }
    ///         };
    ///
    ///         newArchive.SaveTo(@"C:\Users\fbergmann\Desktop\Boris.omex");
    ///]]>
    /// </example>
    public class CombineArchive : IEnumerable<Entry>
    {
        const string omexNs = "http://identifiers.org/combine.specifications/omex-manifest";

        public List<Entry> Entries { get; set; }
        
        public List<OmexDescription> Descriptions { get; set; }

        public string ArchiveFileName { get; set; }

        public string BaseDir { get; set; }

        public Entry MainEntry { get; set; }

        public Entry AddEntry(string fileName, string format, OmexDescription description)
        {
            if (string.IsNullOrWhiteSpace(BaseDir))
                BaseDir = Path.GetTempPath();

            if (!File.Exists(fileName))
                return null;

            var name = Path.GetFileName(fileName);
            string tempFile = Path.Combine(BaseDir, name);

            if (tempFile != fileName)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                File.Copy(fileName, tempFile);
            }

            var entry = new Entry
            {
                Archive = this,
                Format = format,
                Location = name
            };

            Entries.Add(entry);

            if (description != null && !description.Empty)
            {
                description.About = name;
                Descriptions.Add(description);
            }


            return entry;


        }

        public List<Entry> GetEntriesWithFormat(string format)
        {
            UpdateRefs();

            var files = Entries.Where(e => e.Format == format || Entry.KnownFormats[format.ToLowerInvariant()].Contains(e.Format)).ToList();
            return files;
        }

        public int GetNumEntriesWithFormat(string format)
        {
            return GetEntriesWithFormat(format).Count;
        }

        public bool HasEntriesWithFormat(string format)
        {
            return GetEntriesWithFormat(format).Any();
        }

        public List<string> GetFilesWithFormat(string format)
        {
            var files = GetEntriesWithFormat(format).Where(e => e.GetLocalFileName() != null).Select(s => s.GetLocalFileName()).ToList();
            return files;
        }

        public int GetNumFilesWithFormat(string format)
        {
            return GetFilesWithFormat(format).Count;
        }

        public bool HasFilesWithFormat(string format)
        {
            return GetFilesWithFormat(format).Any();
        }


        /// <summary>
        /// Constructs a new archive document from the buffer
        /// </summary>
        /// <param name="buffer">buffer holding an omex archive.</param>
        /// <returns>the archive</returns>
        public static CombineArchive FromBuffer(byte[] buffer)
        {
            var file = Path.GetTempFileName();
            File.WriteAllBytes(file, buffer);
            try
            {
                return FromFile(file);
            }
            finally
            {
                try
                {
                    File.Delete(file);
                }
                catch 
                {
                    // ignore
                }
            }
            
        }

        /// <summary>
        /// Constructs a new archive document from the given filename
        /// </summary>
        /// <param name="fileName">Name of the file to load.</param>
        /// <returns>the document representing the file</returns>
        public static CombineArchive FromFile(string fileName)
        {
            var result = new CombineArchive();
            result.InitializeFromArchive(fileName);
            return result;
        }

        private void ParseManifest(string fileName)
        {
            MainEntry = null;
            var doc = new XmlDocument();
            doc.Load(fileName);
            var list = doc.DocumentElement.GetElementsByTagName("content", omexNs);
            foreach (XmlNode xmlNode in list)
            {
                var element = (XmlElement)xmlNode;
                var location = element.GetAttribute("location");
                var format = element.GetAttribute("format");
                var master = element.GetAttribute("master");
                var entry = new Entry
                                {
                                    Archive = this,
                                    Location = location,
                                    Format = format
                                };
                if (!string.IsNullOrWhiteSpace(master) && master.ToLowerInvariant() == "true")
                    MainEntry = entry;
                Entries.Add(entry);
            }

            var descEntries = Entries.Where(s => Entry.KnownFormats["omex"].Contains(s.Format)).ToList();
            foreach (var entry in descEntries)
            {
                string entryLocation = entry.Location;
                if (entryLocation.Contains("http://"))
                    continue;
                Descriptions.AddRange(OmexDescription.ParseFile(Path.Combine(BaseDir, entryLocation)));
            }

            Entries.RemoveAll(e => Entry.KnownFormats["omex"].Contains(e.Format) || Entry.KnownFormats["manifest"].Contains(e.Format));

            if (Descriptions.Count > 0 && MainEntry == null)
            {
                MainEntry = Entries.FirstOrDefault(e => e.Description != null);
            }

        }

        public void InitializeFromArchive(string fileName)
        {
            BaseDir = Util.UnzipArchive(fileName);
            ParseManifest(Path.Combine(BaseDir, "manifest.xml"));
            ArchiveFileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombineArchive"/> class.
        /// </summary>
        public CombineArchive()
        {
            ArchiveFileName = "untitled.omex";
            Entries = new List<Entry>();
            Descriptions = new List<OmexDescription>();
        }

        /// <summary>
        /// Writes the manifest to.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void WriteManifestTo(string fileName)
        {
            File.WriteAllText(fileName, ToManifest());
        }
        
        internal void UpdateRefs()
        {
            Entries.ForEach(entry => entry.Archive = this);
        }

        /// <summary>
        /// Saves to.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveTo(string fileName)
        {
            var manifestFile = Path.Combine(BaseDir, "manifest.xml");

            Entries.RemoveAll(e => e.Location == manifestFile || Entry.KnownFormats["omex"].Contains(e.Format) && e.GetLocalFileName() != null);
            
            Entries.Insert(0, new Entry { Location = manifestFile, Format = Entry.KnownFormatsList["manifest"].FirstOrDefault() });            

            UpdateRefs();


            for (int i = 0; i < Descriptions.Count; i++)
            {
                var metadataFile = Path.Combine(BaseDir, string.Format("manifest{0}.xml", i));
                File.WriteAllText(metadataFile, Descriptions[i].ToXML());
                Entries.Add(new Entry { Archive = this, Location = metadataFile, Format = Entry.KnownFormatsList["omex"].FirstOrDefault() });
            }

            WriteManifestTo(manifestFile);
            

            var fileNames = Entries.Select(e => e.GetLocalFileName()).Where(s => s != null).ToList();
            

            Util.CreateArchive(fileName, fileNames, BaseDir);
        }

        /// <summary>
        /// Converts it to the manifest, 
        /// </summary>
        /// <returns></returns>
        public string ToManifest()
        {
          XNamespace ns = Entry.KnownFormatsList["manifest"].FirstOrDefault();
            var root = new XElement(ns + "omexManifest");
            foreach (var entry in Entries)
            {
                root.Add(
                    new XElement(ns +"content", 
                        new XAttribute("location", 
                            entry.Location
                            .Replace(BaseDir, "./")
                            .Replace("././", "./")
                            .Replace("./\\", "./")
                            ), 
                        new XAttribute("format", entry.Format),
                        new XAttribute("master", entry == MainEntry ? "true" : "false")));
            }
            var srcTree = new XDocument(root);
            return 
                "<?xml version='1.0' encoding='utf-8' standalone='yes'?>\n" + 
                srcTree.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Entry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
