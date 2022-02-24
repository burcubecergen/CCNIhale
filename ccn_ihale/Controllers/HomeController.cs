using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ccn_ihale.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ccn_ihale.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    { 
        IhalePortalContext context;
        IhaleViewModel ivm;
        IhaleSoruModel ism;
        KullaniciViewModel kvm;
        SoruViewModel svm;
        DateTime myDateTime;
        SoruCevap sr;
        Kullanici ku;
        DateTime? TeslimTarihidt;
        DateTime? IhaleTarihidt;

        public HomeController(IhalePortalContext ctx)//modellerimiz üzerinden veritabanımızda bulunan verileri homecontroller uzerinde islemek için kurucu methoda atıyoruz.
        {
            context = ctx;
        }

        private string sessionLoginNameControl()//oturuma sahip kullanıcının bilgilerini alıyoruz.
        {
            var loginName = HttpContext.Session.GetString("kullaniciAdi");
            return loginName;
        }
        private int sessionLoginIdControl(){
            try { 
                int ?loginID = HttpContext.Session.GetInt32("kullaniciID");
                return loginID.Value;
            }
            catch(Exception e){
                return 0;
            }
        }

        private bool checkAdmin(List<Kullanici> kullaniciList, string loginName)//admin kontrolu yapıyoruz.
        {
            bool adminYetkiBool = false;
            foreach (Kullanici ku in kullaniciList)//foreach duzeltilebiliriz
            {
                if (loginName == ku.LoginName)
                adminYetkiBool = ku.AdminYetki;
                
            }
            return adminYetkiBool;
        }      
        private bool checkIhaleYetki(int userID,int ihaleID,List<KullaniciIhaleYetkisi> kullaniciihaleyetkisiList)//ihale yetki kontrolu yapıyoruz
        {
            bool ihaleYetki = false;
           
            foreach(KullaniciIhaleYetkisi ki in kullaniciihaleyetkisiList){
                if(ki.IhalePaketID == ihaleID && ki.UserID == userID)
                ihaleYetki = ki.IhaleYetkilisi;
            } 
            return ihaleYetki;
        }
        private bool checkIhaleSameihaleKonu(string pIhaleKonu)
        {
            var sameIhaleKonu = context.IhalePaket.Where(ih => ih.Konu == pIhaleKonu).FirstOrDefault();
            if (sameIhaleKonu != null)
             return true;
            else    
            return false;
        }

        private bool checkAuthorizationSameihaleID(int pKullaniciIhaleID, int pIhaleID)
        {
            var sameIhaleID = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == pKullaniciIhaleID && ki.IhalePaketID == pIhaleID).FirstOrDefault();
            if (sameIhaleID != null)
            return true;
            
            else    
            return false;
            
        }

        private bool checkIhaleYetkilisiUrlControl(int pLoginID, IhaleViewModel ihale)
        {
            var ihalepaketControl = context.KullaniciIhaleYetkisi.
                                                                Where(ki => ki.UserID == pLoginID && ki.IhalePaketID == ihale.ihalepaket.ID).FirstOrDefault();
            if (ihalepaketControl != null)
            {
                return true;
            }
            else
                return false;
        }

        private Tuple<string, string> dateTrConvert()//Tuple classını kullanarak datetime formatındaki veriyi string olarak tarih formatına ceviriyor
        {

            if (ivm != null){
                ViewBag.ihaletarihi = ivm.ihalepaket.IhaleTarihi != null ? ivm.ihalepaket.IhaleTarihi.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture): ViewBag.ihaletarihi = ivm.ihalepaket.IhaleTarihi;
                ViewBag.teslimtarihi = ivm.ihalepaket.TeslimTarihi != null ? ivm.ihalepaket.TeslimTarihi.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture):ViewBag.teslimtarihi = ivm.ihalepaket.TeslimTarihi;

                if (ViewBag.ihaletarihi != null && ViewBag.teslimtarihi != null)
                    return Tuple.Create(ViewBag.ihaletarihi, ViewBag.teslimtarihi);
                else
                return null;
            }
            else if  (ism != null)
            {
                ViewBag.ihaletarihi = ism.ihalepaket.IhaleTarihi != null ? ism.ihalepaket.IhaleTarihi.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ViewBag.ihaletarihi = ism.ihalepaket.IhaleTarihi;
                ViewBag.teslimtarihi = ism.ihalepaket.TeslimTarihi != null ? ism.ihalepaket.TeslimTarihi.Value.ToString("dd/MM/yyyy HH:mm ", CultureInfo.InvariantCulture) : ViewBag.teslimtarihi = ism.ihalepaket.TeslimTarihi;

                if (ViewBag.ihaletarihi != null && ViewBag.teslimtarihi != null)
                    return Tuple.Create(ViewBag.ihaletarihi, ViewBag.teslimtarihi);
                else
                return null;
            }
            else
            return null;

        }
        private Tuple<DateTime?, DateTime?> dateIhledittFormatControl(string pIhaleTarihi,string pSonTeslimTarihi)
        {

            if (!String.IsNullOrEmpty(pIhaleTarihi)){
                var ihaleTarihiChanged = pIhaleTarihi.Split('-');
                IhaleTarihidt = ihaleTarihiChanged != null ? Convert.ToDateTime(pIhaleTarihi, CultureInfo.InvariantCulture) : IhaleTarihidt = null;
            }

            if (!String.IsNullOrEmpty(pSonTeslimTarihi)){ 
                var teslimTarihiChanged = pSonTeslimTarihi.Split('-');
                TeslimTarihidt = teslimTarihiChanged != null ? Convert.ToDateTime(pSonTeslimTarihi, CultureInfo.InvariantCulture) : TeslimTarihidt = null;
            }

            return Tuple.Create(IhaleTarihidt, TeslimTarihidt);
        }
        private bool ihlDelete(IhalePaket pIhalepaketDelete, List<KullaniciIhaleYetkisi> pKullaniciIhaleYetkisiDelete,List<SoruCevap> pKullaniciSoruDeleteIhaleList, int? pDeleteID)
        {

            if (pKullaniciSoruDeleteIhaleList.Count == 0)
            {
                context.IhalePaket.Remove(pIhalepaketDelete);
                if (pKullaniciIhaleYetkisiDelete != null)
                {
                    var deleteitemList = context.KullaniciIhaleYetkisi.Where(ki => ki.IhalePaketID == pDeleteID).ToList();
                    foreach (var item in deleteitemList)
                    {
                        context.KullaniciIhaleYetkisi.Remove(item);
                    }
                }
                return true;
            }
            else
            return false;

        }
        
        private void srDelete(SoruCevap pSorucevapDelete)
        {
            if (pSorucevapDelete != null)
            context.SoruCevap.Remove(pSorucevapDelete);
            
        }

        private bool userDelete(Kullanici pKullaniciDelete, List<SoruCevap> pSoruCevapDeleteSoru, List<SoruCevap> pSoruCevapDeleteCevap, List<KullaniciIhaleYetkisi> pKullaniciihaleyetkisiDelete,int? pDeleteID)
        {
            if (pSoruCevapDeleteSoru.Count == 0 && pSoruCevapDeleteCevap.Count == 0)
            {
                if (pKullaniciDelete != null)
                {
                    if (pKullaniciihaleyetkisiDelete != null){
                        context.Kullanici.Remove(pKullaniciDelete);
                        var deleteitemList = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == pDeleteID).ToList();
                        foreach (var item in deleteitemList)
                        {
                            context.KullaniciIhaleYetkisi.Remove(item);
                        }
                    }
                }
                return true;
            }
            else
            return false;
        }

        private void kullaniciCombobox(int pIhaleID, int pUserID, int pLoginID, bool pAdminYetki, bool pIhaleYetki)
        {
            if (pUserID != 0)
            {
                kvm.ihalepaket = context.IhalePaket.Where(ih => ih.ID == pIhaleID).FirstOrDefault();
                if (pAdminYetki)
                {
                    kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.
                                                                            Where(ki => ki.UserID == pUserID).ToList();
                }
                else if (!pAdminYetki && pIhaleYetki)
                {
                    var nonAdminandUser = context.KullaniciIhaleYetkisi.Where(ku => ku.UserID == 1 || ku.UserID == pLoginID);

                    kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.IhalePaketID == pIhaleID).Except(nonAdminandUser).ToList();
                    kvm.kullaniciihaleyetkisiListCombo = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == pLoginID).ToList();
                }
            }
            else
            {
                kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.ToList();
                kvm.ihalepaket = context.IhalePaket.FirstOrDefault();
            }
        }

        private void ihaleCombobox(int pIhaleID, int pLoginID, bool pAdminYetki, bool pIhaleYetki)
        {
            if (pIhaleID != 0)
            {
                kvm.ihalepaket = context.IhalePaket.Where(ih => ih.ID == pIhaleID).FirstOrDefault();
                if (pAdminYetki)
                {
                    kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.
                                                                            Where(ki => ki.IhalePaketID == pIhaleID).ToList();
                }
                else if (!pAdminYetki && pIhaleYetki)
                {
                    var nonAdminandUser = context.KullaniciIhaleYetkisi.Where(ku => ku.UserID == 1 || ku.UserID == pLoginID);

                    kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.IhalePaketID == pIhaleID).Except(nonAdminandUser).ToList();
                    kvm.kullaniciihaleyetkisiListCombo = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == pLoginID).ToList();
                }
            }
            else
            {
                kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.ToList();
                kvm.ihalepaket = context.IhalePaket.FirstOrDefault();
            }
        }

        [HttpGet]
        public IActionResult Ihl(IhaleViewModel ihale,int ?id)
        {
            var loginName = sessionLoginNameControl();
            int loginID = sessionLoginIdControl();
            if (loginName != null && loginID != 0){ 
                ivm = new IhaleViewModel();

                ivm.kullaniciList = context.Kullanici.ToList();
                bool adminYetki = checkAdmin(ivm.kullaniciList,loginName);  
                
                

                ivm.kullaniciihaleyetkilisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).ToList();
                ViewBag.adminYetki = adminYetki;
                ivm.ihalepaketList = context.IhalePaket.ToList();
           
                if (id != null){
                    var sorucevapDelete = context.SoruCevap.Find(id);
                    srDelete(sorucevapDelete);
                    context.SaveChanges();
                }

                if (ihale.ihalepaket != null){
                    if (checkIhaleYetkilisiUrlControl(loginID, ihale)){ 
                        bool ihaleYetki = checkIhaleYetki(loginID, ihale.ihalepaket.ID, ivm.kullaniciihaleyetkilisiList);
                        ViewBag.ihaleYetki = ihaleYetki;
                        ivm.ihalepaket = context.IhalePaket.Where(ih => ih.ID == ihale.ihalepaket.ID).FirstOrDefault();

                        if (ivm.ihalepaket != null)
                        dateTrConvert();
                     
                        ivm.sorucevapList = context.SoruCevap.Where(sr => sr.IhalePaketID == ihale.ihalepaket.ID).ToList();
                    }
                    else{
                        return RedirectToAction("Login", "Account");
                    }
                }   
                else{
                    ivm.kullaniciihaleyetkilisi = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).FirstOrDefault();//null degerde  kabul eder

                    if (ivm.kullaniciihaleyetkilisi != null) { //herhangi bir ihale paketine baglı ise
                        ivm.ihalepaket = context.IhalePaket.Where(ih => ih.ID == ivm.kullaniciihaleyetkilisi.IhalePaketID).First();
                        bool ihaleYetki = checkIhaleYetki(loginID,ivm.kullaniciihaleyetkilisi.IhalePaketID, ivm.kullaniciihaleyetkilisiList);
                        ViewBag.ihaleYetki = ihaleYetki;
                    }
                    else{
                        bool ihaleYetki = false;
                        ViewBag.ihaleYetki = ihaleYetki;
                    }

                    if (ivm.ihalepaket != null) //ihalepaket hic olmadıgında kontrol 
                    dateTrConvert();
                    
                    ivm.sorucevapList = context.SoruCevap.ToList();
                }

                ivm.projeler = context.Project.ToList();
                ViewBag.ihalePaket = ivm.ihalepaket;

                return View(ivm);
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
        [HttpPost]
        public IActionResult Ihl(int quno,int ?quid)
        {
            var loginName = sessionLoginNameControl();
            if(loginName != null){
                if (quno != 0){
                    if (quid != null){
                        var toUpdateQu = context.SoruCevap.Find(quid);
                        toUpdateQu.SoruNo = quno;
                        context.Update(toUpdateQu);
                        context.SaveChanges();
                        return RedirectToAction("Ihl", "Home");
                    }
                    else    
                    return RedirectToAction("Login", "Account");
                    
                }
                else
                return RedirectToAction("Login", "Account");
            }
            else
            return RedirectToAction("Login", "Account");
            
       
        }
        public IActionResult Ihladd(bool error,bool topicerror)
        {
            var loginName = sessionLoginNameControl();
            if(loginName != null) { 
                ism = new IhaleSoruModel();
                ism.kullaniciList = context.Kullanici.ToList();
                bool adminYetki = checkAdmin(ism.kullaniciList, loginName);
                ViewBag.adminYetki = adminYetki;
                if (adminYetki != false) {
                    ism.ihalepaket = context.IhalePaket.FirstOrDefault();
                    ism.ihalepaketList = context.IhalePaket.ToList();
                    ism.projeler = context.Project.ToList();
                    ism.proje = context.Project.FirstOrDefault();
                    ViewBag.ihalePaket = ism.ihalepaket;
                }
                else
                return View();
                
            }
            else
            return RedirectToAction("Login", "Account");
            
            if (error){
                ViewBag.sign = "*";
                ViewBag.error = "Yıldızlı alanlar boş bırakılamaz.";
            }
            else if (topicerror)
            ViewBag.error = "Bu isimde bir ihale konusu vardır.";
            
            return View(ism);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IhlAdd(IhaleSoruModel ism)
        {
            var loginName = sessionLoginNameControl();
            
            IhaleTarihidt = String.IsNullOrEmpty(ism.IhaleTarihi)  ? IhaleTarihidt = null: Convert.ToDateTime(ism.IhaleTarihi, CultureInfo.InvariantCulture);
            TeslimTarihidt = String.IsNullOrEmpty(ism.SonTeslimTarihi)  ? TeslimTarihidt = null: Convert.ToDateTime(ism.SonTeslimTarihi, CultureInfo.InvariantCulture);

            if (loginName != null){
                if (!String.IsNullOrEmpty(ism.IhaleKonu) && !String.IsNullOrEmpty(ism.YapilacagiYer) && !String.IsNullOrEmpty(ism.IhaleTuru)) {
                    if (!checkIhaleSameihaleKonu(ism.IhaleKonu)){
                        context.IhalePaket.Add(new IhalePaket { ProjectID = ism.ProjectID, Konu = ism.IhaleKonu, YapilacagiYer =ism.YapilacagiYer,
                                                                IhaleTarihi = IhaleTarihidt, TeslimTarihi =TeslimTarihidt, IhaleTuru = ism.IhaleTuru,
                                                                FTPDosyaLink = ism.FtpDosyaLink,FTPDosyaLink1 = ism.FtpDosyaLink1,FTPDosyaLink2 = ism.FtpDosyaLink2});
                        context.SaveChanges();
                        return RedirectToAction("Ihl", new { LoginName = loginName });
                    }
                    else
                    return RedirectToAction("IhlAdd", new { topicerror = true });
                   
                }
                else
                return RedirectToAction("IhlAdd",new {error = true});
                
                
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
        
        [HttpGet]
        public IActionResult Ihleditt(int id,int ?deleteid,bool sign,bool error)
        {
            if (sign && error) { 
                ViewBag.sign = "*";
                ViewBag.error = "Yıldızlı alanlar boş bırakılamaz";
            }

            var loginName = sessionLoginNameControl();
            var loginID = sessionLoginIdControl();
            if(loginName != null){
                ism = new IhaleSoruModel();
              
                ism.kullaniciList = context.Kullanici.ToList();
                ism.kullaniciIhaleYetkisiList = context.KullaniciIhaleYetkisi.ToList();
                bool adminYetki = checkAdmin(ism.kullaniciList, loginName);
                bool ihaleYetki = checkIhaleYetki(loginID, id, ism.kullaniciIhaleYetkisiList);
                ViewBag.adminYetki = adminYetki;
                ViewBag.ihaleYetki = ihaleYetki;
                if(adminYetki || ihaleYetki){
                    if(deleteid != null && adminYetki){//deleteid null degilse gerekli ihalepaketi silme

                        var kullaniciSoruDeleteIhaleList = context.SoruCevap.Where(ku => ku.IhalePaketID == deleteid).ToList();

                        var ihalepaketDelete = context.IhalePaket.Find(deleteid);
                        var kullaniciIhaleYetkisiDelete = context.KullaniciIhaleYetkisi.Where(ki => ki.IhalePaketID == deleteid).ToList();

                        if (ihlDelete(ihalepaketDelete, kullaniciIhaleYetkisiDelete, kullaniciSoruDeleteIhaleList, deleteid)){
                            context.SaveChanges();
                            return Content("success");
                        }

                    }
                    else{
                        ism.projeler = context.Project.ToList();
                        ism.ihalepaket = context.IhalePaket.Find(id);
                        ViewBag.ihalePaket = ism.ihalepaket;
                        ism.proje = context.Project.Find(ism.ihalepaket.ProjectID);

                        if (ism.ihalepaket != null)
                        dateTrConvert();
                    }

                }
                else
                return RedirectToAction("Login", "Account");
                
            }
            return View(ism);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ihleditt(IhalePaket ih ,IhaleSoruModel ism,int ?id)
        {
            var loginName = sessionLoginNameControl();
            //ism.IhaleTarihi.sp
            if(loginName != null)
            {
                if(!String.IsNullOrEmpty(ism.ihalepaket.Konu) && !String.IsNullOrEmpty(ism.ihalepaket.YapilacagiYer) && !String.IsNullOrEmpty(ism.ihalepaket.IhaleTuru)){

                    dateIhledittFormatControl(ism.IhaleTarihi,ism.SonTeslimTarihi);

                    
                    var toUpdate = context.IhalePaket.Find(ih.ID);

                    toUpdate.ProjectID = ism.proje.ID;
                    toUpdate.Konu = ism.ihalepaket.Konu;
                    toUpdate.YapilacagiYer = ism.ihalepaket.YapilacagiYer;
                    
                    if(IhaleTarihidt != null){ 
                        toUpdate.IhaleTarihi = IhaleTarihidt;
                        ViewBag.ihaletarihi = IhaleTarihidt;
                    }
                    else
                    ViewBag.ihaletarihi = toUpdate.IhaleTarihi;
                    if (TeslimTarihidt != null)
                    {
                        toUpdate.TeslimTarihi = TeslimTarihidt;
                        ViewBag.teslimtarihi = TeslimTarihidt;
                    }
                    else
                    ViewBag.teslimtarihi = toUpdate.IhaleTarihi;

                    toUpdate.IhaleTuru = ism.ihalepaket.IhaleTuru;
                    toUpdate.FTPDosyaLink = ism.ihalepaket.FTPDosyaLink;
                    toUpdate.FTPDosyaLink1 = ism.ihalepaket.FTPDosyaLink1;
                    toUpdate.FTPDosyaLink2 = ism.ihalepaket.FTPDosyaLink2;
                    context.Update(toUpdate);
                    context.SaveChanges();
                    return RedirectToAction("Ihl", "Home");
                }
                else
                return RedirectToAction("Ihleditt",new {sign = true, error = true });
                
                
            }
            else    
            return RedirectToAction("Login", "Home");
            
        }
        public IActionResult Qusend(bool error)
        {
            var loginName = sessionLoginNameControl();
            int loginID = sessionLoginIdControl();
            if (loginName != null && loginID != 0) { 
                svm = new SoruViewModel();
                svm.kullaniciList = context.Kullanici.ToList();
                bool adminYetki = checkAdmin(svm.kullaniciList, loginName);
           
                ViewBag.adminYetki = adminYetki;
                svm.kullaniciihaleyetkilisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).ToList();
                svm.kullaniciList = context.Kullanici.ToList();
                svm.kullanici = context.Kullanici.FirstOrDefault();
                svm.ihalepaketList = context.IhalePaket.ToList();
                //svm.projectList = context.Project.ToList();
                
            }
            else
            return RedirectToAction("Account", "Login");
            
            if (error){
                ViewBag.sign = "*";
                ViewBag.error = "Yıldızlı alanlar boş bırakılamaz.";
                return View(svm);
            }
            return View(svm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Qusend(SoruViewModel svm)
        {
            //anlık zaman cekilecek,soruno ile ilgili bir dongu doneceksin,son olarak kullanıcıid eslestireceksin       
            myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("s");
            DateTime sqlFormattedDatedt = Convert.ToDateTime(sqlFormattedDate, CultureInfo.InvariantCulture);
            var loginName = sessionLoginNameControl();
            int loginID = sessionLoginIdControl();
            if (loginName != null && loginID != 0){
                if (ModelState.IsValid && svm.IhaleID != 0){
                    context.SoruCevap.Add(new SoruCevap
                    {

                        IhalePaketID = svm.IhaleID,
                        Soru = svm.SoruPost,
                        KullaniciID_Soru = loginID,
                        SoruZamani = sqlFormattedDatedt,
                        Aktif = true
                    });
                    context.SaveChanges();
                    return RedirectToAction("Thanks");
                }
                else
                return RedirectToAction("Qusend",new {error = true});
                
            }
            else
            return RedirectToAction("Login", "Account");
            

        }
        [HttpGet]
        public IActionResult Users(int ?id)
        {
            kvm = new KullaniciViewModel();
            kvm.kullaniciList = context.Kullanici.ToList();
            var loginName = sessionLoginNameControl();
            bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
            ViewBag.adminYetki = adminYetki;
            
            if (loginName != null){
                if(adminYetki != false){
                    if (id != null) {
                        var kullaniciDelete = context.Kullanici.Find(id);

                        var kullaniciSoruDeleteSoruList = context.SoruCevap.Where(ku => ku.KullaniciID_Soru == id).ToList();
                        var kullaniciSoruDeleteCevapList = context.SoruCevap.Where(ku => ku.KullaniciID_Cevap == id).ToList();

                        var kullaniciihaleyetkisiDelete = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == id).ToList();
                        if (userDelete(kullaniciDelete, kullaniciSoruDeleteSoruList, kullaniciSoruDeleteCevapList, kullaniciihaleyetkisiDelete, id))
                            context.SaveChanges();

                        else
                        ViewBag.error = "Kullanıcıyı silmeden önce kullanıcıya ait soruları siliniz";

                        kvm.kullaniciList = context.Kullanici.ToList();
                    }
                    return View("Users", kvm);
                }
                else
                return View();
                
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
     
        public IActionResult Useradd(bool error){
            kvm = new KullaniciViewModel();
            kvm.kullaniciList = context.Kullanici.ToList();
            var loginName = sessionLoginNameControl();
            bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
            ViewBag.adminYetki = adminYetki;
            if (loginName != null){
                if(adminYetki != false){
                    if (error)
                    ViewBag.error = "Bu isimde bir kullanıcı adı vardır.";
                    
                    return View(kvm);//?
                }
                else
                return View();
                    
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
        public bool checkUserSameLoginName(string pLoginName)
        {
            var sameLoginName = context.Kullanici.Where(ku => ku.LoginName == pLoginName).FirstOrDefault();
            if (sameLoginName != null)
            return true;
            else
            return false;
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Useradd(KullaniciViewModel kvm){
            var loginName = sessionLoginNameControl();
            if (loginName != null){
                //if ekleyebiliriz
                if(!String.IsNullOrWhiteSpace(kvm.LoginName) && !String.IsNullOrWhiteSpace(kvm.Sifre) 
                    && !String.IsNullOrWhiteSpace(kvm.AdSoyad)){
                    if (!checkUserSameLoginName(kvm.LoginName)) {
                        context.Kullanici.Add(new Kullanici
                        {
                            LoginName = kvm.LoginName,
                            Sifre = kvm.Sifre,
                            AdSoyad = kvm.AdSoyad,
                            MailAdres = kvm.MailAdres,
                            Unvan = kvm.Unvan,
                            AdminYetki = kvm.AdminYetki,
                            Aktif = true
                        });
                        context.SaveChanges();
                        return RedirectToAction("Users");
                    }
                    else
                    return RedirectToAction("Useradd", new { error = true });   
                }
                else
                return RedirectToAction("Useradd");
                
               
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
      
        public IActionResult Usereditt(int ?id)
        {
            ku = new Kullanici();
            kvm = new KullaniciViewModel();
            ku = context.Kullanici.FirstOrDefault();
            kvm.kullaniciList = context.Kullanici.ToList();
            var loginName = sessionLoginNameControl();
            bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
            ViewBag.adminYetki = adminYetki;
            if (loginName != null){
                if(adminYetki != false){
                    if(id != null) {
                        ku = context.Kullanici.Find(id);
                        return View(ku);//duzenlenecek
                    }
                    else
                    return RedirectToAction("Users", "Home");
                    
                }
                else
                return View();
                
            }
            else
            return RedirectToAction("Login", "Account");
   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Usereditt(Kullanici ku)
        {
            var loginName = sessionLoginNameControl();
            if (loginName != null){
                var toUpdate = context.Kullanici.Find(ku.ID);
                
                toUpdate.LoginName = ku.LoginName;
                toUpdate.AdSoyad = ku.AdSoyad;
                toUpdate.Sifre = ku.Sifre;
                toUpdate.MailAdres = ku.MailAdres;
                toUpdate.Unvan = ku.Unvan;
                toUpdate.AdminYetki = ku.AdminYetki;
                toUpdate.Aktif = ku.Aktif;
                context.Update(toUpdate);
                context.SaveChanges();
                return RedirectToAction("Users", "Home");
            }
            else
            return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public IActionResult Authorization(int IhaleId,int ?id,int UserId)
        {
            bool ihaleYetki = false;
            var loginName = sessionLoginNameControl();
            var loginID = sessionLoginIdControl();
            if(loginName != null && loginID != 0) {
                kvm = new KullaniciViewModel();
                kvm.kullaniciList = context.Kullanici.ToList();

                bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
                ViewBag.adminYetki = adminYetki;
                if (IhaleId != 0){
                    kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).ToList();
                    ihaleYetki = checkIhaleYetki(loginID, IhaleId, kvm.kullaniciihaleyetkisiList);
                    ViewBag.ihaleYetki = ihaleYetki;
                }

                //HttpContext.Session.SetInt32("adminyetki",adminYetki);
                if (adminYetki || ihaleYetki){ 
                    if(id != null){
                        kvm.kullaniciihaleyetkisi = context.KullaniciIhaleYetkisi.Find(id);
                        if(kvm.kullaniciihaleyetkisi != null) { 
                        context.KullaniciIhaleYetkisi.Remove(kvm.kullaniciihaleyetkisi);
                        context.SaveChanges();
                        }
                    }
                    kvm.ihalepaketList = context.IhalePaket.ToList();
                    ViewBag.ihaleid = IhaleId;

                    if(UserId != 0)
                    kullaniciCombobox(IhaleId, UserId, loginID, adminYetki, ihaleYetki);
                    else
                    ihaleCombobox(IhaleId, loginID, adminYetki, ihaleYetki);
                   
                    return View(kvm);
                }
                else
                return View();
                
            }
            else
            return RedirectToAction("Login", "Account");
            
        }

        [HttpGet]
        public IActionResult Authorizationadd(bool error,int IhaleID) {
            var loginName = sessionLoginNameControl();
            var loginID = sessionLoginIdControl();
            if(loginName != null && loginID != 0){ 
                kvm = new KullaniciViewModel();
                var nonAdminandUser = context.Kullanici.Where(ku => ku.LoginName == "satinalma" || ku.LoginName == loginName);

                kvm.kullaniciList = context.Kullanici.ToList();
                kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).ToList();

                bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
                bool ihaleYetki = checkIhaleYetki(loginID, IhaleID, kvm.kullaniciihaleyetkisiList);
           
                if(!adminYetki && ihaleYetki)
                kvm.kullaniciList = context.Kullanici.Except(nonAdminandUser).ToList();

             
                ViewBag.adminYetki = adminYetki;
                ViewBag.ihaleYetki = ihaleYetki;
                if (adminYetki || ihaleYetki){
                    kvm.ihalepaket = context.IhalePaket.Where(ih => ih.ID == IhaleID).FirstOrDefault();
                    kvm.ihalepaketList = context.IhalePaket.ToList();
                    kvm.projectList = context.Project.ToList();
                    if (error)
                    ViewBag.error = "Kullanıcı ihale paketine zaten sahiptir.";
                    
                    return View(kvm);
                }
                else
                return View();
                
            }
            else{
                return RedirectToAction("Login", "Account");
            }
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authorizationadd(KullaniciViewModel kvm) {
            var loginName = sessionLoginNameControl();
            if (loginName != null){
                kvm.kullaniciList = context.Kullanici.ToList();
                if(!checkAuthorizationSameihaleID(kvm.KullaniciIhaleID, kvm.IhaleID)){ 
                    context.KullaniciIhaleYetkisi.Add(new KullaniciIhaleYetkisi
                    {
                        UserID = kvm.KullaniciIhaleID,
                        IhalePaketID = kvm.IhaleID,
                        IhaleYetkilisi = kvm.IhaleYetki
                    });

                    context.SaveChanges();
                }
                else
                return RedirectToAction("Authorizationadd", new { error = true, IhaleID = kvm.IhaleID });
                
                
                return RedirectToAction("Authorization",new {IhaleID = kvm.IhaleID }); 
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
        [HttpGet]
        public IActionResult Authorizationedit(int ?id,int IhaleID)
        {
            var loginName = sessionLoginNameControl();
            var loginID = sessionLoginIdControl();
            if (loginName != null && loginID != 0){
                kvm = new KullaniciViewModel();

                kvm.ihalepaket = context.IhalePaket.FirstOrDefault();
                kvm.ihalepaketList = context.IhalePaket.ToList();

               
   
                kvm.kulllanici = context.Kullanici.FirstOrDefault();
                kvm.kullaniciList = context.Kullanici.ToList();
                kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.ToList();
                kvm.ihaleIDList = context.KullaniciIhaleYetkisi.Select(ki => ki.IhalePaketID).Distinct().ToList();

                bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
                ViewBag.adminYetki = adminYetki;
                bool ihaleYetki = checkIhaleYetki(loginID, IhaleID, kvm.kullaniciihaleyetkisiList);
                ViewBag.ihaleYetki = ihaleYetki;

                if (!adminYetki && ihaleYetki)
                kvm.kullaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.Where(ku => ku.UserID == loginID).ToList();

                if (adminYetki || ihaleYetki){
                    if(id != null){
                        kvm.kullaniciihaleyetkisi = context.KullaniciIhaleYetkisi.Where
                                                                                  (ki => ki.ID == id).FirstOrDefault();
                        kvm.kulllanici = context.Kullanici.Find(kvm.kullaniciihaleyetkisi.UserID);
                        return View(kvm);
                    }
                    else
                    return RedirectToAction("Authorization", "Home");
                    
                }
                else
                return View();
                
            }
            else
            return RedirectToAction("Login","Account");
            
             
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authorizationedit(KullaniciIhaleYetkisi ki,KullaniciViewModel kvm)
        {
            var loginName = sessionLoginNameControl();
            if (loginName != null){
              var toUpdate = context.KullaniciIhaleYetkisi.Find(ki.ID);

              toUpdate.IhalePaketID = kvm.kullaniciihaleyetkisi.IhalePaketID;
              toUpdate.IhaleYetkilisi = kvm.kullaniciihaleyetkisi.IhaleYetkilisi;
              context.Update(toUpdate);
              context.SaveChanges();
              return RedirectToAction("Authorization", "Home",new { IhaleID = kvm.kullaniciihaleyetkisi.IhalePaketID });
            }
            else
            return RedirectToAction("Login", "Home");
            
        }
        public IActionResult Thanks()
        {
            var loginName = sessionLoginNameControl();
            if(loginName != null){
                kvm = new KullaniciViewModel();
                kvm.kullaniciList = context.Kullanici.ToList();
                bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
                ViewBag.adminYetki = adminYetki;
                return View();
            }
            else
            return RedirectToAction("Login", "Account");
        }
        
        public IActionResult Answer(int ?id,int IhaleID)
        {
            sr = new SoruCevap();
            sr = context.SoruCevap.FirstOrDefault();
            var loginName = sessionLoginNameControl();
            var loginID = sessionLoginIdControl();

            var kullanicilist = context.Kullanici.ToList();
            var kullaniciihaleyetkisi = context.KullaniciIhaleYetkisi.Where(ki => ki.UserID == loginID).FirstOrDefault();
            var kulllaniciihaleyetkisiList = context.KullaniciIhaleYetkisi.ToList();

            bool adminYetki = checkAdmin(kullanicilist, loginName);
            bool ihaleYetki = checkIhaleYetki(loginID,IhaleID, kulllaniciihaleyetkisiList);
            ViewBag.adminYetki = adminYetki;
            ViewBag.ihaleYetki = ihaleYetki;
            if (loginName != null && loginID != 0){ 
                sr = context.SoruCevap.Find(id);
                if (id != null)
                return View(sr);
                
                else
                return View("Ihl");
               
            }
            else
            return RedirectToAction("Login", "Account");
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Answer(SoruCevap sr)
        {
            myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("s");
            DateTime sqlFormattedDatedt = Convert.ToDateTime(sqlFormattedDate, CultureInfo.InvariantCulture);
            var loginName = sessionLoginNameControl();
            if (loginName != null){
                if(sr.SoruNo != 0) { 
                    var kullanici_id = context.Kullanici.Where(ku => ku.LoginName == loginName).FirstOrDefault();
               
                    var toUpdate = context.SoruCevap.Find(sr.ID);
                    toUpdate.SoruNo = sr.SoruNo;
                    toUpdate.Soru = sr.Soru;
                    toUpdate.Cevap = sr.Cevap;
                    toUpdate.CevapZamani = sqlFormattedDatedt;
                    toUpdate.KullaniciID_Cevap = kullanici_id.ID;
                    context.Update(toUpdate);
                    context.SaveChanges();
                    return RedirectToAction("Ihl", "Home");
                }
                else
                return RedirectToAction("Login", "Account");
            }
            else
            return RedirectToAction("Login", "Account");
            
           
        }
        public IActionResult Contact()
        {
            var loginName = sessionLoginNameControl();
            if(loginName != null){
                kvm = new KullaniciViewModel();
                kvm.kullaniciList = context.Kullanici.ToList();
                bool adminYetki = checkAdmin(kvm.kullaniciList, loginName);
                ViewBag.adminYetki = adminYetki;
                return View();
            }
            else
            return RedirectToAction("Login", "Account");
            

        
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
