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
        public static bool debug = true;
        public static String locationDir = "C:\\Users\\Michael Rajchert\\source\\repos\\isoget\\isoget\\locations.xml",
            downloadDir = "C:";
        static void Main(string[] args)
        {
            if (debug == true)
            {
                Console.WriteLine("Debug is Enabled \n" +
                    "   REQ: Program Request \n" +
                    "   RES: Program Resolution/Returned \n" +
                    "   ACT: Action \n");
            }
            Console.WriteLine("isogetr");
            try {
                switch (args[0])
                {
                    case "get":
                        try
                        {
                            String URL = getXMLData(args[1].ToLower(), "location");
                            String filename = args[1];
                            String filetype = getXMLData(args[1].ToLower(), "type");

                            download(URL, filename, filetype);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("ERROR: Missing Arguments.");
                        }
                        break;
                    case "add":
                        try
                        {
                            Console.WriteLine("add");
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("ERROR: Missing Arguments.");
                        }
                        break;
                    case "list":
                        try
                        {
                            listXMLData();
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("ERROR: Missing Arguments.");
                        }
                        break;
                    case "query":
                        try
                        {
                            String rN = args[1];
                            String rT = args[2];
                            Console.WriteLine(getXMLData(rN, rT));
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("ERROR: Missing Arguments.");
                        }
                        break;
                    case "man":
                        try
                        {
                            Console.WriteLine("man");
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("ERROR: Missing Arguments.");
                        }
                        break;
                    default:
                        Console.WriteLine("help");
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("help");
            }
            Console.Read();
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
