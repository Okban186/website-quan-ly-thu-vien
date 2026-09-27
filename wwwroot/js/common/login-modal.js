document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("login-modal");
    const closeBtn = document.getElementById("btn-close-modal");
    const roleTabs = document.querySelectorAll(".role-tab");
    const roleHint = document.getElementById("role-hint-text");
    const roleInput = document.getElementById("login-role-input");

    const passwordInput = document.getElementById("login-password");
    const togglePasswordBtn = document.getElementById("btn-toggle-password");
    const iconEye = document.getElementById("icon-eye");
    const iconEyeOff = document.getElementById("icon-eye-off");

    const quickFillBtn = document.getElementById("btn-quick-fill");
    const loginForm = document.getElementById("login-form");

    // Map mô tả theo vai trò
    const roleHints = {
        reader: "Đăng nhập với vai trò Bạn đọc để mượn sách và quản lý tài khoản",
        librarian: "Đăng nhập hệ thống Quản lý và xử lý mượn trả của Thủ thư",
        cataloger: "Đăng nhập phân hệ Biên mục và Quản trị danh mục tài liệu"
    };

    // ---  MỞ / ĐÓNG MODAL ---
    window.openLoginModal = () => {
        modal.classList.remove("hidden");
        modal.classList.add("flex");
    };

    window.closeLoginModal = () => {
        modal.classList.add("hidden");
        modal.classList.remove("flex");
    };

    closeBtn?.addEventListener("click", window.closeLoginModal);

    // Click ra ngoài phông đen để đóng
    modal?.addEventListener("click", (e) => {
        if (e.target === modal) window.closeLoginModal();
    });



    // ---  ẨN / HIỆN MẬT KHẨU ---
    togglePasswordBtn?.addEventListener("click", () => {
        const isPassword = passwordInput.type === "password";
        passwordInput.type = isPassword ? "text" : "password";
        iconEye.classList.toggle("hidden", isPassword);
        iconEyeOff.classList.toggle("hidden", !isPassword);
    });



    //login
    loginForm.addEventListener('submit', async (event) => {

        event.preventDefault();

        const identifier = document.getElementById('login-username').value.trim();

        const password = document.getElementById('login-password').value;

        const rememberMe = document.getElementById('login-remember').checked;

        const antiForgeryToken = getAntiForgeryToken();

        try {
            const response = await fetch('/api/account/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'RequestVerificationToken': antiForgeryToken
                },
                body: JSON.stringify({
                    identifier,
                    password,
                    rememberMe
                })
            });

            const result = await response.json();

            if (!response.ok) {
                // Hiển thị lỗi login trong modal
                showToast(result.detail)
                return;
            }



            if (result.success) {
                showToast("Đăng nhập thành công!", "success");
                setTimeout(() => { window.location.reload(); }, 1000);
                return;
            }
        }
        catch (error) {

        }
    });

});