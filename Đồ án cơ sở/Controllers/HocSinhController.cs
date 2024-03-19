using Đồ_án_cơ_sở.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Đồ_án_cơ_sở.Controllers
{
    public class HocSinhController : KtHSController
    {
        ThiTNEntities1 db = new ThiTNEntities1();
        // GET: HocSinh
        public ActionResult Index(string thongbao)
        {
            SinhVien sv = (SinhVien)Session["SinhVien"];
            var dt=db.DeThi_SInhVien.Where(m=>m.MSSV==sv.MSSV);
            ViewBag.ThongBao = thongbao;
            return View(dt);
        }
        [HttpGet]
        public void LayBaiThi(int id)
        {
            List<DeThi_CauHoi> qs = (from x in db.DeThi_CauHoi
                                     where x.MaDT == id
                                     select x).OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var item in qs)
            {
                SinhVien sv = (SinhVien)Session["SinhVien"];
                var StudentTest = new BaiLam();
                StudentTest.MaDT = id;
                StudentTest.MaCH = item.MaCH;
                StudentTest.MSSV = sv.MSSV;
                CauHoi q = db.CauHois.SingleOrDefault(x => x.MaCH == item.MaCH);
                StudentTest.DeBai = q.CauHoi1;
                string[] answer = { q.CauA, q.CauB, q.CauC, q.CauD };
                StudentTest.CauA = answer[0];
                StudentTest.CauB = answer[1];
                StudentTest.CauC = answer[2];
                StudentTest.CauD = answer[3];
                db.BaiLams.Add(StudentTest);
                db.SaveChanges();
            }
        }
        public ActionResult VaoThi(int id)
        {
            SinhVien sv = (SinhVien)Session["SinhVien"];
            if (db.Diems.FirstOrDefault(m => m.MaDT == id&&m.MSSV==sv.MSSV) == null)
            {
                LayBaiThi(id);
                var baithi = db.BaiLams.Where(m => m.MaDT == id && m.MSSV == sv.MSSV);
                sv.DTdanglam = id;
                var dt = db.DeThis.Find(id);
                string[] time = { dt.ThoiGianThi.ToString(), "00" };
                ViewBag.min = time[0];
                ViewBag.sec = time[1];
                db.SaveChanges();
                return View(baithi);
            }
            else
            {
                return RedirectToAction("Index", new { thongbao = "Ban da lam bai thi này" });
            }
        }
        public List<StudentQuestViewModel> GetListQuest(int? id)
        {
            List<StudentQuestViewModel> list = new List<StudentQuestViewModel>();
            SinhVien sv = (SinhVien)Session["SinhVien"];
            try
            {
                list = (from x in db.BaiLams
                        join t in db.DeThis on x.MaDT equals t.MaDT
                        join q in db.CauHois on x.MaCH equals q.MaCH
                        where x.MaDT == id && x.MSSV==sv.MSSV
                        select new StudentQuestViewModel { DeThi = t, BaiLam = x, CauHoi = q }).OrderBy(x => x.BaiLam.MSSV).ToList();
            }
            catch (Exception) { }
            return list;
        }
        public ActionResult SubmitTest()
        {
            SinhVien sinhvien = (SinhVien)Session["SinhVien"];
            var sv = db.DeThi_SInhVien.FirstOrDefault(m => m.MaDT == sinhvien.DTdanglam);
            var list = GetListQuest(sv.MaDT);
            int total_quest = (int)list.First().DeThi.SoLuongCauHoi;
            int test_code = (int)sinhvien.DTdanglam;
            double coefficient = 10.0 / (double)total_quest;
            int count_correct = 0;
            foreach (var item in list)
            {
                if (item.BaiLam.DapAn != null && item.BaiLam.DapAn.Trim().Equals(item.CauHoi.DapAn.Trim()))
                    count_correct++;
            }
            double score = coefficient * count_correct;
            string detail = count_correct + "/" + total_quest;
            InsertScore(score, detail);
            return RedirectToAction("PreviewTest/" + test_code);
        }
        [HttpPost]
        public void UpdateStudentTest(FormCollection form)
        {
            SinhVien sv = (SinhVien)Session["SinhVien"];
            int id_quest = Convert.ToInt32(form["id"]);
            string answer = form["answer"];
            var update = db.BaiLams.FirstOrDefault(m => m.MaCH == id_quest&&m.MSSV==sv.MSSV);
            update.DapAn = answer;
            db.SaveChanges();
        }
        [HttpPost]
        public void UpdateTiming(FormCollection form)
        {
            string min = form["min"];
            string sec = form["sec"];
            string time = min + ":" + sec;
        }
        public ActionResult PreviewTest(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SinhVien sv = (SinhVien)Session["SinhVien"];
            var diem = db.Diems.FirstOrDefault(m=>m.MaDT==id&&m.MSSV==sv.MSSV);
            if (diem == null)
            {
                return RedirectToAction("Index",new {thongbao= "Ban chua lam bai thi này" });
            }
            ViewBag.score = GetScore(id);
            return View(GetListQuest(id));
        }
        public Diem GetScore(int test_code)
        {
            Diem score = new Diem();
            SinhVien sv = (SinhVien)Session["SinhVien"];
            try
            {
                score = db.Diems.SingleOrDefault(x => x.MaDT == test_code&&x.MSSV==sv.MSSV);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return score;
        }
        public void InsertScore(double score, string detail)
        {
            SinhVien sinhvien = (SinhVien)Session["SinhVien"];
            var sv = db.DeThis.FirstOrDefault(m => m.MaDT == sinhvien.DTdanglam);
            Diem diem = (Diem)Session["Diem"];
            var s = new Diem();
            s.Diem1 = (decimal?)score;
            s.MSSV = sinhvien.MSSV;
            s.MaDT = sinhvien.DTdanglam;
            s.ThoiGianLam = sv.ThoiGianThi;
            db.Diems.Add(s);
            db.SaveChanges();
        }
    }
}