using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyCrane
{
    public interface StateChangeListener
    {
        void handleStateChange(StateChange s);
    }
}
