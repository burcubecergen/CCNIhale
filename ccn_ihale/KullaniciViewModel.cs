using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale
{
    public class KullaniciViewModel
    {

        public List<Models.Kullanici> kullaniciList { get; set; }
        public Models.Kullanici kulllanici { get; set; }
        public List<Models.KullaniciIhaleYetkisi> kullaniciihaleyetkisiList { get; set; }
        public Models.KullaniciIhaleYetkisi kullaniciihaleyetkisi { get; set; }
        public List<Models.IhalePaket> ihalepaketList { get; set; }
        public Models.IhalePaket ihalepaket { get; set; }
        public List<Models.KullaniciIhaleYetkisi> kullaniciihaleyetkisiListCombo { get; set; }
        public List<int> ihaleIDList { get; set; }
        public List<Models.Project> projectList { get; set; }
        /*public Models.IhalePaket ihalepaket { get; set; }
        */
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Lütfen Kullanıcı adını belirtiniz.", MinimumLength = 1)]
        [Display(Name = "LoginName")]
        public string LoginName { get; set; }
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "AdSoyad")]
        public string AdSoyad { get; set;}
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Sifre")]
        public string Sifre { get; set; }
        [Required]
        [Display(Name = "MailAdres")]
        public string MailAdres { get; set; }
        [Required]
        [Display(Name = "Unvan")]
        public string Unvan { get; set; }
        [Required]
        [Display(Name = "AdminYetki")]
        public bool AdminYetki { get; set; }
        [Required]
        [Display(Name = "IhaleID")]
        public int IhaleID { get; set; }
        [Required]
        [Display(Name = "UserID")]
        public int UserID { get; set; }
        [Required]
        [Display(Name = "IhaleYetki")]
        public bool IhaleYetki { get; set; }

        [Required]
        [Display(Name = "KullaniciIhaleID")]
        public int KullaniciIhaleID { get; set; }

    }
}
