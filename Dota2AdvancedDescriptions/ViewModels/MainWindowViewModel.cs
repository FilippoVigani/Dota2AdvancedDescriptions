using Dota2AdvancedDescriptions.Helpers;
using Dota2AdvancedDescriptions.Properties;
using Dota2AdvancedDescriptions.Tools;
using Dota2AdvancedDescriptions.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Dota2AdvancedDescriptions.ViewModels
{
    public partial class MainWindowViewModel : BindableBase
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
            _htmlParser = new DotaHtmlParser();
            _resourcesParser = new DotaResourcesParser();
            _resourcesEditor = new DotaResourcesEditor();
            StatusBarViewModel = new StatusBarViewModel();
            StatusBarHelper.Instance = new StatusBarHelper(StatusBarViewModel);
            PreviewViewModel = new PreviewViewModel();
            Settings.Default.PropertyChanged += SettingsPropertyChanged;
        }

        private void SettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResourcesFolderPath")
            {
                OnPropertyChanged(() => ResourcesFilePath);
                OnPropertyChanged(() => AvailableResourceFileNames);
            } else if (e.PropertyName == "SelectedResourcesFileName")
            {
                OnPropertyChanged(() => ResourcesFilePath);
            }
        }

        internal void Load()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                _htmlParser.ParseAbilitiesCastPoints(Settings.Default.CastPointsTableAddress, Settings.Default.CastPointsTableXPath, Settings.Default.CastPointsTableIndex);
            };
            worker.RunWorkerCompleted += delegate
            {
                CreateAdvancedDescriptionCommand.RaiseCanExecuteChanged();
            };
            worker.RunWorkerAsync();
        }

        public string ResourcesFilePath
        {
            get
            {
                return Path.Combine(Settings.Default.ResourcesFolderPath, Settings.Default.SelectedResourcesFileName);
            }
        }

        public List<string> AvailableResourceFileNames
        {
            get
            {
                return Directory.GetFiles(Settings.Default.ResourcesFolderPath).Select(f => Path.GetFileName(f)).Where(f => f.StartsWith("dota")).ToList();
            }
        }
    }
}
