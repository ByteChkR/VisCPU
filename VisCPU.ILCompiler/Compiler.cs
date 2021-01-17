using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

using VisCPU.Instructions;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;

namespace VisCPU.ILCompiler
{

    internal class Compiler
    {

        #region Public

        public static byte[] Base64Decode( string base64EncodedData )
        {
            return Convert.FromBase64String( base64EncodedData );
        }

        public static string Base64Encode( byte[] data )
        {
            return Convert.ToBase64String( data );
        }

        public byte[] Compile( string filepath, string name )
        {
            Console.WriteLine( $"Starting compilation of: '{filepath}'" );

            string sourceCode = File.ReadAllText( "./resources/SelfRunTemplate.txt" ).
                                     Replace( "$$$$CODEHERE$$$$", Base64Encode( File.ReadAllBytes( filepath ) ) );

            using ( MemoryStream peStream = new MemoryStream() )
            {
                EmitResult result = GenerateCode( name, sourceCode ).Emit( peStream );

                if ( !result.Success )
                {
                    Console.WriteLine( "Compilation done with error." );

                    IEnumerable < Diagnostic > failures =
                        result.Diagnostics.Where(
                                                 diagnostic =>
                                                     diagnostic.IsWarningAsError ||
                                                     diagnostic.Severity == DiagnosticSeverity.Error
                                                );

                    foreach ( Diagnostic diagnostic in failures )
                    {
                        Console.Error.WriteLine( "{0}: {1}", diagnostic.Id, diagnostic.GetMessage() );
                    }

                    return null;
                }

                Console.WriteLine( "Compilation done without any error." );

                peStream.Seek( 0, SeekOrigin.Begin );

                return peStream.ToArray();
            }
        }

        #endregion

        #region Private

        private static CSharpCompilation GenerateCode( string name, string sourceCode )
        {
            SourceText codeString = SourceText.From( sourceCode );
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion( LanguageVersion.CSharp9 );

            SyntaxTree parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree( codeString, options );

            List < MetadataReference > references = new List < MetadataReference >
                                                    {
                                                        MetadataReference.CreateFromFile(
                                                             typeof( object ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( CPU ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( File ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( UIntExtensions ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( Memory ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( DefaultSet ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( Console ).Assembly.Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( AssemblyTargetedPatchBandAttribute ).Assembly.
                                                                 Location
                                                            ),
                                                        MetadataReference.CreateFromFile(
                                                             typeof( CSharpArgumentInfo ).Assembly.Location
                                                            ),
                                                    };

            List < Assembly > asms = AppDomain.CurrentDomain.GetAssemblies().ToList();

            Assembly.GetEntryAssembly().
                     GetReferencedAssemblies().
                     Concat( asms.Where( x => x.GetName().Name == "netstandard" ).Select( x => x.GetName() ) ).
                     ToList().
                     ForEach( x => references.Add( MetadataReference.CreateFromFile( Assembly.Load( x ).Location ) ) );

            CSharpCompilationOptions copts = new CSharpCompilationOptions(
                                                                          OutputKind.ConsoleApplication,
                                                                          optimizationLevel: OptimizationLevel.Release,
                                                                          assemblyIdentityComparer:
                                                                          DesktopAssemblyIdentityComparer.Default
                                                                         );

            return CSharpCompilation.Create(
                                            name,
                                            new[] { parsedSyntaxTree },
                                            references,
                                            copts
                                           );
        }

        #endregion

    }

}
