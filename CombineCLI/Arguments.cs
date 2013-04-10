using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineCLI
{
    public class Arguments
    {
        public bool HaveLocation
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Location);
            }
        }

        public bool HaveArchive
        {
            get
            {
                return !string.IsNullOrWhiteSpace(InputArchive) && File.Exists(InputArchive);
            }
        }

        public bool HaveDirectory
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Directory);
            }
        }

        public bool HaveFormat
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Format);
            }
        }
        public bool Valid
        {
            get
            {

                return
                    (ListEntries && HaveArchive) ||
                    (DisplayMetaDataForEntry && HaveArchive && (HaveFormat || HaveLocation)) ||
                    (AddEntry && (HaveArchive || CreateArchive) && (HaveFormat && HaveLocation)) ||
                    (RemoveEntry && HaveArchive && (HaveFormat || HaveLocation)) ||
                    (SetMaster && HaveArchive && (HaveFormat || HaveLocation)) ||
                    (UnsetMaster && HaveArchive && (HaveFormat || HaveLocation)) ||
                    (ExtractArchiveTo && HaveDirectory && HaveArchive)
                    ;
            }
        }

        public bool HaveOperation
        {
            get
            {
                return ListEntries 
                    | DisplayMetaDataForEntry 
                    | AddEntry 
                    | RemoveEntry 
                    | SetMaster 
                    | UnsetMaster 
                    | ExtractArchiveTo;
            }
        }

        public string InputArchive { get; set; }
        public string Location { get; set; }
        public string Format { get; set; }
        public string Directory { get; set; }

        public bool CreateArchive { get; set; }
        public bool ListEntries { get; set; }
        public bool DisplayMetaDataForEntry { get; set; }
        public bool AddEntry { get; set; }
        public bool RemoveEntry { get; set; }
        public bool SetMaster { get; set; }
        public bool UnsetMaster { get; set; }
        public bool ExtractArchiveTo { get; set; }

        public void PrintHeader()
        {
            Console.WriteLine("CombineCLI");
            Console.WriteLine("==========");
            Console.WriteLine("Utility program for handling COMBINE archives.");
            Console.WriteLine();            
            
        }
        public void PrintUsageAndExit(int exitCode)
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine();
            Console.WriteLine("    -o | --omex <archive>");
            Console.WriteLine("    -l | --location <location>");
            Console.WriteLine("    -f | --format <format>");
            Console.WriteLine("    -d | --dir <directory>");
            Console.WriteLine("    -c | --create  ");
            Console.WriteLine("    -a | --add");
            Console.WriteLine("    -r | --remove");
            Console.WriteLine("    -s | --set");
            Console.WriteLine("    -u | --unset");
            Console.WriteLine("    -v | --view");
            Console.WriteLine("    -x | --extract");
            Console.WriteLine();
            Environment.Exit(exitCode);
        }

        public void ParseArgs(string[] args)
        {
            if (args.Length == 0)
                return;
            for (int i = 0; i < args.Length; i++)
            {
                var current = args[i].ToLowerInvariant();
                var hasNext = i + 1 < args.Length;
                var next = hasNext ? args[i + 1] : null;

                if ((current == "-o" || current == "--omex") && hasNext)
                {
                    InputArchive = next;
                    i++;
                }
                else if ((current == "-f" || current == "--format") && hasNext)
                {
                    Format = next;
                    i++;
                }
                else if ((current == "-l" || current == "--location") && hasNext)
                {
                    Location = next;
                    i++;
                }
                else if ((current == "-d" || current == "--dir") && hasNext)
                {
                    Directory = next;
                    i++;
                }
                else if (current == "-c" || current == "--create") 
                {
                    CreateArchive = true;
                }
                else if (current == "-s" || current == "--set")
                {
                    SetMaster = true;
                }
                else if (current == "-u" || current == "--unset")
                {
                    UnsetMaster = true;
                }
                else if (current == "-v" || current == "--view")
                {
                    DisplayMetaDataForEntry= true;
                }
                else if (current == "-a" || current == "--add")
                {
                    AddEntry = true;
                }
                else if (current == "-r" || current == "--remove")                
                {
                    RemoveEntry = true;
                }
                else if (current == "list")
                {
                    ListEntries = true;                    
                }
                else if (current == "-x" || current == "--extract")
                {
                    ExtractArchiveTo = true;
                }
            }

            if (!HaveOperation)
            {
                ListEntries = true;
            }

        }

        public Arguments(string[] args)
        {
            ParseArgs(args);
        }
    }
}
