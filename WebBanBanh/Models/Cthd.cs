using System;
using System.Collections.Generic;

namespace WebBanBanh.Models;

public partial class Cthd
{
    public string MaBanh { get; set; } = null!;

    public string MaHd { get; set; } = null!;

    public int Dgban { get; set; }

    public int Sl { get; set; }

    public virtual Banh MaBanhNavigation { get; set; } = null!;

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
