using Dota2AdvancedDescriptions.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions
{
    public class MainWindowViewModel
    {
        private DotaHtmlParser _htmlParser;

        public MainWindowViewModel()
        {
            _htmlParser = new DotaHtmlParser();
            _htmlParser.ParseAbilitiesCastPoints(null, null);
        }
    }
}
