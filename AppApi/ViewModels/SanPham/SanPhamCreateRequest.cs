using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppApi.ViewModels.SanPham
{
    public class SanPhamCreateRequest
    {
        public Guid? IDSanPham { get; set; }
        public Guid DanhMucID { get; set; } // Khóa ngoại đến DanhMuc
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
        public double TrongLuong { get; set; }
        public bool GioiTinh { get; set; } 
        public bool TrangThai { get; set; } = true; // Mặc định là true (còn hàng)
        public DateTime NgayTao { get; set; } 
        public DateTime NgaySua { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }  // Danh sách ảnh


    }
}
