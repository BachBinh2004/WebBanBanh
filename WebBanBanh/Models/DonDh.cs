using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanBanh.Models;

public partial class DonDh
{
    [Key]
    public string MaDdh { get; set; } = null!;

    public DateTime NgayDh { get; set; }

    public DateTime NgayGiao { get; set; }

    public string MaNcc { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public virtual ICollection<Ctddh> Ctddhs { get; set; } = new List<Ctddh>();

    public virtual ICollection<Ctdnh> Ctdnhs { get; set; } = new List<Ctdnh>();

    public virtual Ncc MaNccNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual ICollection<PhieuNh> PhieuNhs { get; set; } = new List<PhieuNh>();
}
