using System;
using System.Windows.Forms;

namespace VisCPU.HL.Forms
{

    internal static class Program
    {

        #region Private

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main( string[] args )
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            HLLiveOutputViewerForm frm =
                args.Length == 0 ? new HLLiveOutputViewerForm() : new HLLiveOutputViewerForm( args[0] );

            Application.Run( frm );
        }

        #endregion

    }

}
