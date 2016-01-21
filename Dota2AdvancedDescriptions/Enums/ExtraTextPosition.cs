using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions.Enums
{
    public enum ExtraTextPosition
    {
        [Description("Above description")]
        AboveDescription,
        [Description("Below description")]
        BelowDescription,
        [Description("Above notes")]
        AboveNotes,
        [Description("Below notes")]
        BelowNotes
    }
}
