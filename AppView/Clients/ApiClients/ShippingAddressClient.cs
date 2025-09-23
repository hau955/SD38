using System.Net.Http.Json;
using System.Text.Json;

namespace AppView.Clients.ApiClients
{
	public interface IShippingAddressClient
	{
		Task<List<ShippingAddressListVm>> GetMyAddressesAsync();
		Task<ShippingAddressVm?> GetAsync(Guid id);
		Task<ApiCallResult<ShippingAddressVm>> CreateAsync(ShippingAddressCreateVm dto);
		Task<ShippingAddressVm?> UpdateAsync(Guid id, ShippingAddressCreateVm dto);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> SetDefaultAsync(Guid id);
		Task<ShippingAddressVm?> GetDefaultAsync();
	}

	public class ShippingAddressClient : IShippingAddressClient
	{
		private readonly HttpClient _http;
		private readonly JsonSerializerOptions _options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};

		public ShippingAddressClient(HttpClient http)
		{
			_http = http;
			// Forward user id via header if present in DOM will be set per request on view side
		}

		public async Task<List<ShippingAddressListVm>> GetMyAddressesAsync()
		{
			var res = await _http.GetAsync("api/ShippingAddress");
			if (!res.IsSuccessStatusCode) return new List<ShippingAddressListVm>();
			var json = await res.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<ShippingAddressListVm>>(json, _options) ?? new List<ShippingAddressListVm>();
		}

		public async Task<ShippingAddressVm?> GetAsync(Guid id)
		{
			var res = await _http.GetAsync($"api/ShippingAddress/{id}");
			if (!res.IsSuccessStatusCode) return null;
			return await res.Content.ReadFromJsonAsync<ShippingAddressVm>(_options);
		}

		public async Task<ApiCallResult<ShippingAddressVm>> CreateAsync(ShippingAddressCreateVm dto)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, "api/ShippingAddress")
			{
				Content = JsonContent.Create(dto)
			};
			// Optionally forward X-User-Id from environment variable or similar in future
			var res = await _http.SendAsync(req);
			if (!res.IsSuccessStatusCode)
			{
				var error = await res.Content.ReadAsStringAsync();
				return new ApiCallResult<ShippingAddressVm>
				{
					Success = false,
					StatusCode = (int)res.StatusCode,
					Error = error
				};
			}
			var data = await res.Content.ReadFromJsonAsync<ShippingAddressVm>(_options);
			return new ApiCallResult<ShippingAddressVm>
			{
				Success = true,
				StatusCode = (int)res.StatusCode,
				Data = data
			};
		}

		public async Task<ShippingAddressVm?> UpdateAsync(Guid id, ShippingAddressCreateVm dto)
		{
			var res = await _http.PutAsJsonAsync($"api/ShippingAddress/{id}", dto);
			if (!res.IsSuccessStatusCode) return null;
			return await res.Content.ReadFromJsonAsync<ShippingAddressVm>(_options);
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var res = await _http.DeleteAsync($"api/ShippingAddress/{id}");
			return res.IsSuccessStatusCode;
		}

		public async Task<bool> SetDefaultAsync(Guid id)
		{
			var res = await _http.PutAsync($"api/ShippingAddress/{id}/set-default", null);
			return res.IsSuccessStatusCode;
		}

		public async Task<ShippingAddressVm?> GetDefaultAsync()
		{
			var res = await _http.GetAsync("api/ShippingAddress/default");
			if (!res.IsSuccessStatusCode) return null;
			return await res.Content.ReadFromJsonAsync<ShippingAddressVm>(_options);
		}
	}

	public class ShippingAddressListVm
	{
		public Guid Id { get; set; }
		public string DiaChiChiTiet { get; set; } = string.Empty;
		public string HoTenNguoiNhan { get; set; } = string.Empty;
		public string SoDienThoai { get; set; } = string.Empty;
		public bool IsDefault { get; set; }
		public bool TrangThai { get; set; }
		public decimal? PhiVanChuyen { get; set; }
		public int? ThoiGianGiaoHang { get; set; }
	}

	public class ShippingAddressVm
	{
		public Guid Id { get; set; }
		public string DiaChiChiTiet { get; set; } = string.Empty;
		public string SoDienThoai { get; set; } = string.Empty;
		public string HoTenNguoiNhan { get; set; } = string.Empty;
		public DateTime? NgayTao { get; set; }
		public bool TrangThai { get; set; }
		public bool IsDefault { get; set; }
		public Guid IDUser { get; set; }
		public string SoNha { get; set; } = string.Empty;
		public string PhuongXa { get; set; } = string.Empty;
		public string QuanHuyen { get; set; } = string.Empty;
		public string TinhThanh { get; set; } = string.Empty;
		public string? DiaDiemGan { get; set; }
	}

	public class ShippingAddressCreateVm
	{
		public string SoNha { get; set; } = string.Empty;
		public string PhuongXa { get; set; } = string.Empty;
		public string QuanHuyen { get; set; } = string.Empty;
		public string TinhThanh { get; set; } = string.Empty;
		public string? DiaDiemGan { get; set; }
		public string SoDienThoai { get; set; } = string.Empty;
		public string HoTenNguoiNhan { get; set; } = string.Empty;
		public Guid IDUser { get; set; }
	}

	public class ApiCallResult<T>
	{
		public bool Success { get; set; }
		public int StatusCode { get; set; }
		public T? Data { get; set; }
		public string? Error { get; set; }
	}
}

