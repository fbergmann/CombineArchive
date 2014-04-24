using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibCombine
{
    /// <summary>
    /// Entries in the COMBINE archive
    /// </summary>
    public class Entry
    {
      public static Dictionary<string, string> KnownFormats
      {
        get
        {
          Dictionary<string, string> result = new Dictionary<string, string>();
          foreach (KeyValuePair<string, List<string>> keyValuePairString in KnownFormatsList)
          {
            result[keyValuePairString.Key] = keyValuePairString.Value.FirstOrDefault();
          }
          return result;
        }
      }
        /// <summary>
        /// Dictionary of known formats so that they will be recognized among tools
        /// </summary>
        public static Dictionary<string, List<string>> KnownFormatsList = new Dictionary<string, List<string>> { 
            {"sbml", new List<string>{"http://identifiers.org/combine.specifications/sbml" } },
            {"sedml", new List<string>
            {
              "http://identifiers.org/combine.specifications/sed-ml", "http://identifiers.org/combine.specifications/sedml"
            }},
            {"cellml", new List<string>{"http://identifiers.org/combine.specifications/cellml" }},
            {"sed-ml", new List<string>
            {
              "http://identifiers.org/combine.specifications/sed-ml", "http://identifiers.org/combine.specifications/sedml"
            }},
            {"sbgn", new List<string>{"http://identifiers.org/combine.specifications/sbgn" }},
            {"omex", new List<string>{"http://identifiers.org/combine.specifications/omex-metadata" }},
            {"manifest", new List<string>{"http://identifiers.org/combine.specifications/omex-manifest" }},
            {"sedx", new List<string>{"application/x-sed-ml-archive" }},
            {"png", new List<string>{"image/png" }},            
            {"csv", new List<string>{"text/csv" }},
            {"323", new List<string>{"text/h323"}},
            {"acx", new List<string>{"application/internet-property-stream"}},
            {"ai", new List<string>{"application/postscript"}},
            {"aif", new List<string>{"audio/x-aiff"}},
            {"aifc", new List<string>{"audio/x-aiff"}},
            {"aiff", new List<string>{"audio/x-aiff"}},
            {"asf", new List<string>{"video/x-ms-asf"}},
            {"asr", new List<string>{"video/x-ms-asf"}},
            {"asx", new List<string>{"video/x-ms-asf"}},
            {"au", new List<string>{"audio/basic"}},
            {"avi", new List<string>{"video/x-msvideo"}},
            {"axs", new List<string>{"application/olescript"}},
            {"bas", new List<string>{"text/plain"}},
            {"bcpio", new List<string>{"application/x-bcpio"}},
            {"bin", new List<string>{"application/octet-stream"}},
            {"bmp", new List<string>{"image/bmp"}},
            {"c", new List<string>{"text/plain"}},
            {"cat", new List<string>{"application/vnd.ms-pkiseccat"}},
            {"cdf", new List<string>{"application/x-cdf"}},
            {"cer", new List<string>{"application/x-x509-ca-cert"}},
            {"class", new List<string>{"application/octet-stream"}},
            {"clp", new List<string>{"application/x-msclip"}},
            {"cmx", new List<string>{"image/x-cmx"}},
            {"cod", new List<string>{"image/cis-cod"}},
            {"cpio", new List<string>{"application/x-cpio"}},
            {"crd", new List<string>{"application/x-mscardfile"}},
            {"crl", new List<string>{"application/pkix-crl"}},
            {"crt", new List<string>{"application/x-x509-ca-cert"}},
            {"csh", new List<string>{"application/x-csh"}},
            {"css", new List<string>{"text/css"}},
            {"dcr", new List<string>{"application/x-director"}},
            {"der", new List<string>{"application/x-x509-ca-cert"}},
            {"dir", new List<string>{"application/x-director"}},
            {"dll", new List<string>{"application/x-msdownload"}},
            {"dms", new List<string>{"application/octet-stream"}},
            {"doc", new List<string>{"application/msword"}},
            {"dot", new List<string>{"application/msword"}},
            {"dvi", new List<string>{"application/x-dvi"}},
            {"dxr", new List<string>{"application/x-director"}},
            {"eps", new List<string>{"application/postscript"}},
            {"etx", new List<string>{"text/x-setext"}},
            {"evy", new List<string>{"application/envoy"}},
            {"exe", new List<string>{"application/octet-stream"}},
            {"fif", new List<string>{"application/fractals"}},
            {"flr", new List<string>{"x-world/x-vrml"}},
            {"gif", new List<string>{"image/gif"}},
            {"gtar", new List<string>{"application/x-gtar"}},
            {"gz", new List<string>{"application/x-gzip"}},
            {"h", new List<string>{"text/plain"}},
            {"hdf", new List<string>{"application/x-hdf"}},
            {"hlp", new List<string>{"application/winhlp"}},
            {"hqx", new List<string>{"application/mac-binhex40"}},
            {"hta", new List<string>{"application/hta"}},
            {"htc", new List<string>{"text/x-component"}},
            {"htm", new List<string>{"text/html"}},
            {"html", new List<string>{"text/html"}},
            {"htt", new List<string>{"text/webviewhtml"}},
            {"ico", new List<string>{"image/x-icon"}},
            {"ief", new List<string>{"image/ief"}},
            {"iii", new List<string>{"application/x-iphone"}},
            {"ins", new List<string>{"application/x-internet-signup"}},
            {"isp", new List<string>{"application/x-internet-signup"}},
            {"jfif", new List<string>{"image/pipeg"}},
            {"jpe", new List<string>{"image/jpeg"}},
            {"jpeg", new List<string>{"image/jpeg"}},
            {"jpg", new List<string>{"image/jpeg"}},
            {"js", new List<string>{"application/x-javascript"}},
            {"latex", new List<string>{"application/x-latex"}},
            {"lha", new List<string>{"application/octet-stream"}},
            {"lsf", new List<string>{"video/x-la-asf"}},
            {"lsx", new List<string>{"video/x-la-asf"}},
            {"lzh", new List<string>{"application/octet-stream"}},
            {"m", new List<string>{"application/x-matlab"}},
            {"mat", new List<string>{"application/x-matlab"}},
            {"m13", new List<string>{"application/x-msmediaview"}},
            {"m14", new List<string>{"application/x-msmediaview"}},
            {"m3u", new List<string>{"audio/x-mpegurl"}},
            {"man", new List<string>{"application/x-troff-man"}},
            {"mdb", new List<string>{"application/x-msaccess"}},
            {"me", new List<string>{"application/x-troff-me"}},
            {"mht", new List<string>{"message/rfc822"}},
            {"mhtml", new List<string>{"message/rfc822"}},
            {"mid", new List<string>{"audio/mid"}},
            {"mny", new List<string>{"application/x-msmoney"}},
            {"mov", new List<string>{"video/quicktime"}},
            {"movie", new List<string>{"video/x-sgi-movie"}},
            {"mp2", new List<string>{"video/mpeg"}},
            {"mp3", new List<string>{"audio/mpeg"}},
            {"mpa", new List<string>{"video/mpeg"}},
            {"mpe", new List<string>{"video/mpeg"}},
            {"mpeg", new List<string>{"video/mpeg"}},
            {"mpg", new List<string>{"video/mpeg"}},
            {"mpp", new List<string>{"application/vnd.ms-project"}},
            {"mpv2", new List<string>{"video/mpeg"}},
            {"ms", new List<string>{"application/x-troff-ms"}},
            {"mvb", new List<string>{"application/x-msmediaview"}},
            {"nws", new List<string>{"message/rfc822"}},
            {"oda", new List<string>{"application/oda"}},
            {"p10", new List<string>{"application/pkcs10"}},
            {"p12", new List<string>{"application/x-pkcs12"}},
            {"p7b", new List<string>{"application/x-pkcs7-certificates"}},
            {"p7c", new List<string>{"application/x-pkcs7-mime"}},
            {"p7m", new List<string>{"application/x-pkcs7-mime"}},
            {"p7r", new List<string>{"application/x-pkcs7-certreqresp"}},
            {"p7s", new List<string>{"application/x-pkcs7-signature"}},
            {"pbm", new List<string>{"image/x-portable-bitmap"}},
            {"pdf", new List<string>{"application/pdf"}},
            {"pfx", new List<string>{"application/x-pkcs12"}},
            {"pgm", new List<string>{"image/x-portable-graymap"}},
            {"pko", new List<string>{"application/ynd.ms-pkipko"}},
            {"pma", new List<string>{"application/x-perfmon"}},
            {"pmc", new List<string>{"application/x-perfmon"}},
            {"pml", new List<string>{"application/x-perfmon"}},
            {"pmr", new List<string>{"application/x-perfmon"}},
            {"pmw", new List<string>{"application/x-perfmon"}},
            {"pnm", new List<string>{"image/x-portable-anymap"}},
            {"pot,", new List<string>{"application/vnd.ms-powerpoint"}},
            {"ppm", new List<string>{"image/x-portable-pixmap"}},
            {"pps", new List<string>{"application/vnd.ms-powerpoint"}},
            {"ppt", new List<string>{"application/vnd.ms-powerpoint"}},
            {"prf", new List<string>{"application/pics-rules"}},
            {"ps", new List<string>{"application/postscript"}},
            {"pub", new List<string>{"application/x-mspublisher"}},
            {"qt", new List<string>{"video/quicktime"}},
            {"ra", new List<string>{"audio/x-pn-realaudio"}},
            {"ram", new List<string>{"audio/x-pn-realaudio"}},
            {"ras", new List<string>{"image/x-cmu-raster"}},
            {"rgb", new List<string>{"image/x-rgb"}},
            {"rmi", new List<string>{"audio/mid"}},
            {"roff", new List<string>{"application/x-troff"}},
            {"rtf", new List<string>{"application/rtf"}},
            {"rtx", new List<string>{"text/richtext"}},
            {"scd", new List<string>{"application/x-msschedule"}},
            {"sct", new List<string>{"text/scriptlet"}},
            {"setpay", new List<string>{"application/set-payment-initiation"}},
            {"setreg", new List<string>{"application/set-registration-initiation"}},
            {"sh", new List<string>{"application/x-sh"}},
            {"shar", new List<string>{"application/x-shar"}},
            {"sit", new List<string>{"application/x-stuffit"}},
            {"snd", new List<string>{"audio/basic"}},
            {"spc", new List<string>{"application/x-pkcs7-certificates"}},
            {"spl", new List<string>{"application/futuresplash"}},
            {"src", new List<string>{"application/x-wais-source"}},
            {"sst", new List<string>{"application/vnd.ms-pkicertstore"}},
            {"stl", new List<string>{"application/vnd.ms-pkistl"}},
            {"stm", new List<string>{"text/html"}},
            {"svg", new List<string>{"image/svg+xml"}},
            {"sv4cpio", new List<string>{"application/x-sv4cpio"}},
            {"sv4crc", new List<string>{"application/x-sv4crc"}},
            {"swf", new List<string>{"application/x-shockwave-flash"}},
            {"t", new List<string>{"application/x-troff"}},
            {"tar", new List<string>{"application/x-tar"}},
            {"tcl", new List<string>{"application/x-tcl"}},
            {"tex", new List<string>{"application/x-tex"}},
            {"texi", new List<string>{"application/x-texinfo"}},
            {"texinfo", new List<string>{"application/x-texinfo"}},
            {"tgz", new List<string>{"application/x-compressed"}},
            {"tif", new List<string>{"image/tiff"}},
            {"tiff", new List<string>{"image/tiff"}},
            {"tr", new List<string>{"application/x-troff"}},
            {"trm", new List<string>{"application/x-msterminal"}},
            {"tsv", new List<string>{"text/tab-separated-values"}},
            {"txt", new List<string>{"text/plain"}},
            {"uls", new List<string>{"text/iuls"}},
            {"ustar", new List<string>{"application/x-ustar"}},
            {"vcf", new List<string>{"text/x-vcard"}},
            {"vrml", new List<string>{"x-world/x-vrml"}},
            {"wav", new List<string>{"audio/x-wav"}},
            {"wcm", new List<string>{"application/vnd.ms-works"}},
            {"wdb", new List<string>{"application/vnd.ms-works"}},
            {"wks", new List<string>{"application/vnd.ms-works"}},
            {"wmf", new List<string>{"application/x-msmetafile"}},
            {"wps", new List<string>{"application/vnd.ms-works"}},
            {"wri", new List<string>{"application/x-mswrite"}},
            {"wrl", new List<string>{"x-world/x-vrml"}},
            {"wrz", new List<string>{"x-world/x-vrml"}},
            {"xaf", new List<string>{"x-world/x-vrml"}},
            {"xbm", new List<string>{"image/x-xbitmap"}},
            {"xla", new List<string>{"application/vnd.ms-excel"}},
            {"xlc", new List<string>{"application/vnd.ms-excel"}},
            {"xlm", new List<string>{"application/vnd.ms-excel"}},
            {"xls", new List<string>{"application/vnd.ms-excel"}},
            {"xlt", new List<string>{"application/vnd.ms-excel"}},
            {"xlw", new List<string>{"application/vnd.ms-excel"}},
            {"xof", new List<string>{"x-world/x-vrml"}},
            {"xpm", new List<string>{"image/x-xpixmap"}},
            {"xwd", new List<string>{"image/x-xwindowdump"}},
            {"xml", new List<string>{"application/xml"}},
            {"z", new List<string>{"application/x-compress"}},
            {"zip", new List<string>{"application/zip"}},
        };

        internal CombineArchive Archive { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format { get; set; }

        /// <summary>
        /// Resolves the local file name of this entry
        /// </summary>
        /// <param name="baseDir">The base dir, used to override the archive base dir.</param>
        /// <returns>a local filename for the entry or 'null'</returns>
        public string GetLocalFileName(string baseDir = null)
        {
            if (Location.Contains("http://"))
                return null;

            string location = Location.Replace("./", "");
            if (File.Exists(location))
                return location;

            if (baseDir != null && File.Exists(Path.Combine(baseDir, location)))
                return Path.Combine(baseDir, location);

            if (Archive != null && Directory.Exists(Archive.BaseDir) && File.Exists(Path.Combine(Archive.BaseDir, location)))
                return Path.Combine(Archive.BaseDir, location);

            return null;

        }

        /// <summary>
        /// Opens the Entry with the program associated with it.
        /// </summary>
        /// <param name="baseDir">The base dir, used to override the archive base dir.</param>
        public void OpenLocation(string baseDir = null)
        {
            try
            {
                if (Location.Contains("http://"))
                {
                    System.Diagnostics.Process.Start(Location);
                }

                var localFileName = GetLocalFileName(baseDir);
                if (localFileName != null)
                    System.Diagnostics.Process.Start(localFileName);
            }
            catch
            {
                
            }
        }

        public OmexDescription Description
        {
            get
            {

                if (Archive == null || Archive.Descriptions == null)
                    return null;
                return Archive.Descriptions.Where(e => e.About.Replace("./", "") == Location.Replace("./", "")).FirstOrDefault();

            }
        }


        /// <summary>
        /// Returns the contents of the entry as byte array
        /// </summary>
        /// <param name="baseDir">The base dir, used to override the archive base dir.</param>
        /// <returns>the contents of the entry as byte array</returns>
        public byte[] GetBytes(string baseDir = null)
        {
            if (Location.Contains("http://"))
                return Util.GetBytesForUrl(Location);

            var fileName = GetLocalFileName(baseDir);

            if (fileName == null)
                return null;

            return File.ReadAllBytes(fileName);


        }

        /// <summary>
        /// Gets the contents of the entry as string.
        /// </summary>
        /// <param name="baseDir">The base dir, used to override the archive base dir.</param>
        /// <returns>the contents of the entry as string</returns>
        public string GetContents(string baseDir = null)
        {
            if (Location.Contains("http://"))
                return Util.GetStringForUrl(Location);

            var fileName = GetLocalFileName(baseDir);

            if (fileName == null)
                return null;

            return File.ReadAllText(fileName);

        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} : {1}", Location, Format);
        }
    }
}