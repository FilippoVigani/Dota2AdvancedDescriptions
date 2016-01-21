using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Dota2AdvancedDescriptions.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                SetProperty(ref status, value);
            }
        }
    }
}
