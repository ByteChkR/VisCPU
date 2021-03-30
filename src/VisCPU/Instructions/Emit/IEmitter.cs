namespace VisCPU.Instructions.Emit
{

    public interface IEmitter
    {
        object Emit( string instructionKey, params string[] arguments );
    }

}
