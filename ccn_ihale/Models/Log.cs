using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale.Models
{
    public class Log
    {
        public int ID { get; set; }
        public string Mesaj { get; set; }
        public DateTime Tarih { get; set; }
        public int? Kullanici_ID { get; set; }
        public string LoginName { get; set; }
    }
}
