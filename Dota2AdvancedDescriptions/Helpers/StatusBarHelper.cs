using Dota2AdvancedDescriptions.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Dota2AdvancedDescriptions.Helpers
{
    public class StatusBarHelper
    {
        public static StatusBarHelper Instance { get; set; }

        private StatusBarViewModel _viewModel;

        public StatusBarHelper(StatusBarViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void SetStatus(string status)
        {
            Debug.WriteLine(status);
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { _viewModel.Status = status; }), DispatcherPriority.ContextIdle, null);
        }
    }
}
