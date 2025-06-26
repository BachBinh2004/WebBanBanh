using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace WebBanBanh.Models;

public partial class HoaDon
{
    [Key]
    public string MaHd { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public DateTime NgayGd { get; set; }

    public string HinhThuc { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public virtual ICollection<Cthd> Cthds { get; set; } = new List<Cthd>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual ICollection<PhieuGh> PhieuGhs { get; set; } = new List<PhieuGh>();
}
