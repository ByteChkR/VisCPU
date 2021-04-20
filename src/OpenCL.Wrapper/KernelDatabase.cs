using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenCL.Wrapper.ExtPP.API;
using OpenCL.Wrapper.TypeEnums;

using Utility.ADL;
using Utility.ExtPP.API;

namespace OpenCL.Wrapper
{

    /// <summary>
    ///     A class used to store and manage Kernels
    /// </summary>
    public class KernelDatabase : ALoggable < LogType >, IDisposable
    {

        /// <summary>
        ///     The currently loaded kernels
        /// </summary>
        private readonly Dictionary < string, CLKernel > loadedKernels;

        private readonly List < CLProgram > loadedPrograms;

        public IEnumerable < CLProgram > Programs => loadedPrograms;

        public int ProgramCount => loadedPrograms.Count;

        public int KernelCount => loadedKernels.Count;

        public List < string > KernelNames => new List < string >( loadedKernels.Keys );

        public string GenDataType { get; }

        #region Public

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="folderName">Folder name where the kernels are located</param>
        /// <param name="genDataVectorType">The DataVectorTypes used to compile the FL Database</param>
        public KernelDatabase( CLAPI instance, string folderName, DataVectorTypes genDataVectorType ) : base(
             OpenCLDebugConfig.Settings,
             "DB"
            )
        {
            GenDataType = KernelParameter.GetDataString( genDataVectorType );

            loadedPrograms = new List < CLProgram >();
            loadedKernels = new Dictionary < string, CLKernel >();

            if ( Directory.Exists( folderName ) )
            {
                LoadFolder( instance, folderName );
            }
        }

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="folderName">Folder name where the kernels are located</param>
        /// <param name="genDataVectorType">The DataVectorTypes used to compile the FL Database</param>
        public KernelDatabase( DataVectorTypes genDataVectorType ) : base(
                                                                          OpenCLDebugConfig.Settings,
                                                                          "DB"
                                                                         )
        {
            GenDataType = KernelParameter.GetDataString( genDataVectorType );
            loadedPrograms = new List < CLProgram >();
            loadedKernels = new Dictionary < string, CLKernel >();
        }

        public CLProgram AddProgram(
            CLAPI instance,
            string source,
            string filePath,
            bool throwEx,
            out CLProgramBuildResult ex )
        {
            ex = new CLProgramBuildResult( filePath, "", new List < CLProgramBuildError >() );
            CLProgramBuildResult br = CLProgram.TryBuildProgram( instance, source, filePath, out CLProgram program );
            ex.Source = br.Source;

            if ( !br )
            {
                if ( throwEx )
                {
                    throw br.GetAggregateException();
                }

                ex.BuildErrors.AddRange( br.BuildErrors );

                return null;
            }

            loadedPrograms.Add( program );

            foreach ( KeyValuePair < string, CLKernel > containedKernel in program.ContainedKernels )
            {
                if ( !loadedKernels.ContainsKey( containedKernel.Key ) )
                {
                    Logger.Log( LogType.Log, "Adding Kernel: " + containedKernel.Key, 4 );
                    loadedKernels.Add( containedKernel.Key, containedKernel.Value );
                }
                else
                {
                    Logger.Log(
                               LogType.Log,
                               "Kernel with name: " + containedKernel.Key + " is already loaded. Skipping...",
                               5
                              );
                }
            }

            return program;
        }

        public CLProgram AddProgram( CLAPI instance, string file, bool throwEx, out CLProgramBuildResult ex )
        {
            ex = new CLProgramBuildResult( file, "", new List < CLProgramBuildError >() );

            if ( !File.Exists( file ) )
            {
                Exception e = new FileNotFoundException( "File not found: " + file );

                if ( throwEx )
                {
                    throw e;
                }

                ex.BuildErrors.Add( new CLProgramBuildError( ErrorType.ProgramBuild, e ) );
            }

            string path = file;

            Logger.Log( LogType.Log, "Creating CLProgram from file: " + file, 3 );
            CLProgramBuildResult br = CLProgram.TryBuildProgram( instance, path, out CLProgram program );
            ex.Source = br.Source;

            if ( !br )
            {
                if ( throwEx )
                {
                    throw br.GetAggregateException();
                }

                ex.BuildErrors.AddRange( br.BuildErrors );

                return null;
            }

            loadedPrograms.Add( program );

            foreach ( KeyValuePair < string, CLKernel > containedKernel in program.ContainedKernels )
            {
                if ( !loadedKernels.ContainsKey( containedKernel.Key ) )
                {
                    Logger.Log( LogType.Log, "Adding Kernel: " + containedKernel.Key, 4 );
                    loadedKernels.Add( containedKernel.Key, containedKernel.Value );
                }
                else
                {
                    Logger.Log(
                               LogType.Log,
                               "Kernel with name: " + containedKernel.Key + " is already loaded. Skipping...",
                               5
                              );
                }
            }

            return program;
        }

        /// <summary>
        ///     Manually adds a Program to the database
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="file">Path fo the file</param>
        public void AddProgram( CLAPI instance, string file )
        {
            AddProgram( instance, file, true, out CLProgramBuildResult _ );
        }

        public void Dispose()
        {
            foreach ( KeyValuePair < string, CLKernel > loadedKernel in loadedKernels )
            {
                loadedKernel.Value?.Dispose();
            }

            loadedKernels.Clear();

            foreach ( CLProgram loadedProgram in loadedPrograms )
            {
                loadedProgram?.Dispose();
            }

            loadedPrograms.Clear();
        }

        public CLKernel GetClKernel( string name )
        {
            return loadedKernels[name];
        }

        public CLProgram GetProgram( CLKernel containingKernel )
        {
            return loadedPrograms.First( x => x.ContainedKernels.ContainsValue( containingKernel ) );
        }

        /// <summary>
        ///     Initializes the Kernel Database
        /// </summary>
        public void LoadFolder( CLAPI instance, string folderName )
        {
            string[] files = Directory.GetFiles( folderName, "*.cl", SearchOption.AllDirectories );

            List < CLProgramBuildResult > results = new List < CLProgramBuildResult >();
            bool throwEx = false;

            foreach ( string file in files )
            {
                AddProgram( instance, file, false, out CLProgramBuildResult res );
                throwEx |= !res;
                results.Add( res );
            }

            if ( throwEx )
            {
                throw new CLBuildException( results );
            }
        }

        /// <summary>
        ///     Tries to get the CLKernel by the specified name
        /// </summary>
        /// <param name="name">The name of the kernel</param>
        /// <param name="kernel">The kernel. Null if not found</param>
        /// <returns>Returns True if the kernel has been found</returns>
        public bool TryGetClKernel( string name, out CLKernel kernel )
        {
            if ( loadedKernels.ContainsKey( name ) )
            {
                kernel = loadedKernels[name];

                return true;
            }

            kernel = null;

            return false;
        }

        #endregion

        #region Private

        static KernelDatabase()
        {
            TextProcessorAPI.Configs[".cl"] = new CLPreProcessorConfig();
            TextProcessorAPI.Configs[".cle"] = new CLPreProcessorConfig();
        }

        #endregion

    }

}
