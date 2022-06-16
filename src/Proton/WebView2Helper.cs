using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton {

    ///<summary>Exposes static methods for creating, helping in using <see cref="Microsoft.Web.WebView2"/>.</summary>
    public static class WebView2Helper {

        ///<summary>Gets the latest browser executable folder available.</summary>
        public static string GetLatestBrowserExecutableFolder(string majorVersion = default) {
            if (majorVersion == null) majorVersion = "";

            var folders = Directory.GetDirectories(@"C:\Program Files (x86)\Microsoft\EdgeCore", majorVersion + "*");

            var latest = folders.OrderByDescending(name => {
                // 1. C:\Program Files (x86)\Microsoft\EdgeCore\102.0.1245.33 => 102.0.1245.33
                // 2. [102, 0, 1234, 33]
                // 3. sum all

                // --1--
                var parts = Path.GetFileName(name).Split('.');
                if (parts.Length == 0) return 0;

                // --2--
                var versions = parts.Select(part => { int.TryParse(part, out int number); return number; }).Take(4).ToArray();

                // --3--
                var version = 0L;
                for (var i = 0; i < versions.Length; i++) version += versions[i] << ((3 - i) * 16);

                return version;
            }).FirstOrDefault();

            return latest;
        }
    }
}
