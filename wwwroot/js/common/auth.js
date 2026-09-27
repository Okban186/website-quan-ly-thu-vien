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


