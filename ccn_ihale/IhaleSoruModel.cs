using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale
{
    public class IhaleSoruModel
    {     
        public List<Models.Kullanici> kullaniciList { get; set; }
        public List<Models.KullaniciIhaleYetkisi> kullaniciIhaleYetkisiList { get; set; }
        public List<Models.IhalePaket> ihalepaketList { get; set; }
        public Models.IhalePaket ihalepaket { get; set; }
        public List<Models.Project> projeler { get; set; }
        public Models.Project proje { get; set; }

        [Required]
        [Display(Name = "ProjectID")]
        public int ProjectID { get; set; }
        [Required]
        [Display(Name = "YapilacagiYer")]
        public string YapilacagiYer { get; set; }
        [Required]
        [Display(Name = "IhaleTarihi")]
        public string IhaleTarihi { get; set; }
        [Required]
        [Display(Name = "SonTeslimTarihi")]
        public string SonTeslimTarihi { get; set; }
        [Required]
        [Display(Name = "IhaleKonu")]
        public string IhaleKonu { get; set; }
        [Required]
        [Display(Name = "IhaleTuru")]
        public string IhaleTuru { get; set; }
        [Required]
        [Display(Name = "FtpDosyaLink")]
        public string FtpDosyaLink { get; set; }
        [Required]
        [Display(Name = "FtpDosyaLink1")]
        public string FtpDosyaLink1 { get; set; }
        [Required]
        [Display(Name = "FtpDosyaLink2")]
        public string FtpDosyaLink2 { get; set; }

        [Required]
        [Display(Name = "DeleteIhaleID")]
        public int DeleteIhaleID { get; set; }

    }
}
