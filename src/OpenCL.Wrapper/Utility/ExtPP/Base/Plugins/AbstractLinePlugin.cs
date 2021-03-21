using System.Globalization;

using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Base.Plugins
{
    /// <summary>
    ///     AbstractLinePlugin but with preconfigured PluginTypeToggle, Process Stages and only one function for all passes.
    /// </summary>
    public abstract class AbstractLinePlugin : AbstractPlugin
    {

        /// <summary>
        ///     Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public override PluginType PluginTypeToggle =>
            Order.ToLower(CultureInfo.InvariantCulture) == "after"
                ? PluginType.LinePluginAfter
                : PluginType.LinePluginBefore;

        /// <summary>
        ///     Specifies the order on what "event" the plugin should execute
        /// </summary>
        public override ProcessStage ProcessStages =>
            Stage.ToLower(CultureInfo.InvariantCulture) == "onload"
                ? ProcessStage.OnLoadStage
                : ProcessStage.OnFinishUp;

        public string Order { get; set; } = "after";

        public string Stage { get; set; } = "onfinishup";

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public abstract string LineStage(string source);

    }
}