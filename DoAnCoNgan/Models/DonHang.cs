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
    
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            this.ChiTietDonHangs = new HashSet<ChiTietDonHang>();
        }
    
        public int MaDonHang { get; set; }
        public System.DateTime NgayDatHang { get; set; }
        public string TrangThai { get; set; }
        public double PhiVanChuyen { get; set; }
        public double TongTien { get; set; }
        public int MaNguoiGui { get; set; }
        public int SDTNguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public int TongSL { get; set; }
        public int TongSoTien { get; set; }
        public int GiamGia { get; set; }
        public int TienPhaiTra { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        public int MaNguoiDung { get; internal set; }
    }
}