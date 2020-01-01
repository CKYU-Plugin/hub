using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.MAssembly
{
    public class AssemblyModel
    {
        public void Init()
        {
            string source = "";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"link.toroko.rsshub.MAssembly.AssemblyModelTemplete.cs"))
            {
                StreamReader reader = new StreamReader(stream);
                source = reader.ReadToEnd();
            }


        }  
    }
}
