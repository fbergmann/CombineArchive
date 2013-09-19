using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LibCombine
{
    /// <summary>
    /// A utility class making it easier dealing with the description format
    /// </summary>
    public class OmexDescription
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the about filed, ought to be the same as a location in the archive.
        /// </summary>
        /// <value>
        /// The location of one of the entries in the archive
        /// </value>
        public string About { get; set; }
        public List<VCard> Creators { get; set; }
        public DateTime Created { get; set; }
        public bool Empty
        {
            get
            {
              bool haveDescription = !string.IsNullOrWhiteSpace(Description);
              if (!haveDescription) return true;
              bool haveCreator = Creators != null && Creators.Count > 0;
              if (!haveCreator) return true;
              bool firstCreatorEmpty = Creators[0].Empty;
              if (firstCreatorEmpty) return true;
              return false;
            }
        }
        public List<DateTime> Modified { get; set; }

        private const string rdfNS = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private const string dcNS = "http://purl.org/dc/terms/";

        /// <summary>
        /// Parses the given file and returns the list of description objects.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>the list of description objects in the metadata file</returns>
        public static List<OmexDescription> ParseFile(string fileName)
        {
            return ParseString(File.ReadAllText(fileName));
        }

        /// <summary>
        /// Parses the xml string of metadata information and returns the 
        /// descriptions encoded in it.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>list of descriptions in that string</returns>
        public static List<OmexDescription> ParseString(string xml)
        {
            var result = new List<OmexDescription>();
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                if (doc.DocumentElement != null)
                {
                    var descs = doc.DocumentElement.GetElementsByTagName("Description", rdfNS);
                    if (descs.Count > 0)
                    {
                        try
                        {

                            result.AddRange(from XmlNode desc in descs select new OmexDescription((XmlElement)desc));

                        }
                        catch 
                        {
                            
                        }
                    }
                }
            }
            catch
            {
                
            }
            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OmexDescription"/> class.
        /// </summary>
        public OmexDescription()
        {
            Creators = new List<VCard>();
            Modified = new List<DateTime>();
            Created = DateTime.UtcNow;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OmexDescription"/> class.
        /// </summary>
        /// <param name="element">The element, should alredy be the Description element</param>
        public OmexDescription(XmlElement element)
            : this()
        {
            About = element.GetAttribute("about", rdfNS);

            var list = element.GetElementsByTagName("description", dcNS);
            if (list.Count > 0)
            {
                Description = list[0].InnerText;
            }
            list = element.GetElementsByTagName("creator", dcNS);
            if (list.Count > 0)
            {
                foreach (XmlNode xmlNode in list)
                {
                    Creators.Add(new VCard((XmlElement)xmlNode));
                }
            }
            list = element.GetElementsByTagName("created", dcNS);
            if (list.Count > 0)
            {
                list = ((XmlElement)list[0]).GetElementsByTagName("W3CDTF", dcNS);
                if (list.Count > 0)
                    Created = DateTime.Parse(list[0].InnerText.Replace('T', ' '));
            }
            list = element.GetElementsByTagName("modified", dcNS);
            if (list.Count > 0)
            {
                foreach (XmlNode xmlNode in list)
                {
                    var date = ((XmlElement)xmlNode).GetElementsByTagName("W3CDTF", dcNS);
                    if (date.Count > 0)
                        Modified.Add(DateTime.Parse(date[0].InnerText.Replace('T', ' ')));
                }                
            }

        }

        /// <summary>
        /// Converts this description to an XML string
        /// </summary>
        /// <param name="omitDeclaration">if set to <c>true</c> the XML declaration will be omitted].</param>
        /// <returns></returns>
        public string ToXML(bool omitDeclaration = false)
        {
            if (Modified.Count == 0)
                Modified.Add(DateTime.UtcNow);

            const string modified = 
                "<dcterms:modified rdf:parseType='Resource'>" +
                "<dcterms:W3CDTF>{0}</dcterms:W3CDTF>" +
                "</dcterms:modified>";

            var modBuilder = new StringBuilder();
            foreach (var dateTime in Modified)
            {
                modBuilder.AppendFormat(modified, dateTime.ToString("u").Replace(' ', 'T'));
            }

            const string format = @"
                <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
                                xmlns:dcterms='http://purl.org/dc/terms/'
                                xmlns:vCard='http://www.w3.org/2006/vcard/ns#'>
                    <rdf:Description rdf:about='{0}'>
                        <dcterms:description>{1}</dcterms:description>
                        {2}
                        <dcterms:created rdf:parseType='Resource'>
                            <dcterms:W3CDTF>{3}</dcterms:W3CDTF>
                        </dcterms:created>
                        {4}
                    </rdf:Description>
                </rdf:RDF>
                ";

            var c_builder = new StringBuilder();
            foreach (VCard creator in Creators)
            {
                c_builder.AppendLine(creator.ToXML());
            }

            return Util.PrettyPrint(
                string.Format(format, 
                    About, 
                    Description, 
                    c_builder.ToString(), 
                    Created.ToString("u").Replace(' ', 'T'),
                    modBuilder.ToString()), omitDeclaration);
        }


    }
}