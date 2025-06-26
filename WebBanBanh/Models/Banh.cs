using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanBanh.Models;

public partial class Banh
{

    [Key]
    public string MaBanh { get; set; } = null!;

    public string TenBanh { get; set; } = null!;

    public DateTime Nsx { get; set; }

    public DateTime Hsd { get; set; }

    public string Mota { get; set; } = null!;

    public string Hinhanh { get; set; } = null!;

    public string MaLb { get; set; } = null!;

    public int Gia { get; set; }

    public virtual ICollection<Cthd> Cthds { get; set; } = new List<Cthd>();

    public virtual ICollection<Ctpgh> Ctpghs { get; set; } = new List<Ctpgh>();
    [NotMapped]
    public virtual LoaiBanh MaLbNavigation { get; set; } = null!;
    public bool IsHidden { get; set; } = false; // Thêm thuộc tính này
}
