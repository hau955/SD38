﻿using AppView.Areas.Admin.IRepo;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AppData.Models;

namespace AppView.Areas.Admin.Repository
{
    public class SanPhamCTRepo : ISanPhamCTRepo
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SanPhamCTRepo(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateMultipleAsync(List<SanPhamCT> list)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7221/api/SanPhamCT/create-multiple", list);
            return response.IsSuccessStatusCode;
        }

       
        public async Task<List<MauSac>> GetMauSacsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.GetAsync("https://localhost:7221/api/MauSac");

            if (!res.IsSuccessStatusCode)
                return new();

            var response = await res.Content.ReadFromJsonAsync<ApiResponse<List<MauSac>>>();
            return response?.Data ?? new();
        }

        public async Task<List<Size>> GetSizesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.GetAsync("https://localhost:7221/api/Size");

            if (!res.IsSuccessStatusCode)
                return new();

            var response = await res.Content.ReadFromJsonAsync<ApiResponse<List<Size>>>();
            return response?.Data ?? new();
        }

        public async Task<List<ChatLieu>> GetChatLieuAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.GetAsync("https://localhost:7221/api/ChatLieux");

            if (!res.IsSuccessStatusCode)
                return new();

            var response = await res.Content.ReadFromJsonAsync<ApiResponse<List<ChatLieu>>>();
            return response?.Data ?? new();
        }

        public async Task<List<SanPhamCT>> GetBySanPhamIdAsync(Guid idSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.GetAsync($"https://localhost:7221/api/SanPhamCT/by-sanpham/{idSanPham}");

            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<List<SanPhamCT>>() ?? new()
                : new();
        }
        public async Task<SanPhamCT?> GetByIdAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.GetAsync($"https://localhost:7221/api/SanPhamCT/{id}");
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<SanPhamCT>()
                : null;
        }

        public async Task<bool> UpdateAsync(SanPhamCT model)
        {
            var client = _httpClientFactory.CreateClient();
            var res = await client.PutAsJsonAsync("https://localhost:7221/api/SanPhamCT/update", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ExistsAsync(Guid idSanPham, Guid idMauSac, Guid idSize, Guid idchatlieu)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7221/api/SanPhamCT/exists?idSanPham={idSanPham}&idMau={idMauSac}&idSize={idSize}&idchatlieu={idchatlieu}";

            var res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return false;

            var result = await res.Content.ReadFromJsonAsync<bool>();
            return result;
        }
    }
}