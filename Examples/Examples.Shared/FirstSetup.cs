using System;
using System.IO;

using VisCPU;
using VisCPU.Compiler.Linking;
using VisCPU.Instructions;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace Examples.Shared
{

    public class FirstSetup
    {

        private static bool s_IsStarted = false;

        internal static string SourceDirectory => Path.Combine(AppRootHelper.AppRoot, "src");
        internal static string OutputDirectory => Path.Combine(AppRootHelper.AppRoot, "build");
        internal static string InternalDirectory => Path.Combine(AppRootHelper.AppRoot, "internal");


        public static string DefaultFile
        {
            get
            {
                if ( !s_IsStarted )
                {
                    return null;
                }

                string ret = Path.Combine( SourceDirectory, "main.vhl" );

                if ( !File.Exists( ret ) )
                {
                    File.WriteAllText( ret, DefaultSource);
                }

                return ret;
            }
        }

        internal static string DefaultOutput
        {
            get
            {
                if ( !s_IsStarted )
                {
                    return null;
                }

                string ret = Path.Combine( OutputDirectory, "main.vbin" );

                return !File.Exists( ret ) ? null : ret;
            }
        }

        public static string DefaultSource => @"
//Entry Script
public const uint CONSOLE_OUT_PIN = 0xFFFF1001;
public const uint CONSOLE_NUM_OUT_PIN = 0xFFFF1002;
public const uint CONSOLE_IN_PIN = 0xFFFF1004;
public const uint CONSOLE_CLEAR_PIN = 0xFFFF1005;

public static string str_hello_world = ""Hello World!"";

public uint MyVar = 123;

public void Clear()
{
    CONSOLE_CLEAR_PIN = 1; //Enable Clear Pin(Enable by setting to anything but 0)
}

public void WriteNumber(uint num)
{
    CONSOLE_NUM_OUT_PIN = num; //Use Const Variable as Pointer
}

public void PrintVar()
{
	WriteNumber(MyVar);
}

public void WriteCharacter(uint chr)
{
    CONSOLE_OUT_PIN = chr;
}

public void WriteString(uint strPtr, uint strLen)
{
    for(uint i = strPtr; i < strPtr + strLen; i++)
    {
        WriteCharacter(*i);
    }
}

public void WriteLine(uint strPtr, uint strLen)
{
    WriteString(strPtr, strLen);
    WriteCharacter('\n');
}

public void HelloWorld()
{
    WriteLine(&str_hello_world, size_of(str_hello_world));
}

public void Add(uint a, uint b)
{
    return a + b;
}

HelloWorld();

";


        public static void Start( InstructionSet set )
        {
            if ( s_IsStarted )
            {
                return;
            }

            AppRootHelper.SetAppDomainBase();

            CpuSettings.FallbackSet = set;

            s_IsStarted = true;

            Directory.CreateDirectory( SourceDirectory );
            Directory.CreateDirectory( OutputDirectory );
            Directory.CreateDirectory( InternalDirectory );

            EventManager.RegisterDefaultHandlers();

            Logger.OnLogReceive += ( tag, msg ) => Console.WriteLine( $"[{tag}]{msg}" );


            //Set Linker Export Flag
            //Useful for Debugging and Required for DLR Wrapper.
            LinkerSettings settings = SettingsManager.GetSettings < LinkerSettings >();
            settings.ExportLinkerInfo = true;
            SettingsManager.SaveSettings( settings );

        }

        public static void Start()
        {
            Start( new DefaultSet() );
        }

        #region Public

        public static void End( EndOptions endOptions )
        {
            if ( !s_IsStarted )
            {
                return;
            }

            if ( ( endOptions & EndOptions.CleanOutput ) != 0 )
            {
                Directory.Delete( OutputDirectory , true);
            }

            if ( ( endOptions & EndOptions.CleanInternal ) != 0 )
            {
                Directory.Delete( InternalDirectory, true);
            }

            if ( ( endOptions & EndOptions.CleanSource ) != 0 )
            {
                Directory.Delete( SourceDirectory, true);
            }
        }

        #endregion

    }

}
