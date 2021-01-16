using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

using VisCPU.Instructions;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;

namespace VisCPU.ILCompiler
{

    internal class Program
    {

        private static void Main( string[] args )
        {
            Compiler c = new Compiler();
            string name = Path.GetFileNameWithoutExtension( args[0] );
            string outP = Path.Combine(
                                       Path.GetDirectoryName( args[0] ),
                                       name + ".exe"
                                      );
            byte[] asm = c.Compile(args[0] , name);
            File.WriteAllBytes( outP ,asm );

            string runtimeConfig = @"{
  ""runtimeOptions"": {
    ""tfm"": ""net5.0"",
    ""framework"": {
      ""name"": ""Microsoft.NETCore.App"",
      ""version"": ""5.0.0""
    }
  }
}";

            File.WriteAllText(Path.Combine(
                                           Path.GetDirectoryName(args[0]),
                                           name + ".runtimeconfig.json"
                                          ) , runtimeConfig );
        }

    }

    internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public SimpleUnloadableAssemblyLoadContext()
            : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }

    internal class Runner
    {
        public void Execute(byte[] compiledAssembly, string[] args)
        {
            WeakReference assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, args);

            for (int i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] compiledAssembly, string[] args)
        {
            using (MemoryStream asm = new MemoryStream(compiledAssembly))
            {
                SimpleUnloadableAssemblyLoadContext assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                Assembly assembly = assemblyLoadContext.LoadFromStream(asm);

                MethodInfo? entry = assembly.EntryPoint;

                _ = entry != null && entry.GetParameters().Length > 0
                        ? entry.Invoke(null, new object[] { args })
                        : entry.Invoke(null, null);

                assemblyLoadContext.Unload();

                return new WeakReference(assemblyLoadContext);
            }
        }
    }

    internal class Compiler
    {
        public byte[] Compile(string filepath, string name)
        {
            Console.WriteLine($"Starting compilation of: '{filepath}'");

            string sourceCode = File.ReadAllText( "./resources/SelfRunTemplate.txt" ).
                                     Replace( "$$$$CODEHERE$$$$", Base64Encode( File.ReadAllBytes( filepath ) ) );

            using (MemoryStream peStream = new MemoryStream())
            {
                EmitResult result = GenerateCode(name,sourceCode).Emit(peStream);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation done with error.");

                    IEnumerable < Diagnostic > failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                Console.WriteLine("Compilation done without any error.");

                peStream.Seek(0, SeekOrigin.Begin);

                return peStream.ToArray();
            }
        }
        public static string Base64Encode(byte[] data)
        {
            return System.Convert.ToBase64String(data);
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
        }
        private static CSharpCompilation GenerateCode(string name, string sourceCode)
        {
            SourceText codeString = SourceText.From(sourceCode);
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
            

            SyntaxTree parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);


            List< MetadataReference> references = new List <MetadataReference>
                                                 {
                                                     MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(CPU).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(File).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(UIntExtensions).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(Memory).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(DefaultSet).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                                                     MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                                                 };

            List < Assembly > asms = AppDomain.CurrentDomain.GetAssemblies().ToList();

            Assembly.GetEntryAssembly().
                     GetReferencedAssemblies().
                     Concat( asms.Where( x => x.GetName().Name == "netstandard").Select( x => x.GetName() ) ).
                     ToList().
                     ForEach( x => references.Add( MetadataReference.CreateFromFile( Assembly.Load( x ).Location ) ) );

            CSharpCompilationOptions copts = new CSharpCompilationOptions(
                                                                          OutputKind.ConsoleApplication,
                                                                          optimizationLevel: OptimizationLevel.Release,
                                                                          assemblyIdentityComparer:
                                                                          DesktopAssemblyIdentityComparer.Default
                                                                         );

            return CSharpCompilation.Create(name,
                                            new[] { parsedSyntaxTree },
                                            references, copts
                                           );
        }
    }

}