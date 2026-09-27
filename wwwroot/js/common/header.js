document.addEventListener("DOMContentLoaded", () => {
    const notifBtn = document.getElementById("btn-notifications-toggle");
    const notifMenu = document.getElementById("notif-menu");

    const userBtn = document.getElementById("btn-user-menu-toggle");
    const userMenu = document.getElementById("user-menu");

    const mobileMenuBtn = document.getElementById("btn-mobile-menu");
    const mobileMenu = document.getElementById("mobile-menu");

    const iconMenu = document.getElementById("icon-menu");
    const iconClose = document.getElementById("icon-close");



    notifBtn?.addEventListener("click", (event) => {
        event.stopPropagation();

        notifMenu?.classList.toggle("hidden");
        userMenu?.classList.add("hidden");
    });



    userBtn?.addEventListener("click", (event) => {
        event.stopPropagation();

        userMenu?.classList.toggle("hidden");
        notifMenu?.classList.add("hidden");
    });


    mobileMenuBtn?.addEventListener("click", () => {
        mobileMenu?.classList.toggle("hidden");
        iconMenu?.classList.toggle("hidden");
        iconClose?.classList.toggle("hidden");
    });



    document.addEventListener("click", (event) => {
        const notifContainer =
            document.getElementById("notif-container");

        const userContainer =
            document.getElementById("user-container");

        if (!notifContainer?.contains(event.target)) {
            notifMenu?.classList.add("hidden");
        }

        if (!userContainer?.contains(event.target)) {
            userMenu?.classList.add("hidden");
        }
    });



    document.querySelectorAll(".nav-mobile-btn").forEach(button => {
        button.addEventListener("click", () => {
            const target = button.dataset.target;

            if (!target) {
                return;
            }

            mobileMenu?.classList.add("hidden");
            iconMenu?.classList.remove("hidden");
            iconClose?.classList.add("hidden");
        });
    });

    document.addEventListener("click", (event) => {
        const notifContainer = document.getElementById("notif-container");
        const userContainer = document.getElementById("user-container");
        const mobileMenu = document.getElementById("mobile-menu");
        const mobileMenuBtn = document.getElementById("btn-mobile-menu");

        if (!notifContainer?.contains(event.target)) {
            document.getElementById("notif-menu")?.classList.add("hidden");
        }

        if (!userContainer?.contains(event.target)) {
            document.getElementById("user-menu")?.classList.add("hidden");
        }

        // Đóng mobile menu khi click ra ngoài header
        if (!mobileMenu?.contains(event.target) && !mobileMenuBtn?.contains(event.target)) {
            mobileMenu?.classList.add("hidden");
            document.getElementById("icon-menu")?.classList.remove("hidden");
            document.getElementById("icon-close")?.classList.add("hidden");
        }
    });
});

