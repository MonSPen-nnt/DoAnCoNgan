//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnCoNgan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NguoiQuanTri
    {
        public int MaNguoiQuanTri { get; set; }
        public string TenNguoiQuanTri { get; set; }
        public string ChucVu { get; set; }
        public int MaTaiKhoan { get; set; }
    
        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}