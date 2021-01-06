using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using VisCPU.Compiler.Linking;
using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.Importer
{

    public abstract class AImporter : VisBase, IImporter
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Importer;

        protected string CacheDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        protected AImporter()
        {
            Directory.CreateDirectory(CacheDirectory);
        }

        public abstract bool CanImport(string input);

    }

    //public class InstructionFileImporter : AImporter, IFileImporter
    //{

    //    private string InstructionDirectory => Path.Combine(CacheDirectory, "instruction-src");

    //    private InstructionSet InstructionSet;
    //    public InstructionFileImporter(InstructionSet set)
    //    {
    //        InstructionSet = set;
    //        Directory.CreateDirectory(InstructionDirectory);
    //    }

    //    public override bool CanImport(string input)
    //    {
    //        return input.StartsWith("vasm-bridge");
    //    }

    //    public string ProcessImport(string input)
    //    {
    //        int tagLen = "vasm-bridge".Length + 1;

    //        if (input.Length < tagLen)
    //        {
    //            throw new Exception("Invalid vasm-bridge command");
    //        }
    //        string cmd = input.Remove(0, tagLen);
    //        int argCount = -1;

    //        if (cmd.Contains(' '))
    //        {
    //            string[] s = cmd.Split(' ');
    //            argCount = int.Parse(s[1]);
    //            cmd = s[0];
    //        }

    //        Instruction[] iis = InstructionSet.GetInstructions(cmd);
    //        Instruction target = argCount != -1 ? iis.First(x => x.ArgumentCount == argCount) : iis.First();

    //        string path = Path.Combine(InstructionDirectory, $"{target.Key}_{target.ArgumentCount}.vasm");
    //        if (!File.Exists(path))
    //        {
    //            Log($"Generating File: {path}");

    //            StringBuilder sb = new StringBuilder($"; Generated vasm file for instruction: {target.Key}\n");

    //            sb.AppendLine($":data tmp_ret 0x01");
    //            for (int i = 0; i < target.ArgumentCount; i++)
    //            {
    //                sb.AppendLine($":data arg_{i} 0x01");
    //            }


    //            sb.AppendLine($".I_{target.Key}");

    //            for (int i = 0; i < target.ArgumentCount; i++)
    //            {
    //                sb.AppendLine($"POP arg_{i}");
    //            }

    //            sb.Append($"{target.Key}");

    //            for (int i = 0; i < target.ArgumentCount; i++)
    //            {
    //                sb.Append($" arg_{i}");
    //            }
    //            sb.AppendLine();

    //            sb.AppendLine($"PUSH tmp_ret");
    //            sb.AppendLine($"RET");

    //            File.WriteAllText(path, sb.ToString());

    //            //Generate Source File
    //        }
    //        return path;
    //    }

    //}

    public class LinkerImporter : AImporter, IDataImporter
    {

        public override bool CanImport( string input )
        {
            return input.StartsWith("link");
        }

        public IExternalData[] ProcessImport( string input )
        {
            int tagLen = "link".Length;

            if (input.Length < tagLen)
            {
                throw new Exception("Invalid link command");
            }
            string cmd = input.Remove(0, tagLen);
            uint offset = 0;
            if ( cmd.StartsWith( "-" ) )
            {
                int end = cmd.IndexOf('-', 1);
                offset = uint.Parse( cmd.Substring( 1, end - 1 ) );
                cmd = cmd.Substring( end + 1 );
            }
            LinkerInfo info = LinkerInfo.Load( cmd );

            return info.Labels.ApplyOffset( offset ).
                        Select( x => ( IExternalData ) new LinkedData( x.Key, x.Value ) ).
                        ToArray();
        }

    }

    public class InstructionDataImporter : AImporter, IDataImporter, IFileImporter
    {
        private InstructionSet InstructionSet;
            private string InstructionDirectory => Path.Combine(CacheDirectory, "instruction-src");
        public InstructionDataImporter(InstructionSet set)
        {
            Directory.CreateDirectory(InstructionDirectory);
            InstructionSet = set;
        }
        private Instruction Parse(string input)
        {
            int tagLen = "vasm-bridge".Length + 1;

            if (input.Length < tagLen)
            {
                throw new Exception("Invalid vasm-bridge command");
            }
            string cmd = input.Remove(0, tagLen);

            if ( cmd == "all" )
                return null;
            int argCount = -1;

            if (cmd.Contains(' '))
            {
                string[] s = cmd.Split(' ');
                argCount = int.Parse(s[1]);
                cmd = s[0];
            }

            Instruction[] iis = InstructionSet.GetInstructions(cmd);
            return argCount != -1 ? iis.First(x => x.ArgumentCount == argCount) : iis.First();

        }

        public override bool CanImport(string input)
        {
            return input.StartsWith("vasm-bridge");
        }

        string IFileImporter.ProcessImport( string input )
        {
            Instruction target = Parse(input);

            if ( target == null )
            {
                string allPath = Path.Combine(InstructionDirectory, $"all.vasm");

                if (!File.Exists(allPath))
                {
                    List < string > data = new List < string >();

                    foreach ( Instruction instruction in InstructionSet.GetInstructions() )
                    {
                        data.Add( $":include {(this as IFileImporter).ProcessImport($"vasm-bridge {instruction.Key} {instruction.ArgumentCount}")}");
                    }
                    File.WriteAllLines(allPath, data);
                }
                return allPath;
            }
            
            string path = Path.Combine(InstructionDirectory, $"{target.Key}_{target.ArgumentCount}.vasm");

            if ( !File.Exists( path ) )
            {
                File.WriteAllLines(path,GenerateInstructionData(target));
            }
            return path;
        }

        IExternalData[] IDataImporter.ProcessImport(string input)
        {
            Instruction target = Parse( input );

            if ( target == null )
            {
                List<IExternalData> data = new List<IExternalData>();
                foreach (Instruction instruction in InstructionSet.GetInstructions())
                {
                    data.Add( new FunctionData(
                                                       $"I{instruction.ArgumentCount}_{instruction.Key}",
                                                       true,
                                                       null,
                                                       (int)instruction.ArgumentCount,
                                                       true
                                                      ));
                }
                return data.ToArray();
            }
            IExternalData d = new FunctionData(
                                               $"I{target.ArgumentCount}_{target.Key}",
                                               true,
                                               null,
                                               (int)target.ArgumentCount,
                                               true
                                              );
            return new[] { d };
        }


        private string[] GenerateInstructionData(Instruction target)
        {
            List<string> data = new List<string>();
            Log($"Generating Function for Instruction {target.Key}");
            data.Add( $"; Generated vasm file for instruction: {target.Key}" );

            data.Add($":data tmp_ret 0x01");
            for (int i = 0; i < target.ArgumentCount; i++)
            {
                data.Add($":data arg_{i} 0x01");
                data.Add($":data arg_{i}_v 0x01");
            }


            data.Add($".I{target.ArgumentCount}_{target.Key}");

            for (int i = 0; i < target.ArgumentCount; i++)
            {
                data.Add($"POP arg_{i}");
                data.Add($"DREF arg_{i} arg_{i}_v");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"{target.Key}");

            for (int i = 0; i < target.ArgumentCount; i++)
            {
                sb.Append($" arg_{i}_v");
            }

            data.Add( sb.ToString() );

            for (int i = 0; i < target.ArgumentCount; i++)
            {
                data.Add($"LOAD tmp_ret arg_{i}_v");
                data.Add($"CREF tmp_ret arg_{i}");
            }

            data.Add($"LOAD tmp_ret 0");
            data.Add($"PUSH tmp_ret");
            data.Add($"RET");

            return data.ToArray();
        }
    }

    public interface IImporter
    {

        bool CanImport(string input);

    }

    public interface IFileImporter : IImporter
    {

        string ProcessImport(string input);

    }

    public interface IDataImporter : IImporter
    {

        IExternalData[] ProcessImport(string input);

    }

    public static class ImporterSystem
    {

        private static List<IImporter> importer = new List<IImporter>();

        public static void Add(params IImporter[] imp) => importer.AddRange(imp);
        public static IImporter Get(string input) => importer.FirstOrDefault(x => x.CanImport(input));

    }

}
