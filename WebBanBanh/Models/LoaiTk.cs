using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class LoaiTk
{
    [Key]
    public string MaLtk { get; set; } = null!;

    public string TenLtk { get; set; } = null!;

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
