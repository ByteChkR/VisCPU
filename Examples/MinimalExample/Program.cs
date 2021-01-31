using System;
using System.IO;
using System.Linq;
using System.Threading;
using DynamicExample;
using VisCPU;
using VisCPU.HL;
using VisCPU.HL.Integration;
using VisCPU.Instructions;
using VisCPU.Peripherals.Benchmarking;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Console.IO;
using VisCPU.Peripherals.HostFS;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace MinimalExample
{

    internal class Program
    {
        private static CpuInstanceBuilder s_Builder;

        private static readonly string s_Source = @"

//Creates Delay through C# Thread.Sleep
#import {QUOT}api-integration Sleep{QUOT}

public const uint C_OUT = {C_OUT_ADDR}; //Console Output Pin
public const uint C_CLEAR = {C_CLEAR_ADDR}; //Console Clear Pin
public static string STR = {QUOT}Hello World!{QUOT};

public uint c_new_line = '\n'; //New Line


public void Clear()
{
    C_CLEAR = 1; //Send anything to Clear Pin
}

private void WriteLine(uint strPtr, uint strLen)
{
    for(uint i = 0; i < strLen; i++)
    {
        C_OUT = strPtr[i];
    }
    C_OUT = c_new_line;
}

public void Do()
{
    uint i = 0;
    while(i < 25)
    {
        WriteLine(&STR, size_of(STR)); //Write String
        i++; //Increment Counter
        Sleep(250); //Create Time Delay
    }
}

public void Loop()
{
    while(1)
    {
        Do();
    }
}


Do(); //Call Entry Point
";

        private static string TempSrc => Path.Combine( UnityIsAPieceOfShitHelper.AppRoot, "src" );

        private static string TempFile => Path.Combine( TempSrc, "file.vhl" );

        private static string TempBuild => Path.Combine( UnityIsAPieceOfShitHelper.AppRoot, "build" );

        private static string TempInternalBuild => Path.Combine( UnityIsAPieceOfShitHelper.AppRoot, "internal" );

        #region Private

        private static string Compile( ConsoleOutInterfaceSettings cOutSettings )
        {
           
            //Compile
            return CompilerHelper.Compile(
                TempFile,
                TempBuild,
                TempInternalBuild,
                false,                     //Does not clean temp files, useful for debugging
                new[] { "HL-expr", "bin" } //Build Steps, last step has to be "bin" to generate .vbin binary
            );
        }

        private static void Main( string[] args )
        {
            int idx = args.ToList().IndexOf( "-r" );
            bool useDynamic = args.Contains("-d");

            UnityIsAPieceOfShitHelper.SetAppDomainBase(); //Set default Root Directory


            //Process -o flag to enable optimizations
            HlCompilerSettings hlSettings = SettingsManager.GetSettings < HlCompilerSettings >();
            hlSettings.OptimizeAll = args.Contains( "-o" );
            SettingsManager.SaveSettings( hlSettings );
            CpuSettings.FallbackSet = new DefaultSet(); //Set Fallback Instruction Set

            //Setup Logging and Events
            EventManager.RegisterDefaultHandlers();
            
            //Log to Console
            Logger.OnLogReceive +=
                ( sys, str ) => Console.WriteLine( $"[{sys}]{str}" );

            //Load Console Out Device Settings for the Correct Pins
            ConsoleOutInterfaceSettings cOutSettings = SettingsManager.GetSettings < ConsoleOutInterfaceSettings >();

            //Prepare CPU Builder before Compilation as the Exposed APIs have to be present at compile time
            s_Builder = new CpuInstanceBuilder().WithPeripherals( //Add Console Output and Memory(RAM)
                                                    new ConsoleOutInterface(),
                                                    new ConsoleInInterface(),
                                                    new ConsoleInterface(),
                                                    new HostFileSystem(),
                                                    new BenchmarkDevice(),
                                                    new Memory()
                                                )

                                                //Expose C# Function
                                                .
                                                WithExposedApi( SleepTime, "Sleep", 1 )
                ;

            if (idx != -1 && idx < args.Length - 1)
            {
                Run(args[idx + 1], useDynamic);

                return;
            }
            else
            {
                if ( !File.Exists( TempFile ) )
                {
                    Directory.CreateDirectory( TempSrc );
                    string src = s_Source.Replace( "{QUOT}", "\"" ).
                                          Replace( "{C_OUT_ADDR}", cOutSettings.WriteOutputAddress.ToString() ).
                                          Replace( "{C_CLEAR_ADDR}", cOutSettings.InterfaceClearPin.ToString() );

                    File.WriteAllText( TempFile, src );
                }
            }
            // The Output File(*.vbin)
            string buildOut = Compile( cOutSettings );

            Run( buildOut, useDynamic );

        }

        private static void Run( string file, bool dyn )
        {
            //Build CPU instance
            Cpu instance = s_Builder.Build();

            if ( dyn )
            {

                instance.LoadBinary(File.ReadAllBytes(file).ToUInt());
                //Create Dynamic Wrapper
                HlProgramWrapper wrapper = new( instance );

                //List Available Functions
                Console.WriteLine( "Available Functions: " );

                foreach ( string dynamicMemberName in wrapper.GetDynamicMemberNames() )
                {
                    Console.WriteLine( $"\t{dynamicMemberName}" );
                }

                //Command
                string str = null;

                //Exit Condition
                while ( str != "exit" )
                {
                    if ( str == "list" )
                    {
                        //List Available Functions
                        Console.WriteLine("Available Functions: ");

                        foreach (string dynamicMemberName in wrapper.GetDynamicMemberNames())
                        {
                            Console.WriteLine($"\t{dynamicMemberName}");
                        }
                    }
                    else if ( !string.IsNullOrEmpty(str) )
                    {
                        //Parse Commands
                        string[] parts = str.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
                        object[] args = parts.Skip( 1 ).Select( uint.Parse ).Cast < object >().ToArray();
                        
                        //Invoke Function.
                        //Note:
                        //  If Function Name is known at compile time, the functions can be invoked by using default C# Syntax.
                        //  wr.Clear(); or wr.Sleep(1000);
                        uint ret = wrapper.Invoke( parts[0], args );
                        //Display Return value of function
                        Console.WriteLine( $"Function {parts[0]} returned: {ret}" );
                    }

                    //Read Command from Console Input
                    Console.Write( "Enter Function to Execute(<cmd> <arg0> <arg1>): " );
                    str = Console.ReadLine();
                }
            }
            else
            {
                //Load Compiled Binary from File
                uint[] binary = File.ReadAllBytes( file ).ToUInt();

                //Write to CPU Memory Bus
                instance.LoadBinary( binary );

                //Run the Compiled Binary
                instance.Run();
            }
        }

        //Exposed as "Sleep" function in HL Code.
        private static uint SleepTime( Cpu instance )
        {
            uint ms = instance.Pop();
            Thread.Sleep( ( int ) ms );

            return 0;
        }

        #endregion
    }

}
