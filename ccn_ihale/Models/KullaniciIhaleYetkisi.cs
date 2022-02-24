using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale.Models
{
    public class KullaniciIhaleYetkisi
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int IhalePaketID { get; set; }
        public bool IhaleYetkilisi { get; set; }
    }
}

