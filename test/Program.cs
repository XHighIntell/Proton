using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Test {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) => {

                var filename = e.Name.Split(',')[0] + ".dll";
                var fullPath = Path.GetFullPath(@"..\..\output\" + filename);


                if (File.Exists(fullPath) == true) return Assembly.LoadFile(fullPath);

                return null;
            };


            Entry();
        }
        static void Entry() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
