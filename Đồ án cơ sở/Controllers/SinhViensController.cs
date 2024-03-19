using Đồ_án_cơ_sở.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Đồ_án_cơ_sở.Controllers
{
    public class SinhViensController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddSinhVien(string id, int madt, DeThi_SInhVien dtch)
        {
            ThiTNEntities1 db = new ThiTNEntities1();

            if (db.DeThi_SInhVien.Any(p => p.MaDT == madt && p.MSSV == id))
            {
                db.DeThi_SInhVien.Remove(db.DeThi_SInhVien.SingleOrDefault(p => p.MaDT == madt && p.MSSV == id));
                dtch.isShowGoing = false;
                db.SaveChanges();
                return Ok("cancel");
            }
            var attendance = new DeThi_SInhVien()
            {
                MSSV = id,
                MaDT = madt
            };
            attendance.isShowGoing = true;
            db.DeThi_SInhVien.Add(attendance);
            db.SaveChanges();
            return Ok();
        }
    }
}
