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
            if (args.Contains("-d") || args.Contains("-D"))
            {
                debug = true;
                Console.WriteLine("Debug is Enabled \n" +
                    "   REQ: Program Request \n" +
                    "   RES: Program Resolution/Returned \n" +
                    "   ACT: Action \n");
            }

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
