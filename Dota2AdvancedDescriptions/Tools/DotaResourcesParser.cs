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

        public DotaResourcesParser(string filePath)
        {
            _filePath = filePath;
        }

        public void ParseResources()
        {
            ParsedResources = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            HeroNameResToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = File.ReadAllLines(_filePath);
            Regex regex = new Regex("\"(.*?)\"", RegexOptions.IgnoreCase);
            KeyValuePair<string, Dictionary<string, string>> heroSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>();
            foreach (var line in lines)
            {
                //Joining hero name with display name
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
                //Group by hero
                if (line.Contains(Settings.Default.AbilityResourcePrefix))
                {
                    string heroName = HeroNameResToName.FirstOrDefault(x => line.Replace(Settings.Default.AbilityResourcePrefix, "").Trim().Contains(x.Key)).Key;
                    if(heroName != null)
                    {
                        if (!ParsedResources.ContainsKey(HeroNameResToName[heroName]))
                        {
                            heroSpecificResourceContainer = new KeyValuePair<string, Dictionary<string, string>>(HeroNameResToName[heroName], new Dictionary<string, string>());
                            ParsedResources.Add(heroSpecificResourceContainer.Key, heroSpecificResourceContainer.Value);
                        }
                    } else
                    {

                    }
                }
                //Adding matches
                var matches = new List<object>(regex.Matches(line).Cast<object>());
                if (matches.Count == 2 && CultureInfo.CurrentCulture.CompareInfo.IndexOf(matches[0].ToString(), Settings.Default.DotaResourceMatch, CompareOptions.IgnoreCase) >= 0)
                {
                    if (heroSpecificResourceContainer.Value != null)
                    {
                        heroSpecificResourceContainer.Value.Add(matches[0].ToString().Replace("\"", ""), matches[1].ToString().Replace("\"", ""));
                    }
                }
            }
        }

    }
}
