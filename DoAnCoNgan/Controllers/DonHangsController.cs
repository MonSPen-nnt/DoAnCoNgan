using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnCoNgan.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DoAnCoNgan.Controllers
{
    public class DonHangsController : Controller
    {
        private DAWebBanGiayEntities db = new DAWebBanGiayEntities();
        /*
        // GET: DonHangs
        public ActionResult Index()
        {
            var donHangs = db.DonHangs.Include(d => d.NguoiDung);
            return View(donHangs.ToList());
        }
        */
        //[HttpPost]
        // GET: ChiTietDonHang/Details/5
        public ActionResult ChiTiet(int? maDonHang)
        {
            var chiTietDonHangs = db.ChiTietDonHangs
                                    .Where(c => c.MaDonHang == maDonHang)
                                    .Include(c => c.SanPham) // Bao gồm thông tin sản phẩm
                                    .ToList();
            ViewBag.MaDonHang = maDonHang; // Để hiển thị mã đơn hàng trên trang
            return View(chiTietDonHangs);
        }
        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var donHangs = db.DonHangs.Include(d => d.NguoiDung).AsQueryable();

            if (startDate.HasValue)
            {
                donHangs = donHangs.Where(d => DbFunctions.TruncateTime(d.NgayDatHang) >= DbFunctions.TruncateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                donHangs = donHangs.Where(d => DbFunctions.TruncateTime(d.NgayDatHang) <= DbFunctions.TruncateTime(endDate.Value));
            }

            return View(donHangs.ToList());
        }

        public ActionResult XuatHoaDon(int id)
        {
            var detailProduct = db.DonHangs.Find(id);
            if (detailProduct == null)
            {
                return HttpNotFound();
            }

            return View(detailProduct);
        }
        private const string TimesNewRomanFontPath = @"C:\Windows\Fonts\times.ttf"; // hoặc "C:\Windows\Fonts\timesbd.ttf" cho phiên bản in đậm
        [HttpPost]
        public ActionResult XuatHoaDon(int? id)
        {

            var detailProduct = db.DonHangs.Find(id);
            if (detailProduct == null)
            {
                return HttpNotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                // Tạo tài liệu PDF
                var document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Thêm tiêu đề
                var baseFont = BaseFont.CreateFont(TimesNewRomanFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var titleFont = new Font(baseFont, 20, Font.BOLD);
                var contentFont = new Font(baseFont, 12, Font.NORMAL);

                var titleParagraph = new Paragraph("Bill", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(titleParagraph);

                // Thêm nội dung hóa đơn
                document.Add(new  Paragraph($"Recipient Name: {detailProduct.TenNguoiNhan}", contentFont));
                document.Add(new  Paragraph($"Recipient No: {detailProduct.SDTNguoiNhan}", contentFont));
                 document.Add(new  Paragraph($"Sender Name: {detailProduct.NguoiDung.TenNguoiDung}", contentFont));
                 document.Add(new  Paragraph($"Address: {detailProduct.DiaChiNguoiNhan}", contentFont));
                 document.Add(new  Paragraph($"Total Quantity: {detailProduct.TongSL}", contentFont));

                document.Add(new  Paragraph($"Order Date: {detailProduct.NgayDatHang}", contentFont));
                document.Add(new  Paragraph($"Reduce: {detailProduct.GiamGia}", contentFont));
                document.Add(new  Paragraph($"Total Price: {detailProduct.TongTien:N0}đ", contentFont));
                document.Add(new  Paragraph($"Shipping Fees: {detailProduct.PhiVanChuyen:N0}đ", contentFont));
                document.Add(new  Paragraph($"Pay: {detailProduct.TienPhaiTra:N0}đ", contentFont));


                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "HoaDon.pdf");

            }
        }
                public ActionResult UpdateTrangThai(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs.Find(id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Đã Giao";
            db.Entry(donHang).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UpdateTrangThai_ChuaGiao(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs.Find(id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Chưa Giao";
            db.Entry(donHang).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult UpdateTrangThai_HuyDon(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs.Find(id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            donHang.TrangThai = "Hủy Đơn";
            db.Entry(donHang).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: DonHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // GET: DonHangs/Create
        public ActionResult Create()
        {
            ViewBag.MaNguoiGui = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenNguoiDung");
            return View();
        }

        // POST: DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDonHang,NgayDatHang,TrangThai,PhiVanChuyen,TongTien,MaNguoiGui,SDTNguoiNhan,DiaChiNguoiNhan,TenNguoiNhan,TongSL,TongSoTien,GiamGia,TienPhaiTra")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.DonHangs.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiGui = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenNguoiDung", donHang.MaNguoiGui);
            return View(donHang);
        }

        // GET: DonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoiGui = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenNguoiDung", donHang.MaNguoiGui);
            return View(donHang);
        }

        // POST: DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDonHang,NgayDatHang,TrangThai,PhiVanChuyen,TongTien,MaNguoiGui,SDTNguoiNhan,DiaChiNguoiNhan,TenNguoiNhan,TongSL,TongSoTien,GiamGia,TienPhaiTra")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoiGui = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenNguoiDung", donHang.MaNguoiGui);
            return View(donHang);
        }

        // GET: DonHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonHang donHang = db.DonHangs.Find(id);
            db.DonHangs.Remove(donHang);
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

        public ActionResult ChartData()
        {
            var donHangs = db.DonHangs
                .Where(dh => dh.NgayDatHang != null) // Đảm bảo ngày đặt hàng không null
                .ToList();

            var groupedData = donHangs
                .GroupBy(dh => dh.NgayDatHang)
                .Select(g => new ChartViewModel
                {
                    Date = g.Key.ToString("yyyy-MM-dd"), // Định dạng ngày tháng
                    TotalAmount = g.Sum(dh => dh.TongSoTien)
                })
                .OrderBy(g => g.Date)
                .ToList();

            return Json(groupedData, JsonRequestBehavior.AllowGet);
        }





        public ActionResult Chart()
        {
            return View(); // Trả về view chứa biểu đồ
        }
    }
}
