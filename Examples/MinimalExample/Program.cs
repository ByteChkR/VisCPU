using System;
using System.IO;
using VisCPU;
using VisCPU.HL;
using VisCPU.HL.Integration;
using VisCPU.Instructions;
using VisCPU.Peripherals.Console.IO;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;
using System.Linq;

namespace MinimalExample
{

    internal class Program
    {
        private static string TempSrc => Path.Combine(UnityIsAPieceOfShitHelper.AppRoot, "src");
        private static string TempFile => Path.Combine(TempSrc, "file.vhl");
        private static string TempBuild => Path.Combine(UnityIsAPieceOfShitHelper.AppRoot, "build");
        private static string TempInternalBuild => Path.Combine(UnityIsAPieceOfShitHelper.AppRoot, "internal");
        
        private static readonly string s_Source = @"
private const uint C_OUT = {C_OUT_ADDR}; //Console Output Pin
private const uint C_CLEAR = {C_CLEAR_ADDR}; //Console Clear Pin
private static string STR = {QUOT}Hello World!{QUOT};

private uint c_new_line = '\n'; //New Line

private void Delay(uint count)
{
    uint c = count; //Create Delay
    while(c)
    {
        c--;
    }
}

private void Clear()
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

private void Main()
{
    uint i = 0;
    while(1) //Endless Loop
    {
        if(i > 25)
        {
            Clear(); //Clear Console
            i = 0; //Reset Counter
        }
        else
        {
            WriteLine(&STR, size_of(STR)); //Write String
            i++; //Increment Counter
            Delay(100000); //Create Time Delay
        }
    }
}


Main(); //Call Entry Point

";

        #region Private

        private static void Main(string[] args)
        {

            UnityIsAPieceOfShitHelper.SetAppDomainBase(); //Set default Root Directory

            //Process -o flag to enable optimizations
            HlCompilerSettings hlSettings = SettingsManager.GetSettings<HlCompilerSettings>();
            hlSettings.OptimizeAll = args.Contains( "-o" );
            SettingsManager.SaveSettings( hlSettings );

            CpuSettings.FallbackSet = new DefaultSet(); //Set Fallback Instruction Set

            //Setup Logging and Events
            EventManager.RegisterDefaultHandlers();

            //Log to Console
            Logger.OnLogReceive += 
                ( sys, str ) => Console.WriteLine( $"[{sys}]{str}" );

            //Load Console Out Device Settings for the Correct Pins
            ConsoleOutInterfaceSettings cOutSettings = SettingsManager.GetSettings<ConsoleOutInterfaceSettings>();

            string buildOut = Compile( cOutSettings );

            Run(buildOut);

        }


        private static string Compile(ConsoleOutInterfaceSettings cOutSettings)
        {
            if(!File.Exists(TempFile))
            {
                Directory.CreateDirectory( TempSrc ); //Make Sure Temp Directory Exists

                //Prepare Source
                //  Insert Quotation Marks and console pin addresses
                string src = s_Source.Replace( "{QUOT}", "\"" ).
                                      Replace( "{C_OUT_ADDR}", cOutSettings.WriteOutputAddress.ToString() ).
                                      Replace( "{C_CLEAR_ADDR}", cOutSettings.InterfaceClearPin.ToString() );

                //Write to disk
                File.WriteAllText( TempFile, src );
            }
            //Compile
            return CompilerHelper.Compile(
                TempFile,
                TempBuild,
                TempInternalBuild,
                false,                     //Does not clean temp files, useful for debugging
                new[] { "HL-expr", "bin" } //Build Steps, last step has to be "bin" to generate .vbin binary
            );
        }


        private static void Run(string file)
        {
            //Build CPU instance
            Cpu instance = new CpuInstanceBuilder()
                           .WithPeripherals( //Add Console Output and Memory(RAM)
                               new ConsoleOutInterface(),
                               new Memory()
                               )
                           .Build();

            //Load Compiled Binary from File
            uint[] binary = File.ReadAllBytes(file).ToUInt();

            //Write to CPU Memory Bus
            instance.LoadBinary(binary);

            //Run the Compiled Binary
            instance.Run();

        }

        #endregion
    }

}
