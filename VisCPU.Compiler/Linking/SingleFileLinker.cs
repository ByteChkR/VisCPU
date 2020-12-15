using System;
using System.Linq;

using VisCPU.Compiler.Compiler;
using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Linking
{
    public class FileReferencesUnsupportedEvent:ErrorEvent
    {

        private const string EVENT_KEY = "lnk-file-ref-unsupported";
        public FileReferencesUnsupportedEvent( ) : base("Single file linker does not support file references.", EVENT_KEY, false )
        {
        }

    }
    
    public class SingleFileLinker : Linker
    {

        public override LinkerResult Link(LinkerTarget target, Compilation compilation)
        {
            if (target.FileCompilation.FileReferences.Count != 0)
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

    }
}