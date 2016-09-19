using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Data
{
    public static class Reset
    {
        public static string GetMessage()
        {
            string prefix = "{R:";

            int v = (byte)Encoding.ASCII.GetBytes(prefix + "*").Sum(x => (int)x);
            string suffix = v.ToString("X2");
            return String.Format("{0}{1},*{2}", prefix, "*", suffix);
        }
    }
}
