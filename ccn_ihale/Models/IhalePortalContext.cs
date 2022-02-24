using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ccn_ihale.Models
{
    public class IhalePortalContext : DbContext
    {
        public IhalePortalContext(DbContextOptions<IhalePortalContext> options)
           : base(options)
        { }
        public DbSet<IhalePaket> IhalePaket { get; set; }
        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<KullaniciIhaleYetkisi> KullaniciIhaleYetkisi { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<SoruCevap> SoruCevap { get; set; }

   
    }
  
}
