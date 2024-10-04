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
    public class MAUsController : Controller
    {
        private DAWebBanGiayEntities db = new DAWebBanGiayEntities();

        // GET: MAUs
        public ActionResult Index()
        {
            return View(db.MAUs.ToList());
        }

        // GET: MAUs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAU mAU = db.MAUs.Find(id);
            if (mAU == null)
            {
                return HttpNotFound();
            }
            return View(mAU);
        }

        // GET: MAUs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MAUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAMAU,TENMAU,RBG")] MAU mAU)
        {
            if (ModelState.IsValid)
            {
                db.MAUs.Add(mAU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mAU);
        }

        // GET: MAUs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAU mAU = db.MAUs.Find(id);
            if (mAU == null)
            {
                return HttpNotFound();
            }
            return View(mAU);
        }

        // POST: MAUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAMAU,TENMAU,RBG")] MAU mAU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mAU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mAU);
        }

        // GET: MAUs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAU mAU = db.MAUs.Find(id);
            if (mAU == null)
            {
                return HttpNotFound();
            }
            return View(mAU);
        }

        // POST: MAUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MAU mAU = db.MAUs.Find(id);
            db.MAUs.Remove(mAU);
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
