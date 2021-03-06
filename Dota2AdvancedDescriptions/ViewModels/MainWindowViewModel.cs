﻿using Dota2AdvancedDescriptions.Helpers;
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
            Settings.Default.Upgrade();
        }

        private void SettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResourcesFolderPath")
            {
                OnPropertyChanged(() => ResourcesFilePath);
                OnPropertyChanged(() => AvailableResourceFileNames);
                ResetResourcesCommand.RaiseCanExecuteChanged();
                CreateAdvancedDescriptionCommand.RaiseCanExecuteChanged();
                LaunchDota.RaiseCanExecuteChanged();
                SelectFileCommand.RaiseCanExecuteChanged();
            } else if (e.PropertyName == "SelectedResourcesFileName")
            {
                OnPropertyChanged(() => ResourcesFilePath);
                ResetResourcesCommand.RaiseCanExecuteChanged();
                CreateAdvancedDescriptionCommand.RaiseCanExecuteChanged();
                SelectFileCommand.RaiseCanExecuteChanged();
            }
        }

        internal void Load()
        {
            Settings.Default.ResourcesFolderPath = Utility.GetResourcesFolder() ?? Settings.Default.ResourcesFolderPath;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                _htmlParser.ParseData(Settings.Default.CastPointsTableAddress, Settings.Default.TableXPath);
                _htmlParser.ParseData(Settings.Default.CastRangesTableAddress, Settings.Default.TableXPath);
            };
            worker.RunWorkerCompleted += delegate
            {
                CreateAdvancedDescriptionCommand.RaiseCanExecuteChanged();
                RetryDownloadCommand.RaiseCanExecuteChanged();
            };
            worker.RunWorkerAsync();
        }

        public string ResourcesFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.Default.SelectedResourcesFileName))
                {
                    return Settings.Default.ResourcesFolderPath;
                }
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
