using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale
{
    public class IhaleViewModel 
    {
        public List<Models.Kullanici> kullaniciList { get; set; }
        public List<Models.IhalePaket> ihalepaketList { get; set; }
        public Models.IhalePaket ihalepaket { get; set; }
        public List<Models.SoruCevap> sorucevapList { get; set; }
        public Models.SoruCevap sorucevap { get; set; }
        public List<Models.Project> projeler { get; set; }
        public List<Models.KullaniciIhaleYetkisi> kullaniciihaleyetkilisiList { get; set; }
        public Models.KullaniciIhaleYetkisi kullaniciihaleyetkilisi { get; set; }
        [Required]
        [Display(Name = "IhaleID")]
        public int IhaleID { get; set; }
     
    }
}
