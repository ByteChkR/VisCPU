using System.Collections.Generic;
using System.Globalization;

using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Plugins
{
    public class ChangeCharCase : AbstractLinePlugin
    {

        public string CaseChange { get; set; } = "tolower";

        private bool ToLower => CaseChange.ToLower(CultureInfo.InvariantCulture) == "tolower";

        public override List<CommandInfo> Info =>
            new List<CommandInfo>
            {
                new CommandInfo(
                                "set-order",
                                "o",
                                PropertyHelper.GetPropertyInfo(typeof(ChangeCharCase), nameof(Order)),
                                "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"
                               ),
                new CommandInfo(
                                "set-stage",
                                "ss",
                                PropertyHelper.GetPropertyInfo(typeof(ChangeCharCase), nameof(Stage)),
                                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"
                               ),
                new CommandInfo(
                                "set-case",
                                "sc",
                                PropertyHelper.GetPropertyInfo(typeof(ChangeCharCase), nameof(CaseChange)),
                                "Sets the Case that will transform the text. Options: tolower(default)/toupper"
                               )
            };

        public override string[] Prefix => new[] { "ccc", "ChangeCharCase" };


        public override string LineStage(string source)
        {
            return ToLower
                       ? source.ToLower(CultureInfo.InvariantCulture)
                       : source.ToUpper(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     No initialization needed for this plugin.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            settings.ApplySettings(Info, this);
        }

    }
}