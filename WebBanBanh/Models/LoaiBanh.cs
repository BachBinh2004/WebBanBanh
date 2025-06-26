using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class LoaiBanh
{
    [Key]
    public string MaLb { get; set; } = null!;

    public string TenLb { get; set; } = null!;

    public virtual ICollection<Banh> Banhs { get; set; } = new List<Banh>();
}
