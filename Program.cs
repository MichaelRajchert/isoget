using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Net;

namespace isoget
{
    class Program
    {
        public static bool debug = false;
        public static String locationDir = "C:\\Users\\Michael Rajchert\\source\\repos\\isoget\\isoget\\locations.xml",
            downloadDir = "C:";
        static void Main(string[] args)
        {
            Console.WriteLine();
            if (args.Contains("-d") || args.Contains("-D"))
            {
                debug = true;
                Console.WriteLine("Debug is Enabled \n" +
                    "   REQ: Program Request \n" +
                    "   RES: Program Resolution/Returned \n" +
                    "   ACT: Action \n");
            }

            /* Possible Commands
             *    -get
             *    -add
             *    -list
             *    -query
             */

            if (args.Contains("-get"))
            {
                int argIndex = Array.IndexOf(args, "-get"),
                    argCondition = argIndex + 1;
                try
                {
                    String URL = getXMLData(args[argCondition].ToLower(), "location");
                    String filename = args[argCondition];
                    String filetype = getXMLData(args[argCondition].ToLower(), "type");

                    download(URL, filename, filetype);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            if (args.Contains("-add"))
            {
                int argIndex = Array.IndexOf(args, "-add"),
                    argCondition = argIndex + 1;
                try
                {
                    Console.WriteLine("add");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            if (args.Contains("-list"))
            {
                try
                {
                    listXMLData();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            if (args.Contains("-query"))
            {
                try
                {
                    String rN = args[1];
                    String rT = args[2];
                    Console.WriteLine(getXMLData(rN, rT));
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            if (args.Contains("-help"))
            {
                if (debug == true)
                {
                    Console.WriteLine("DEBUG-ACT: Running -help");
                }
                try
                {
                    int helpArgIndex = Array.IndexOf(args, "-help")+1;
                    switch (args[helpArgIndex])
                    {
                        case "get":
                            Console.WriteLine("USAGE:\n" +
                                ">isoget -get [imageName]         ... Downloads the disk image based off a given name using\n" +
                                "                                     data that exists in the local database.\n\n" +
                                "Additonal Info:\n" +
                                "   This command uses the System Webclient, therefore, an internet connection is required to\n" +
                                "   execute this command. If you have issues, ensuring that there is a valid internet\n" +
                                "   connection is generally a good first step.\n" +
                                "   If you are unsure on what disk images you can download, run >isoget -list.");
                            break;
                        case "add":
                            Console.WriteLine("USAGE:\n" +
                                ">isoget -add [imageName][URL][fileType]    ... Adds a new entry to the local database.\n\n" +
                                "Additional Info:\n" +
                                "   Please note, this ONLY updates the local database.\n" +
                                "   All parts of the command must be fulfiled and valid otherwise when\n" +
                                "   calling the new entry an error can occur. Please pay close attention\n" +
                                "   to the URL ensuring this it is a download URL and not a webpage.");
                            break;
                        case "list":
                            Console.WriteLine("USEAGE\n" +
                                ">isoget -list          ... Lists existing info in the local database.");
                            break;
                        case "query":
                            Console.WriteLine("USAGE:\n" +
                                ">isoget -query [imageName][requestType]    ... Returns a value from the database based off a given disk image name.\n\n" +
                                "Valid Request Types: name, location, type\n\n" +
                                "Additional Info:\n" +
                                "   imageName must be an existing item in the database. If you are unsure what exists\n" +
                                "   in the database, use >isoget -list");
                            break;
                        case "d":
                            Console.WriteLine("USAGE:\n" +
                                ">isoget -d                 ... displays debug information\n\n" +
                    "   REQ: Program Request \n" +
                    "   RES: Program Resolution/Returned \n" +
                    "   ACT: Action \n");
                            break;
                        default:
                            defaultHelp();
                            break;
                    }
                }
                catch (Exception)
                {
                    defaultHelp();
                }
            }
        }
        static void defaultHelp()
        {
            Console.WriteLine("isoget by Michael Rajchert \n\n" +
                    "USAGE: \n\n" +
                    "   isoget [-get [imageName] |\n" +
                    "           -add [imageName][URL][fileType] |\n" +
                    "           -list |\n" +
                    "           -help [command]| \n" +
                    "           -query [imageName][requestType] ] \n" +
                    "           -d\n\n" +
                    "WHERE: \n" +
                    "   -get    Downloads a new Disk Image from URL using given name \n" +
                    "   -add    Adds new Disk Image information into the database \n" +
                    "   -list   Lists existing information in the database \n" +
                    "   -query  Returns information requested based off a given Disk Image Name \n" +
                    "   -d      Debug\n" +
                    "   -help   Lists this information again, or additional information based on a command\n\n" +
                    "Examples:\n" +
                    "   >isoget -get ubuntu                             ... Downloads ubuntu disk image\n" +
                    "   >isoget -help list                              ... Gives more info on the list argument\n" +
                    "   >isoget -add ubuntu downloadCDN.ubuntu.com .iso ... Adds ubuntu with a location to database\n" +
                    "   >isoget -query ubuntu location                  ... Gets ubuntu's download URL");
        }
        static void listXMLData()
        {
            if (debug == true)
            {
                Console.WriteLine("DEBUG-ACT: Listing existing XML data.");
            }
            double consoleWidth = Console.WindowWidth*0.80,
                col0Width = Math.Round(consoleWidth*0.15),
                col1Width = Math.Round(consoleWidth * 0.75),
                col2Width = Math.Round(consoleWidth * 0.10);
            String stringFormatArgument = "| {0,-" + col0Width + "} | {1,-" + col1Width + "} | {2,-" + col2Width + "} |";

            XmlReader xmlr = XmlReader.Create(locationDir);
            Console.WriteLine("+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col0Width+2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col1Width+2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col2Width+2))) + "+");
            Console.WriteLine(String.Format(stringFormatArgument,"Name","Location (URL)","Type"));
            Console.WriteLine("+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col0Width+2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col1Width+2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col2Width+2))) + "+");
            while (xmlr.Read())
            {
                if((xmlr.NodeType == XmlNodeType.Element) && (xmlr.Name == "iso") && (xmlr.HasAttributes))
                {
                    String nodeName = Truncate(xmlr.GetAttribute("name"),Convert.ToInt32(col0Width)), 
                        nodeLocation = Truncate(xmlr.GetAttribute("location"),Convert.ToInt32(col1Width)),
                        nodeType = Truncate(xmlr.GetAttribute("type"),Convert.ToInt32(col2Width));

                    String line = String.Format(stringFormatArgument,nodeName,nodeLocation,nodeType);
                    Console.WriteLine(line);
                    
                }
            }
            Console.WriteLine("+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col0Width + 2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col1Width + 2))) + "+" + String.Concat(Enumerable.Repeat("-", Convert.ToInt32(col2Width + 2))) + "+");
        }
        static String getXMLData(String requestName, String requestType) //requestType can be name, location, type
        {
            if(debug == true)
            {
                Console.WriteLine("DEBUG-REQ: Get " + requestType + " of " + requestName + " using " + locationDir);
            }
            try
            {
                XmlReader xmlr = XmlReader.Create(locationDir);
                while (xmlr.Read())
                {
                    if((xmlr.NodeType == XmlNodeType.Element) && (xmlr.Name == "iso"))
                    {
                        if ((xmlr.HasAttributes) && (xmlr.GetAttribute("name") == requestName))
                        {
                            if(debug == true)
                            {
                                Console.WriteLine("DEBUG-RES: Request for " + requestName + " " + requestType + " returned " + xmlr.GetAttribute("location"));
                            }
                            return xmlr.GetAttribute(requestType);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);

            }
            return null;
        }
        static void download(String URL, String filename, String filetype)
        {
            WebClient webClient = new WebClient();
            try
            {
                if (debug == true)
                {
                    Console.WriteLine("DEBUG-ACT: Downloading " + filename + " to " + downloadDir);
                } else
                {
                    Console.WriteLine(filename + " is downloading.");
                }
                webClient.DownloadFile(URL,downloadDir + filename + filetype);
                Console.WriteLine("Download completed.");
            } catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                Console.WriteLine("ERROR: " + e.InnerException);
            }
        }
        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length-3) + "...";
            }
            return source;
        }
    }
}
