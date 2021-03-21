using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Base.Plugins
{
    /// <summary>
    ///     AbstractLinePlugin but with fixed plugin type toggle
    /// </summary>
    public abstract class AbstractLineBeforePlugin : AbstractLinePlugin
    {

        public override PluginType PluginTypeToggle { get; } = PluginType.LinePluginBefore;

    }
}