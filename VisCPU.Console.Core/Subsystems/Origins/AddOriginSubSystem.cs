using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    [Serializable]
    public class OriginSettings
    {
        static OriginSettings()
        {
            Settings.RegisterDefaultLoader(new JSONSettingsLoader(), Path.Combine(
                                                AppDomain.CurrentDomain.BaseDirectory,
                                                "config/origins.json"
                                               ), new OriginSettings());
        }
        public Dictionary<string, string> origins = new Dictionary<string, string>
                                                       {
                                                           { "local", Settings.GetDefaultFile < OriginSettings >() }
                                                       };

        public static OriginSettings Create() => Settings.GetSettings < OriginSettings >();

    }
    public class OriginSubSystem : ConsoleSubsystem
    {

        

        private readonly Dictionary<string, ConsoleSubsystem> subsystems =
            new Dictionary<string, ConsoleSubsystem>
            {
                { "add", new AddOriginSubSystem() },
                { "remove", new RemoveOriginSubSystem() },
                { "refresh", new RefreshOriginSubSystem() },
                { "list", new ListOriginSubSystem() },
            };

        public override void Run(IEnumerable<string> args)
        {
            CLISettings s = CLISettings.Create();
            ArgumentSyntaxParser.Parse(args.ToArray(), s);
            VisConsole.RunConsole(s, args.ToArray(), subsystems);
        }

    }

    public class AddOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = Settings.GetSettings<OriginSettings>();
            string[] a = args.ToArray();
            string name = a[0];
            string url = a[1];
            s.origins.Add(name, url);
            Settings.SaveSettings(s);
        }

    }

    public class RemoveOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = Settings.GetSettings<OriginSettings>();
            string name = args.First();
            s.origins.Remove(name);
            Settings.SaveSettings(s);
        }

    }
    public class RefreshOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = Settings.GetSettings<OriginSettings>();

            foreach (KeyValuePair<string, string> keyValuePair in s.origins)
            {
                Uri url = new Uri(keyValuePair.Value, UriKind.RelativeOrAbsolute);

                if (url.Scheme == "file")
                {
                    LocalModuleManager lm = new LocalModuleManager(url.OriginalString);

                    foreach (ModulePackage modulePackage in lm.GetPackages())
                    {
                        if (ModuleResolver.Manager.HasPackage(modulePackage.ModuleName))
                        {
                            foreach (string moduleVersion in modulePackage.ModuleVersions)
                            {
                                ModuleTarget t = modulePackage.GetInstallTarget(moduleVersion);
                                ModuleResolver.Manager.AddPackage(t, lm.GetTargetDataPath(t));
                            }
                        }
                        else
                        {
                            ModulePackage existing = ModuleResolver.Manager.GetPackage(modulePackage.ModuleName);
                            foreach (string moduleVersion in modulePackage.ModuleVersions)
                            {
                                if (!existing.HasTarget(moduleVersion))
                                {
                                    ModuleTarget t = modulePackage.GetInstallTarget(moduleVersion);
                                    ModuleResolver.Manager.AddPackage(t, lm.GetTargetDataPath(t));
                                }
                            }
                        }
                    }
                    continue;
                }

                throw new Exception($"Scheme '{url.Scheme}' is not supported");
            }
        }

    }

    public class ListOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = Settings.GetSettings<OriginSettings>();

            foreach (KeyValuePair<string, string> keyValuePair in s.origins)
            {
                Log($"{keyValuePair.Key} : {keyValuePair.Value}");
            }
        }

    }

}
