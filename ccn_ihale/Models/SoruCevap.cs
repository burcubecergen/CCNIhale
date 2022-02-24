using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale.Models
{
    public class SoruCevap
    {
        public int ID { get; set; } 
        public int IhalePaketID { get; set; }
        public int SoruNo { get; set; }
        public string Soru { get; set; }
        public string Cevap { get; set; }
        public bool? Aktif { get; set; }
        public int? KullaniciID_Soru { get; set; }
        public DateTime? SoruZamani { get ; set; }//datetime istiyor veri gelirken 
        public int? KullaniciID_Cevap { get; set; }
        public DateTime? CevapZamani { get; set; }
       
    }

}
