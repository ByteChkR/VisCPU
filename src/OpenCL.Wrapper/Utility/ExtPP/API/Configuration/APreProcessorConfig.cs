using System.Collections.Generic;
using System.Linq;

using Utility.ADL;
using Utility.ExtPP.API.Exceptions;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.API.Configuration
{
    /// <summary>
    ///     Abstract PreProcessor Configuration
    /// </summary>
    public abstract class APreProcessorConfig : ALoggable<LogType>
    {

        protected APreProcessorConfig() : base(ExtPPDebugConfig.Settings)
        {
            Logger.SetSubProjectName(FileExtension);
        }

        public abstract string FileExtension { get; }

        protected abstract List<AbstractPlugin> Plugins { get; }

        public abstract string GetGenericInclude(string filename, string[] genType);

        public string[] Preprocess(IFileContent[] filenames, Dictionary<string, bool> defs)
        {
            PreProcessor pp = new PreProcessor();


            pp.SetFileProcessingChain(Plugins);

            Definitions definitions;
            if (defs == null)
            {
                definitions = new Definitions();
            }
            else
            {
                definitions = new Definitions(defs);
            }

            string[] ret;
            try
            {
                ret = pp.Run(filenames, new Settings(), definitions);
            }
            catch (ProcessorException ex)
            {
                throw
                    new TextProcessingException(
                                                "Could not preprocess file: " +
                                                filenames.FirstOrDefault()?.GetFilePath(),
                                                ex
                                               );
            }

            return ret;
        }

    }
}