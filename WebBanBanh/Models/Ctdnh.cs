using System;
using System.Collections.Generic;

namespace WebBanBanh.Models;

public partial class Ctdnh
{

    public string MaNl { get; set; } = null!;

    public string MaDdh { get; set; } = null!;

    public string MaPnh { get; set; } = null!;

    public int Sl { get; set; }

    public virtual DonDh MaDdhNavigation { get; set; } = null!;

    public virtual NguyenLieu MaNlNavigation { get; set; } = null!;

    public virtual PhieuNh MaPnhNavigation { get; set; } = null!;
}
