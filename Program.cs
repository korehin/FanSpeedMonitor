using System;
using System.Windows.Forms;

namespace MSI_Claw_Fan_PRM
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new MainForm());
        }
    }
}