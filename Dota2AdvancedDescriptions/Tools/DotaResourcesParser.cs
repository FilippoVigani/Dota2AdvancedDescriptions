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
        private Dictionary<string, string> HeroNameResToName;
        private string _filePath;

        public DotaResourcesParser()
        {
        }

        public void ParseResources(string filePath)
        {
            _filePath = filePath;
            StatusBarHelper.Instance.SetStatus("Parsing resources from local file...");
            ParsedResources = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            HeroNameResToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = File.ReadAllLines(_filePath);
            Regex regex = new Regex("\"(.*?)\"", RegexOptions.IgnoreCase);
            KeyValuePair<string, Dictionary<string, string>> heroSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>();
            //Joining hero name with display name (different file)
            bool heroNamesCreatedFromDifferentFile = Path.Combine(Path.GetDirectoryName(_filePath), Settings.Default.DefaultResourcesFileName) != _filePath;
            if (heroNamesCreatedFromDifferentFile)
            {
                var sourceLines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(_filePath), Settings.Default.DefaultResourcesFileName));
                foreach (var line in sourceLines)
                {
                    if (line.Contains(Settings.Default.HeroNamePrefix) && !line.Contains(Settings.Default.ExcludeFromHeroesNames))
                    {
                        var names = new List<object>(regex.Matches(line).Cast<object>());
                        if (names.Count == 2)
                        {
                            var heroNameRes = names[0].ToString().Trim().Replace(Settings.Default.HeroNamePrefix, "").Replace("\"", "");
                            var heroName = names[1].ToString().Trim().Replace("\"", "");
                            HeroNameResToName.Add(heroNameRes, heroName);
                        }
                    }
                }
            }
            foreach (var line in lines)
            {
                //Joining hero name with display name (same file)
                if (!heroNamesCreatedFromDifferentFile)
                {
                    if (line.Contains(Settings.Default.HeroNamePrefix) && !line.Contains(Settings.Default.ExcludeFromHeroesNames))
                    {
                        var names = new List<object>(regex.Matches(line).Cast<object>());
                        if (names.Count == 2)
                        {
                            var heroNameRes = names[0].ToString().Trim().Replace(Settings.Default.HeroNamePrefix, "").Replace("\"", "");
                            var heroName = names[1].ToString().Trim().Replace("\"", "");
                            HeroNameResToName.Add(heroNameRes, heroName);
                        }
                    }
                }

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

                //Group by hero
                if (line.Contains(Settings.Default.AbilityResourcePrefix))
                {
                    var hero = HeroNameResToName.FirstOrDefault(x => matches[0].Replace(Settings.Default.AbilityResourcePrefix, "").Trim().StartsWith(x.Key));
                    string heroKey = hero.Key;
                    if (heroKey != null)
                    {
                        if (!ParsedResources.ContainsKey(HeroNameResToName[heroKey]))
                        {
                            heroSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>(HeroNameResToName[heroKey], new Dictionary<string, string>());
                            ParsedResources.Add(heroSpecificResourceContainer.Key, heroSpecificResourceContainer.Value);
                            StatusBarHelper.Instance.SetStatus("Parsing resources from local file: " + heroSpecificResourceContainer.Key);
                        }
                    }
                    else
                    {

                    }
                }
                //Adding matches

                if (matches.Count == 2 && CultureInfo.CurrentCulture.CompareInfo.IndexOf(matches[0].ToString(), Settings.Default.DotaResourceMatch, CompareOptions.IgnoreCase) >= 0)
                {
                    if (heroSpecificResourceContainer.Value != null)
                    {
                        heroSpecificResourceContainer.Value.Add(matches[0], matches[1]);
                    }
                }
            }
            StatusBarHelper.Instance.SetStatus("Resources parsing completed.");
        }

    }
}
