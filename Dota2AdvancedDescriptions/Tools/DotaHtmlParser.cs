using Dota2AdvancedDescriptions.Helpers;
using Dota2AdvancedDescriptions.Properties;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dota2AdvancedDescriptions.Tools
{
    public class DotaHtmlParser
    {
        public DotaHtmlParser()
        {
            ParseCompleted = false;
        }

        public Dictionary<string, Dictionary<string, string>> ParsedData;
        public bool ParseCompleted { get; private set; }
        public bool ParseFailed { get; private set; }
        private bool working = false;

        public void ParseAbilitiesCastPoints(string address, string xpath, int tableIndex)
        {
            if (working) return;
            working = true;
            ParseFailed = false;
            try
            {
                StatusBarHelper.Instance.SetStatus("Downloading data from gamepedia...");
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                using (WebClient webClient = new WebClient()){
                    webClient.Proxy = null;
                    string page = webClient.DownloadString(address);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(page);
                    var nodes = doc.DocumentNode.SelectNodes(xpath);
                    if (nodes == null)
                    {
                        ParseFailed = true;
                        StatusBarHelper.Instance.SetStatus("Error while getting data");
                        var r = MessageBox.Show("Error while getting data from " + address + ".\nCheck your internet connection.\nThe application will be closed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (r == MessageBoxResult.OK)
                        {
                            Environment.Exit(-1);
                        }
                    }
                    //HtmlNode node = nodes.ElementAt(tableIndex);

                    ParsedData = new Dictionary<string, Dictionary<string, string>>();

                    foreach(HtmlNode node in nodes)
                    {
                        foreach (var row in node.Descendants(Settings.Default.Tr).Skip(1).Where(tr => tr.Elements(Settings.Default.Td).Count() > 1))
                        {
                            Dictionary<string, string> parsedRow = new Dictionary<string, string>();
                            for (int i = 0; i < row.Elements(Settings.Default.Td).Count(); i++)
                            {
                                string value = row.Elements(Settings.Default.Td).ElementAt(i).InnerText.Trim();
                                if (value.Contains(Settings.Default.HtmlParsingIgnoreModifier)) value = value.Substring(value.IndexOf(Settings.Default.TableAbilityHeroSeparator) + Settings.Default.TableAbilityHeroSeparator.Length).Trim();
                                parsedRow.Add(node.Descendants(Settings.Default.Tr).ElementAt(0).Elements(Settings.Default.Th).ElementAt(i).InnerText.Trim(), value);
                            }
                            //string header = parsedRow.ElementAt(Settings.Default.TableIdIndex).Value;
                            //if (header.Contains(Settings.Default.HtmlParsingIgnoreModifier)) header = header.Substring(header.IndexOf(Settings.Default.TableAbilityHeroSeparator) + Settings.Default.TableAbilityHeroSeparator.Length).Trim();
                            ParsedData.Add(parsedRow.ElementAt(Settings.Default.TableIdIndex).Value, parsedRow);
                            StatusBarHelper.Instance.SetStatus("Parsing data from html page: " + parsedRow.ElementAt(Settings.Default.TableIdIndex).Value);
                        }
                    }
                   
                    ParseCompleted = true;
                    StatusBarHelper.Instance.SetStatus("Data parsing from gamepedia completed.");
                }
            }
            catch (Exception e)
            {
                StatusBarHelper.Instance.SetStatus("Error while getting data");
                ParseFailed = true;
                var r = MessageBox.Show("Connection to the server " + address + "has failed:\n" + e.Message + "\nCheck the connection to the server or retry later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } finally
            {
                working = false;
            }
        } 
    }
}
