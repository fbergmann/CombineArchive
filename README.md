# COMBINE Archive
In this project I keep a prototype library and a couple of applications that make working with the archive easier. This project contains of three parts: `LibCombine`, a library reading the format, `CombineCLI` a command line application and `FormsCombineArchive` that represents the information in a UI. At this point mostly the library is in use. 

## The UI
The `CombineArchive` execuable is a windows application for creating / reading / modifying COMBINE archives. It associates with all files of type `*.omex` during installation. 

![Frontend of CombineArchive](https://raw.github.com/fbergmann/CombineArchive/master/FormsCombineArchive/2013-04-10_-_Screenshot.png)

Once the archive is opened, the files are displayed on the left side. When selected, their contents is displayed on the right. A **double click** on the file opens it. A **right click** displays a context menu from where the file or its contents can be copied to the clipboard. If an SBML file is selected, and SBW is available on the machine, the file can be sent to any SBW capable application, such as COPASI, JDesigner, RoadRunner to name but a few.  
### Download
You can download the latest Windows binary from: 

<http://sourceforge.net/projects/sbw/files/modules/CombineArchive/>

## Command Line version
With `CombineCLI`, a basic command line interface is included. At this point in time it is rather basic, and only lists the contents, or displays meta information associated with a document. For example:


	CombineCLI -o <path to omex file>

will list the contents of the archive.

	CombineCLI -o <path to omex file> -v -l <location>

will display associated with the given location of the archive. Alternatively 

	CombineCLI -o <path to omex file> -v -f <format>

can be used to view the information with the first file of the given format. 

## Library
The main class of the library is `CombineArchive`, you can construct new classes, and add local documents to it before saving it, or load existing ones, and browse through the contents. If you want to get hold of the manifest document, call the `.ToXML()` method.


### How to use
Simply reference `LibCombine.dll` in your project, ensure that the file `ICSharpCode.SharpZipLib.dll` is also available, and you are ready to call the library. 

### OmexDescription
Apart from that the library features an `OmexDescription` description class, that can be used by those that don't have a proper RDF library handy, with it you could create the description for an element like so: 

 			var desc =
                new OmexDescription
                {
                    About = "./test.xml",
                    Description = "Some Description about the file",
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
                };

To write that to an XML string, use the `ToXML` method. Conversely to parse from file or string, use the functions: 

		var list = OmexDescription.ParseString(xmlString);

or: 

		var list = OmexDescription.ParseFile(fileName);


### Creating a New Archive
The following is an example on how I would create a new `omex` file, by bundling a local SBML file and accompabying paper together with a reference to the [BioModel](http://biomodels.net/biomodel). First I define a `BaseDirectory` that serves as a root for my archive later on, and then I add a number of files: 

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

### Finding out what is in a COMBINE Archive
Suppose I would get a COMBINE archive, how would I find out about its contents? First you load the file: 

	var omex = CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\Boris.omex");

And then you would interrogate it by asking for its `Entries` property. Usually it will be the case that your application will only handle some of the COMBINE standards. In that case you want to use the methods: 

	// get the number of SBML files in the archive
	omex.GetNumFilesWithFormat("sbml") 

    // get the local filenames to them 
    omex.GetFilesWithFormat("sbml")

    // get the actual entries
    omex.GetEntriesWithFormat("sbml")

Since there can be any number of files in the archive, I use the convention, that the first `OmexDescription` in the metadata file represents the `MainFile` the name of which you can get through the according property.

Using the `Entry` objects directly has the advantage, that you can *open* the files, in whatever application is associated with it, or read the contents either in form of `byte[]` or strings. Just as example, to open the PDF file encoded in the file above one could use these lines: 

             var omex = CombineArchive.FromFile(@"C:\Users\fbergmann\Desktop\Boris.omex");

             if (omex.HasEntriesWithFormat("pdf"))
                 omex.GetEntriesWithFormat("PDF").First().OpenLocation();

Here `GetEntriesWithFormat` returns a list of all PDFs, `First()` gives us the first entry, and `OpenLocation` opens the file. 


## The Format
Frequently we are faced with project spanning more than just one file. Be it experimental data, that should be stored together with a publication describing the experiment, or a computational model together with diagrams and metadata describing it in detail. [SED-ML](http://sed-ml.org), one of the [COMBINE Standards](http://co.mbine.org), started to develop a basic archive format that allowed storing a Simulation Experiment Description along with all computational models referenced. The [COMBINE archive](http://co.mbine.org/documents/archive) aims to broaden the scope. It still is a [ZIP Archive](http://en.wikipedia.org/wiki/Zip_(file_format)), but also features a manifest called `manifest.xml` that describes the contents of the archive. 

    <?xml version="1.0" encoding="utf-8"?>
    <omexManifest xmlns="http://identifiers.org/combine.specifications/omex-manifest">
      <content location="./manifest.xml" format="http://identifiers.org/combine.specifications/omex-manifest"/>
      <content location="./model/model.xml" format="http://identifiers.org/combine.specifications/sbml"/>
      <content location="./simulation.xml" format="http://identifiers.org/combine.specifications/sedml"/>
      <content location="./article.pdf" format="application/pdf"/>
      <content location="./metadata.rdf" format="http://identifiers.org/combine.specifications/omex-metadata"/>
    </omexManifest>
    
## Third party libraries used
This project uses the following third party libraries: 

- [#ZipLib](http://www.icsharpcode.net/opensource/sharpziplib/)

## License 
This project is open source and freely available under the [Simplified BSD](http://opensource.org/licenses/BSD-2-Clause) license. Should that license not meet your needs, please contact me. 

Copyright (c) 2013, Frank T. Bergmann  
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.