using System.Linq;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Linking.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Implementations
{

    public class SingleFileLinker : Linker
    {
        #region Public

        public override LinkerResult Link( LinkerTarget target, Compilation compilation )
        {
            if ( target.FileCompilation.FileReferences.Count != 0 )
            {
                EventManager < ErrorEvent >.SendEvent( new FileReferencesUnsupportedEvent() );
            }

            LinkerResult ret = new LinkerResult(
                new[] { target },
                target.FileCompilation.Constants,
                target.FileCompilation.Labels,
                target.FileCompilation.DataSectionHeader,
                target.FileCompilation.Tokens.ToList(),
                target.FileCompilation.DataSection.ToArray()
            );

            return ret;
        }

        #endregion
    }

}
