using System.Reflection;
using System.Runtime.Loader;

namespace VisCPU.ILCompiler
{

    internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {

        #region Public

        public SimpleUnloadableAssemblyLoadContext()
            : base( true )
        {
        }

        #endregion

        #region Protected

        protected override Assembly Load( AssemblyName assemblyName )
        {
            return null;
        }

        #endregion

    }

}
