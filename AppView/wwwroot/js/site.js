document.addEventListener('DOMContentLoaded', function () {
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

    // Highlight active nav-link
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
    const modalEl = document.getElementById('loginModal');
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
}

function showRegisterModal() {
    const modalEl = document.getElementById('registerModal');
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
}

async function submitLogin(event) {
    event.preventDefault();

    const form = document.getElementById('loginForm');
    if (!form) return;

    const formData = new FormData(form);

    const response = await fetch(form.action, {
        method: 'POST',
        body: formData
    });

    if (response.redirected) {
        // ✅ Đăng nhập thành công, chuyển trang
        window.location.href = response.url;
    } else {
        // ❌ Có lỗi, load lại nội dung modal
        const html = await response.text();
        document.getElementById('loginModalBody').innerHTML = html;
    }
}



function submitRegistration(event) {
    event.preventDefault(); // Ngăn reload

    const form = document.getElementById('registerForm');
    if (!form) return;

    if (form.checkValidity()) {
        fetch('/Account/Register', {
            method: 'POST',
            body: new FormData(form)
        })
            .then(response => {
                if (response.ok) {
                    bootstrap.Modal.getInstance(document.getElementById('registerModal'))?.hide();
                    setTimeout(() => {
                        const successModal = document.getElementById('successModal');
                        if (successModal) {
                            new bootstrap.Modal(successModal).show();
                        }
                    }, 300);
                    form.reset();
                } else {
                    return response.text();
                }
            })
            .then(message => {
                if (message) {
                    alert('Đăng ký thất bại: ' + message);
                }
            })
            .catch(err => {
                console.error('Lỗi đăng ký:', err);
                alert('Có lỗi xảy ra khi đăng ký.');
            });
    } else {
        form.reportValidity();
    }
}
