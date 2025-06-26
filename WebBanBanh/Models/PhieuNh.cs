using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class PhieuNh
{
    [Key]
    public string MaPnh { get; set; } = null!;

    public DateTime NgayNh { get; set; }

    public string MaDdh { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public virtual ICollection<Ctdnh> Ctdnhs { get; set; } = new List<Ctdnh>();

    public virtual DonDh MaDdhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
