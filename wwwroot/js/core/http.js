let refreshPromise = null;

async function refreshAccessToken() {
    // Nếu đang có một tiến trình refresh chạy rồi, trả về chính tiến trình đó
    if (refreshPromise) {
        return refreshPromise;
    }

    // Tạo promise để gọi API refresh token
    refreshPromise = (async () => {
        try {
            const response = await fetch('/api/accounts/refresh', {
                method: 'POST',
                headers: {
                    ...getAntiForgeryHeaders()
                }
            });

            if (!response.ok) {
                throw new Error('Refresh token expired');
            }


            return true; // Trả về true nếu refresh thành công
        } catch (error) {
            return false; // Trả về false nếu thất bại
        } finally {
            refreshPromise = null; // Xóa khóa để các lần sau có thể refresh tiếp
        }
    })();

    return refreshPromise;
}

async function fetchWithAuth(url, options = {}) {

    let response = await fetch(url, options);

    if (response.status !== 401) {
        return response;
    }

    //nếu bị 401, tiến hành làm mới token
    const isRefreshed = await refreshAccessToken();

    //làm mới thất bại đưa người dùng về trang chủ
    if (!isRefreshed) {
        window.location.href = '/';
        return response;
    }
    return fetch(url, options);
}
