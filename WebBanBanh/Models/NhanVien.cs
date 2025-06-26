using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanBanh.Models;

public partial class NhanVien
{
    [Key]
    public string MaNv { get; set; } = null!;

    public string TenNv { get; set; } = null!;

    public string GioiTinh { get; set; } = null!;

    public DateTime NgaySinh { get; set; }

    public string ChucVu { get; set; } = null!;

    public string Dcnv { get; set; } = null!;

    public string Sdtnv { get; set; } = null!;

    public virtual ICollection<DonDh> DonDhs { get; set; } = new List<DonDh>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<PhieuGh> PhieuGhs { get; set; } = new List<PhieuGh>();

    public virtual ICollection<PhieuNh> PhieuNhs { get; set; } = new List<PhieuNh>();
}
