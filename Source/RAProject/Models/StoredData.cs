using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Models
{
    [Serializable]
    public class StoredData
    {
        public List<SupportedConsole> consoles;
        public StoredData()
        {
            consoles = new List<SupportedConsole>();
        }
    }
}
