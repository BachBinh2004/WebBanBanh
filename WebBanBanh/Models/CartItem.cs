using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanBanh.Models;

public partial class CartItem
{

    [Key]
    //thông tin sản phẩm
    public string Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    //lưu số lượng
    public int Quantity { get; set; }
  
}
