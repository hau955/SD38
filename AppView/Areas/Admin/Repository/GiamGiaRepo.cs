using AppData.Models;
using AppView.Areas.Admin.IRepo;

namespace AppView.Areas.Admin.Repository
{
    public class GiamGiaRepo:IGiamGiaRepo
    {
        private readonly HttpClient _http;

        public GiamGiaRepo(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<GiamGia>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<GiamGia>>("https://localhost:7221/api/GiamGia")
                ?? new List<GiamGia>();
        }

        public async Task<GiamGia?> GetByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<GiamGia>($"https://localhost:7221/api/GiamGia/{id}");
        }

        public async Task<GiamGia?> CreateAsync(GiamGia model)
        {
            var res = await _http.PostAsJsonAsync("https://localhost:7221/api/GiamGia", model);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<GiamGia>();
        }

        public async Task<bool> UpdateAsync(Guid id, GiamGia model)
        {
            var res = await _http.PutAsJsonAsync($"https://localhost:7221/api/GiamGia/{id}", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var res = await _http.DeleteAsync($"https://localhost:7221/api/GiamGia/{id}");
            return res.IsSuccessStatusCode;
        }

        public async Task<GiamGiaSanPham?> AddProductToDiscountAsync(Guid giamGiaId, Guid sanPhamId)
        {
            var res = await _http.PostAsync($"https://localhost:7221/api/GiamGia/{giamGiaId}/add-product/{sanPhamId}", null);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<GiamGiaSanPham>();
        }

        public async Task<List<GiamGiaSanPham>> GetProductsByDiscountAsync(Guid giamGiaId)
        {
            return await _http.GetFromJsonAsync<List<GiamGiaSanPham>>($"https://localhost:7221/api/GiamGia/{giamGiaId}/products")
                ?? new List<GiamGiaSanPham>();
        }

        public async Task<GiamGiaSPCT?> AddSanPhamCTToDiscountAsync(Guid giamGiaId, Guid sanPhamCTId)
        {
            var res = await _http.PostAsync($"https://localhost:7221/api/GiamGia/{giamGiaId}/add-spct/{sanPhamCTId}", null);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<GiamGiaSPCT>();
        }

        public async Task<List<GiamGiaSPCT>> GetSanPhamCTByDiscountAsync(Guid giamGiaId)
        {
            return await _http.GetFromJsonAsync<List<GiamGiaSPCT>>($"https://localhost:7221/api/GiamGia/{giamGiaId}/spcts")
                ?? new List<GiamGiaSPCT>();
        }

        public async Task<bool> AddCategoryToDiscountAsync(Guid giamGiaId, Guid danhMucId)
        {
            var res = await _http.PostAsync($"https://localhost:7221/api/GiamGia/{giamGiaId}/add-category/{danhMucId}", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<TinhGiaResult?> TinhGiaSauGiamAsync(Guid idSanPhamCT, Guid danhMucId)
        {
            var res = await _http.GetAsync($"https://localhost:7221/api/GiamGia/tinhgia/{idSanPhamCT}/{danhMucId}");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<TinhGiaResult>();
        }
    }
}

