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
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new HLLiveOutputViewerForm() );
        }

        #endregion

    }

}
