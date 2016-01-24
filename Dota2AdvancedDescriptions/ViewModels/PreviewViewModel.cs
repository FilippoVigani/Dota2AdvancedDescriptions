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
            if (e.PropertyName == "ExtraTextPosition" || e.PropertyName == "CastPointTextFormat" || e.PropertyName == "CastBackswingTextFormat" || 
                e.PropertyName == "RubickCastBackswingTextFormat" || e.PropertyName == "HideValuesIfEqualToZero" || e.PropertyName== "NewLineAfterText")
            {
                OnPropertyChanged(() => PreviewText);
                OnPropertyChanged(() => PreviewTextColor);
            }
            if (e.PropertyName == "SelectedColor" || e.PropertyName == "UseCustomColor")
            {
                OnPropertyChanged(() => PreviewTextColor);
            }
        }

        public string PreviewText
        {
            get
            {
                string s1 = Settings.Default.CastPointTextFormat;
                string s2 = Settings.Default.CastBackswingTextFormat;
                string s3 = Settings.Default.RubickCastBackswingTextFormat;
                if (Settings.Default.HideValuesIfEqualToZero)
                {
                    s1 = "";
                } 
               
                try{ s1 = string.Format(s1, 0); } catch (Exception) { }
                try { s2 = string.Format(s2, 1.33); } catch (Exception) { }
                try { s3 = string.Format(s3, 1.07); } catch (Exception) { }

                List<string> s = new List<string>{ s1,s2,s3 };
                return string.Join(Settings.Default.NewLineAfterText ? Environment.NewLine : "", s.Where(x => !String.IsNullOrEmpty(x)));
            }
        }

        public string PreviewTextColor
        {
            get
            {
                if (Settings.Default.UseCustomColor)
                {
                    return Settings.Default.SelectedColor;
                }
                else
                {
                    if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.BelowNotes ||
                        (ExtraTextPosition)Settings.Default.ExtraTextPosition == ExtraTextPosition.AboveNotes)
                    {
                        return "#70ea72";
                    }
                    return "#c8c8c8";
                }
            }
        }
    }
}
