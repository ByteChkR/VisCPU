using System.Collections.Generic;
using System.Linq;

using Utility.ADL;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Base.Plugins
{
    /// <summary>
    ///     Specifies the functionality needed to be incorporated in the processing chain of ext_pp
    /// </summary>
    public abstract class AbstractPlugin : ALoggable<LogType>
    {

        protected const int PLUGIN_MIN_SEVERITY = 5;

        protected AbstractPlugin() : base(ExtPPDebugConfig.Settings)
        {
            Logger.SetSubProjectName(Prefix.First());
        }

        /// <summary>
        ///     Returns a list of prefixes the plugin should be able to listen to when receiving settings
        /// </summary>
        public abstract string[] Prefix { get; }

        /// <summary>
        ///     A flag that will, when turned on, redirect all settings that have a global prefix
        /// </summary>
        public virtual bool IncludeGlobal => false;

        /// <summary>
        ///     Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public virtual PluginType PluginTypeToggle => PluginType.FullScriptPlugin;

        /// <summary>
        ///     Specifies the order on what "event" the plugin should execute
        /// </summary>
        public virtual ProcessStage ProcessStages => ProcessStage.OnMain;

        /// <summary>
        ///     A list of command infos. This list contains all the different commands of the plugin/program
        /// </summary>
        public virtual List<CommandInfo> Info => new List<CommandInfo>();

        /// <summary>
        ///     A list of statements that need to be removed as a last step of the processing routine
        /// </summary>
        public virtual string[] Cleanup => new string[0];

        /// <summary>
        ///     Returns the plugins that are meant to be run at the specified stage
        /// </summary>
        /// <param name="plugins">All plugins loaded</param>
        /// <param name="type">The plugin type</param>
        /// <param name="stage">the process stage</param>
        /// <returns></returns>
        public static List<AbstractPlugin> GetPluginsForStage(
            List<AbstractPlugin> plugins, PluginType type,
            ProcessStage stage)
        {
            return plugins.Where(
                                 x => BitMask.IsContainedInMask((int) x.PluginTypeToggle, (int) type, true) &&
                                      BitMask.IsContainedInMask((int) x.ProcessStages, (int) stage, true)
                                ).ToList();
        }

        /// <summary>
        ///     Gets called once on each file.
        ///     Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script">the current source script</param>
        /// <param name="sourceManager">the current source manager</param>
        /// <param name="defTable">the current definitions</param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public virtual bool OnMain_FullScriptStage(
            ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return true;
        }

        /// <summary>
        ///     Gets called once on each file.
        ///     Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script">the current source script</param>
        /// <param name="sourceManager">the current source manager</param>
        /// <param name="defTable">the current definitions</param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public virtual bool OnLoad_FullScriptStage(
            ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return true;
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public virtual string OnLoad_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public virtual string OnMain_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        ///     Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public virtual string OnFinishUp_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        ///     Initialization of the plugin
        ///     Set all your changes to the objects here(not in the actual processing)
        /// </summary>
        /// <param name="settings">the settings</param>
        /// <param name="sourceManager">the source manager</param>
        /// <param name="defTable">the definitions</param>
        public abstract void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable);

    }
}