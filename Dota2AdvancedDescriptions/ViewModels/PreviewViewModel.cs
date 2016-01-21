using Dota2AdvancedDescriptions.Enums;
using Dota2AdvancedDescriptions.Properties;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions.ViewModels
{
    public class PreviewViewModel : BindableBase
    {
        public PreviewViewModel()
        {
            Settings.Default.PropertyChanged += SettingsPropertyChanged;
        }

        private void SettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ExtraTextPosition")
            {
                OnPropertyChanged(() => TextAboveDescription);
                OnPropertyChanged(() => TextBelowDescription);
                OnPropertyChanged(() => TextAboveNotes);
                OnPropertyChanged(() => TextBelowNotes);
            }
        }

        public string TextAboveDescription
        {
            get
            {
                if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.AboveDescription)
                {
                    return Settings.Default.ExtraTextFormat;
                }
                return null;
            }
        }

        public string TextBelowDescription
        {
            get
            {
                if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.BelowDescription)
                {
                    return Settings.Default.ExtraTextFormat;
                }
                return null;
            }
        }

        public string TextAboveNotes
        {
            get
            {
                if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.AboveNotes)
                {
                    return Settings.Default.ExtraTextFormat;
                }
                return null;
            }
        }

        public string TextBelowNotes
        {
            get
            {
                if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.BelowNotes)
                {
                    return Settings.Default.ExtraTextFormat;
                }
                return null;
            }
        }
    }
}
