using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ccn_ihale
{
    public class LoginViewModel
    {
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Lütfen Kullanıcı adınızı giriniz.", MinimumLength = 1)]
        [Display(Name = "LoginName")]
        public string LoginName { get; set; }

        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Sifre")]
        public string Sifre { get; set; }

        [Required]
        public bool RememberMe { get; set; }

    }
}
