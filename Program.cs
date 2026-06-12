using System;
using System.Windows.Forms;

namespace AAC_Optimizer
{
    static class Program
    {
        /// <summary>
        /// De hoofdentree van de applicatie.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
