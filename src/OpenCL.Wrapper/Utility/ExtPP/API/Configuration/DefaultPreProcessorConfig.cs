using System.Collections.Generic;
using System.Text;

using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Plugins;

namespace Utility.ExtPP.API.Configuration
{
    /// <summary>
    ///     The Default PreProcessor Configuration
    /// </summary>
    public class DefaultPreProcessorConfig : APreProcessorConfig
    {

        private static readonly StringBuilder Sb = new StringBuilder();

        public override string FileExtension => "***";

        protected override List<AbstractPlugin> Plugins =>
            new List<AbstractPlugin>
            {
                new FakeGenericsPlugin(),
                new IncludePlugin(),
                new ConditionalPlugin { EnableDefine = true },
                new ExceptionPlugin(),
                new MultiLinePlugin()
            };

        public override string GetGenericInclude(string filename, string[] genType)
        {
            Sb.Append(" ");
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