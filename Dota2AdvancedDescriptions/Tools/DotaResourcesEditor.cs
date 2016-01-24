using Dota2AdvancedDescriptions.Enums;
using Dota2AdvancedDescriptions.Helpers;
using Dota2AdvancedDescriptions.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dota2AdvancedDescriptions.Tools
{
    public class DotaResourcesEditor
    {
        public string TmpFilePath;
        public string AppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Process.GetCurrentProcess().ProcessName);

        public string GetBackupFile(string filePath)
        {
            return Path.Combine(AppData, Path.GetFileName(filePath));
        }

        public DotaResourcesEditor()
        {
        }

        public void PrepareResources(string filePath, Dictionary<string, Dictionary<string, string>> data, Dictionary<string, Dictionary<string, string>> parsedResources)
        {
            StatusBarHelper.Instance.SetStatus("Creating new resources file...");
            //TmpFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));
            //File.Copy(filePath, TmpFilePath, true);
            TmpFilePath = Path.GetTempFileName();
            /*
            StreamReader reader = new StreamReader(filePath);
            string input = reader.ReadToEnd();
            reader.Close();
            */
            var lines = File.ReadAllLines(filePath).ToList();
            using (StreamWriter writer = new StreamWriter(TmpFilePath, true, Utility.GetEncoding(filePath)))
            {
                {
                    //Insert credits, so I can detect if the file is customized or not later on
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("Language"))
                        {
                            lines[i] = lines[i].Insert(lines[i].Length, Resources.CreditsText);
                            break;
                        }
                    }

                    List<string> addedAbilitiesNames = new List<string>();

                    foreach (var abilityData in data)
                    {
                        string heroName = abilityData.Key.Substring(0, abilityData.Key.IndexOf(Settings.Default.TableAbilityHeroSeparator)).Trim();
                        string abilityName = abilityData.Key.Substring(abilityData.Key.IndexOf(Settings.Default.TableAbilityHeroSeparator) + Settings.Default.TableAbilityHeroSeparator.Length).Trim();
                        string modifier = "";
                        //Fix for spells such as brewmaster with double separator
                        if (abilityName.IndexOf(Settings.Default.TableAbilityHeroSeparator) >= 0)
                        {
                            modifier = abilityName.Substring(0, abilityName.IndexOf(Settings.Default.TableAbilityHeroSeparator)).Trim();
                            abilityName = abilityName.Substring(abilityName.IndexOf(Settings.Default.TableAbilityHeroSeparator) + Settings.Default.TableAbilityHeroSeparator.Length).Trim();
                        }

                        var heroResources = parsedResources.FirstOrDefault(res => heroName.Equals(res.Key, StringComparison.OrdinalIgnoreCase)).Value;
                        if (heroResources == null) continue;
                        var abilityKeys = heroResources.Where(res => abilityName.Equals(res.Value, StringComparison.OrdinalIgnoreCase));
                        var unlocalizedKeys = abilityKeys.Select(a => a.Key.Replace(Settings.Default.ResourcesEnglishModifier, ""));
                        abilityKeys = heroResources.Where(res => unlocalizedKeys.Contains(res.Key));

                        //Most of form-changing abilities have the same cast point, so it shouldn't be a problem
                        if (abilityKeys.Count() > 1)
                        {
                            Console.WriteLine("Warning: multiple data found on the same ability \"" + abilityName + "\": (" + string.Join(", ", abilityKeys.Select(x => x.Key).ToArray()) + "). There might be unattendable values. Values will be set to first match by default.");
                        }
                        if (abilityKeys.Count() == 0)
                        {
                            Console.WriteLine("Resource not found: " + abilityData.Key);
                            if (abilityName.IndexOf("(") >= 0)
                            {
                                abilityName = abilityName.Substring(0, abilityName.IndexOf("(")).Trim();
                                heroResources = parsedResources.FirstOrDefault(res => heroName.Equals(res.Key, StringComparison.OrdinalIgnoreCase)).Value;
                                if (heroResources == null) continue;
                                abilityKeys = heroResources.Where(res => abilityName.Equals(res.Value, StringComparison.OrdinalIgnoreCase));
                                unlocalizedKeys = abilityKeys.Select(a => a.Key.Replace(Settings.Default.ResourcesEnglishModifier, ""));
                                abilityKeys = heroResources.Where(res => unlocalizedKeys.Contains(res.Key));
                            }
                            if (abilityKeys.Count() == 0)
                            {
                                continue;
                            }
                        }
                        int abInsertCount = addedAbilitiesNames.Where(a => a == abilityName).Count();
                        if (string.IsNullOrEmpty(modifier) && abInsertCount >= abilityKeys.Count() && abilityData.Value.Count== 4) continue; //Prevent adding duplicates
                        //Perform editing
                        int abCount = abilityKeys.Count();
                        var abilityKey = !string.IsNullOrEmpty(modifier) && abilityKeys.Count() > 1 ? abilityKeys.ElementAt(1).Key : abilityKeys.ElementAt(abInsertCount < abilityKeys.Count() ? abInsertCount : 0).Key;
                        //foreach (var abilityKey in abilityKeys.Select(a => a.Key))
                        //{
                        var txtPos = (ExtraTextPosition)Settings.Default.ExtraTextPosition;
                        string extraText = GetExtraText(abilityData.Value.Values.ElementAt(1), abilityData.Value.Values.ElementAt(2), abilityData.Value.Values.ElementAt(3), abilityKey == abilityKeys.ElementAt(0).Key ? modifier: "");
                        if (txtPos == ExtraTextPosition.AboveDescription || txtPos == ExtraTextPosition.BelowDescription)
                        {
                            if (heroResources.ContainsKey(abilityKey + Settings.Default.DescriptionSuffix))
                            {
                                string desc = heroResources[abilityKey + Settings.Default.DescriptionSuffix];

                                if (txtPos == ExtraTextPosition.AboveDescription)
                                {
                                    int lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.DescriptionSuffix));
                                    lines[lineIndex] = lines[lineIndex].Insert(lines[lineIndex].IndexOf(desc), extraText);
                                }
                                else
                                {
                                    int lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.DescriptionSuffix));
                                    lines[lineIndex] = lines[lineIndex].Insert(lines[lineIndex].IndexOf(desc) + desc.Length, extraText);
                                }
                            }
                            else
                            {
                                //Some resources are missing on foreign languages
                                Console.WriteLine("Missing resource: " + abilityKey + Settings.Default.DescriptionSuffix);
                            }
                        }

                        else if (txtPos == ExtraTextPosition.BelowNotes)
                        {
                            bool noteInserted = false;
                            for (int i = 0; !noteInserted; i++)
                            {
                                if (!(heroResources.ContainsKey(abilityKey + Settings.Default.NoteSuffix + i)))
                                {
                                    string fullLine = String.Format("\"{0}\"\t\"{1}\"", abilityKey + Settings.Default.NoteSuffix + i, extraText);
                                    if (i > 0)
                                    {
                                        string note = heroResources[abilityKey + Settings.Default.NoteSuffix + (i - 1)];
                                        int lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.NoteSuffix + (i - 1)));
                                        lines.Insert(lineIndex + 1, fullLine);
                                        noteInserted = true;
                                    }
                                    else
                                    {
                                        int lineIndex;
                                        if (heroResources.ContainsKey(abilityKey + Settings.Default.LoreSuffix))
                                        {
                                            string lore = heroResources[abilityKey + Settings.Default.LoreSuffix];
                                            lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.LoreSuffix));
                                        }
                                        else if (heroResources.ContainsKey(abilityKey + Settings.Default.DescriptionSuffix))
                                        {
                                            string desc = heroResources[abilityKey + Settings.Default.DescriptionSuffix];
                                            lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.DescriptionSuffix));
                                        }
                                        else
                                        {
                                            string name = heroResources[abilityKey];
                                            lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey));
                                        }
                                        lineIndex = lineIndex + 1;
                                        lines.Insert(lineIndex, "\t\t" + fullLine);
                                        noteInserted = true;
                                    }
                                }
                            }
                        }
                        else if (txtPos == ExtraTextPosition.AboveNotes)
                        {
                            string fullLine = String.Format("\"{0}\"\t\"{1}\"", abilityKey + Settings.Default.NoteSuffix + 0, extraText);
                            int lineIndex;
                            if (!(heroResources.ContainsKey(abilityKey + Settings.Default.NoteSuffix + 0))) // new note
                            {
                                if (heroResources.ContainsKey(abilityKey + Settings.Default.LoreSuffix))
                                {
                                    string lore = heroResources[abilityKey + Settings.Default.LoreSuffix];
                                    lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.LoreSuffix));
                                }
                                else if (heroResources.ContainsKey(abilityKey + Settings.Default.DescriptionSuffix))
                                {
                                    string desc = heroResources[abilityKey + Settings.Default.DescriptionSuffix];
                                    lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.DescriptionSuffix));
                                }
                                else
                                {
                                    string name = heroResources[abilityKey];
                                    lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey));
                                }
                                lineIndex = lineIndex + 1;
                            }
                            else
                            {
                                lineIndex = lines.FirstIndexMatch(l => l.Contains(abilityKey + Settings.Default.NoteSuffix + 0));
                            }
                            lines.Insert(lineIndex, "\t\t" + fullLine);
                            lineIndex++;
                            int k = 0;
                            while (lines[lineIndex].Contains(abilityKey + Settings.Default.NoteSuffix))
                            {
                                lines[lineIndex] = lines[lineIndex].Replace(abilityKey + Settings.Default.NoteSuffix + k, abilityKey + Settings.Default.NoteSuffix + (++k));
                                lineIndex++;
                            }
                        }
                        //}
                        addedAbilitiesNames.Add(abilityName);
                        StatusBarHelper.Instance.SetStatus("Creating new resources file: " + abilityData.Value.Values.ElementAt(0));
                    }
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }

                }
                writer.Close();
                StatusBarHelper.Instance.SetStatus("Resource file creation completed.");
            }
        }

        private string GetExtraText(string castPoint, string castBackswing, string rubickCastBackswing, string modifier)
        {
            string s1 = Settings.Default.CastPointTextFormat;
            string s2 = Settings.Default.CastBackswingTextFormat;
            string s3 = Settings.Default.RubickCastBackswingTextFormat;
            try { s1 = string.Format(s1, castPoint); } catch (Exception) { }
            try { s2 = string.Format(s2, castBackswing); } catch (Exception) { }
            try { s3 = string.Format(s3, rubickCastBackswing); } catch (Exception) { }

            if (Settings.Default.HideValuesIfEqualToZero)
            {
                if (castPoint == "0") s1 = "";
                if (castBackswing == "0") s2 = "";
                if (rubickCastBackswing == "0") s3 = "";
            }

            List<string> s = new List<string> { s1, s2, s3 };
            string separated = string.Join(Settings.Default.NewLineAfterText ? @"\n" : "", s.Where(x => !string.IsNullOrEmpty(x)));
            if (Settings.Default.UseCustomColor)
            {
                separated = string.Format(Settings.Default.FontColorFormat, Utility.ArgbToRgb(Settings.Default.SelectedColor), ((string.IsNullOrEmpty(modifier) ? "" : (modifier + ": " + (Settings.Default.NewLineAfterText ? @"\n" : ""))) + separated));
            }
            var txtPos = (ExtraTextPosition)Settings.Default.ExtraTextPosition;
            if (txtPos == ExtraTextPosition.AboveDescription)
            {
                return separated + @"\n";
            }
            else if (txtPos == ExtraTextPosition.BelowDescription)
            {
                return @"\n" + separated;
            }
            return separated;
        }

        public void PublishResources(string filePath)
        {
            StatusBarHelper.Instance.SetStatus("Replacing resources file...");
            if (!File.Exists(TmpFilePath))
            {
                MessageBox.Show("File doesn't exist at the path: " + TmpFilePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Backing up old file
            Directory.CreateDirectory(AppData);
            if (!File.Exists(GetBackupFile(filePath)))
            {
                File.Copy(filePath, GetBackupFile(filePath));
            }

            MoveAsAdmin(TmpFilePath, filePath);
            StatusBarHelper.Instance.SetStatus("Process completed! You're good to go!");

        }

        public void RevertResources(string filePath)
        {
            StatusBarHelper.Instance.SetStatus("Reverting resources file...");
            //Restoring old file
            Directory.CreateDirectory(AppData);

            if (!File.Exists(GetBackupFile(filePath)))
            {
                StatusBarHelper.Instance.SetStatus("Backup resources file not found.");
                return;
            }

            MoveAsAdmin(GetBackupFile(filePath), filePath);
            StatusBarHelper.Instance.SetStatus("Resources reverted.");
        }

        private void CopyAsAdmin(string sourcePath, string destPath)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Process p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = String.Format("/C copy /y \"{0}\" \"{1}\"", sourcePath, destPath);
                    startInfo.UseShellExecute = true;
                    startInfo.Verb = "runas";
                    p.StartInfo = startInfo;
                    p.Start();
                    p.WaitForExit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while copying the file into the Dota 2 resources folder:\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoveAsAdmin(string sourcePath, string destPath)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Process p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = String.Format("/C move /y \"{0}\" \"{1}\"", sourcePath, destPath);
                    startInfo.UseShellExecute = true;
                    startInfo.Verb = "runas";
                    p.StartInfo = startInfo;
                    p.Start();
                    p.WaitForExit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while moving the file into the Dota 2 resources folder:\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
