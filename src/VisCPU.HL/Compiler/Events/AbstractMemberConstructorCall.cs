using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class AbstractConstructorCallEvent: ErrorEvent
    {

        public AbstractConstructorCallEvent( HlTypeDefinition tdef ) : base(
                                                                            $"The constructor of type '{tdef.Name}' can not be called as constructor type.\nCall the constructor as static function e.g. '{tdef.Name}.{tdef.Name}(arguments);' or '{tdef.Name}.base(arguments);'",
                                                                            ErrorEventKeys.s_AbstractConstructorCall,
                                                                            false
                                                                           )
        {
        }

    }

}
