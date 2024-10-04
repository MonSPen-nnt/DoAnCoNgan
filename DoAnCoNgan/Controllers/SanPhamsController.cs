using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DoAnCoNgan.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace DoAnCoNgan.Controllers
{
    public class SanPhamsController : Controller
    {
        private DAWebBanGiayEntities db = new DAWebBanGiayEntities();

        // GET: SanPhams
        public ActionResult Index()
        {
            var sanPhams = db.SanPhams.Include(s => s.CauHinh).Include(s => s.DanhMuc).Include(s => s.MAU);
            return View(sanPhams.ToList());
        }
/*
        //public ActionResult XuatHoaDon(int id)
        //{
        //    var detailProduct = db.SanPhams.Find(id);
        //    if (detailProduct == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(detailProduct);
        //}
        //[HttpPost]
        //public ActionResult XuatHoaDon(int? id)
        //{

        //    var detailProduct = db.SanPhams.Find(id);
        //    if (detailProduct == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        // Tạo tài liệu PDF
        //        var document = new iTextSharp.text.Document(PageSize.A4);
        //        PdfWriter.GetInstance(document, memoryStream);

        //        document.Open();

        //        // Thêm tiêu đề
        //        var titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 20);
        //        var titleParagraph = new iTextSharp.text.Paragraph("Bill", titleFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER
        //        };
        //        document.Add(titleParagraph);

        //        // Thêm nội dung hóa đơn
        //        var contentFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);
        //        document.Add(new iTextSharp.text.Paragraph($"Product: {detailProduct.TenSanPham}", contentFont));
        //        document.Add(new iTextSharp.text.Paragraph($"Quantity: {detailProduct.Soluong}", contentFont));
        //        document.Add(new iTextSharp.text.Paragraph($"Total Price: {detailProduct.GiaTien:N0}đ", contentFont));

        //        document.Close();

        //        return File(memoryStream.ToArray(), "application/pdf", "HoaDon.pdf");
            



        //    //var detailProduct = db.SanPhams.Find(id);
        //    //if (detailProduct == null)
        //    //{
        //    //    return new HttpNotFoundResult("Detail product not found.");
        //    //}

        //    //using (var doc = DocX.Create(new MemoryStream()))
        //    //{
        //    //    doc.InsertParagraph("Hóa Đơn").FontSize(20).Bold().Alignment = Alignment.center;

        //    //    doc.InsertParagraph($"Product: {detailProduct.TenSanPham}").FontSize(12);
        //    //    doc.InsertParagraph($": {detailProduct.Soluong}").FontSize(12);
        //    //    doc.InsertParagraph($"Total Price: {detailProduct.GiaTien:N0}đ").FontSize(12);



        //    //    var content = new MemoryStream();
        //    //    doc.SaveAs(content);
        //    //    content.Position = 0;

        //    //    return File(content.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Hóa đơn.docx");
        //    }
        //}
    
        */

        public ActionResult TrangChu()
        {
            var sanPhams = db.SanPhams.Include(s => s.CauHinh).Include(s => s.DanhMuc).Include(s => s.MAU).ToList();
            return View(sanPhams);
        }
        public ActionResult LoaiSanPham(int id)
        {
            var sanPhams = db.SanPhams.Where(sp => sp.DanhMuc.MaDanhMuc == id).ToList();
            return View(sanPhams);
        }
        public ActionResult SanPhamTheoTenDanhMuc(string tenDanhMuc)
        {
            var sanPhams = db.SanPhams.Where(sp => sp.DanhMuc.TenDanhMuc == tenDanhMuc).ToList();
            return View(sanPhams);
        }

        public ActionResult Search(string searchTerm)
        {

            if (string.IsNullOrWhiteSpace(searchTerm))
            {

                return RedirectToAction("TrangChu");
            }


            var searchTermLower = searchTerm.ToLower();

            var searchResults = db.SanPhams
                .Where(p => p.TenSanPham.ToLower().Contains(searchTermLower))
                .ToList();
            ViewBag.SearchTerm = searchTerm;
            return View("TrangChu", searchResults);
        }

        // GET: SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            var binhLuans = db.BinhLuans.Where(b => b.MaSanPham == id).ToList();
            ViewBag.BinhLuans = binhLuans;
            return View(sanPham);
        }
        [HttpPost]
        public ActionResult AddComment(int MaSanPham, string NoiDung)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            string email = Session["Email"].ToString();
            var nguoiDung = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email)?.NguoiDungs.FirstOrDefault();

            if (nguoiDung == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            BinhLuan binhLuan = new BinhLuan
            {
                MaSanPham = MaSanPham,
                MaNguoiDung = nguoiDung.MaNguoiDung,
                NoiDung = NoiDung,
                NgayBinhLuan = DateTime.Now
            };

            db.BinhLuans.Add(binhLuan);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = MaSanPham });
        }
        // GET: SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.Thue = new SelectList(db.CauHinhs, "IdCauHinh", "TenCauHinh");
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MauSac = new SelectList(db.MAUs, "MAMAU", "TENMAU");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaSanPham,TenSanPham,GiaTien,HangGiay,SoLuong,MoTa,Size,MauSac,AnhSP1,AnhSP2,AnhSP3,MaDanhMuc,Thue,GiamGia")] SanPham sanPham,  HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string directoryPath = Server.MapPath("~/Images");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    string path = Path.Combine(directoryPath, fileName);
                    ImageFile.SaveAs(path);
                    sanPham.AnhSP1 = "/Images/" + fileName; // Save the path to the image in the model
                }

                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.Thue = new SelectList(db.CauHinhs, "IdCauHinh", "TenCauHinh", sanPham.Thue);
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MauSac = new SelectList(db.MAUs, "MAMAU", "TENMAU", sanPham.MauSac);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSanPham,TenSanPham,GiaTien,SoLuong,HangGiay,MoTa,Size,MauSac,AnhSP1,AnhSP2,AnhSP3,MaDanhMuc,Thue,GiamGia")] SanPham sanPham, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Đường dẫn thư mục lưu trữ ảnh
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    // Lưu file vào thư mục
                    ImageFile.SaveAs(path);

                    // Cập nhật đường dẫn ảnh vào model
                    sanPham.AnhSP1= Path.Combine("/Images", fileName);
                }

              
                

                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Thue = new SelectList(db.CauHinhs, "IdCauHinh", "TenCauHinh", sanPham.Thue);
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MauSac = new SelectList(db.MAUs, "MAMAU", "TENMAU", sanPham.MauSac);
            return View(sanPham);
        }

        // GET: SanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanPham);
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
        public ActionResult ThongKeDanhMuc()
        {
            var data = db.SanPhams
                .GroupBy(sp => sp.DanhMuc.TenDanhMuc)
                .Select(g => new DanhMucStatisticsViewModel
                {
                    TenDanhMuc = g.Key,
                    SoLuong = g.Sum(sp => sp.Soluong)
                })
                .ToList();

            return View(data);
        }
        public ActionResult NewestProducts()
        {
            DateTime DaysAgo = DateTime.Now.AddDays(-10);
            var newestProducts = db.SanPhams
                .Where(sp => sp.NgayTao >= DaysAgo)
                .OrderByDescending(sp => sp.NgayTao)
                .ToList();

            return View(newestProducts);
        }
    }
}
