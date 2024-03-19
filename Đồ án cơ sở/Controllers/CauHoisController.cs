using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Đồ_án_cơ_sở.Models;
using System.Web.Routing;
using System.Web.Services.Description;
using System.Web.Http.Results;

namespace Đồ_án_cơ_sở.Controllers
{
    public class CauHoisController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        public IHttpActionResult AddCauHoi(int id,int madt, DeThi_CauHoi dtch)
        {
            ThiTNEntities1 db = new ThiTNEntities1();
            
            if (db.DeThi_CauHoi.Any(p => p.MaDT == madt && p.MaCH == id))
            {
                db.DeThi_CauHoi.Remove(db.DeThi_CauHoi.SingleOrDefault(p => p.MaDT == madt && p.MaCH == id));
                dtch.isShowGoing = false;
                db.SaveChanges();
                return Ok("cancel");
            }
            var dt = db.DeThis.Find(madt);
            if(db.DeThi_CauHoi.Where(m=>m.MaDT==madt).Count()<dt.SoLuongCauHoi) 
            {
                var attendance = new DeThi_CauHoi()
                {
                    MaCH = id,
                    MaDT = madt
                };
                attendance.isShowGoing = true;
                db.DeThi_CauHoi.Add(attendance);
                db.SaveChanges();
            }
            else
            {
                return BadRequest("Da du so luong cau hoi");
            }
            return Ok();
        }
    }
}