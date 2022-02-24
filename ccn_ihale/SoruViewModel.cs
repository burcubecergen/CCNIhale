using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale
{
    public class SoruViewModel
    {
        public List<Models.Kullanici> kullaniciList { get; set; }
        public Models.Kullanici kullanici { get; set; }
        public List<Models.IhalePaket> ihalepaketList { get; set; }
        public List<Models.Project> projectList { get; set; }
        public Models.SoruCevap soru { get; set; }  
        public List<Models.KullaniciIhaleYetkisi> kullaniciihaleyetkilisiList { get; set; }
     

        [Required]
        [Display(Name = "Soru")]
        public string SoruPost { get; set; }
        [Required]
        [Display(Name = "IhaleID")]
        public int IhaleID { get; set; }
    }
}
