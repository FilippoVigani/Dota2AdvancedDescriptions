using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions.ViewModels
{
    public partial class MainWindowViewModel
    {
        private DelegateCommand<object> resetResourcesCommand;

        public DelegateCommand<object> ResetResourcesCommand
        {
            get
            {
                return this.resetResourcesCommand ?? (this.resetResourcesCommand = new DelegateCommand<object>((x) =>
                {
                    
                }));
            }
        }

        private DelegateCommand<object> createAdvancedDescriptionCommand;

        public DelegateCommand<object> CreateAdvancedDescriptionCommand
        {
            get
            {
                return this.createAdvancedDescriptionCommand ?? (this.createAdvancedDescriptionCommand = new DelegateCommand<object>((x) =>
                {
                    if (!_htmlParser.ParseCompleted) return;

                    new Thread(() => {
                        _resourcesParser.ParseResources();
                        _resourcesEditor.PrepareResources(ResourcesFilePath, _htmlParser.ParsedData, _resourcesParser.ParsedResources);
                        _resourcesEditor.PublishResources(ResourcesFilePath);
                    }).Start();
                }, x => { return _htmlParser != null && _htmlParser.ParseCompleted; }));
            }
        }
    }
}
