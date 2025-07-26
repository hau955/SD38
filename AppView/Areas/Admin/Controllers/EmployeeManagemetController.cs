using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.EmployeeManagerment;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeManagementController : Controller
    {
        private readonly IEmployeeManagementRepo _employeeRepo;

        public EmployeeManagementController(IEmployeeManagementRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(
    int page = 1,
    int pageSize = 10,
    string? fullName = null,
    string? email = null,
    string? phoneNumber = null,
    bool? isActive = null,
    bool? gender = null)
        {
            // Debug: Log các tham số
            Console.WriteLine($"Calling API with page={page}, pageSize={pageSize}, fullName={fullName}, email={email}");

            var result = await _employeeRepo.GetAllAsync(page, pageSize, fullName, email, phoneNumber, isActive, gender);

            // Debug: Kiểm tra dữ liệu nhận được
            Console.WriteLine($"Received {result?.Items?.Count ?? 0} items from repository");
            if (result?.Items != null)
            {
                foreach (var item in result.Items)
                {
                    Console.WriteLine($"Employee: {item.FullName}, Email: {item.Email}");
                }
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result?.TotalPages ?? 1;
            ViewBag.TotalCount = result?.TotalCount ?? 0;
            ViewBag.PageSize = pageSize;

            ViewBag.FullName = fullName;
            ViewBag.Email = email;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.IsActive = isActive;
            ViewBag.Gender = gender;

            return View(result?.Items ?? new List<EmployeeListViewModel>());
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _employeeRepo.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập";
                return View(model);
            }
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                // Xử lý upload file ở đây
                var filePath = Path.Combine("wwwroot/uploads", model.AvatarFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }
            }
            var result = await _employeeRepo.CreateAsync(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Tạo nhân viên thành công";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Tạo nhân viên thất bại. Email có thể đã tồn tại";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _employeeRepo.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên";
                return RedirectToAction("Index");
            }

            var editModel = new UpdateEmployeeViewModel
            {
                Id = id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl
            };

            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UpdateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập";
                return View(model);
            }

            var result = await _employeeRepo.UpdateAsync(id, model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Details", new { id });
            }

            TempData["ErrorMessage"] = "Cập nhật thông tin thất bại";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _employeeRepo.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên";
                return RedirectToAction("Index");
            }

            var availableRoles = new List<string> { "Admin", "Customer", "Employee"};

            var model = new AssignRoleViewModel
            {
                UserId = id,
                Role = user.Role,
                AvailableRoles = availableRoles
            };

            return View(model); // 🚨 thiếu dòng này là view không hiển thị
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn vai trò";
                return RedirectToAction("Details", new { id = userId });
            }

            var result = await _employeeRepo.AssignRoleAsync(userId, role);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Gán quyền {role} thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Gán quyền thất bại";
            }

            return RedirectToAction("Details", new { id = userId });
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _employeeRepo.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên";
                return RedirectToAction("Index");
            }

            var model = new ResetPasswordViewmodelEm
            {
                UserId = id,
                ConfirmPassword = string.Empty,
                NewPassword = string.Empty
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewmodelEm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin mật khẩu";
                return RedirectToAction("Details", new { id = model.UserId });
            }

            var result = await _employeeRepo.ResetPasswordAsync(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Đặt lại mật khẩu thất bại";
            }

            return RedirectToAction("Details", new { id = model.UserId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var result = await _employeeRepo.ToggleStatusAsync(id);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
    }
}