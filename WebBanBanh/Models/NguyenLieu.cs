using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanBanh.Models;

public partial class NguyenLieu
{
    [Key]
    public string MaNl { get; set; } = null!;

    public string TenNl { get; set; } = null!;

    public virtual ICollection<Ctddh> Ctddhs { get; set; } = new List<Ctddh>();

    public virtual ICollection<Ctdnh> Ctdnhs { get; set; } = new List<Ctdnh>();
}
