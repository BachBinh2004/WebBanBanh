using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class PhieuGh
{
    [Key]
    public string MaPgh { get; set; } = null!;

    public DateTime NgayGiao { get; set; }

    public string MaHd { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public virtual ICollection<Ctpgh> Ctpghs { get; set; } = new List<Ctpgh>();

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
