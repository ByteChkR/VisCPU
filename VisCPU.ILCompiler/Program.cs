using System.IO;

namespace VisCPU.ILCompiler
{

    internal class Program
    {

        #region Private

        private static void Main( string[] args )
        {
            Compiler c = new Compiler();
            string name = Path.GetFileNameWithoutExtension( args[0] );

            string outP = Path.Combine(
                                       Path.GetDirectoryName( args[0] ),
                                       name + ".exe"
                                      );

            byte[] asm = c.Compile( args[0], name );
            File.WriteAllBytes( outP, asm );

            string runtimeConfig = @"{
  ""runtimeOptions"": {
    ""tfm"": ""net5.0"",
    ""framework"": {
      ""name"": ""Microsoft.NETCore.App"",
      ""version"": ""5.0.0""
    }
  }
}";

            File.WriteAllText(
                              Path.Combine(
                                           Path.GetDirectoryName( args[0] ),
                                           name + ".runtimeconfig.json"
                                          ),
                              runtimeConfig
                             );
        }

        #endregion

    }

}
