namespace VisCPU.HL.Compiler.Relational
{

    public enum CompilerOutputOptions
    {

        ORIGINAL,   //Vars with temp var
        READ_ONLY,  //Vars => no temp var
        READ_WRITE, //Vars with temp var

    }

}
