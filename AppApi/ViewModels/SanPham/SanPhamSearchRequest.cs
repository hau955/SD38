using System.ComponentModel.DataAnnotations;

namespace AppApi.ViewModels.SanPham
{
    public class SanPhamSearchRequest
    {
        public string? Ten { get; set; }
        public Guid? DanhMucId { get; set; }
        public bool? GioiTinh { get; set; }
        public bool? TrangThai { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal? GiaMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa phải lớn hơn hoặc bằng 0")]
        public decimal? GiaMax { get; set; }

        public decimal? GiaMinSauGiam { get; set; }
        public decimal? GiaMaxSauGiam { get; set; }

        public string? MauSac { get; set; }
        public string? Size { get; set; }
        public string? ChatLieu { get; set; }

        public string? SapXep { get; set; } // ten_az, ten_za, gia_tang, gia_giam, moi_nhat

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Validate giá min/max
        public bool IsValidPriceRange()
        {
            if (GiaMin.HasValue && GiaMax.HasValue)
                return GiaMin.Value <= GiaMax.Value;
            return true;
        }

        // Validate giá sau giảm min/max  
        public bool IsValidDiscountPriceRange()
        {
            if (GiaMinSauGiam.HasValue && GiaMaxSauGiam.HasValue)
                return GiaMinSauGiam.Value <= GiaMaxSauGiam.Value;
            return true;
        }
    }

    // Response model cho phân trang
    public class SanPhamSearchResponse
    {
        public List<SanPhamDetailWithDiscountView> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

