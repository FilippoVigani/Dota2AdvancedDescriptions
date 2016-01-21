using Dota2AdvancedDescriptions.Properties;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions.Tools
{
    public class DotaHtmlParser
    {
        public DotaHtmlParser()
        {
        }

        public Dictionary<string, Dictionary<string, string>> ParsedData;

        public void ParseAbilitiesCastPoints(string address, string xpath, int tableIndex)
        {
            WebClient webClient = new WebClient();
            string page = webClient.DownloadString(address);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes(xpath);
            HtmlNode node = nodes.ElementAt(tableIndex);

            ParsedData = new Dictionary<string, Dictionary<string, string>>();

            foreach (var row in node.Descendants(Settings.Default.Tr).Skip(1).Where(tr => tr.Elements(Settings.Default.Td).Count() > 1))
            {
                Dictionary<string, string> parsedRow = new Dictionary<string, string>();
                for (int i = 0; i < row.Elements(Settings.Default.Td).Count(); i++)
                {
                    parsedRow.Add(node.Descendants(Settings.Default.Tr).ElementAt(0).Elements(Settings.Default.Th).ElementAt(i).InnerText.Trim(), row.Elements(Settings.Default.Td).ElementAt(i).InnerText.Trim());
                }
                ParsedData.Add(parsedRow.ElementAt(Settings.Default.TableIdIndex).Value, parsedRow);
            }
        }
    }
}
