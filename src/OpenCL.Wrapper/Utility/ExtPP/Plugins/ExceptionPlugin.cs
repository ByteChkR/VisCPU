using System.Collections.Generic;

using Utility.ADL;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;
using Utility.FastString;

using Utils = Utility.ExtPP.Base.Utils;

namespace Utility.ExtPP.Plugins
{
    public class ExceptionPlugin : AbstractLinePlugin
    {

        public override string[] Prefix => new[] { "ex", "ExceptionPlugin" };

        public string WarningKeyword { get; set; } = "#warning";

        public string ErrorKeyword { get; set; } = "#error";

        public string Separator { get; set; } = " ";

        public bool ThrowOnError { get; set; } = true;

        public bool ThrowOnWarning { get; set; }


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
                                                          {
                                                              new CommandInfo(
                                                                              "set-error",
                                                                              "e",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       ErrorKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to trigger errors during compilation"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-warning",
                                                                              "w",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       WarningKeyword
                                                                                   )
                                                                                  ),
                                                                              "sets the keyword that is used to trigger warnings during compilation"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-separator",
                                                                              "s",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Separator
                                                                                   )
                                                                                  ),
                                                                              "Sets the separator that is used to separate different generic types"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-order",
                                                                              "o",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Order)
                                                                                  ),
                                                                              "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-stage",
                                                                              "ss",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Stage)
                                                                                  ),
                                                                              "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"
                                                                             ),
                                                              new CommandInfo(
                                                                              "throw-on-error",
                                                                              "toe",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       ThrowOnError
                                                                                   )
                                                                                  ),
                                                                              "When an Error Occurs it will throw an exception that will halt the processing."
                                                                             ),
                                                              new CommandInfo(
                                                                              "throw-on-warning",
                                                                              "tow",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ExceptionPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       ThrowOnWarning
                                                                                   )
                                                                                  ),
                                                                              "When a Warning Occurs it will throw an exception that will halt the processing."
                                                                             )
                                                          };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            settings.ApplySettings(Info, this);
        }


        public override string LineStage(string source)
        {
            if (Utils.IsStatement(source, WarningKeyword))
            {
                string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");

                Logger.Log(LogType.Error, $"Warning: {err}", 1);
                if (ThrowOnWarning)
                {
                    throw new ErrorException(err);
                }

                return "";
            }


            if (Utils.IsStatement(source, ErrorKeyword))
            {
                string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
                Logger.Log(LogType.Error, $"Error {err}", 1);
                if (ThrowOnError)
                {
                    throw new ErrorException(err);
                }

                return "";
            }

            return source;
        }

    }
}