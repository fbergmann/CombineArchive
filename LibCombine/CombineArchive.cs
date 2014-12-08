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

    /// <summary>
    /// Number of issues encountered during reading the file (first element is a state: warning / error / info, second is a message describing what is wrong)
    /// </summary>
    public List<Tuple<string, string>> Issues { get; set; }

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

      var files = Entries.Where(e => e.Format == format || Entry.IsFormat(format.ToLowerInvariant(), e.Format)).ToList();
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

      if (doc.DocumentElement != null)
      {
        foreach (var currentNs in new[] {omexNs, Entry.KnownFormats["manifest"]})
        {
          var list = doc.DocumentElement.GetElementsByTagName("content", currentNs);
          foreach (XmlNode xmlNode in list)
          {
            var element = (XmlElement) xmlNode;
            var location = element.GetAttribute("location");
            var format = element.GetAttribute("format");
            var master = element.GetAttribute("master");
            var entry = new Entry
            {
              Archive = this,
              Location = location,
              Format = format, 
            };
            if (!string.IsNullOrWhiteSpace(master) && master.ToLowerInvariant() == "true")
            {
              entry.IsMaster = true;
              MainEntry = entry;              
            }
            Entries.Add(entry);

          }
          if (Entries.Count > 0)
            break;
          else if (currentNs != omexNs)
          {
            Issues.Add(new Tuple<string, string>("error", string.Format("invalid ns '{0}' used", currentNs)));
          }
        }

      }
      else
      {
        Issues.Add(new Tuple<string, string>("error", "no valid xml document given"));
      }

      var descEntries = Entries.Where(s => Entry.IsFormat("omex", s.Format)).ToList();
      foreach (var entry in descEntries)
      {
        string entryLocation = entry.Location;
        if (entryLocation.Contains("http://"))
        {
          Issues.Add(new Tuple<string, string>("warning", "archive contains web locations, that are not in the first version of the format"));
          continue;
        }
        Descriptions.AddRange(OmexDescription.ParseFile(Path.Combine(BaseDir, entryLocation)));
      }

      // remove all non-relevant entries
      Entries.RemoveAll(e => Entry.IsFormat("omex", e.Format) || Entry.IsFormat("manifest", e.Format));

      if (Entries.Count == 0)
      {
        Issues.Add(new Tuple<string, string>("warning", "archive is empty"));
      } 
      else if (Entries.Count > 0 && MainEntry == null)
      {
        Issues.Add(new Tuple<string, string>("warning", "archive has more than one entry, but no main element"));
      }

      // if we don't have a master tag the first element that is left
      if (Descriptions.Count > 0 && MainEntry == null)
      {
        MainEntry = Entries.FirstOrDefault(e => e.Description != null);
      }
      
    }

    private void InitializeFromDir(string fileName)
    {
      bool isSed = Path.GetExtension(fileName).IsOneOf(".sedx");
      string mainFile = Path.Combine(BaseDir, Path.GetFileName(fileName) + ".xml");
      if (!File.Exists(mainFile))
        mainFile = Path.Combine(BaseDir, Path.GetFileNameWithoutExtension(fileName) + ".xml");
      Entries.Clear();

      var files = Directory.GetFiles(BaseDir);
      foreach (var file in files)
      {
        var entry = new Entry
                    {
                      Archive = this,
                      Format = Entry.GuessFormat(file),
                      Location = file
                        .Replace(BaseDir, "./")
                        .Replace("././", "./")
                        .Replace("./\\", "./")
                        .Replace("./manifest.xml", ".")
                    };
        Entries.Add(entry);

        if (file == mainFile)
          MainEntry = entry;
      }
    }
    public void InitializeFromArchive(string fileName)
    {
      BaseDir = Util.UnzipArchive(fileName);
      string manifest = Path.Combine(BaseDir, "manifest.xml");
      if (File.Exists(manifest))
        ParseManifest(manifest);
      else InitializeFromDir(fileName);

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
      Issues = new List<Tuple<string, string>>();
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

      Entries.RemoveAll(e => e.Location == manifestFile || Entry.IsFormat("omex", e.Format) && e.GetLocalFileName() != null);

      Entries.Insert(0, new Entry { Location = manifestFile, Format = Entry.KnownFormatsList["manifest"].FirstOrDefault() });

      UpdateRefs();


      for (int i = 0; i < Descriptions.Count; i++)
      {
        var metadataFile = Path.Combine(BaseDir, string.Format("metadata{0}.xml", i));
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
      XNamespace ns = omexNs;
      var root = new XElement(ns + "omexManifest");
      foreach (var entry in Entries)
      {
        root.Add(
            new XElement(ns + "content",
                new XAttribute("location",
                    entry.Location
                    .Replace(BaseDir, "./")
                    .Replace("././", "./")
                    .Replace("./\\", "./")
                    .Replace("./manifest.xml", ".")
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

    public List<Tuple<string, string>> Validate()
    {
      var result = new List<Tuple<string, string>>(Issues);    
 
      return result;
    }
  }
}
