namespace Utility.ExtPP.Base.Interfaces
{
    /// <summary>
    ///     Defines the scheme on how the keys for identifying scripts get created.
    /// </summary>
    /// <param name="var"></param>
    /// <param name="currentPath"></param>
    /// <param name="filePath"></param>
    /// <param name="key"></param>
    /// <param name="pluginCache"></param>
    /// <returns></returns>
    public delegate ImportResult DelKeyComputingScheme(string[] var, string currentPath);
}