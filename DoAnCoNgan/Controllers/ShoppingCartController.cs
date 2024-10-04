using DoAnCoNgan.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DoAnCoNgan.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DAWebBanGiayEntities db = new DAWebBanGiayEntities();

        // Trang gio hang 
        public ActionResult ShowCart()
        {
            int? userId = Session["UserId"] as int?;
            var customerId = Session["CustomerId"] as int?;
            if (Session["Cart"] == null)
                return View("EmptyCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        // phuong thuc gio hang 
        public ActionResult AddtoCart(int id)
        {
            var _pro = db.SanPhams.SingleOrDefault(s => s.MaSanPham == id);
            if (_pro != null)
            {
                GetCart().Add_Product_Cart(_pro);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        /*  public ActionResult Update_Cart_Quantity(FormCollection form)
          {
              Cart cart = Session["Cart"] as Cart;
              int id_pro = int.Parse(form["ID_Prouct"]);
              int quantity = int.Parse(form["Quantity"]);
              cart.Update_Quantity_Shopping(id_pro, quantity);
              return RedirectToAction("ShowCart", "ShoppingCart");
          } */
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            // Lấy giỏ hàng từ Session
            Cart cart = Session["Cart"] as Cart;

            // Lấy thông tin sản phẩm và số lượng từ form
            int id_pro = int.Parse(form["ID_Prouct"]);
            int requestedQuantity = int.Parse(form["Quantity"]);

            // Lấy sản phẩm từ cơ sở dữ liệu
            var product = db.SanPhams.FirstOrDefault(p => p.MaSanPham == id_pro);

            if (product == null)
            {
                // Nếu không tìm thấy sản phẩm, trả về lỗi hoặc thông báo
                return HttpNotFound("Sản phẩm không tồn tại.");
            }

            // Kiểm tra số lượng yêu cầu không vượt quá số lượng sản phẩm
            if (requestedQuantity > product.Soluong)
            {
                // Có thể thêm thông báo lỗi cho người dùng
                TempData["ErrorMessage"] = "Số lượng yêu cầu vượt quá số lượng sản phẩm có sẵn.";
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            // Cập nhật số lượng trong giỏ hàng
            cart.Update_Quantity_Shopping(id_pro, requestedQuantity);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public PartialViewResult BagCart()
        {
            int toltal_quantity_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                toltal_quantity_item = cart.Total_quantity();
            ViewBag.QuantityCart = toltal_quantity_item;
            return PartialView("BagCart");
        }
        public ActionResult CheckOut(FormCollection form)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "LoginUser"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }
            string userEmail = Session["Email"].ToString();

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == userEmail);

            try
            {
                NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

                Cart cart = Session["Cart"] as Cart;
                DonHang _order = new DonHang();

        _order.NgayDatHang = DateTime.Now;
                _order.MaNguoiGui = nguoiDung.MaNguoiDung;
                _order.DiaChiNguoiNhan = form["AddresDelivery"];
                if(cart.Total_quantity() > 5)
                {
                    _order.PhiVanChuyen = 0;
                    _order.GiamGia = (int)cart.Total_money()*(10/100);
                }
                else
                {
                    _order.PhiVanChuyen = 10000;
                    _order.GiamGia = 0;
                }
                
               
                _order.TenNguoiNhan = form["NameNguoiNhan"];
                _order.SDTNguoiNhan = int.Parse(form["SDtNguoiNhan"]);
                _order.TrangThai = "Chưa Giao";
                _order.TongTien = (double)cart.Total_money();
                _order.TongSL = cart.Total_quantity();
                _order.TongSoTien =(int) _order.TongTien * (int)_order.TongSL;
                _order.TienPhaiTra = _order.TongSoTien - _order.GiamGia;
                db.DonHangs.Add(_order);
                db.SaveChanges();
                foreach (var item in cart.Items)
                {
                   
                    ChiTietDonHang _order_detail = new ChiTietDonHang();
                     _order_detail.TienThue = item._product.Thue;
                    _order_detail.TieuGiam = 0;
                    _order_detail.MaDonHang = _order.MaDonHang;
                    _order_detail.MaSanPham = item._product.MaSanPham;
                    _order_detail.DonGia =(int) item._product.GiaTien;
                    _order_detail.Soluong = item._product.Soluong;
                    _order_detail.TongTien = _order_detail.Soluong * _order_detail.DonGia - _order_detail.TienThue;
                    db.ChiTietDonHangs.Add(_order_detail);
               
                    foreach (var p in db.SanPhams.Where(s => s.MaSanPham == _order_detail.MaSanPham))
                    {
                        if (p.Soluong == 0) break;
                        var update_quan_pro = p.Soluong - item._quantity;
                        p.Soluong = update_quan_pro;
                    }
                         db.SaveChanges();
                }
        db.SaveChanges();
                cart.ClearCart();
                return RedirectToAction("CheckOut_Success", "ShoppingCart");
            }
            catch
            {
                return Content("Error checkout. Please check information Cutsmer...Thank.");
            }


        }
        public ActionResult CheckOut_Success()
        {
            return View();
        }
      
    }
}