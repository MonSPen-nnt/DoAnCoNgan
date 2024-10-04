using DoAnCoNgan.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Helpers;

namespace DoAnCoNgan.Controllers
{
    public class LoginUserController : Controller
    {
        DAWebBanGiayEntities db = new DAWebBanGiayEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAccount(TaiKhoan _user)
        {
            var check = db.TaiKhoans.Where(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "SaiInfo";
                return View("Index");
            }
            else
            {



                db.Configuration.ValidateOnSaveEnabled = false;
                Session["Email"] = _user.Email;
                Session["MatKhau"] = _user.MatKhau;
                if (check.VaiTro)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("TrangChu", "SanPhams");
                }
              
            }
        }

        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(TaiKhoan _user)
        {
            if (ModelState.IsValid)
            {
                var check_ID = db.TaiKhoans.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_ID == null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.TaiKhoans.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "This ID is exixst";
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("TrangChu", "SanPhams");
        }



        public ActionResult UserDetails()
        {
            string email = Session["Email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);
            if (nguoiDung == null)
            {
                return RedirectToAction("CreateUserDetails");
            }

            return View(nguoiDung);
        }

        public ActionResult EditUserDetails()
        {
            string email = Session["Email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            return View(nguoiDung);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserDetails(NguoiDung nguoiDung, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có file nào được upload không
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Đường dẫn thư mục lưu trữ ảnh
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    // Lưu file vào thư mục
                    ImageFile.SaveAs(path);

                    // Cập nhật đường dẫn ảnh vào model
                    nguoiDung.AnhDaiDien = Path.Combine("/Images", fileName);
                }

                // Tìm người dùng trong cơ sở dữ liệu
                var existingUser = db.NguoiDungs.Find(nguoiDung.MaNguoiDung);
                if (existingUser != null)
                {
                    existingUser.TenNguoiDung = nguoiDung.TenNguoiDung;
                    existingUser.DiaChi = nguoiDung.DiaChi;
                    existingUser.SDT = nguoiDung.SDT;
                    existingUser.AnhDaiDien = nguoiDung.AnhDaiDien;

                    db.Entry(existingUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                // Chuyển hướng đến trang thông tin người dùng
                return RedirectToAction("UserDetails", new { id = nguoiDung.MaNguoiDung });
            }

            // Nếu có lỗi, trả lại view
            return View(nguoiDung);
        }



        /* -------------------------------- */


        public ActionResult CreateUserDetails()
        {
            string email = Session["Email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            if (taiKhoan == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            NguoiDung nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                // Các thuộc tính khác có thể được khởi tạo ở đây nếu cần
            };

            return View(nguoiDung);
        }

        [HttpPost]
        public ActionResult CreateUserDetails(NguoiDung nguoiDung, HttpPostedFileBase ImageFile)
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
                    nguoiDung.AnhDaiDien = "/Images/" + fileName; // Save the path to the image in the model
                }

                db.NguoiDungs.Add(nguoiDung);
                db.SaveChanges();
                return RedirectToAction("UserDetails");
            }

            return View(nguoiDung);
        }

        public ActionResult OrderHistory(int id)
        {
            var orders = db.DonHangs.Where(d => d.MaNguoiGui == id ).ToList();
            return View(orders);
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
                document.Add(new Paragraph($"Recipient Name: {detailProduct.TenNguoiNhan}", contentFont));
                document.Add(new Paragraph($"Recipient No: {detailProduct.SDTNguoiNhan}", contentFont));
                document.Add(new Paragraph($"Sender Name: {detailProduct.NguoiDung.TenNguoiDung}", contentFont));
                document.Add(new Paragraph($"Address: {detailProduct.DiaChiNguoiNhan}", contentFont));
                document.Add(new Paragraph($"Total Quantity: {detailProduct.TongSL}", contentFont));

                document.Add(new Paragraph($"Order Date: {detailProduct.NgayDatHang}", contentFont));
                document.Add(new Paragraph($"Reduce: {detailProduct.GiamGia}", contentFont));
                document.Add(new Paragraph($"Total Price: {detailProduct.TongTien:N0}đ", contentFont));
                document.Add(new Paragraph($"Shipping Fees: {detailProduct.PhiVanChuyen:N0}đ", contentFont));
                document.Add(new Paragraph($"Pay: {detailProduct.TienPhaiTra:N0}đ", contentFont));


                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "HoaDon.pdf");

            }

        }
    

    }
}