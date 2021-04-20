using System;

namespace VisCPU.HL.DataTypes
{

    [Flags]
    public enum VariableDataEmitFlags
    {

        None = 0,
        CStyle = 1,
        Packed = 2,
        Visible = 4,
        Pointer = 8,

    }

}
