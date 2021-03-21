using System;
using System.Collections.Generic;

using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Plugins
{
    public class ChainCollection : IChainCollection
    {

        public string Name { get; } = "Default";

        public List<Type> Chain { get; } = new List<Type>
                                           {
                                               typeof(MultiLinePlugin),
                                               typeof(KeyWordReplacer),
                                               typeof(ConditionalPlugin),
                                               typeof(FakeGenericsPlugin),
                                               typeof(IncludePlugin),
                                               typeof(ExceptionPlugin),
                                               typeof(BlankLineRemover),
                                               typeof(ChangeCharCase),
                                               typeof(TextEncoderPlugin)
                                           };

    }
}