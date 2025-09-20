// === Các hàm chung ===
function getCsrfToken() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : '';
}

// === Modal functions ===
function showLoginModal() {
    const modal = new bootstrap.Modal('#loginModal', {
        backdrop: true,
        keyboard: true
    });
    modal.show();

    setTimeout(() => {
        document.querySelector('#loginModal input[type="email"]')?.focus();
    }, 500);
}

function showRegisterModal() {
    const modal = new bootstrap.Modal('#registerModal', {
        backdrop: true,
        keyboard: true
    });
    modal.show();

    setTimeout(() => {
        document.querySelector('#registerForm input[name="FullName"]')?.focus();
    }, 500);
}
async function submitLogin(event) {
    event.preventDefault();
    const form = event.target;
    const loginBtn = form.querySelector('#loginBtn');
    const originalBtnContent = loginBtn.innerHTML;

    try {
        loginBtn.disabled = true;
        loginBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang xử lý...';

        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': getCsrfToken(),
                'Accept': 'application/json'
            }
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Đăng nhập thất bại');
        }

        const result = await response.json();

        // Kiểm tra cả 2 trường hợp (hoa/thường) cho chắc chắn
        const isSuccess = result.isSuccess || result.IsSuccess;

        if (isSuccess) {
            await Swal.fire({
                icon: 'success',
                title: 'Đăng nhập thành công!',
                text: 'Chào mừng bạn trở lại!',
                timer: 1500,
                showConfirmButton: false
            });

            // Cập nhật header nếu có email
            if (result.email) {
                updateUserHeader(result.email, result.isAdmin || result.IsAdmin);
            }

            // Đóng modal đăng nhập
            const modal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
            if (modal) modal.hide();

            // Redirect sau 1.5 giây
            setTimeout(() => {
                window.location.href = result.redirectUrl ||
                    (result.isAdmin ? "/Admin/SanPham" :
                        result.IsAdmin ? "/Admin/SanPham" : "/");
            }, 1500);
        } else {
            // Xử lý trường hợp cần xác nhận email
            if (result.requiresConfirmation) {
                showEmailConfirmationAlert(result.email);
            }

            // Hiển thị thông báo lỗi
            throw new Error(result.message || 'Email hoặc mật khẩu không đúng');
        }
    } catch (error) {
        console.error('Login error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Đăng nhập thất bại',
            text: error.message || 'Có lỗi xảy ra khi đăng nhập',
            confirmButtonText: 'Thử lại'
        });
    } finally {
        loginBtn.disabled = false;
        loginBtn.innerHTML = originalBtnContent;
    }
}
let isRegistering = false;

async function submitRegistration(event) {
    event.preventDefault();
    if (isRegistering) return;
    isRegistering = true;

    const form = event.target;
    const submitButton = form.querySelector('button[type="submit"]');
    const originalText = submitButton.innerHTML;

    try {
        submitButton.disabled = true;
        submitButton.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang xử lý...';

        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': getCsrfToken()
            }
        });
        const result = await response.json();

        if (!response.ok || result.isSuccess === false || result.success === false) {
            throw new Error(result.message || "Đăng ký thất bại");
        }

        await Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: result.message || 'Đăng ký thành công!',
            timer: 3000,
            showConfirmButton: true
        });

        const modal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
        if (modal) modal.hide();
        form.reset();
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: error.message,
            timer: 3000,
            showConfirmButton: true
        });
    } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalText;
        isRegistering = false;
    }
}
function initDropdowns() {
    // Chỉ cần khởi tạo dropdown Bootstrap, không cần thêm sự kiện click thủ công
    const dropdownElements = document.querySelectorAll('.dropdown-toggle');
    dropdownElements.forEach(toggleEl => {
        // Khởi tạo dropdown Bootstrap
        new bootstrap.Dropdown(toggleEl, {
            autoClose: true
        });
    });

    // Thêm sự kiện cho form logout
    const logoutForms = document.querySelectorAll('form[action*="Logout"]');
    logoutForms.forEach(form => {
        form.addEventListener('submit', handleLogout);
    });
}
function updateUserHeader(email, isAdmin = false) {
    const userHeader = document.getElementById("userHeader");
    if (!userHeader) return;

    userHeader.innerHTML = `
        <!-- Giữ nguyên nội dung HTML như hiện tại -->
    `;

    // Khởi tạo lại dropdown sau khi cập nhật
    initDropdowns();

    // Thêm sự kiện click cho các dropdown mới
    document.querySelectorAll('[data-bs-toggle="dropdown"]').forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const dropdown = bootstrap.Dropdown.getInstance(toggle);
            dropdown.toggle();
        });
    });
    const logoutForm = document.querySelector('form[action*="Logout"]');
    if (logoutForm) {
        logoutForm.addEventListener('submit', handleLogout);
    }
}
document.addEventListener('DOMContentLoaded', function () {
    initDropdowns();
    // Khởi tạo form
    const registerForm = document.getElementById("registerForm");
    if (registerForm) {
        registerForm.addEventListener("submit", submitRegistration);
    }

    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", submitLogin);
    }

    // Navbar scroll effect
    const navbar = document.querySelector('.custom-navbar');
    if (navbar) {
        const rootStyles = getComputedStyle(document.documentElement);
        const white = rootStyles.getPropertyValue('--white-color') || '#ffffff';
        const light = rootStyles.getPropertyValue('--light-gray') || '#f8f9fa';

        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                navbar.style.background = 'rgba(255, 255, 255, 0.95)';
                navbar.style.backdropFilter = 'blur(10px)';
            } else {
                navbar.style.background = `linear-gradient(135deg, ${white} 0%, ${light} 100%)`;
                navbar.style.backdropFilter = 'none';
            }
        });
    }

    // Active nav link
    const currentPath = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === currentPath ||
            (currentPath === '/' && link.getAttribute('href') === '/')) {
            link.classList.add('active');
        }
    });
});

