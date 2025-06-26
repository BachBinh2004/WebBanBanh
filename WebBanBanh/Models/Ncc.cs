using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class Ncc
{
    [Key]
    public string MaNcc { get; set; } = null!;

    public string TenNcc { get; set; } = null!;

    public string Dcncc { get; set; } = null!;

    public string Sdtncc { get; set; } = null!;

    public virtual ICollection<DonDh> DonDhs { get; set; } = new List<DonDh>();
}
