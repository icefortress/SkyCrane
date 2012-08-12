using System;

namespace SkyCrane.NetCode
{
    public interface StateChangeListener
    {
        void handleStateChange(StateChange s);
    }
}
