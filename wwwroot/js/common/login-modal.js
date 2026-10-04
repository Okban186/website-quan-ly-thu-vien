document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("login-modal");
    const closeBtn = document.getElementById("btn-close-modal");
    const passwordInput = document.getElementById("login-password");
    const togglePasswordBtn = document.getElementById("btn-toggle-password");
    const iconEye = document.getElementById("icon-eye");
    const iconEyeOff = document.getElementById("icon-eye-off");
    const loginForm = document.getElementById("login-form");



    window.openLoginModal = () => {
        modal.classList.remove("hidden");
        modal.classList.add("flex");
    };

    window.closeLoginModal = () => {
        modal.classList.add("hidden");
        modal.classList.remove("flex");
    };

    closeBtn?.addEventListener("click", window.closeLoginModal);

    modal?.addEventListener("click", (e) => {
        if (e.target === modal) {
            window.closeLoginModal();
        }
    });

    togglePasswordBtn?.addEventListener("click", () => {
        const isPassword = passwordInput.type === "password";

        passwordInput.type = isPassword ? "text" : "password";

        iconEye.classList.toggle("hidden", isPassword);
        iconEyeOff.classList.toggle("hidden", !isPassword);
    });

    loginForm?.addEventListener("submit", async (event) => {
        event.preventDefault();

        const identifier = document
            .getElementById("login-username")
            .value
            .trim();

        const password = document.getElementById("login-password").value;

        const rememberMe = document
            .getElementById("login-remember")
            .checked;

        const antiForgeryToken = getAntiForgeryToken();

        try {
            const response = await apiClient.public.post(
                API.public.auth.login,
                {
                    identifier,
                    password,
                    rememberMe
                },
                {
                    headers: {
                        "Accept": "application/json",
                        "RequestVerificationToken": antiForgeryToken
                    }
                }
            );

            const result = await response.json();

            if (!response.ok) {
                showToast(result.detail);
                return;
            }

            if (result.success) {
                showToast("Đăng nhập thành công!", "success");

                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            }
        } catch (error) {
            console.error(error);
        }
    });
});