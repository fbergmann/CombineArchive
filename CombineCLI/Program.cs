using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibCombine;

namespace CombineCLI
{
    class Program
    {
        static void Main(string[] args)
        {
           
             var archive = 
                 CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\isola_oscillations_sed.sed.omex");

             Console.WriteLine("Num SEDML files: {0}", archive.GetNumFilesWithFormat("SEDML"));
             Console.WriteLine("Num SBML files: {0}", archive.GetNumFilesWithFormat("sbml"));
             Console.WriteLine("Num SBML files: {0}", archive.GetNumFilesWithFormat("sbml"));

             archive.SaveTo(@"C:\Users\fbergmann\Desktop\isola.omex");


             var newArchive = new CombineArchive
             {
                 BaseDir = @"C:\Users\fbergmann\Documents\SBML Models\",
                 Descriptions = new List<OmexDescription> { 
                     new OmexDescription
                {
                    About = "./BorisEJB.xml",
                    Description = "original JDesigner model for Kholodenko2000 - MAPK feedback",
                    Creators =
                        new List<VCard> 
                        { 
                            new VCard 
                            { 
                                FamilyName = "Bergmann", 
                                GivenName = "Frank", 
                                Email = "fbergman@caltech.edu", 
                                Organization = "California Institute of Technology" 
                            } 
                        },
                    Created = DateTime.Parse("2013-04-04 16:00+1")
                }
                 },
                 Entries = new List<Entry> { 
                     new Entry { Location = "./BorisEJB.xml", Format = Entry.KnownFormats["sbml"] }, 
                     new Entry { Location = "./paper/Kholodenko2000.pdf", Format = Entry.KnownFormats["pdf"] }, 
                     new Entry { Location = "http://www.ebi.ac.uk/biomodels-main/BIOMD0000000010", Format = Entry.KnownFormats["sbml"] }, 
                 }
             };

             newArchive.SaveTo(@"C:\Users\fbergmann\Desktop\Boris.omex");

             var omex = CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\Boris.omex");

             if (omex.HasEntriesWithFormat("pdf"))
                 omex.GetEntriesWithFormat("PDF").First().OpenLocation();


            Console.ReadKey();
        }
    }
}

