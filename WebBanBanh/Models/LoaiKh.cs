using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class LoaiKh
{
    [Key]
    public string MaLkh { get; set; } = null!;

    public string TenLkh { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
