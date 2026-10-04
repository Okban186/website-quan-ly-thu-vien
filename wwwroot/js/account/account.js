document.addEventListener("DOMContentLoaded", () => {
    const logoutButton = document.getElementById("menu-item-logout");

    logoutButton?.addEventListener("click", async () => {
        const antiForgeryToken = getAntiForgeryToken();

        try {
            const response = await apiClient.private.post(
                API.private.auth.logout,
                null,
                {
                    headers: {
                        'Accept': 'application/json',
                        'RequestVerificationToken': antiForgeryToken
                    }
                }
            );

            if (!response.ok) {
                throw new Error("Logout failed.");
            }

            window.location.href = "/";
        } catch (error) {
            console.error(error);
        }
    });
});