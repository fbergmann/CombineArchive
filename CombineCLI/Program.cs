using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibCombine;

namespace CombineCLI
{
    class Program
    {
        private static void PrintEntries(CombineArchive archive)
        {
            Console.WriteLine("Archive:           {0}", Path.GetFileName(archive.ArchiveFileName));
            Console.WriteLine("Number of Entries: {0}", archive.Entries.Count);
            Console.WriteLine();
            var max = archive.Entries.Max(e=> e.Location.Length);
            foreach (Entry entry in archive.Entries)
            {
                var spaces = max - entry.Location.Length;
                Console.WriteLine("  {0}{1} : {2}", entry.Location, new string(' ', spaces),  entry.Format);
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var arguments = new Arguments(args);
            arguments.PrintHeader();
            if (!arguments.Valid)
                arguments.PrintUsageAndExit(1);

            var archive = arguments.CreateArchive ? new CombineArchive()
                : CombineArchive.FromFile(arguments.InputArchive);

            if (arguments.ListEntries)
            {
                PrintEntries(archive);
            }
            else if (arguments.DisplayMetaDataForEntry)
            {
                var entry =
                    arguments.HaveLocation
                     ? archive.Entries.FirstOrDefault(e => e.Location == arguments.Location)
                     : archive.Entries.FirstOrDefault(e => e.Format == arguments.Format);

                if (entry == null)
                {
                    Console.WriteLine("No such entry.");
                    Console.WriteLine();
                    return;
                }

                var desc = entry.Description;
                if (desc == null)
                {
                    Console.WriteLine("No description available.");
                    Console.WriteLine();
                    return;
                }

                Console.WriteLine(" location : {0}", entry.Location);
                Console.WriteLine(" format   : {0}", entry.Format);
                Console.WriteLine();
                Console.WriteLine(" description: {0}", desc.Description);
                Console.WriteLine();
                Console.WriteLine(" created: {0}", desc.Created);
                foreach (DateTime dateTime in desc.Modified)
                {
                    Console.WriteLine(" modified: {0}", dateTime);
                } 
                Console.WriteLine();
                foreach (VCard creator in desc.Creators)
                {
                    if (creator.Empty) continue;
                    Console.WriteLine("  first: " + creator.GivenName);
                    Console.WriteLine("  last : " + creator.FamilyName);
                    Console.WriteLine("  email: " + creator.Email);
                    Console.WriteLine("  org  : " + creator.Organization);
                    Console.WriteLine();
                }

                
            }

            //var archive = 
            //    CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\isola_oscillations_sed.sed.omex");

            //Console.WriteLine("Num SEDML files: {0}", archive.GetNumFilesWithFormat("SEDML"));
            //Console.WriteLine("Num SBML files: {0}", archive.GetNumFilesWithFormat("sbml"));
            //Console.WriteLine("Num SBML files: {0}", archive.GetNumFilesWithFormat("sbml"));

            //archive.SaveTo(@"C:\Users\fbergmann\Desktop\isola.omex");



            //var omex = CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\Boris.omex");

            //if (omex.HasEntriesWithFormat("pdf"))
            //    omex.GetEntriesWithFormat("PDF").First().OpenLocation();


            //Console.ReadKey();
        }
    }
}

