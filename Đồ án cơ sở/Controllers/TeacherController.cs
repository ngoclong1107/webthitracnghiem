using Đồ_án_cơ_sở.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Đồ_án_cơ_sở.Controllers
{
    public class TeacherController : KtGVController
    {
        ThiTNEntities1 db = new ThiTNEntities1();
        // GET: Teacher
        public ActionResult Index()
        {
            GiaoVien gv = (GiaoVien)Session["GiaoVien"];
            var dt = db.DeThis.Where(n => n.MaMH == gv.MaMH);
            return View(dt);
        }
        public ActionResult TaoDeThi()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoDeThi([Bind(Include = "MaDT,NgayThi,ThoiGianThi,SoLuongCauHoi")] DeThi deThi)
        {
            GiaoVien gv = (GiaoVien)Session["GiaoVien"];

            if (ModelState.IsValid)
            {
                deThi.MaMH = gv.MaMH;
                Session["DeThi"] = deThi;
                db.DeThis.Add(deThi);
                db.SaveChanges();
                return RedirectToAction("ChonCauHoi", new { id = deThi.MaDT });
            }

            return View(deThi);
        }
        public ActionResult ChonCauHoi(int id)
        {
            var dethi = db.DeThis.Find(id);
            var cauhoi = db.CauHois.Where(c => c.MaMH == dethi.MaMH);
            Session["DeThi"] = dethi;
            return View(cauhoi);
        }
        public ActionResult ChiTietDeThi(int id)
        {
            var dethi = db.DeThis.Find(id);
            Session["DeThi"] = dethi;
            var ctdt = db.DeThi_CauHoi.Where(m => m.MaDT == id);
            return View(ctdt);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeThi deThi = db.DeThis.Find(id);
            if (deThi == null)
            {
                return HttpNotFound();
            }
            return View(deThi);
        }

        // POST: DeThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeThi deThi = db.DeThis.Find(id);
            var dtch=db.DeThi_CauHoi.Where(m=>m.MaDT== id);
            foreach (var item in dtch)
            {
                db.DeThi_CauHoi.Remove(item);
            }
            db.DeThis.Remove(deThi);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ThemSinhVien(int id)
        {
            var dethi = db.DeThis.Find(id);
            var sinhvien = db.SinhViens;
            Session["DeThi"] = dethi;
            return View(sinhvien);
        }
        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {
            var cauhoilist = new List<DeThi_CauHoi>();
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
                            var cauhoi = new DeThi_CauHoi();
                            cauhoi.MaCH = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value.ToString());
                            cauhoi.MaDT = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());
                            cauhoilist.Add(cauhoi);
                        }
                    }
                }
            }
            using (ThiTNEntities1 excelImportDBEntities = new ThiTNEntities1())
            {
                foreach (var item in cauhoilist)
                {
                    excelImportDBEntities.DeThi_CauHoi.Add(item);
                }
                excelImportDBEntities.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Uploadsv(FormCollection formCollection)
        {
            var cauhoilist = new List<DeThi_SInhVien>();
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
                            var cauhoi = new DeThi_SInhVien();
                            cauhoi.MSSV = workSheet.Cells[rowIterator, 1].Value.ToString();
                            cauhoi.MaDT = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());
                            cauhoilist.Add(cauhoi);
                        }
                    }
                }
            }
            using (ThiTNEntities1 excelImportDBEntities = new ThiTNEntities1())
            {
                foreach (var item in cauhoilist)
                {
                    excelImportDBEntities.DeThi_SInhVien.Add(item);
                }
                excelImportDBEntities.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}