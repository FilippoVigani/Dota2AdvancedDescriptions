using Dota2AdvancedDescriptions.Helpers;
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
        public Dictionary<string, Dictionary<string, string>> ParsedResources;
        private Dictionary<string, string> UnitNameResToName;
        private string _filePath;

        public DotaResourcesParser()
        {
        }

        public void ParseResources(string filePath)
        {
            _filePath = filePath;
            StatusBarHelper.Instance.SetStatus("Parsing resources from local file...");
            if (!File.Exists(_filePath))
            {
                MessageBox.Show("Resources file not found at: " + filePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ParsedResources = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            UnitNameResToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = File.ReadAllLines(_filePath);
            Regex regex = new Regex("\"(.*?)\"", RegexOptions.IgnoreCase);
            KeyValuePair<string, Dictionary<string, string>> unitSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>();
            //Joining hero name with display name (different file)
            bool heroNamesCreatedFromDifferentFile = Path.Combine(Path.GetDirectoryName(_filePath), Settings.Default.DefaultResourcesFileName) != _filePath;
            var unitLines = heroNamesCreatedFromDifferentFile ? File.ReadAllLines(Path.Combine(Path.GetDirectoryName(_filePath), Settings.Default.DefaultResourcesFileName)) : lines;
            //Joining hero name with display name
            foreach (var line in unitLines)
            {
                if (line.Contains(Settings.Default.HeroNamePrefix) && !line.Contains(Settings.Default.ExcludeFromHeroesNames) || line.Contains(Settings.Default.NeutralNamePrefix))
                {
                    var names = new List<object>(regex.Matches(line).Cast<object>());
                    if (names.Count == 2)
                    {
                        var unitNameRes = names[0].ToString().Trim().Replace("\"", "");
                        var unitName = names[1].ToString().Trim().Replace("\"", "");
                        if (unitNameRes == "npc_dota_hero_sand_king") //Sandking hard fix: volvo pls...
                            unitNameRes = "npc_dota_hero_sandking";
                        UnitNameResToName.Add(unitNameRes, unitName);
                    }
                }
            }
            foreach (var line in lines)
            {
                var matches = new List<string>(regex.Matches(line).Cast<object>().Select(s => s.ToString()));
                if (matches.Count > 2)
                {
                    string keyString = matches[0];
                    string matchWithQuotationMarks = line.Replace(keyString, "").Trim();
                    matches = new List<string>();
                    matches.Add(keyString);
                    matches.Add(matchWithQuotationMarks);
                }
                matches = matches.Select(s => s.Length >= 2 ? s.Substring(1, s.Length - 2) : s).ToList();

                //Group by unit
                if (line.IndexOf(Settings.Default.AbilityResourcePrefix, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var units = UnitNameResToName.Where(x => matches[0].Replace(Settings.Default.AbilityResourcePrefix, "").Trim().StartsWith(x.Key.Replace(Settings.Default.HeroNamePrefix, "").Replace(Settings.Default.NeutralNamePrefix, "")));
                    var unit = units.OrderByDescending(x => x.Value.Length).OrderByDescending(x => x.Key.Replace(Settings.Default.HeroNamePrefix, "").Replace(Settings.Default.NeutralNamePrefix, "").Length).FirstOrDefault();
                    string unitKey = unit.Key;
                    if (unitKey != null)
                    {
                        if (!ParsedResources.ContainsKey(UnitNameResToName[unitKey]))
                        {
                            unitSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>(UnitNameResToName[unitKey], new Dictionary<string, string>());
                            ParsedResources.Add(unitSpecificResourceContainer.Key, unitSpecificResourceContainer.Value);
                            StatusBarHelper.Instance.SetStatus("Parsing resources from local file: " + unitSpecificResourceContainer.Key);
                        } else
                        {
                            unitSpecificResourceContainer = ParsedResources.FirstOrDefault(x => x.Key == UnitNameResToName[unitKey]);
                        }
                    }
                }
                //Adding matches

                if (matches.Count == 2 && CultureInfo.CurrentCulture.CompareInfo.IndexOf(matches[0].ToString(), Settings.Default.DotaResourceMatch, CompareOptions.IgnoreCase) >= 0)
                {
                    if (unitSpecificResourceContainer.Value != null)
                    {
                        unitSpecificResourceContainer.Value.Add(matches[0], matches[1]);
                    }
                }
            }
            StatusBarHelper.Instance.SetStatus("Resources parsing completed.");
        }

    }
}
