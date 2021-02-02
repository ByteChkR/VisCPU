using System;
using System.Linq;

using Examples.Shared;

using VisCPU;
using VisCPU.Dynamic;
using VisCPU.Utility;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace Examples.Dynamic
{

    internal class Program
    {

        #region Private

        private static void ConsoleLoop( string binary )
        {
            Console.WriteLine( "Interactive Console:" );
            string input = null;

            Cpu instance = VisHelper.Default( binary );
            instance.SymbolServer.LoadSymbols( binary );
            DynamicCpuWrapper wrapper = new DynamicCpuWrapper( instance );

            while ( input != "exit" )
            {
                if ( !string.IsNullOrEmpty( input ) )
                {
                    if ( input == "help" )
                    {
                        WriteHelp();
                    }
                    else if ( input.StartsWith( "call " ) )
                    {
                        string[] funcParts = input.Remove( 0, "call ".Length ).
                                                   Split( ' ', StringSplitOptions.RemoveEmptyEntries );

                        if ( funcParts.Length != 0 )
                        {
                            uint returnValue = wrapper.InvokeLabel(
                                                                   funcParts[0],
                                                                   funcParts.Skip( 1 ).
                                                                             Select( x => x.ParseUInt() ).
                                                                             Cast < object >().
                                                                             ToArray()
                                                                  );

                            Console.WriteLine(
                                              $"Function '{funcParts[0]}' returned '{returnValue}'({returnValue.ToHexString()})"
                                             );
                        }
                        else
                        {
                            Console.WriteLine(
                                              "Invalid usage.\nUsage: call <FunctionName> <Argument0> <Argument1> <....>"
                                             );
                        }
                    }
                    else if ( input.StartsWith( "get " ) )
                    {
                        string name = input.Remove( 0, "get ".Length );

                        if ( name.Length != 0 )
                        {
                            uint val = wrapper.GetDataValue( name );
                            Console.WriteLine( $"Variable '{name}': {val}({val.ToHexString()})" );
                        }
                        else
                        {
                            Console.WriteLine(
                                              "Invalid usage.\nUsage: get <VariableName>"
                                             );
                        }
                    }
                    else if ( input.StartsWith( "set " ) )
                    {
                        string[] parts = input.Remove( 0, "set ".Length ).
                                               Split( ' ', StringSplitOptions.RemoveEmptyEntries );

                        if ( parts.Length == 2 )
                        {
                            string name = parts[0];
                            string value = parts[1];
                            uint num = value.ParseUInt();
                            wrapper.SetDataValue( name, num );
                            Console.WriteLine( $"Variable '{name}': {num}({num.ToHexString()})" );
                        }
                        else
                        {
                            Console.WriteLine(
                                              "Invalid usage.\nUsage: set <VariableName>"
                                             );
                        }
                    }
                    else if ( input.StartsWith( "const " ) )
                    {
                        string name = input.Remove( 0, "const ".Length );

                        if ( name.Length != 0 )
                        {
                            uint val = wrapper.GetDataValue( name );
                            Console.WriteLine( $"Constant '{name}': {val}({val.ToHexString()})" );
                        }
                        else
                        {
                            Console.WriteLine(
                                              "Invalid usage.\nUsage: const <ConstantName>"
                                             );
                        }
                    }
                    else if ( input.StartsWith( "list " ) )
                    {
                        string type = input.Remove( 0, "list ".Length );

                        if ( type.Length != 0 )
                        {
                            Console.WriteLine( $"Viewing Entries for '{type}'" );

                            if ( type == "functions" )
                            {
                                foreach ( string wrapperLabel in wrapper.Labels )
                                {
                                    AddressItem item = wrapper.GetLabel( wrapperLabel );
                                    Console.WriteLine( $"'{wrapperLabel}' at address {item.Address.ToHexString()}" );
                                }
                            }
                            else if ( type == "const" )
                            {
                                foreach ( string wrapperLabel in wrapper.Constants )
                                {
                                    AddressItem item = wrapper.GetConstant( wrapperLabel );

                                    Console.WriteLine(
                                                      $"'{wrapperLabel}': {item.Address}({item.Address.ToHexString()})"
                                                     );
                                }
                            }
                            else if ( type == "variables" )
                            {
                                foreach ( string wrapperLabel in wrapper.Data )
                                {
                                    AddressItem item = wrapper.GetData( wrapperLabel );
                                    Console.WriteLine( $"'{wrapperLabel}' at address {item.Address.ToHexString()}" );
                                }
                            }
                            else
                            {
                                Console.WriteLine( $"Type not found: '{type}'" );
                                Console.WriteLine( $"Available Types: \n\t'functions'\n\t'const'\n\t'variables'" );
                            }
                        }
                        else
                        {
                            Console.WriteLine(
                                              "Invalid usage.\nUsage: const <ConstantName>"
                                             );
                        }
                    }
                    else
                    {
                        Console.WriteLine( $"Invalid Command: '{input}'" );
                        WriteHelp();
                    }
                }
                else
                {
                    if ( input != null )
                    {
                        Console.WriteLine( $"Invalid Command: '{input}'" );
                    }

                    WriteHelp();
                }

                Console.Write( "Dynamic Console >>> " );
                input = Console.ReadLine();
            }
        }

        private static void Main( string[] args )
        {
            bool compile = args.Any( x => x == "-c" );
            bool clean = args.Any( x => x == "-clean" );
            bool run = args.Any( x => x == "-r" );

            FirstSetup.Start();

            string output = null;

            output = compile ? VisHelper.Compile( clean ) : FirstSetup.DefaultFile;

            if ( run )
            {
                if ( output == null )
                {
                    Console.WriteLine( $"Output file error." );

                    FirstSetup.End( EndOptions.Default );

                    return;
                }

                ConsoleLoop( output );
            }

            FirstSetup.End( EndOptions.Default );
        }

        private static void WriteHelp()
        {
            Console.WriteLine( "Available Commands:" );
            Console.WriteLine( "\tcall <FunctionName> : Invokes a Function" );
            Console.WriteLine( "\tget <VariableName> : Displays the current value of <VariableName>" );
            Console.WriteLine( "\tset <VariableName> <NewValue> : Changes the Value of <VariableName>" );
            Console.WriteLine( "\tconst <ConstantName> : Displays the constant value of <ConstantName>" );
            Console.WriteLine( "\tlist functions : Lists all Invokable Functions" );
            Console.WriteLine( "\tlist variables : Lists all Variables" );
            Console.WriteLine( "\tlist const : Lists all Constants" );
        }

        #endregion

    }

}
