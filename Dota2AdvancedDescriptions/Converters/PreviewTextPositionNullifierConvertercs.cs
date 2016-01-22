using Dota2AdvancedDescriptions.Enums;
using Dota2AdvancedDescriptions.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Dota2AdvancedDescriptions.Converters
{
    public class PreviewTextPositionNullifierConvertercs : MarkupExtension, IValueConverter
    {
        public ExtraTextPosition VisiblePosition { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ExtraTextPosition)Settings.Default.ExtraTextPosition == VisiblePosition)
            {
                return value;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
