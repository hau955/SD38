using AppData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppData.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("Server=LAPTOP-Q4DACOEF\\MSSQLSERVER02;Database=AoDaiModel1;Trusted_Connection=True;TrustServerCertificate=True");//Server=KHUATNGAT201;Database=AoDaiModel;User Id=sa;Password=123456;TrustServerCertificate=true;");/*"Server=DESKTOP-A99GQBL;Database=AoDaiModell;Trusted_Connection=True;TrustServerCertificate=True")*/

            //optionsBuilder.UseSqlServer("Server=QUOC-AN\\QUOC_AN;Database=AoDaiModel;User Id=SA;Password=An344763;TrustServerCertificate=true;");//Server=KHUATNGAT201;Database=AoDaiModel;User Id=sa;Password=123456;TrustServerCertificate=true;");/*"Server=DESKTOP-A99GQBL;Database=AoDaiModell;Trusted_Connection=True;TrustServerCertificate=True")*/

        }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<GioHangCT> GioHangChiTiets { get; set; }
        public DbSet<DiaChiNhanHang> DiaChiNhanHangs { get; set; }
        public DbSet<HinhThucTT> HinhThucTTs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonCT> HoaDonChiTiets { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<AnhSanPham> AnhSanPham { get; set; }

        public DbSet<SanPhamGG> SanPhamGiamGias { get; set; }
        public DbSet<GiamGia> GiamGias { get; set; }
        public DbSet<SanPhamCT> SanPhamChiTiets { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<Size> Sizes { get; set; }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Cần thiết cho Identity

            // Quan hệ ApplicationUser - ApplicationRole (IDRole nullable)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.GioHang)
                .WithOne(r => r.User)
                .HasForeignKey<GioHang>(u => u.IDGioHang)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-1: User - GioHang
            builder.Entity<GioHang>()
                .HasOne(g => g.User)
                .WithOne(u => u.GioHang)
                .HasForeignKey<GioHang>(g => g.IDGioHang)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DanhMuc>()
                .HasMany(d => d.SanPhams)
                .WithOne(sp => sp.DanhMuc)
                .HasForeignKey(sp => sp.DanhMucId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n: GioHang - GioHangChiTiet
            builder.Entity<GioHangCT>()
                .HasOne(ct => ct.GioHang)
                .WithMany(g => g.GioHangChiTiets)
                .HasForeignKey(ct => ct.IDGioHang)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-n: HinhThucTT - HoaDon
            builder.Entity<HoaDon>()
                .HasOne(hd => hd.HinhThucTT)
                .WithMany(ht => ht.HoaDons)
                .HasForeignKey(hd => hd.IDHinhThucTT)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n: DiaChiNhanHang - HoaDon
            builder.Entity<HoaDon>()
                .HasOne(hd => hd.DiaChiNhanHang)
                .WithMany(dc => dc.HoaDons)
                .HasForeignKey(hd => hd.IDDiaChiNhanHang)
                .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade paths

            // 1-n: ApplicationUser - HoaDon
            // Khách hàng đặt đơn
            builder.Entity<HoaDon>()
                .HasOne(hd => hd.User)
                .WithMany(u => u.HoaDonsAsKhachHang)
                .HasForeignKey(hd => hd.IDUser)
                .OnDelete(DeleteBehavior.Restrict);

            // Người tạo đơn (ví dụ là nhân viên tạo thay)
            builder.Entity<HoaDon>()
                .HasOne(hd => hd.User2)
                .WithMany(u => u.HoaDonsAsNguoiTao)
                .HasForeignKey(hd => hd.IDNguoiTao)
                .OnDelete(DeleteBehavior.Restrict);


            // 1-n: HoaDon - HoaDonChiTiet
            builder.Entity<HoaDonCT>()
                .HasOne(ct => ct.HoaDon)
                .WithMany(hd => hd.HoaDonChiTiets)
                .HasForeignKey(ct => ct.IDHoaDon)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AnhSanPham>()
                  .HasOne(spct => spct.SanPham)
                  .WithMany(sp => sp.AnhSanPhams)
                  .HasForeignKey(spct => spct.IDSanPham)
                  .OnDelete(DeleteBehavior.Cascade);

            // 1-n: SanPham - SanPhamChiTiet
            builder.Entity<SanPhamCT>()
                .HasOne(spct => spct.SanPham)
                .WithMany(sp => sp.SanPhamChiTiets)
                .HasForeignKey(spct => spct.IDSanPham)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-n: MauSac - SanPhamChiTiet
            builder.Entity<SanPhamCT>()
                .HasOne(spct => spct.MauSac)
                .WithMany(ms => ms.SanPhamChiTiets)
                .HasForeignKey(spct => spct.IDMauSac)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n: Size - SanPhamChiTiet
            builder.Entity<SanPhamCT>()
                .HasOne(spct => spct.SizeAo)
                .WithMany(sz => sz.SanPhamChiTiets)
                .HasForeignKey(spct => spct.IDSize)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n: CoAo - SanPhamChiTiet
            builder.Entity<SanPhamCT>()
                .HasOne(spct => spct.ChatLieu)
                .WithMany(ca => ca.SanPhamChiTiets)
                .HasForeignKey(spct => spct.IdChatLieu)
                .OnDelete(DeleteBehavior.Restrict);

            // Kiểu decimal mặc định là (18,2)
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                if (string.IsNullOrEmpty(property.GetColumnType()))
                {
                    property.SetColumnType("decimal(18,2)");
                }
            }

        }
        // Ví dụ trong DbContext
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is SanPham && // Hoặc một base entity nếu có
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((SanPham)entityEntry.Entity).NgaySua = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((SanPham)entityEntry.Entity).NgayTao = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
