using System.Collections.Generic;
using System.Text;

using Utility.ExtPP.API.Configuration;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Plugins;

namespace OpenCL.Wrapper.ExtPP.API
{
    /// <summary>
    ///     The PreProcessor Configuration used for OpenGL and OpenCL files
    /// </summary>
    public class CLPreProcessorConfig : APreProcessorConfig
    {

        private static readonly StringBuilder Sb = new StringBuilder();

        public override string FileExtension => ".cl";

        protected override List<AbstractPlugin> Plugins =>
            new List<AbstractPlugin>
            {
                new FakeGenericsPlugin { Stage = "onload" },
                new IncludePlugin(),
                new ConditionalPlugin { Stage = "onload" },
                new ExceptionPlugin(),
                new MultiLinePlugin()
            };

        public override string GetGenericInclude(string filename, string[] genType)
        {
            Sb.Clear();
            foreach (string gt in genType)
            {
                Sb.Append(gt);
                Sb.Append(' ');
            }


            string gens = Sb.Length == 0 ? "" : Sb.ToString();
            return "#include " + filename + " " + gens;
        }

    }
}