using KsDumper11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsDumper11
{
    public class KduProviderSettings
    {
        public List<KduProvider> Providers { get; set; }

        public int DefaultProvider { get; set; } = -1;
    }
}