// === Các hàm hỗ trợ khác ===
async function resendConfirmationEmail(email) {
    try {
        const resendBtn = document.querySelector('#resultContainer button');
        if (!resendBtn) return;
        const originalHtml = resendBtn.innerHTML;

        resendBtn.disabled = true;
        resendBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang gửi...';

        const response = await fetch('/Auth/Auth/ResendConfirmationEmail', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCsrfToken()
            },
            body: JSON.stringify(email)
        });

        const result = await response.json();
        resendBtn.disabled = false;
        resendBtn.innerHTML = originalHtml;

        if (response.ok && result.success) {
            Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: result.message || 'Đã gửi lại email xác nhận thành công!',
                confirmButtonText: 'OK'
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: result.message || 'Không thể gửi lại email xác nhận',
                confirmButtonText: 'OK'
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Đã xảy ra lỗi khi gửi yêu cầu.',
            confirmButtonText: 'OK'
        });
    }
}

function showSuccessMessage(message) {
    const resultContainer = document.getElementById('resultContainer');
    if (resultContainer) {
        resultContainer.innerHTML = `
            <div class="text-center mb-4">
                <i class="fas fa-check-circle text-success" style="font-size: 5rem;"></i>
            </div>
            <div class="alert alert-success">
                ${message}
            </div>
            <div class="d-grid gap-2">
                <a href="/Auth/Login" class="btn btn-primary btn-lg">
                    <i class="fas fa-sign-in-alt me-2"></i> Đăng nhập ngay
                </a>
            </div>`;
        resultContainer.style.display = 'block';
    }
}

function showErrorMessage(message, email) {
    const resultContainer = document.getElementById('resultContainer');
    if (resultContainer) {
        resultContainer.innerHTML = `
            <div class="text-center mb-4">
                <i class="fas fa-times-circle text-danger" style="font-size: 5rem;"></i>
            </div>
            <div class="alert alert-danger">
                ${message}
            </div>
            <div class="d-grid gap-2">
                <button onclick="resendConfirmationEmail('${email}')"
                        class="btn btn-warning btn-lg">
                    <i class="fas fa-paper-plane me-2"></i> Gửi lại email xác nhận
                </button>
                <a href="/lien-he" class="btn btn-outline-secondary btn-lg">
                    <i class="fas fa-headset me-2"></i> Liên hệ hỗ trợ
                </a>
            </div>`;
        resultContainer.style.display = 'block';
    }
}
async function handleLogout(event) {
    event.preventDefault();
    const result = await Swal.fire({
        title: 'Bạn có chắc muốn đăng xuất?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đăng xuất',
        cancelButtonText: 'Hủy'
    });

    if (!result.isConfirmed) return;

    const form = event.target;
    const logoutBtn = form.querySelector('button[type="submit"]');
    const originalText = logoutBtn.innerHTML;

    try {
        logoutBtn.disabled = true;
        logoutBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang xử lý...';

        const response = await fetch('/Auth/Auth/Logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCsrfToken()
            },
            body: JSON.stringify({})
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();

        if (data.success) {
            await Swal.fire({
                icon: 'success',
                title: 'Đăng xuất thành công',
                text: 'Bạn đã đăng xuất khỏi hệ thống',
                timer: 2000,
                showConfirmButton: false
            });

            // Reload trang để cập nhật giao diện
            window.location.href = data.redirectUrl || '/';
        } else {
            throw new Error(data.message || 'Đăng xuất thất bại');
        }
    } catch (error) {
        console.error('Logout failed:', error);
        Swal.fire({
            icon: 'error',
            title: 'Đăng xuất thất bại',
            text: error.message || 'Có lỗi xảy ra khi đăng xuất'
        });
    } finally {
        logoutBtn.disabled = false;
        logoutBtn.innerHTML = originalText;
    }
}
// === Notification functions ===
function showToast(type, message, position = 'top-end', timer = 3000) {
    const Toast = Swal.mixin({
        toast: true,
        position: position,
        showConfirmButton: false,
        timer: timer,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });

    Toast.fire({
        icon: type,
        title: message
    });
}

function showSuccess(message, title = 'Thành công', timer = 2000) {
    Swal.fire({
        icon: 'success',
        title: title,
        text: message,
        timer: timer,
        showConfirmButton: timer <= 0
    });
}

function showError(message, title = 'Lỗi') {
    Swal.fire({
        icon: 'error',
        title: title,
        text: message,
        confirmButtonText: 'Đóng'
    });
}

function showLoading(message = 'Đang xử lý...') {
    Swal.fire({
        title: message,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    return Swal; // Return để có thể đóng sau này
}