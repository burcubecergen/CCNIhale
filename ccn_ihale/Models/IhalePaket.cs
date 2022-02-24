using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale.Models
{
    public class IhalePaket
    {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public string Konu { get; set; }
        public string YapilacagiYer { get; set; }
        public DateTime? IhaleTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public string IhaleTuru { get; set; }
        public string FTPDosyaLink{ get; set; }
        public string FTPDosyaLink1{ get; set; }
        public string FTPDosyaLink2{ get; set; }
    }
}
