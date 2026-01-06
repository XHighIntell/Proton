using System.Security.Cryptography;
using System.Text;

namespace ProtonT {
    internal static class Program {
        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new BorderLessProtonForm());
            Application.Run(new RegularForm());
        }
    }
}