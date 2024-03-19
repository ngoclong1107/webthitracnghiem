using Đồ_án_cơ_sở.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Đồ_án_cơ_sở.Controllers
{
    public class AdminController : BaseController
    {
        ThiTNEntities1 db=new ThiTNEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            Dictionary<string, int> ListCount = GetDashBoard();
            return View(ListCount);
        }
        public Dictionary<string, int> GetDashBoard()
        {
            var ListCount = new Dictionary<string, int>();
            int CountAdmin = db.GiaoViens.Where(m=>m.MaQuyen==1).Count();
            ListCount.Add("CountAdmin", CountAdmin);
            int CountTeacher = db.GiaoViens.Where(m => m.MaQuyen == 2).Count();
            ListCount.Add("CountTeacher", CountTeacher);
            int CountStudent = db.SinhViens.Count();
            ListCount.Add("CountStudent", CountStudent);
            int CountSubject = db.MonHocs.Count();
            ListCount.Add("CountSubject", CountSubject);
            int CountQuestion = db.CauHois.Count();
            ListCount.Add("CountQuestion", CountQuestion);
            int CountTest = db.DeThis.Count();
            ListCount.Add("CountTest", CountTest);
            return ListCount;
        }
        public ActionResult ListAdmin()
        {
            var giaoViens = db.GiaoViens.Where(g => g.MaQuyen == 1);
            return View(giaoViens.ToList());
        }
        public ActionResult ThongTinAdmin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            return View(giaoVien);
        }

        // GET: GiaoViens/Create
        public ActionResult ThemAdmin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemAdmin([Bind(Include = "MaGV,HoTen,email,SDT,GioiTinh,QueQuan")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                    var tk = new TaiKhoan();
                    db.GiaoViens.Add(giaoVien);
                    tk.TenDangNhap = giaoVien.MaGV;
                    tk.MatKhau = Encryptor.MD5Hash(giaoVien.MaGV);
                    db.TaiKhoans.Add(tk);
                    giaoVien.MaTK = tk.MaTk;
                    giaoVien.MaQuyen = 1;
                    db.SaveChanges();
                    return RedirectToAction("Index");
            }
            return View(giaoVien);
        }
        public ActionResult SuaThongTinAdmin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            Session["Editgv"] = giaoVien;
            return View(giaoVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinAdmin([Bind(Include = "MaGV,HoTen,email,SDT,GioiTinh,QueQuan")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                    db.Entry(giaoVien).State = EntityState.Modified;
                    GiaoVien gv = (GiaoVien)Session["Editgv"];
                    giaoVien.MaTK = gv.MaTK;
                    giaoVien.MaQuyen = 1;
                    db.SaveChanges();
                    return RedirectToAction("ListAdmin");
                }
            return View(giaoVien);
        }
        public ActionResult XoaThongTinAdmin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            return View(giaoVien);
        }
        [HttpPost, ActionName("XoaThongTinAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaThongTinAd(string id)
        {
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            TaiKhoan tk = db.TaiKhoans.Find(giaoVien.MaTK);
            db.TaiKhoans.Remove(tk);
            db.GiaoViens.Remove(giaoVien);
            db.SaveChanges();
            return RedirectToAction("ListAdmin");
        }
        public ActionResult ListCauHoi(int MaMH=0)
        {
            var cauhoi = from tt in db.CauHois select tt;

            if (MaMH!=0)
            {
                cauhoi=cauhoi.Where(c=>c.MaMH==MaMH);
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
            return View(cauhoi);
        }

        // GET: CauHois/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHoi cauHoi = db.CauHois.Find(id);
            if (cauHoi == null)
            {
                return HttpNotFound();
            }
            return View(cauHoi);
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/images/" + file.FileName));
            return "/images/" + file.FileName;
        }
        // GET: CauHois/Create
        public ActionResult Create()
        {
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
            return View();
        }

        // POST: CauHois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCH,CauHoi1,CauA,CauB,CauC,CauD,DapAn,MaMH,Hinh")] CauHoi cauHoi)
        {
            if (ModelState.IsValid)
            {
                if (cauHoi.DapAn == "CauA")
                {
                    cauHoi.DapAn = cauHoi.CauA;
                }
                if (cauHoi.DapAn == "CauB")
                {
                    cauHoi.DapAn = cauHoi.CauB;
                }
                if (cauHoi.DapAn == "CauC")
                {
                    cauHoi.DapAn = cauHoi.CauC;
                }
                if (cauHoi.DapAn == "CauD")
                {
                    cauHoi.DapAn = cauHoi.CauD;
                }
                db.CauHois.Add(cauHoi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH", cauHoi.MaMH);
            return View(cauHoi);
        }
        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var cauhoilist = new List<CauHoi>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var cauhoi = new CauHoi();
                            cauhoi.CauHoi1 = workSheet.Cells[rowIterator, 1].Value.ToString();
                            cauhoi.CauA = workSheet.Cells[rowIterator, 2].Value.ToString();
                            cauhoi.CauB = workSheet.Cells[rowIterator, 3].Value.ToString();
                            cauhoi.CauC = workSheet.Cells[rowIterator, 4].Value.ToString();
                            cauhoi.CauD = workSheet.Cells[rowIterator, 5].Value.ToString();
                            if(workSheet.Cells[rowIterator, 6].Value.ToString() == "A")
                            {
                                cauhoi.DapAn = cauhoi.CauA;
                            }
                            if (workSheet.Cells[rowIterator, 6].Value.ToString() == "B")
                            {
                                cauhoi.DapAn = cauhoi.CauB;
                            }
                            if (workSheet.Cells[rowIterator, 6].Value.ToString() == "C")
                            {
                                cauhoi.DapAn = cauhoi.CauC;
                            }
                            if (workSheet.Cells[rowIterator, 6].Value.ToString() == "D")
                            {
                                cauhoi.DapAn = cauhoi.CauD;
                            }
                            cauhoi.MaMH = Convert.ToInt32(workSheet.Cells[rowIterator, 7].Value.ToString());
                            cauhoilist.Add(cauhoi);
                        }
                    }
                }
            }
            using (ThiTNEntities1 excelImportDBEntities = new ThiTNEntities1())
            {
                foreach (var item in cauhoilist)
                {
                    excelImportDBEntities.CauHois.Add(item);
                }
                excelImportDBEntities.SaveChanges();
            }
            return RedirectToAction("ListCauHoi");
        }
        // GET: CauHois/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHoi cauHoi = db.CauHois.Find(id);
            if (cauHoi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH", cauHoi.MaMH);
            return View(cauHoi);
        }

        // POST: CauHois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCH,CauHoi1,CauA,CauB,CauC,CauD,DapAn,MaMH,Hinh")] CauHoi cauHoi)
        {
            if (ModelState.IsValid)
            {
                if (cauHoi.DapAn == "CauA")
                {
                    cauHoi.DapAn = cauHoi.CauA;
                }
                if (cauHoi.DapAn == "CauB")
                {
                    cauHoi.DapAn = cauHoi.CauB;
                }
                if (cauHoi.DapAn == "CauC")
                {
                    cauHoi.DapAn = cauHoi.CauC;
                }
                if (cauHoi.DapAn == "CauD")
                {
                    cauHoi.DapAn = cauHoi.CauD;
                }
                db.Entry(cauHoi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH", cauHoi.MaMH);
            return View(cauHoi);
        }

        // GET: CauHois/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHoi cauHoi = db.CauHois.Find(id);
            if (cauHoi == null)
            {
                return HttpNotFound();
            }
            return View(cauHoi);
        }

        // POST: CauHois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CauHoi cauHoi = db.CauHois.Find(id);
            db.CauHois.Remove(cauHoi);
            db.SaveChanges();
            return RedirectToAction("ListCauHoi");
        }
        public ActionResult ListSinhVien(string searchString)
        {
            var sinhvien =from tt in db.SinhViens select tt;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                sinhvien = db.SinhViens.Where(b => b.MSSV.ToLower().Contains(searchString));
                sinhvien = db.SinhViens.Where(b => b.HoTen.ToLower().Contains(searchString));
            }

            return View(sinhvien);
        }


        // GET: SinhViens/Details/5
        public ActionResult ThongTinSinhVien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SinhVien sinhVien = db.SinhViens.Find(id);
            if (sinhVien == null)
            {
                return HttpNotFound();
            }
            return View(sinhVien);
        }

        // GET: SinhViens/Create
        public ActionResult ThemSinhVien()
        {
            return View();
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSinhVien([Bind(Include = "MSSV,HoTen,NgaySInh,GioiTinh")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                var tk = new TaiKhoan();
                tk.TenDangNhap = sinhVien.MSSV;
                tk.MatKhau = Encryptor.MD5Hash(sinhVien.MSSV);
                db.TaiKhoans.Add(tk);
                sinhVien.MaTK = tk.MaTk;
                sinhVien.MaQuyen = 3;
                db.SinhViens.Add(sinhVien);
                db.SaveChanges();
                return RedirectToAction("ListSinhVien");
            }
            return View(sinhVien);
        }
        [HttpPost]
        public ActionResult Uploadsv(FormCollection formCollection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var sinhvienlist = new List<SinhVien>();
            var taikhoanlist=new List<TaiKhoan>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var sinhvien = new SinhVien();
                            sinhvien.MSSV = workSheet.Cells[rowIterator, 1].Value.ToString();
                            sinhvien.HoTen = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var ngaySinhStr = workSheet.Cells[rowIterator, 3].Value.ToString();
                            var ngaySinh = DateTime.MinValue;
                            if (!string.IsNullOrEmpty(ngaySinhStr))
                            {
                                DateTime.TryParse(ngaySinhStr, out ngaySinh);
                            }
                            sinhvien.NgaySInh = ngaySinh;
                            sinhvien.GioiTinh = workSheet.Cells[rowIterator, 4].Value.ToString();
                            sinhvien.MaQuyen = 3;
                            var tk = new TaiKhoan();
                            tk.TenDangNhap = sinhvien.MSSV;
                            tk.MatKhau = Encryptor.MD5Hash(sinhvien.MSSV);
                            taikhoanlist.Add(tk);
                            sinhvien.MaTK = tk.MaTk;
                            sinhvienlist.Add(sinhvien);
                        }
                    }
                }
            }
            using (ThiTNEntities1 excelImportDBEntities = new ThiTNEntities1())
            {
                foreach (var item in sinhvienlist)
                {                    
                    excelImportDBEntities.SinhViens.Add(item);
                }
                foreach (var item in taikhoanlist)
                {
                    excelImportDBEntities.TaiKhoans.Add(item);
                }
                excelImportDBEntities.SaveChanges();
            }
            return RedirectToAction("ListSinhVien");
        }

        // GET: SinhViens/Edit/5
        public ActionResult SuaThongTin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SinhVien sinhVien = db.SinhViens.Find(id);
            if (sinhVien == null)
            {
                return HttpNotFound();
            }
            Session["Editsv"]=sinhVien;
            return View(sinhVien);
        }

        // POST: SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTin([Bind(Include = "MSSV,HoTen,NgaySInh,GioiTinh")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                SinhVien sv = (SinhVien)Session["Editsv"];
                db.Entry(sinhVien).State = EntityState.Modified;
                sinhVien.MaTK = sv.MaTK;
                sinhVien.MaQuyen=sv.MaQuyen;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sinhVien);
        }

        // GET: SinhViens/Delete/5
        public ActionResult XoaThongTin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SinhVien sinhVien = db.SinhViens.Find(id);
            if (sinhVien == null)
            {
                return HttpNotFound();
            }
            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("XoaThongTin")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaThongTinsv(string id)
        {
            SinhVien sinhVien = db.SinhViens.Find(id);
            TaiKhoan tk=db.TaiKhoans.Find(sinhVien.MaTK);
            db.TaiKhoans.Remove(tk);
            db.SinhViens.Remove(sinhVien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ListGiaoVien(string searchString,int MaMH=0)
        {
            var giaovien = from tt in db.GiaoViens where tt.MaQuyen == 2 select tt;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                giaovien = db.GiaoViens.Where(b => b.MaGV.ToLower().Contains(searchString));
                giaovien = db.GiaoViens.Where(b => b.HoTen.ToLower().Contains(searchString));
                ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
                return View(giaovien);
            }
            if (MaMH != 0)
            {
                giaovien = giaovien.Where(c => c.MaMH == MaMH);
                ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
                return View(giaovien);
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
            return View(giaovien);
        }

        // GET: GiaoViens/Details/5
        public ActionResult ThongTinGiaoVien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            return View(giaoVien);
        }

        // GET: GiaoViens/Create
        public ActionResult ThemGiaoVien()
        {
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemGiaoVien([Bind(Include = "MaGV,HoTen,email,SDT,MaMH,GioiTinh,QueQuan")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                    var tk = new TaiKhoan();
                    db.GiaoViens.Add(giaoVien);
                    tk.TenDangNhap = giaoVien.MaGV;
                    tk.MatKhau = Encryptor.MD5Hash(giaoVien.MaGV);
                    db.TaiKhoans.Add(tk);
                    giaoVien.MaTK = tk.MaTk;
                    giaoVien.MaQuyen = 2;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH", giaoVien.MaMH);
            return View(giaoVien);
        }
        public ActionResult Uploadgv(FormCollection formCollection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var giaovienlist = new List<GiaoVien>();
            var taikhoanlist = new List<TaiKhoan>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var giaovien = new GiaoVien();
                            giaovien.MaGV = workSheet.Cells[rowIterator, 1].Value.ToString();
                            giaovien.HoTen = workSheet.Cells[rowIterator, 2].Value.ToString();
                            giaovien.email = workSheet.Cells[rowIterator, 3].Value.ToString();
                            giaovien.SDT = Convert.ToInt32(workSheet.Cells[rowIterator, 4].Value.ToString());
                            giaovien.MaMH = Convert.ToInt32(workSheet.Cells[rowIterator, 5].Value.ToString());
                            giaovien.GioiTinh = workSheet.Cells[rowIterator, 6].Value.ToString();
                            giaovien.QueQuan= workSheet.Cells[rowIterator, 7].Value.ToString();
                            giaovien.MaQuyen = 2;
                            var tk = new TaiKhoan();
                            tk.TenDangNhap = giaovien.MaGV;
                            tk.MatKhau = Encryptor.MD5Hash(giaovien.MaGV);
                            taikhoanlist.Add(tk);
                            giaovien.MaTK = tk.MaTk;
                            giaovienlist.Add(giaovien);
                        }
                    }
                }
            }
            using (ThiTNEntities1 excelImportDBEntities = new ThiTNEntities1())
            {
                foreach (var item in giaovienlist)
                {
                    excelImportDBEntities.GiaoViens.Add(item);
                }
                foreach (var item in taikhoanlist)
                {
                    excelImportDBEntities.TaiKhoans.Add(item);
                }
                excelImportDBEntities.SaveChanges();
            }
            return RedirectToAction("ListGiaoVien");
        }
        public ActionResult SuaThongTinGv(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH");
            Session["Editgv"] =giaoVien;
            return View(giaoVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinGv([Bind(Include = "MaGV,HoTen,email,SDT,MaMH,GioiTinh,QueQuan")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giaoVien).State = EntityState.Modified;
                GiaoVien gv = (GiaoVien)Session["Editgv"];
                giaoVien.MaQuyen= gv.MaQuyen;
                giaoVien.MaTK = gv.MaTK;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaMH = new SelectList(db.MonHocs, "MaMH", "TenMH", giaoVien.MaMH);
            return View(giaoVien);
        }
        public ActionResult XoaThongTingv(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            if (giaoVien == null)
            {
                return HttpNotFound();
            }
            return View(giaoVien);
        }
        [HttpPost, ActionName("XoaThongTingv")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaThongTinGv(string id)
        {
            GiaoVien giaoVien = db.GiaoViens.Find(id);
            TaiKhoan tk = db.TaiKhoans.FirstOrDefault(m=>m.MaTk==giaoVien.MaTK);
            db.TaiKhoans.Remove(tk);
            db.GiaoViens.Remove(giaoVien);
            db.SaveChanges();
            return RedirectToAction("ListGiaoVien");
        }
        public ActionResult ListDeThi()
        {
            return View(db.DeThis.ToList());
        }
        public ActionResult ListBaiLam(int id)
        {
            var diem = db.Diems.Where(m => m.MaDT == id);
            return View(diem);
        }
        public ActionResult ChiTietBaiThi(int id,string mssv)
        {
            ViewBag.score = GetScore(id,mssv);
            return View(GetListQuest(id,mssv));
        }
        public Diem GetScore(int test_code, string mssv)
        {
            Diem score = new Diem();
            try
            {
                score = db.Diems.SingleOrDefault(x => x.MaDT == test_code && x.MSSV == mssv);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return score;
        }
        public List<StudentQuestViewModel> GetListQuest(int? id,string mssv)
        {
            List<StudentQuestViewModel> list = new List<StudentQuestViewModel>();
            SinhVien sv = (SinhVien)Session["SinhVien"];
            try
            {
                list = (from x in db.BaiLams
                        join t in db.DeThis on x.MaDT equals t.MaDT
                        join q in db.CauHois on x.MaCH equals q.MaCH
                        where x.MaDT == id && x.MSSV == mssv
                        select new StudentQuestViewModel { DeThi = t, BaiLam = x, CauHoi = q }).OrderBy(x => x.BaiLam.MSSV).ToList();
            }
            catch (Exception) { }
            return list;
        }
        public ActionResult XoaBaiLam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems.Find(id);
            if (diem == null)
            {
                return HttpNotFound();
            }
            return View(diem);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("XoaBaiLam")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaBaiLam(int id)
        {
            Diem diem = db.Diems.Find(id);
            var bailam = db.BaiLams.Where(m=>m.MSSV==diem.MSSV);
            int madt= (int)diem.MaDT;
            foreach(var item in bailam)
            {
                db.BaiLams.Remove(item);
            }
            db.Diems.Remove(diem);
            db.SaveChanges();
            return RedirectToAction("ListBaiLam", new {id=madt});
        }
        public ActionResult ListMonHoc()
        {
            return View(db.MonHocs.ToList());
        }
        public ActionResult ThemMonHoc()
        {
            return View();
        }

        // POST: Admin/MonHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMonHoc([Bind(Include = "MaMH,TenMH")] MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                db.MonHocs.Add(monHoc);
                db.SaveChanges();
                return RedirectToAction("ListMonHoc");
            }
            return View(monHoc);
        }
        public ActionResult XoaMonHoc(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonHoc monHoc = db.MonHocs.Find(id);
            if (monHoc == null)
            {
                return HttpNotFound();
            }
            return View(monHoc);
        }

        // POST: Admin/MonHocs/Delete/5
        [HttpPost, ActionName("XoaMonHoc")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaMonHoc(int id)
        {
            MonHoc monHoc = db.MonHocs.Find(id);
            db.MonHocs.Remove(monHoc);
            db.SaveChanges();
            return RedirectToAction("ListMonHoc");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
