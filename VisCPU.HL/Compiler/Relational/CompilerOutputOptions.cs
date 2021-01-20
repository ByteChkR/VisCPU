namespace VisCPU.HL.Compiler.Relational
{

    public enum CompilerOutputOptions
    {
        Original,  //Vars with temp var
        ReadOnly,  //Vars => no temp var
        ReadWrite, //Vars with temp var
    }

}
