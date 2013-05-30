using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace LibCombine
{
    /// <summary>
    /// Utility class containing all functions for resolving files, and zipping / unzipping
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Gets the bytes for the given location URL.
        /// </summary>
        /// <param name="location">The location url.</param>
        /// <returns>byte array with the data</returns>
        public static byte[] GetBytesForUrl(string location)
        {
            var client = new WebClient();
            return client.DownloadData(location);
        }

        /// <summary>
        /// Gets the string for the location URL.
        /// </summary>
        /// <param name="location">The location url.</param>
        /// <returns>the contents of the url as string</returns>
        public static string GetStringForUrl(string location)
        {
            var client = new WebClient();
            return client.DownloadString(location);
        }

        /// <summary>
        /// Extension method serializing a list of omex descriptions to XML
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>xml document including all descriptions</returns>
        public static string ToXML(this List<OmexDescription> list)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version='1.0' encoding='utf-8' standalone='yes'?>");
            foreach (OmexDescription omexDescription in list)
            {
                builder.AppendLine(omexDescription.ToXML(true)); 	
            }
            return builder.ToString();
           
        }


        public const int CHUNK_SIZE = 2048;

        /// <summary>
        /// Formats the given XML document
        /// </summary>
        /// <param name="doc">The xml document to format</param>
        /// <param name="omitDeclaration">if set to <c>true</c> the xml declaration will be ommitted.</param>
        /// <returns>a string representing the document</returns>
        public static string PrettyPrint(XmlDocument doc, bool omitDeclaration = false)
        {
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = omitDeclaration,
                IndentChars = ("   "),
                NewLineHandling = NewLineHandling.Replace
            };
            var writer = XmlWriter.Create(builder, settings);
            doc.Save(writer);
            writer.Flush();
            writer.Close();
            return builder.ToString().Replace("utf-16", "UTF-8");
        }

        /// <summary>
        /// Formats the given xml content
        /// </summary>
        /// <param name="content">The content to format.</param>
        /// <param name="omitDeclaration">if set to <c>true</c> the xml declaration will be ommitted.</param>
        /// <returns>formatted xml </returns>
        public static string PrettyPrint(string content, bool omitDeclaration = false)
        {
            var doc = new XmlDocument();
            doc.LoadXml(content);
            return PrettyPrint(doc, omitDeclaration);
        }


        /// <summary>
        /// Returns the path of the unpacked archive (temp+filename)
        /// </summary>
        /// <param name="archiveFilename">name of the archive file</param>
        /// <param name="deleteIfExists"></param>
        /// <returns>base directory with all the unzipped files</returns>
        public static string UnzipArchive(string archiveFilename, bool deleteIfExists = true)
        {
            using (var inputStream = new FileStream(archiveFilename, FileMode.Open))
            {
                // zipped archive ...
                var tempDir = Path.GetTempPath();
                var stream = new ZipInputStream(inputStream);
                var destination = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(archiveFilename));
                if (Directory.Exists(destination) && deleteIfExists)
                    try
                    {
                        Directory.Delete(destination, true);
                    }
                    catch
                    {
                    }
                Directory.CreateDirectory(destination);
                ZipEntry entry;
                while ((entry = stream.GetNextEntry()) != null)
                {
                    var sName = Path.Combine(destination, ZipEntry.CleanName(entry.Name));
                    var dir = Path.GetDirectoryName(sName);
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(Path.Combine(destination, dir)))
                            Directory.CreateDirectory(Path.Combine(destination, dir));
                    }
                    catch 
                    {
                        
                    }
                    if (entry.IsDirectory) continue;
                    try
                    {
                        var streamWriter = File.Create(sName);
                        var data = new byte[CHUNK_SIZE];
                        while (true)
                        {
                            var size = stream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                    catch 
                    {
                        
                    }
                }
                return destination;
            }
        }


        private static void CreateDirs(ZipOutputStream zipStream, IEnumerable<string> files, string baseDir)
        {
            var dirs = new List<string>();
            foreach (string filename in files)
            {
                var fi = new FileInfo(filename);
                var dir = fi.DirectoryName;
                if (!string.IsNullOrEmpty(baseDir) && dir.StartsWith(baseDir))
                    dir = dir.Replace(baseDir, "");
                else dir = "";

                dir = dir.Trim();
                while (dir.StartsWith("" + Path.DirectorySeparatorChar))
                    dir = dir.Substring(1);

                if (!string.IsNullOrWhiteSpace(dir) && !dirs.Contains(dir))
                {
                    dirs.Add(dir);
                    int index;
                    while ((index = dir.LastIndexOf(Path.DirectorySeparatorChar)) != -1)
                    {
                        dir = dir.Substring(0, index);

                        if (!string.IsNullOrWhiteSpace(dir) && !dirs.Contains(dir))
                            dirs.Add(dir);
                    }
                }

            }

            dirs.Sort();
            var factory = new ZipEntryFactory();
            foreach (var item in dirs)
            {
                //System.Diagnostics.Debug.WriteLine("found " + item);
                zipStream.PutNextEntry(factory.MakeDirectoryEntry(ZipEntry.CleanName(item)));
            }

        }
        /// <summary>
        /// Zips up all the given files, as archive with the given name
        /// </summary>
        /// <param name="fileName">Name of the file to save to.</param>
        /// <param name="files">The files to zip.</param>
        /// <returns></returns>
        public static string CreateArchive(string fileName, IEnumerable<string> files, string baseDir = "")
        {
            var fsOut = File.Create(fileName);
            var zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(9);
            
            CreateDirs(zipStream, files, baseDir);


            foreach (string filename in files)
            {
                var fi = new FileInfo(filename);
                var dir = fi.DirectoryName;
                if (!string.IsNullOrEmpty(baseDir) && dir.StartsWith(baseDir))
                    dir = dir.Replace(baseDir, "");
                else dir = "";

                dir = dir.Trim();
                while (dir.StartsWith("" + Path.DirectorySeparatorChar))
                    dir = dir.Substring(1);

                string entryName = ((FileInfo)fi).Name;
                entryName = ZipEntry.CleanName(entryName);
                var newEntry = new ZipEntry(ZipEntry.CleanName(Path.Combine(dir, entryName)));
                ((ZipEntry)newEntry).DateTime = ((FileInfo)fi).LastWriteTime;
                ((ZipEntry)newEntry).Size = ((FileInfo)fi).Length;
                zipStream.PutNextEntry(((ZipEntry)newEntry));
                var buffer = new byte[4096];
                using (var streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            zipStream.IsStreamOwner = true;
            zipStream.Close();
            return fileName;
        }
    }
}