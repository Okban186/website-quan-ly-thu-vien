document.addEventListener("DOMContentLoaded", () => {
    const logoutButton = document.getElementById("menu-item-logout");

    logoutButton?.addEventListener("click", async () => {

        const antiForgeryToken = getAntiForgeryToken();
        try {
            const response = await fetch("/api/account/logout", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'RequestVerificationToken': antiForgeryToken
                },
            });

            if (!response.ok) {
                throw new Error("Logout failed.");
            }

            window.location.href = "/";
        }
        catch (error) {
            console.error(error);
        }
    });
});



let refreshPromise = null;

/// Sử dụng promise để tránh gọi nhiều lần refresh token ko cần thiết
///Giả sử gửi 3 request nhưng đúng lúc hết hạn nếu ko dùng promise để bắt 2 request kia chờ gửi cookies mới thì nó sẽ tạo 3 cookies mới

async function refreshAccessToken() {
    // Đã có request refresh đang chạy
    if (refreshPromise) {
        return refreshPromise;
    }

    refreshPromise = fetch('/Account/Refresh', {
        method: 'POST',
        headers: {
            ...getAntiForgeryHeaders()
        }
    }).finally(() => {
        refreshPromise = null;
    });

    return refreshPromise;
}

async function fetchWithAuth(url, options = {}) {
    let response = await fetch(url, options);

    // Access token vẫn còn hạn
    if (response.status !== 401) {
        return response;
    }

    // Access token hết hạn
    const refreshResponse = await refreshAccessToken();

    // Refresh token hết hạn / không hợp lệ
    if (!refreshResponse.ok) {
        window.location.href = '/';
        return response;
    }

    // Cookie access_token đã được cập nhật
    return await fetch(url, options);
}



