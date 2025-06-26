using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanBanh.Models;

public partial class KhachHang
{
    [Key]
    public string MaKh { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    public string GioiTinh { get; set; } = null!;

    public string Dckh { get; set; } = null!;

    public string Sdtkh { get; set; } = null!;

    public string MaLkh { get; set; } = null!;

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual LoaiKh MaLkhNavigation { get; set; } = null!;
}
