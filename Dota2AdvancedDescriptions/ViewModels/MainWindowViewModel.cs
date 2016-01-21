using Dota2AdvancedDescriptions.Helpers;
using Dota2AdvancedDescriptions.Properties;
using Dota2AdvancedDescriptions.Tools;
using Dota2AdvancedDescriptions.ViewModels;
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

        private StatusBarViewModel _statusBarViewModel;
        public StatusBarViewModel StatusBarViewModel
        {
            get
            {
                return _statusBarViewModel;
            }
            set
            {
                SetProperty(ref _statusBarViewModel, value);
            }
        }
        private PreviewViewModel _previewViewModel;
        public PreviewViewModel PreviewViewModel
        {
            get
            {
                return _previewViewModel;
            }
            set
            {
                SetProperty(ref _previewViewModel, value);
            }
        }

        public MainWindowViewModel()
        {
            StatusBarViewModel = new StatusBarViewModel();
            StatusBarHelper.Instance = new StatusBarHelper(StatusBarViewModel);
            PreviewViewModel = new PreviewViewModel();
        }

        internal void Load()
        {
            _htmlParser = new DotaHtmlParser();
            _resourcesParser = new DotaResourcesParser(ResourcesFilePath);
            _resourcesEditor = new DotaResourcesEditor();
            /*
            _htmlParser.ParseAbilitiesCastPoints(Settings.Default.CastPointsTableAddress, Settings.Default.CastPointsTableXPath, Settings.Default.CastPointsTableIndex);
            _resourcesParser.ParseResources();
            _resourcesEditor.PrepareResources(ResourcesFilePath, _htmlParser.ParsedData, _resourcesParser.ParsedResources);
            _resourcesEditor.PublishResources(ResourcesFilePath);*/
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
