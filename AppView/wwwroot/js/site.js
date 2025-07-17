document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById("registerForm");
    if (registerForm) {
        registerForm.addEventListener("submit", submitRegistration);
    }
    const rootStyles = getComputedStyle(document.documentElement);
    const white = rootStyles.getPropertyValue('--white-color') || '#ffffff';
    const light = rootStyles.getPropertyValue('--light-gray') || '#f8f9fa';

    const navbar = document.querySelector('.custom-navbar');
    if (navbar) {
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
    const currentPath = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === currentPath ||
            (currentPath === '/' && link.getAttribute('href') === '/')) {
            link.classList.add('active');
        }
    });
});

// === Modal functions ===
function showLoginModal() {
    const modal = new bootstrap.Modal('#loginModal', {
        backdrop: true,
        keyboard: true
    });
    modal.show();

    // Focus vào field đầu tiên
    setTimeout(() => {
        document.querySelector('#loginModal input[type="email"]')?.focus();
    }, 500);
}
function showRegisterModal() {
    const modal = new bootstrap.Modal('#registerModal', {
        backdrop: true, // Cho phép click bên ngoài để đóng
        keyboard: true  // Cho phép nhấn ESC để đóng
    });
    modal.show();

    // Focus vào field đầu tiên
    setTimeout(() => {
        document.querySelector('#registerForm input[name="FullName"]')?.focus();
    }, 500);
}
async function submitLogin(event) {
    event.preventDefault();

    const form = document.getElementById('loginForm');
    if (!form) return;

    const formData = new FormData(form);
    const loginBtn = form.querySelector('#loginBtn');
    const originalBtnContent = loginBtn.innerHTML;

    try {
        // Hiển thị loading
        loginBtn.disabled = true;
        loginBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang xử lý...';

        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]')?.value
            }
        });

        const result = await response.json();

        if (result.success) {
            // Hiển thị thông báo đăng nhập thành công
            await Swal.fire({
                icon: 'success',
                title: 'Đăng nhập thành công',
                html: `
                    <p>Xin chào <strong>${result.email}</strong>!</p>
                    <p>Bạn đã đăng nhập với vai trò <strong>${result.roles.join(', ')}</strong></p>
                `,
                showConfirmButton: true,
                confirmButtonText: 'Tiếp tục',
                showCancelButton: true,
                cancelButtonText: 'Đăng xuất',
                timer: 3000
            }).then((result) => {
                if (result.isDismissed) {
                    if (result.dismiss === Swal.DismissReason.cancel) {
                        // Người dùng click Đăng xuất
                        window.location.href = '/Auth/Auth/Logout';
                    }
                } else {
                    // Chuyển hướng sau khi đăng nhập
                    window.location.href = result.redirectUrl || '/';
                }
            });
            updateUserHeader(result.userName || result.email);

            // Ẩn modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
            if (modal) modal.hide();
        } else {
            // Hiển thị thông báo lỗi
            if (result.requiresConfirmation) {
                showEmailConfirmationAlert(result.email);
            }

            await Swal.fire({
                icon: 'error',
                title: 'Lỗi đăng nhập',
                html: result.message || 'Đăng nhập thất bại',
                confirmButtonText: 'OK'
            });
        }
    } catch (error) {
        console.error('Login error:', error);
        await Swal.fire({
            icon: 'error',
            title: 'Lỗi hệ thống',
            text: 'Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.',
            confirmButtonText: 'OK'
        });
    } finally {
        // Khôi phục trạng thái nút
        loginBtn.disabled = false;
        loginBtn.innerHTML = originalBtnContent;
    }
}
function updateUserHeader(email) {
    const userHeader = document.getElementById("userHeader");
    if (!userHeader) return;

    userHeader.innerHTML = `
    <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button"
           data-bs-toggle="dropdown" aria-expanded="false">
            <i class="bi bi-person-circle"></i> ${email}
        </a>
        <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="navbarDropdown">
            <li><a class="dropdown-item" href="/Customer/Profile">Tài khoản của tôi</a></li>
            <li><hr class="dropdown-divider" /></li>
            <li>
                <form action="/Auth/Auth/Logout" method="post" class="d-inline">
                    <input name="__RequestVerificationToken" type="hidden" value="${getCsrfToken()}" />
                    <button class="dropdown-item" type="submit">Đăng xuất</button>
                </form>
            </li>
        </ul>
    </li>
    `;
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
                'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
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
async function resendConfirmationEmail(email) {
    try {
        // Hiển thị loading khi gửi lại
        const resendBtn = document.querySelector('#resultContainer button');
        if (!resendBtn) return;

        const originalHtml = resendBtn.innerHTML;

        resendBtn.disabled = true;
        resendBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang gửi...';

        const response = await fetch('/Auth/Auth/ResendConfirmationEmail', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(email)
        });

        const result = await response.json();

        // Khôi phục trạng thái nút
        resendBtn.disabled = false;
        resendBtn.innerHTML = originalHtml;

        if (response.ok && result.success) {
            Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: result.message || 'Đã gửi lại email xác nhận thành công! Vui lòng kiểm tra hộp thư đến.',
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
            text: 'Đã xảy ra lỗi khi gửi yêu cầu. Vui lòng thử lại sau.',
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
function showSuccessAlert(message) {
    Swal.fire({
        icon: 'success',
        title: 'Thành công',
        text: message,
        confirmButtonText: 'OK'
    });
}
function showErrorAlert(message) {
    Swal.fire({
        icon: 'error',
        title: 'Lỗi',
        text: message,
        confirmButtonText: 'OK'
    });
}
