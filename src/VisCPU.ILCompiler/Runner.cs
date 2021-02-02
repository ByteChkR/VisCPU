#nullable enable
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VisCPU.ILCompiler
{

    internal class Runner
    {
        #region Public

        public void Execute( byte[] compiledAssembly, string[] args )
        {
            WeakReference assemblyLoadContextWeakRef = LoadAndExecute( compiledAssembly, args );

            for ( int i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++ )
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine( assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!" );
        }

        #endregion

        #region Private

        [MethodImpl( MethodImplOptions.NoInlining )]
        private static WeakReference LoadAndExecute( byte[] compiledAssembly, string[] args )
        {
            using ( MemoryStream asm = new MemoryStream( compiledAssembly ) )
            {
                SimpleUnloadableAssemblyLoadContext assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                Assembly assembly = assemblyLoadContext.LoadFromStream( asm );

                MethodInfo? entry = assembly.EntryPoint;

                _ = entry != null && entry.GetParameters().Length > 0
                    ? entry.Invoke( null, new object[] { args } )
                    : entry.Invoke( null, null );

                assemblyLoadContext.Unload();

                return new WeakReference( assemblyLoadContext );
            }
        }

        #endregion
    }

}
