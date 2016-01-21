using Dota2AdvancedDescriptions.Properties;
using Dota2AdvancedDescriptions.Tools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions
{
    public class MainWindowViewModel : BindableBase
    {
        private DotaHtmlParser _htmlParser;
        private DotaResourcesParser _resourcesParser;
        private DotaResourcesEditor _resourcesEditor;

        public MainWindowViewModel()
        {
        }

        internal void Load()
        {
            _htmlParser = new DotaHtmlParser();
            _resourcesParser = new DotaResourcesParser(ResourcesFilePath);
            _htmlParser.ParseAbilitiesCastPoints(Settings.Default.CastPointsTableAddress, Settings.Default.CastPointsTableXPath, Settings.Default.CastPointsTableIndex);
            _resourcesParser.ParseResources();
            _resourcesEditor = new DotaResourcesEditor();
            _resourcesEditor.PrepareResources(ResourcesFilePath, _htmlParser.ParsedData, _resourcesParser.ParsedResources);
            _resourcesEditor.PublishResources(ResourcesFilePath);
        }

        public string ResourcesFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.ResourcesFolderPath, "dota_english.txt");
            }
        }

    }
}
