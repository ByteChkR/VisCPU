using System;
using System.Collections;
using System.Text;

using Utility.ADL.Configs;

namespace Utility.ADL.Crash
{
    public static class CrashHandler
    {

        private static bool initialized;

        private static readonly CrashConfig Config = ConfigManager.GetDefault<CrashConfig>();

        private static readonly ADLLogger<CrashLogType> CrashLogger =
            new ADLLogger<CrashLogType>(InternalADLProjectDebugConfig.Settings, "Crash");

        public static void SaveCurrentConfig(string configPath = "adl_crash.xml")
        {
            ConfigManager.SaveToFile(configPath, Config);
        }

        public static void Initialize(string configPath = "adl_crash.xml")
        {
            Initialize(ConfigManager.ReadFromFile<CrashConfig>(configPath));
        }

        public static void Initialize(CrashConfig config)
        {
            Initialize(config.ShortenCrashInfo);
        }

        public static void Initialize(bool shortenCrashInfo)
        {
            Config.ShortenCrashInfo = shortenCrashInfo;


            initialized = true;
        }

        public static void Log(Exception exception)
        {
            Log(exception, true);
        }

        public static void Log(Exception exception, bool includeInner)
        {
            if (!initialized)
            {
                CrashLogger.Log(CrashLogType.Error, "Crash handler was not initialized", 1);
                return;
            }

            if (Config.ShortenCrashInfo)
            {
                CrashLogger.Log(CrashLogType.CrashShort, ExceptionHeader(exception), 1);
            }
            else
            {
                CrashLogger.Log(CrashLogType.Crash, ExceptionToString(exception, includeInner), 1);
            }
        }

        private static string ExceptionHeader(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nException Logged: ");
            sb.Append(exception.GetType().FullName);
            if (exception.Message != null)
            {
                sb.Append("\nException Message: ");
                sb.Append(exception.Message);
            }

            if (exception.Source != null)
            {
                sb.Append("\nException Source: ");
                sb.Append(exception.Source);
            }

            return sb.ToString();
        }

        private static string ExceptionToString(Exception exception, bool includeInner)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nException Type: ");
            sb.Append(exception.GetType().FullName);
            if (exception.Message != null)
            {
                sb.Append("\nException Message: ");
                sb.Append(exception.Message);
            }

            if (exception.Source != null)
            {
                sb.Append("\nException Source: ");
                sb.Append(exception.Source);
            }

            if (exception.HelpLink != null)
            {
                sb.Append("\nException Help Link: ");
                sb.Append(exception.HelpLink);
            }

            sb.Append("\nException HResult: ");
            sb.Append(exception.HResult.ToString());

            if (exception.StackTrace != null)
            {
                sb.Append("\nException Stacktrace: \n");
                sb.Append(exception.StackTrace);
            }

            if (exception.Data.Count != 0)
            {
                sb.Append("\nException Data:");
                foreach (DictionaryEntry dictionaryEntry in exception.Data)
                {
                    sb.Append("\n");
                    sb.Append(dictionaryEntry.Key);
                    sb.Append(":");
                    if (!dictionaryEntry.Value.GetType().IsArray)
                    {
                        sb.Append(dictionaryEntry.Value);
                    }
                    else
                    {
                        sb.Append(UnpackToString(dictionaryEntry.Value));
                    }
                }
            }

            if (includeInner && exception.InnerException != null)
            {
                sb.Append("\nInner Exception:");
                sb.Append(ExceptionToString(exception.InnerException, true));
            }

            return sb.ToString();
        }


        public static string UnpackToString(object obj, int depth = 0)
        {
            StringBuilder ret = new StringBuilder();
            if (obj.GetType().IsArray)
            {
                IEnumerable o = (IEnumerable) obj;
                foreach (object entry in o)
                {
                    ret.AppendLine(UnpackToString(entry, depth + 1));
                }
            }
            else
            {
                StringBuilder ind = new StringBuilder();
                for (int i = 0; i < depth; i++)
                {
                    ind.Append('\t');
                }

                ret.Append(ind);
                ret.Append(obj);
            }

            return ret.ToString();
        }

        private enum CrashLogType
        {

            CrashShort,
            Error,
            Crash

        }

    }
}