using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace VpnClientApp
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
