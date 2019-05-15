using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UADInstaller
{
    public class Product
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public bool IsViewButtonEnable { get; set; }
        public bool IsButtonEnable { get; set; }
    }
}
