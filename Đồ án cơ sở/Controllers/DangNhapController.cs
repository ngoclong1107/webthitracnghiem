using Đồ_án_cơ_sở.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Đồ_án_cơ_sở.Controllers
{
    public class DangNhapController : Controller
    {
        // GET: DangNhap
        private ThiTNEntities1 db = new ThiTNEntities1();
        // GET: NguoiDung
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            string tdn = collection["tendangnhap"];
            string mk = collection["matkhau"];
            string mk1=Encryptor.MD5Hash(mk);
            TaiKhoan tk = db.TaiKhoans.SingleOrDefault(n => n.TenDangNhap == tdn && n.MatKhau == mk||n.MatKhau==mk1);
            if(tk == null ) {
                ViewBag.ThongBao = "Ten dang nhap hoac mat khau khong dung";
                return RedirectToAction("Dangnhap");
            }
            GiaoVien gv = db.GiaoViens.SingleOrDefault(n => n.MaTK == tk.MaTk);
            SinhVien sv = db.SinhViens.SingleOrDefault(n => n.MaTK == tk.MaTk);
            if(gv != null&&gv.MaQuyen==1)
            {
                Session["Admin"] = gv;
                return RedirectToAction("Index", "Admin");
            }
            if (gv != null && gv.MaQuyen == 2)
            {
                Session["GiaoVien"] = gv;
                return RedirectToAction("Index", "Teacher");
            }
            if (sv != null)
            {
                Session["SinhVien"] = sv;
                return RedirectToAction("Index", "HocSinh");
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhap", "DangNhap");
        }
    }
}