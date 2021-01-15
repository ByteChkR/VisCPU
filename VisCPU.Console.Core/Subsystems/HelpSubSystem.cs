using System.Collections.Generic;

using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems
{

    public class HelpSubSystem : ConsoleSubsystem
    {

        private readonly ConsoleSystem m_Owner;

        #region Public

        public HelpSubSystem( ConsoleSystem owner )
        {
            m_Owner = owner;
        }

        public static void WriteSubsystem( string subName, params object[] settings )
        {
            System.Console.WriteLine( $"Arguments: {subName}" );

            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( settings );

            foreach ( string s1 in args )
            {
                System.Console.WriteLine( "\tArg Name: " + s1 );
            }
        }

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > arguments )
        {
            m_Owner.Help();
        }

        #endregion

    }

}
