using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanBanh.Models;

public partial class TaiKhoan
{
    [Key]
    public string MaTk { get; set; } = null!;

    public string MaLtk { get; set; } = null!;

    public string Mk { get; set; } = null!;

    public string TenDn { get; set; } = null!;

    public virtual LoaiTk MaLtkNavigation { get; set; } = null!;
}
