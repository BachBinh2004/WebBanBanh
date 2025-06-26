using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBanBanh.Data;
using WebBanBanh.Models;

namespace WebBanBanh.Models;

public partial class WebBanBanhContext : DbContext
{

    public WebBanBanhContext()
    {
    }

    public WebBanBanhContext(DbContextOptions<WebBanBanhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Banh> Banhs { get; set; }

    public virtual DbSet<Ctddh> Ctddhs { get; set; }

    public virtual DbSet<Ctdnh> Ctdnhs { get; set; }

    public virtual DbSet<Cthd> Cthds { get; set; }

    public virtual DbSet<Ctpgh> Ctpghs { get; set; }

    public virtual DbSet<DonDh> DonDhs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiBanh> LoaiBanhs { get; set; }

    public virtual DbSet<LoaiKh> LoaiKhs { get; set; }

    public virtual DbSet<LoaiTk> LoaiTks { get; set; }

    public virtual DbSet<Ncc> Nccs { get; set; }

    public virtual DbSet<NguyenLieu> NguyenLieus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuGh> PhieuGhs { get; set; }

    public virtual DbSet<PhieuNh> PhieuNhs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
    public virtual DbSet<FAQ> FAQs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Banh>(entity =>
        {
            entity.HasKey(e => e.MaBanh).HasName("PK__Banh__8DF1BC0F308BCCA7");

            entity.ToTable("Banh");

            entity.Property(e => e.MaBanh).HasMaxLength(10);
            entity.Property(e => e.Hinhanh)
                .HasMaxLength(250)
                .HasColumnName("HINHANH");
            entity.Property(e => e.Hsd)
                .HasColumnType("datetime")
                .HasColumnName("HSD");
            entity.Property(e => e.MaLb)
                .HasMaxLength(10)
                .HasColumnName("MaLB");
            entity.Property(e => e.Mota)
                .HasMaxLength(100)
                .HasColumnName("MOTA");
            entity.Property(e => e.Nsx)
                .HasColumnType("datetime")
                .HasColumnName("NSX");
            entity.Property(e => e.TenBanh).HasMaxLength(100);

            entity.HasOne(d => d.MaLbNavigation).WithMany(p => p.Banhs)
                .HasForeignKey(d => d.MaLb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Banh__MaLB__403A8C7D");
        });

        modelBuilder.Entity<Ctddh>(entity =>
        {
            entity.HasKey(e => new { e.MaNl, e.MaDdh }).HasName("PK__CTDDH__74FD5BBC8A516C7A");

            entity.ToTable("CTDDH");

            entity.Property(e => e.MaNl)
                .HasMaxLength(10)
                .HasColumnName("MaNL");
            entity.Property(e => e.MaDdh)
                .HasMaxLength(10)
                .HasColumnName("MaDDH");
            entity.Property(e => e.Dgmua).HasColumnName("DGMua");
            entity.Property(e => e.Sl).HasColumnName("SL");

            entity.HasOne(d => d.MaDdhNavigation).WithMany(p => p.Ctddhs)
                .HasForeignKey(d => d.MaDdh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTDDH__MaDDH__68487DD7");

            entity.HasOne(d => d.MaNlNavigation).WithMany(p => p.Ctddhs)
                .HasForeignKey(d => d.MaNl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTDDH__MaNL__6754599E");
        });

        modelBuilder.Entity<Ctdnh>(entity =>
        {
            entity.HasKey(e => new { e.MaNl, e.MaDdh, e.MaPnh }).HasName("PK__CTDNH__E1C7B85BF63DAE3F");

            entity.ToTable("CTDNH");

            entity.Property(e => e.MaNl)
                .HasMaxLength(10)
                .HasColumnName("MaNL");
            entity.Property(e => e.MaDdh)
                .HasMaxLength(10)
                .HasColumnName("MaDDH");
            entity.Property(e => e.MaPnh)
                .HasMaxLength(10)
                .HasColumnName("MaPNH");
            entity.Property(e => e.Sl).HasColumnName("SL");

            entity.HasOne(d => d.MaDdhNavigation).WithMany(p => p.Ctdnhs)
                .HasForeignKey(d => d.MaDdh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTDNH__MaDDH__6FE99F9F");

            entity.HasOne(d => d.MaNlNavigation).WithMany(p => p.Ctdnhs)
                .HasForeignKey(d => d.MaNl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTDNH__MaNL__6EF57B66");

            entity.HasOne(d => d.MaPnhNavigation).WithMany(p => p.Ctdnhs)
                .HasForeignKey(d => d.MaPnh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTDNH__MaPNH__70DDC3D8");
        });

        modelBuilder.Entity<Cthd>(entity =>
        {
            entity.HasKey(e => new { e.MaBanh, e.MaHd }).HasName("PK__CTHD__9F83E661CD8AA3A1");

            entity.ToTable("CTHD");

            entity.Property(e => e.MaBanh).HasMaxLength(10);
            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .HasColumnName("MaHD");
            entity.Property(e => e.Dgban).HasColumnName("DGBan");
            entity.Property(e => e.Sl).HasColumnName("SL");

            entity.HasOne(d => d.MaBanhNavigation).WithMany(p => p.Cthds)
                .HasForeignKey(d => d.MaBanh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHD__MaBanh__6383C8BA");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthds)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHD__MaHD__6477ECF3");
        });

        modelBuilder.Entity<Ctpgh>(entity =>
        {
            entity.HasKey(e => new { e.MaBanh, e.MaPgh }).HasName("PK__CTPGH__6E5F81E436DD693A");

            entity.ToTable("CTPGH");

            entity.Property(e => e.MaBanh).HasMaxLength(10);
            entity.Property(e => e.MaPgh)
                .HasMaxLength(10)
                .HasColumnName("MaPGH");
            entity.Property(e => e.Sl).HasColumnName("SL");

            entity.HasOne(d => d.MaBanhNavigation).WithMany(p => p.Ctpghs)
                .HasForeignKey(d => d.MaBanh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTPGH__MaBanh__6B24EA82");

            entity.HasOne(d => d.MaPghNavigation).WithMany(p => p.Ctpghs)
                .HasForeignKey(d => d.MaPgh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTPGH__MaPGH__6C190EBB");
        });

        modelBuilder.Entity<DonDh>(entity =>
        {
            entity.HasKey(e => e.MaDdh).HasName("PK__DonDH__3D88C804B2B24EFF");

            entity.ToTable("DonDH");

            entity.Property(e => e.MaDdh)
                .HasMaxLength(10)
                .HasColumnName("MaDDH");
            entity.Property(e => e.MaNcc)
                .HasMaxLength(10)
                .HasColumnName("MaNCC");
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayDh)
                .HasColumnType("datetime")
                .HasColumnName("NgayDH");
            entity.Property(e => e.NgayGiao).HasColumnType("datetime");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.DonDhs)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonDH__MaNCC__5535A963");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.DonDhs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonDH__MaNV__5629CD9C");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E013FBF120");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .HasColumnName("MaHD");
            entity.Property(e => e.HinhThuc).HasMaxLength(100);
            entity.Property(e => e.MaKh)
                .HasMaxLength(10)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayGd)
                .HasColumnType("datetime")
                .HasColumnName("NgayGD");
            entity.Property(e => e.NgayLap).HasColumnType("datetime");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaKH__4F7CD00D");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNV__5070F446");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E9091E70C");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(10)
                .HasColumnName("MaKH");
            entity.Property(e => e.Dckh)
                .HasMaxLength(100)
                .HasColumnName("DCKH");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.MaLkh)
                .HasMaxLength(10)
                .HasColumnName("MaLKH");
            entity.Property(e => e.Sdtkh)
                .HasMaxLength(15)
                .HasColumnName("SDTKH");
            entity.Property(e => e.TenKh)
                .HasMaxLength(100)
                .HasColumnName("TenKH");

            entity.HasOne(d => d.MaLkhNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaLkh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaLKH__48CFD27E");
        });

        modelBuilder.Entity<LoaiBanh>(entity =>
        {
            entity.HasKey(e => e.MaLb).HasName("PK__LoaiBanh__2725C761B9F2C9D3");

            entity.ToTable("LoaiBanh");

            entity.Property(e => e.MaLb)
                .HasMaxLength(10)
                .HasColumnName("MaLB");
            entity.Property(e => e.TenLb)
                .HasMaxLength(100)
                .HasColumnName("TenLB");
        });

        modelBuilder.Entity<LoaiKh>(entity =>
        {
            entity.HasKey(e => e.MaLkh).HasName("PK__LoaiKH__3B9BFEFC41BB5C7A");

            entity.ToTable("LoaiKH");

            entity.Property(e => e.MaLkh)
                .HasMaxLength(10)
                .HasColumnName("MaLKH");
            entity.Property(e => e.TenLkh)
                .HasMaxLength(100)
                .HasColumnName("TenLKH");
        });

        modelBuilder.Entity<LoaiTk>(entity =>
        {
            entity.HasKey(e => e.MaLtk).HasName("PK__LoaiTK__3B983714629749C6");

            entity.ToTable("LoaiTK");

            entity.Property(e => e.MaLtk)
                .HasMaxLength(10)
                .HasColumnName("MaLTK");
            entity.Property(e => e.TenLtk)
                .HasMaxLength(100)
                .HasColumnName("TenLTK");
        });

        modelBuilder.Entity<Ncc>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK__NCC__3A185DEB4CD8139B");

            entity.ToTable("NCC");

            entity.Property(e => e.MaNcc)
                .HasMaxLength(10)
                .HasColumnName("MaNCC");
            entity.Property(e => e.Dcncc)
                .HasMaxLength(100)
                .HasColumnName("DCNCC");
            entity.Property(e => e.Sdtncc)
                .HasMaxLength(15)
                .HasColumnName("SDTNCC");
            entity.Property(e => e.TenNcc)
                .HasMaxLength(100)
                .HasColumnName("TenNCC");
        });

        modelBuilder.Entity<NguyenLieu>(entity =>
        {
            entity.HasKey(e => e.MaNl).HasName("PK__NguyenLi__2725D73CF847C293");

            entity.ToTable("NguyenLieu");

            entity.Property(e => e.MaNl)
                .HasMaxLength(10)
                .HasColumnName("MaNL");
            entity.Property(e => e.TenNl)
                .HasMaxLength(100)
                .HasColumnName("TenNL");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70A6B3AEDE2");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(100);
            entity.Property(e => e.Dcnv)
                .HasMaxLength(100)
                .HasColumnName("DCNV");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.Sdtnv)
                .HasMaxLength(15)
                .HasColumnName("SDTNV");
            entity.Property(e => e.TenNv)
                .HasMaxLength(100)
                .HasColumnName("TenNV");
        });

        modelBuilder.Entity<PhieuGh>(entity =>
        {
            entity.HasKey(e => e.MaPgh).HasName("PK__PhieuGH__3AE3DEBF1E1B5CA8");

            entity.ToTable("PhieuGH", tb => tb.HasTrigger("trg_Check_NgayGiao"));

            entity.Property(e => e.MaPgh)
                .HasMaxLength(10)
                .HasColumnName("MaPGH");
            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .HasColumnName("MaHD");
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayGiao).HasColumnType("datetime");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.PhieuGhs)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuGH__MaHD__5EBF139D");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuGhs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuGH__MaNV__5FB337D6");
        });

        modelBuilder.Entity<PhieuNh>(entity =>
        {
            entity.HasKey(e => e.MaPnh).HasName("PK__PhieuNH__3AE3E794EDFF3D66");

            entity.ToTable("PhieuNH", tb => tb.HasTrigger("trg_Check_NgayNH"));

            entity.Property(e => e.MaPnh)
                .HasMaxLength(10)
                .HasColumnName("MaPNH");
            entity.Property(e => e.MaDdh)
                .HasMaxLength(10)
                .HasColumnName("MaDDH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayNh)
                .HasColumnType("datetime")
                .HasColumnName("NgayNH");

            entity.HasOne(d => d.MaDdhNavigation).WithMany(p => p.PhieuNhs)
                .HasForeignKey(d => d.MaDdh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNH__MaDDH__59FA5E80");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuNhs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNH__MaNV__5AEE82B9");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TaiKhoan__272500705FFB699A");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.MaTk)
                .HasMaxLength(10)
                .HasColumnName("MaTK");
            entity.Property(e => e.MaLtk)
                .HasMaxLength(10)
                .HasColumnName("MaLTK");
            entity.Property(e => e.Mk)
                .HasMaxLength(100)
                .HasColumnName("MK");
            entity.Property(e => e.TenDn)
                .HasMaxLength(100)
                .HasColumnName("TenDN");

            entity.HasOne(d => d.MaLtkNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaLtk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaiKhoan__MaLTK__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<WebBanBanh.Models.CartItem> CartItem { get; set; } = default!;
}
