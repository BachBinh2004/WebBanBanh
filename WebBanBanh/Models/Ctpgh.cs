using System;
using System.Collections.Generic;

namespace WebBanBanh.Models;

public partial class Ctpgh
{
    public string MaBanh { get; set; } = null!;

    public string MaPgh { get; set; } = null!;

    public int Sl { get; set; }

    public virtual Banh MaBanhNavigation { get; set; } = null!;

    public virtual PhieuGh MaPghNavigation { get; set; } = null!;
}
