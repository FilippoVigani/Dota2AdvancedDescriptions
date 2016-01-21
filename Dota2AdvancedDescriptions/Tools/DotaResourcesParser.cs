using Dota2AdvancedDescriptions.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Dota2AdvancedDescriptions.Tools
{
    public class DotaResourcesParser
    {
        public Dictionary<string, string> ParsedResources;
        private string _filePath;

        public DotaResourcesParser(string filePath)
        {
            _filePath = filePath;
        }

        public void ParseResources()
        {
            ParsedResources = new Dictionary<string, string>();
            var lines = File.ReadAllLines(_filePath);
            Regex regex = new Regex("\"(.*?)\"", RegexOptions.IgnoreCase); 
            foreach (var line in lines)
            {
                var matches = new List<object>(regex.Matches(line).Cast<object>());
                if (matches.Count == 2 && CultureInfo.CurrentCulture.CompareInfo.IndexOf(matches[0].ToString(), Settings.Default.DotaResourceMatch, CompareOptions.IgnoreCase) >= 0)
                {
                    ParsedResources.Add(matches[0].ToString().Replace("\"",""), matches[1].ToString().Replace("\"", ""));
                }
            }
        }

    }
}
