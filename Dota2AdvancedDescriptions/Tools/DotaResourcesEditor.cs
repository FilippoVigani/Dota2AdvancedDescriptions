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
            StreamReader reader = new StreamReader(filePath);
            string input = reader.ReadToEnd();
            reader.Close();
            using (StreamWriter writer = new StreamWriter(TmpFilePath, true, Utility.GetEncoding(filePath)))
            {
                {
                    //Insert credits, so I can detect if the file is customized or not later on
                    string output = "";
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        bool stringFound = false;
                        while (!stringFound)
                        {
                            string line = r.ReadLine();
                            if (line.Contains("Language"))
                            {
                                output = input.Replace(line, line + Resources.CreditsText);
                                stringFound = true;
                            }
                        }
                    }
                   
                    foreach (var abilityData in data)
                    {
                        string heroName = abilityData.Key.Substring(0, abilityData.Key.IndexOf(Settings.Default.TableAbilityHeroSeparator) - 1).Trim();
                        string abilityName = abilityData.Key.Substring(abilityData.Key.IndexOf(Settings.Default.TableAbilityHeroSeparator) + Settings.Default.TableAbilityHeroSeparator.Length).Trim();
                        //Fix for spells such as brewmaster with double separator
                        if (abilityName.IndexOf(Settings.Default.TableAbilityHeroSeparator) >= 0)
                        {
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
                            continue;
                        }
                        //Perform editing
                        foreach (var abilityKey in abilityKeys.Select(a => a.Key))
                        {
                            if (heroResources.ContainsKey(abilityKey + "_Description"))
                            {
                                string desc = heroResources[abilityKey + "_Description"];
                                if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.AboveDescription)
                                {
                                    string extraText = Settings.Default.ExtraTextFormat.Replace("{0}", abilityData.Value["Cast point"]).Replace("{1}", abilityData.Value["Cast backswing"]);
                                    output = output.Insert(output.IndexOf(desc), extraText);
                                }
                            } else
                            {
                                //Some resources are missing on foreign languages
                                Console.WriteLine("Missing resource: " + abilityKey + "_Description");
                            }
                            
                        }
                    }
                    writer.Write(output);
                }
                writer.Close();
            }
        }

        public void PublishResources(string filePath)
        {
            StatusBarHelper.Instance.SetStatus("Replacing resources file...");
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File doesn't exist at the path: " + filePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Backing up old file
            Directory.CreateDirectory(AppData);
            if (!File.Exists(GetBackupFile(filePath)))
            {
                File.Copy(filePath, GetBackupFile(filePath));
            }

            CopyAsAdmin(TmpFilePath, filePath);
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
