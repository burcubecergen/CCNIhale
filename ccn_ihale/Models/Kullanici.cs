using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string LoginName { get; set; }
        public string Sifre { get; set; }
        public string MailAdres { get; set; }
        public string AdSoyad { get; set; }
        public bool AdminYetki { get; set; }
        public string Unvan { get; set; }
        public bool Aktif { get; set; }
    }
}
