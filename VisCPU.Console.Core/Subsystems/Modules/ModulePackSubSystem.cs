using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModulePackSubSystem : ConsoleSubsystem
    {
        public static Version ChangeVersion(Version version, string changeStr)
        {
            string[] subVersions = changeStr.Split('.');
            int[] wrapValues = { ushort.MaxValue, 9, 99, ushort.MaxValue };
            int[] versions = { version.Major, version.Minor, version.Build, version.Revision };
            for (int i = 4 - 1; i >= 0; i--)
            {
                string current = subVersions[i];
                if (current.StartsWith("("))
                {
                    if (i == 0)
                    {
                        continue; //Can not wrap the last digit
                    }

                    int j = 0;
                    for (; j < current.Length; j++)
                    {
                        if (current[j] == ')')
                        {
                            break;
                        }
                    }

                    if (j == current.Length)
                    {
                        continue; //Broken. No number left. better ignore
                    }

                    string max = current.Substring(1, j - 1);
                    if (int.TryParse(max, out int newMax))
                    {
                        wrapValues[i] = newMax;
                    }

                    current = current.Remove(0, j + 1);
                }

                if (i != 0) //Check if we wrapped
                {
                    if (versions[i] >= wrapValues[i])
                    {
                        versions[i] = 0;
                        versions[i - 1]++;
                    }
                }

                if (current == "+")
                {
                    versions[i]++;
                }
                else if (current == "-" && versions[i] != 0)
                {
                    versions[i]--;
                }
                else if (current.ToLower(CultureInfo.InvariantCulture) == "x")
                {
                }
                else if (current.StartsWith("{") && current.EndsWith("}"))
                {
                    string format = current.Remove(current.Length - 1, 1).Remove(0, 1);

                    string value = DateTime.Now.ToString(format);

                    if (long.TryParse(value, out long newValue))
                    {
                        versions[i] = (int)(newValue % ushort.MaxValue);
                    }
                }
                else if (int.TryParse(current, out int v))
                {
                    versions[i] = v;
                }
            }

            return new Version(versions[0], versions[1] < 0 ? 0 : versions[1], versions[2] < 0 ? 0 : versions[2], versions[3] < 0 ? 0 : versions[3]);
        }


        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        private class PackOptions
        {
            [Argument(Name="version")]
            public string VersionString = "X.X.X.+";


        }

        public static void Pack( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string src = a.Length != 0
                             ? Path.Combine(Path.GetFullPath(a[0]), "project.json")
                             : Path.Combine(Directory.GetCurrentDirectory(), "project.json");

            string outDir = Path.Combine(Path.GetDirectoryName(src), "build");

            ModuleCleanSubSystem.Clean(Path.GetDirectoryName(src));

            ModuleTarget t = ModuleManager.LoadModuleTarget(src);

            Version v = Version.Parse(t.ModuleVersion);

            PackOptions po = new PackOptions();
            ArgumentSyntaxParser.Parse(a, po);


            t.ModuleVersion = ChangeVersion(v, po.VersionString).ToString();

            string temp = Path.Combine(
                                       Path.GetDirectoryName(Directory.GetCurrentDirectory()),
                                       "temp_" + t.ModuleName
                                      );

            Directory.CreateDirectory(temp);

            CopyTo(Path.GetDirectoryName(src), temp);

            File.Delete(Path.Combine(temp, "project.json"));

            foreach (ModuleDependency moduleDependency in t.Dependencies)
            {
                string p = Path.Combine(temp, moduleDependency.ModuleName);

                if (Directory.Exists(p))
                {
                    Directory.Delete(p, true);
                }
            }

            Directory.CreateDirectory(outDir);
            ZipFile.CreateFromDirectory(temp, Path.Combine(outDir, "module.zip"));
            Directory.Delete(temp, true);
            ModuleManager.SaveModuleTarget(t, Path.Combine(outDir, "module.json"));
            ModuleManager.SaveModuleTarget(t, src);
        }

        public override void Run( IEnumerable < string > args )
        {
            Pack( args );
        }

        #endregion

        #region Private

        private static void CopyTo( string src, string dst )
        {
            foreach ( string dirPath in Directory.GetDirectories(
                                                                 src,
                                                                 "*",
                                                                 SearchOption.AllDirectories
                                                                ) )
            {
                Directory.CreateDirectory( dirPath.Replace( src, dst ) );
            }

            foreach ( string newPath in Directory.GetFiles(
                                                           src,
                                                           "*.*",
                                                           SearchOption.AllDirectories
                                                          ) )
            {
                File.Copy( newPath, newPath.Replace( src, dst ), true );
            }
        }

        #endregion

    }

}
