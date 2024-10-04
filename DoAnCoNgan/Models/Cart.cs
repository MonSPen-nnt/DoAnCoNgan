using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCoNgan.Models
{
    public class CartItem
    {

        public SanPham _product { get; set; }
        public int _quantity { get; set; }

    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }
        public void Add_Product_Cart(SanPham _pro, int _quan = 1)
        {
            var item = items.FirstOrDefault(s => s._product.MaSanPham == _pro.MaSanPham);
            if (item == null)
                items.Add(new CartItem
                {
                    _product = _pro,
                    _quantity = _quan
                });
            else
                item._quantity += _quan;

        }
        public int Total_quantity()
        {
            return items.Sum(s => s._quantity);
        }
        public decimal Total_money()
        {
            var total = items.Sum(s => s._quantity * s._product.GiaTien);
            return (decimal)total;
        }
        public void Update_quantity(int id, int _new_quan)
        {
            var item = items.Find(s => s._product.MaSanPham == id);
            if (item != null)
            {
                if (items.Find(s => s._product.Soluong > _new_quan) != null)
                    item._quantity = _new_quan;
                else item._quantity = 1;
            }
        }
        public void ClearCart()
        {
            items.Clear();
        }
        public void Update_Quantity_Shopping(int id, int _quantity)
        {
            var item = items.Find(s => s._product.MaSanPham == id);
            if (item != null)
            {
                item._quantity = _quantity;
            }
        }
        public void Remove_CartItem(int id)
        {
            items.RemoveAll(s => s._product.MaSanPham == id);
        }



    }
}