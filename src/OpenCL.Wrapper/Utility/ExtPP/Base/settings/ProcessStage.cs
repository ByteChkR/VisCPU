namespace Utility.ExtPP.Base.settings
{
    /// <summary>
    ///     An enum that contains all possible process stages.
    /// </summary>
    public enum ProcessStage
    {

        Queued = 0,
        OnLoadStage = 1,
        OnMain = 2,
        OnFinishUp = 4,
        Finished = 8

    }
}