using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnCoNgan.Models;

namespace DoAnCoNgan.Controllers
{
    public class NguoiQuanTrisController : Controller
    {
        private DAWebBanGiayEntities db = new DAWebBanGiayEntities();

        // GET: NguoiQuanTris
        public ActionResult Index()
        {
            var nguoiQuanTris = db.NguoiQuanTris.Include(n => n.TaiKhoan);
            return View(nguoiQuanTris.ToList());
        }

        // GET: NguoiQuanTris/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiQuanTri nguoiQuanTri = db.NguoiQuanTris.Find(id);
            if (nguoiQuanTri == null)
            {
                return HttpNotFound();
            }
            return View(nguoiQuanTri);
        }

        // GET: NguoiQuanTris/Create
        public ActionResult Create()
        {
            ViewBag.MaTaiKhoan = new SelectList(db.TaiKhoans, "MaTaiKhoan", "MatKhau");
            return View();
        }

        // POST: NguoiQuanTris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNguoiQuanTri,TenNguoiQuanTri,ChucVu,MaTaiKhoan")] NguoiQuanTri nguoiQuanTri)
        {
            if (ModelState.IsValid)
            {
                db.NguoiQuanTris.Add(nguoiQuanTri);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaTaiKhoan = new SelectList(db.TaiKhoans, "MaTaiKhoan", "MatKhau", nguoiQuanTri.MaTaiKhoan);
            return View(nguoiQuanTri);
        }

        // GET: NguoiQuanTris/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiQuanTri nguoiQuanTri = db.NguoiQuanTris.Find(id);
            if (nguoiQuanTri == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTaiKhoan = new SelectList(db.TaiKhoans, "MaTaiKhoan", "MatKhau", nguoiQuanTri.MaTaiKhoan);
            return View(nguoiQuanTri);
        }

        // POST: NguoiQuanTris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiQuanTri,TenNguoiQuanTri,ChucVu,MaTaiKhoan")] NguoiQuanTri nguoiQuanTri)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoiQuanTri).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaTaiKhoan = new SelectList(db.TaiKhoans, "MaTaiKhoan", "MatKhau", nguoiQuanTri.MaTaiKhoan);
            return View(nguoiQuanTri);
        }

        // GET: NguoiQuanTris/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiQuanTri nguoiQuanTri = db.NguoiQuanTris.Find(id);
            if (nguoiQuanTri == null)
            {
                return HttpNotFound();
            }
            return View(nguoiQuanTri);
        }

        // POST: NguoiQuanTris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NguoiQuanTri nguoiQuanTri = db.NguoiQuanTris.Find(id);
            db.NguoiQuanTris.Remove(nguoiQuanTri);
            db.SaveChanges();
            return RedirectToAction("Index");
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
