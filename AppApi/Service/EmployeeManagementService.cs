using AppApi.Features.Auth.DTOs;
using AppApi.Helpers;
using AppApi.IService;
using AppApi.ViewModels.EmployeeManagementDTOs;
using AppData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace AppApi.Service
{
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public EmployeeManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ApiResponse<object>> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return ApiResponse<object>.Fail("Không tìm thấy người dùng.", 404);

            var roleName = dto.Role.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
                return ApiResponse<object>.Fail($"Quyền '{roleName}' không tồn tại.", 400);

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Xóa quyền cũ thất bại: {errors}", 400);
            }

            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Gán quyền mới thất bại: {errors}", 400);
            }

            return ApiResponse<object>.Success(null, $"Đã gán quyền '{roleName}' cho người dùng.");
        }

        public async Task<ApiResponse<object>> CreateAsync(AddEmployeeDto dto)
        {
            try
            {
                var email = dto.Email.Trim().ToLower();
                var existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser != null)
                    return ApiResponse<object>.Fail("Email đã tồn tại.", 400);
                string avatarUrl = null;
                if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
                {
                    // Logic upload file ở đây
                    avatarUrl = await UploadFile(dto.AvatarFile);
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    HoTen = dto.FullName,
                    SoDienThoai = dto.PhoneNumber,
                    DiaChi = dto.Address,
                    NgaySinh = dto.DateOfBirth,
                    HinhAnh = avatarUrl,
                    GioiTinh = dto.Gender,
                    TrangThai = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return ApiResponse<object>.Fail($"Tạo tài khoản thất bại: {errors}", 400);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Employee");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return ApiResponse<object>.Fail($"Gán quyền thất bại: {errors}", 400);
                }

                return ApiResponse<object>.Success(null, "Tạo nhân viên thành công.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi hệ thống: {ex.Message}", 500);
            }
        }
        private async Task<string> UploadFile(IFormFile file)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }
        public async Task<ApiResponse<object>> ToggleActiveStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ApiResponse<object>.Fail("Không tìm thấy người dùng.", 404);

            user.TrangThai = !user.TrangThai;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Cập nhật trạng thái thất bại: {errors}", 400);
            }

            var message = user.TrangThai ? "Kích hoạt tài khoản thành công." : "Vô hiệu hóa tài khoản thành công.";
            return ApiResponse<object>.Success(null, message);
        }

        public async Task<PagedResult<EmployeeListDto>> GetAllAsync(
     int page = 1,
     int pageSize = 10,
     string? fullName = null,
     string? email = null,
     string? phoneNumber = null,
     bool? isActive = null,
     bool? gender = null)
        {
            // Join Users → UserRoles → Roles để lọc role = Employee
            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Employee"
                        select user;

            // Bộ lọc thêm
            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(u => u.HoTen.Contains(fullName));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                query = query.Where(u => u.SoDienThoai.Contains(phoneNumber));
            if (isActive.HasValue)
                query = query.Where(u => u.TrangThai == isActive.Value);
            if (gender.HasValue)
                query = query.Where(u => u.GioiTinh == gender.Value);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.HoTen)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = new List<EmployeeListDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new EmployeeListDto
                {
                    Id = user.Id.ToString(),
                    FullName = user.HoTen,
                    Email = user.Email,
                    PhoneNumber = user.SoDienThoai,
                    Gender = user.GioiTinh,
                    Address = user.DiaChi,
                    DateOfBirth = user.NgaySinh,
                    AvatarUrl = user.HinhAnh,
                    Role = roles.FirstOrDefault() ?? "Chưa phân quyền",
                    IsActive = user.TrangThai
                });
            }

            return new PagedResult<EmployeeListDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = items
            };
        }

        public async Task<EmployeeDetailDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new EmployeeDetailDto
            {
                Id = user.Id.ToString(),
                FullName = user.HoTen,
                Email = user.Email,
                PhoneNumber = user.SoDienThoai,
                Address = user.DiaChi,
                DateOfBirth = user.NgaySinh,
                Gender = user.GioiTinh,
                AvatarUrl = user.HinhAnh,
                Role = roles.FirstOrDefault() ?? "Chưa có",
                IsActive = user.TrangThai
            };
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(Features.Auth.DTOs.ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ApiResponse<object>.Fail("Không tìm thấy người dùng.", 404);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Đặt lại mật khẩu thất bại: {errors}", 400);
            }

            return ApiResponse<object>.Success(null, "Đặt lại mật khẩu thành công.");
        }

        public async Task<ApiResponse<object>> UpdateAsync(string id, UpdateEmployeeDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ApiResponse<object>.Fail("Không tìm thấy người dùng.", 404);

            user.HoTen = dto.FullName;
            user.SoDienThoai = dto.PhoneNumber;
            user.DiaChi = dto.Address;
            user.NgaySinh = dto.DateOfBirth;
            user.GioiTinh = dto.Gender;
            user.HinhAnh = dto.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Cập nhật thất bại: {errors}", 400);
            }

            return ApiResponse<object>.Success(null, "Cập nhật thông tin thành công.");
        }
    }
}
