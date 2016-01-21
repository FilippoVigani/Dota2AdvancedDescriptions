﻿using System;
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

        public DotaResourcesEditor()
        {
        }

        public void PrepareResources(string filePath, Dictionary<string, Dictionary<string, string>> data, Dictionary<string, string> parsedResources)
        {
            TmpFilePath = Path.GetTempFileName();
            StreamReader reader = new StreamReader(filePath);
            string input = reader.ReadToEnd();

            using (StreamWriter writer = new StreamWriter(TmpFilePath, true))
            {
                {
                    string output = input;
                    foreach(var abilityData in data)
                    {
                        string heroName = abilityData.Key.Substring(0, abilityData.Key.IndexOf(" - ") - 1).Trim();
                        string abilityName = abilityData.Key.Substring(abilityData.Key.IndexOf(" - ") + 1).Trim();
                        var resource = parsedResources.Where(res => abilityName == res.Value);
                        //Perform editing
                        if (resource.Count() > 1)
                        {
                            Console.Write("aatwta");
                            throw new Exception("Too many resources");
                        }
                        if (resource.Count() == 0)
                        {
                            throw new Exception("Resource not found: " + abilityData.Key);
                        }
                    }
                    writer.Write(output);
                }
                writer.Close();
            }
            reader.Close();
        }

        public void PublishResources(string filePath)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Process p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = String.Format("/c COPY /y {0} {1}", TmpFilePath, filePath);
                    startInfo.UseShellExecute = true;
                    startInfo.Verb = "runas";
                    p.StartInfo = startInfo;
                    p.Start();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while copying the file into the Dota 2 resources folder:\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
