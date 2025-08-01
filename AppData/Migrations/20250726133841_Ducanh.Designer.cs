﻿// <auto-generated />
using System;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppData.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]

    [Migration("20250726133841_Ducanh")]
    partial class Ducanh
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AppData.Models.AnhSanPham", b =>
                {
                    b.Property<Guid>("IdAnh")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AnhChinh")
                        .HasColumnType("bit");

                    b.Property<string>("DuongDanAnh")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("IDSanPham")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdAnh");

                    b.HasIndex("IDSanPham");

                    b.ToTable("AnhSanPham");
                });

            modelBuilder.Entity("AppData.Models.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("AppData.Models.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool?>("GioiTinh")
                        .HasColumnType("bit");

                    b.Property<string>("HinhAnh")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoDienThoai")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("AppData.Models.ChatLieu", b =>
                {
                    b.Property<Guid>("IDChatLieu")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenChatLieu")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDChatLieu");

                    b.ToTable("ChatLieus");
                });

            modelBuilder.Entity("AppData.Models.DanhMuc", b =>
                {
                    b.Property<Guid>("DanhMucId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MoTa")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenDanhMuc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("DanhMucId");

                    b.ToTable("DanhMucs");
                });

            modelBuilder.Entity("AppData.Models.DiaChiNhanHang", b =>
                {
                    b.Property<Guid>("IDDiaChiNhanHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DiaChiChiTiet")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("HoTenNguoiNhan")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("IDUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("SoDienThoai")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDDiaChiNhanHang");

                    b.HasIndex("IDUser");

                    b.ToTable("DiaChiNhanHangs");
                });

            modelBuilder.Entity("AppData.Models.GiamGia", b =>
                {
                    b.Property<Guid>("IDGiamGia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DieuKienGiamGia")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<float?>("GiamTheoPhanTram")
                        .HasColumnType("real");

                    b.Property<decimal?>("GiamTheoTien")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("LoaiGiamGia")
                        .HasColumnType("bit");

                    b.Property<DateTime>("NgayBatDau")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayKetThuc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenMaGiamGia")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDGiamGia");

                    b.ToTable("GiamGias");
                });

            modelBuilder.Entity("AppData.Models.GioHang", b =>
                {
                    b.Property<Guid>("IDGioHang")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDGioHang");

                    b.ToTable("GioHangs");
                });

            modelBuilder.Entity("AppData.Models.GioHangCT", b =>
                {
                    b.Property<Guid>("IDGioHangChiTiet")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("IDGioHang")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IDSanPhamCT")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDGioHangChiTiet");

                    b.HasIndex("IDGioHang");

                    b.HasIndex("IDSanPhamCT");

                    b.ToTable("GioHangChiTiets");
                });

            modelBuilder.Entity("AppData.Models.HinhThucTT", b =>
                {
                    b.Property<Guid>("IDHinhThucTT")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MoTa")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenHinhThucTT")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDHinhThucTT");

                    b.ToTable("HinhThucTTs");
                });

            modelBuilder.Entity("AppData.Models.HoaDonCT", b =>
                {
                    b.Property<Guid>("IDHoaDonChiTiet")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("GiaSanPham")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("GiaSauGiamGia")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("IDHoaDon")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IDSanPhamCT")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("SoLuongSanPham")
                        .HasColumnType("int");

                    b.Property<string>("TenSanPham")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDHoaDonChiTiet");

                    b.HasIndex("IDHoaDon");

                    b.HasIndex("IDSanPhamCT");

                    b.ToTable("HoaDonChiTiets");
                });

            modelBuilder.Entity("AppData.Models.MauSac", b =>
                {
                    b.Property<Guid>("IDMauSac")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenMau")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDMauSac");

                    b.ToTable("MauSacs");
                });

            modelBuilder.Entity("AppData.Models.SanPham", b =>
                {
                    b.Property<Guid>("IDSanPham")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DanhMucId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("GioiTinh")
                        .HasColumnType("bit");

                    b.Property<string>("MoTa")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenSanPham")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.Property<double?>("TrongLuong")
                        .HasColumnType("float");

                    b.HasKey("IDSanPham");

                    b.HasIndex("DanhMucId");

                    b.ToTable("SanPhams");
                });

            modelBuilder.Entity("AppData.Models.SanPhamCT", b =>
                {
                    b.Property<Guid>("IDSanPhamCT")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("IDMauSac")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IDSanPham")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IDSize")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdChatLieu")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("SoLuongTonKho")
                        .HasColumnType("int");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDSanPhamCT");

                    b.HasIndex("IDMauSac");

                    b.HasIndex("IDSanPham");

                    b.HasIndex("IDSize");

                    b.HasIndex("IdChatLieu");

                    b.ToTable("SanPhamChiTiets");
                });

            modelBuilder.Entity("AppData.Models.SanPhamGG", b =>
                {
                    b.Property<Guid>("IDSPGiamGia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("DonGia")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("IDGiamGia")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IDSanPhamCT")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("SoTienConLai")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDSPGiamGia");

                    b.HasIndex("IDGiamGia");

                    b.HasIndex("IDSanPhamCT");

                    b.ToTable("SanPhamGiamGias");
                });

            modelBuilder.Entity("AppData.Models.Size", b =>
                {
                    b.Property<Guid>("IDSize")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("SoSize")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("IDSize");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("HoaDon", b =>
                {
                    b.Property<Guid>("IDHoaDon")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("GhiChu")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid?>("IDDiaChiNhanHang")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IDHinhThucTT")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IDNguoiTao")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IDUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("NgaySua")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayThanhToan")
                        .HasColumnType("datetime2");

                    b.Property<float?>("PhanTramGiamGiaHoaDon")
                        .HasColumnType("real");

                    b.Property<decimal>("TienGiam")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal?>("TienGiamHoaDon")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("TongTienSauGiam")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("TongTienTruocGiam")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("TrangThaiDonHang")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TrangThaiThanhToan")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IDHoaDon");

                    b.HasIndex("IDDiaChiNhanHang");

                    b.HasIndex("IDHinhThucTT");

                    b.HasIndex("IDNguoiTao");

                    b.HasIndex("IDUser");

                    b.ToTable("HoaDons");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AppData.Models.AnhSanPham", b =>
                {
                    b.HasOne("AppData.Models.SanPham", "SanPham")
                        .WithMany("AnhSanPhams")
                        .HasForeignKey("IDSanPham")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("AppData.Models.DiaChiNhanHang", b =>
                {
                    b.HasOne("AppData.Models.ApplicationUser", "User")
                        .WithMany("DiaChiNhanHangs")
                        .HasForeignKey("IDUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AppData.Models.GioHang", b =>
                {
                    b.HasOne("AppData.Models.ApplicationUser", "User")
                        .WithOne("GioHang")
                        .HasForeignKey("AppData.Models.GioHang", "IDGioHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AppData.Models.GioHangCT", b =>
                {
                    b.HasOne("AppData.Models.GioHang", "GioHang")
                        .WithMany("GioHangChiTiets")
                        .HasForeignKey("IDGioHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppData.Models.SanPhamCT", "SanPhamCT")
                        .WithMany("GioHangChiTiets")
                        .HasForeignKey("IDSanPhamCT");

                    b.Navigation("GioHang");

                    b.Navigation("SanPhamCT");
                });

            modelBuilder.Entity("AppData.Models.HoaDonCT", b =>
                {
                    b.HasOne("HoaDon", "HoaDon")
                        .WithMany("HoaDonChiTiets")
                        .HasForeignKey("IDHoaDon")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppData.Models.SanPhamCT", "SanPhamCT")
                        .WithMany("HoaDonChiTiets")
                        .HasForeignKey("IDSanPhamCT")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HoaDon");

                    b.Navigation("SanPhamCT");
                });

            modelBuilder.Entity("AppData.Models.SanPham", b =>
                {
                    b.HasOne("AppData.Models.DanhMuc", "DanhMuc")
                        .WithMany("SanPhams")
                        .HasForeignKey("DanhMucId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DanhMuc");
                });

            modelBuilder.Entity("AppData.Models.SanPhamCT", b =>
                {
                    b.HasOne("AppData.Models.MauSac", "MauSac")
                        .WithMany("SanPhamChiTiets")
                        .HasForeignKey("IDMauSac")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AppData.Models.SanPham", "SanPham")
                        .WithMany("SanPhamChiTiets")
                        .HasForeignKey("IDSanPham")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppData.Models.Size", "SizeAo")
                        .WithMany("SanPhamChiTiets")
                        .HasForeignKey("IDSize")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AppData.Models.ChatLieu", "ChatLieu")
                        .WithMany("SanPhamChiTiets")
                        .HasForeignKey("IdChatLieu")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ChatLieu");

                    b.Navigation("MauSac");

                    b.Navigation("SanPham");

                    b.Navigation("SizeAo");
                });

            modelBuilder.Entity("AppData.Models.SanPhamGG", b =>
                {
                    b.HasOne("AppData.Models.GiamGia", "GiamGia")
                        .WithMany("SanPhamGiamGias")
                        .HasForeignKey("IDGiamGia")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppData.Models.SanPhamCT", "SanPhamCT")
                        .WithMany("SanPhamGiamGias")
                        .HasForeignKey("IDSanPhamCT");

                    b.Navigation("GiamGia");

                    b.Navigation("SanPhamCT");
                });

            modelBuilder.Entity("HoaDon", b =>
                {
                    b.HasOne("AppData.Models.DiaChiNhanHang", "DiaChiNhanHang")
                        .WithMany("HoaDons")
                        .HasForeignKey("IDDiaChiNhanHang")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AppData.Models.HinhThucTT", "HinhThucTT")
                        .WithMany("HoaDons")
                        .HasForeignKey("IDHinhThucTT")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AppData.Models.ApplicationUser", "User2")
                        .WithMany("HoaDonsAsNguoiTao")
                        .HasForeignKey("IDNguoiTao")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AppData.Models.ApplicationUser", "User")
                        .WithMany("HoaDonsAsKhachHang")
                        .HasForeignKey("IDUser")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("DiaChiNhanHang");

                    b.Navigation("HinhThucTT");

                    b.Navigation("User");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("AppData.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("AppData.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("AppData.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("AppData.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppData.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("AppData.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppData.Models.ApplicationUser", b =>
                {
                    b.Navigation("DiaChiNhanHangs");

                    b.Navigation("GioHang");

                    b.Navigation("HoaDonsAsKhachHang");

                    b.Navigation("HoaDonsAsNguoiTao");
                });

            modelBuilder.Entity("AppData.Models.ChatLieu", b =>
                {
                    b.Navigation("SanPhamChiTiets");
                });

            modelBuilder.Entity("AppData.Models.DanhMuc", b =>
                {
                    b.Navigation("SanPhams");
                });

            modelBuilder.Entity("AppData.Models.DiaChiNhanHang", b =>
                {
                    b.Navigation("HoaDons");
                });

            modelBuilder.Entity("AppData.Models.GiamGia", b =>
                {
                    b.Navigation("SanPhamGiamGias");
                });

            modelBuilder.Entity("AppData.Models.GioHang", b =>
                {
                    b.Navigation("GioHangChiTiets");
                });

            modelBuilder.Entity("AppData.Models.HinhThucTT", b =>
                {
                    b.Navigation("HoaDons");
                });

            modelBuilder.Entity("AppData.Models.MauSac", b =>
                {
                    b.Navigation("SanPhamChiTiets");
                });

            modelBuilder.Entity("AppData.Models.SanPham", b =>
                {
                    b.Navigation("AnhSanPhams");

                    b.Navigation("SanPhamChiTiets");
                });

            modelBuilder.Entity("AppData.Models.SanPhamCT", b =>
                {
                    b.Navigation("GioHangChiTiets");

                    b.Navigation("HoaDonChiTiets");

                    b.Navigation("SanPhamGiamGias");
                });

            modelBuilder.Entity("AppData.Models.Size", b =>
                {
                    b.Navigation("SanPhamChiTiets");
                });

            modelBuilder.Entity("HoaDon", b =>
                {
                    b.Navigation("HoaDonChiTiets");
                });
#pragma warning restore 612, 618
        }
    }
}
