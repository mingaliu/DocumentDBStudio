using System;
using System.Net;
using System.Windows.Forms;

namespace Microsoft.Azure.DocumentDBStudio
{
    static class Program
    {
        private static MainForm _mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Program._mainForm = new MainForm();
            Application.Run(Program._mainForm);
        }

        public static MainForm GetMain()
        {
            return Program._mainForm;
        }
    }
}