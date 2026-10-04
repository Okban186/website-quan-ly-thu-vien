document.addEventListener("DOMContentLoaded", () => {

    const notifBtn = document.getElementById("btn-notifications-toggle");
    const notifMenu = document.getElementById("notif-menu");
    const notifContainer = document.getElementById("notif-container");

    const userBtn = document.getElementById("btn-user-menu-toggle");
    const userMenu = document.getElementById("user-menu");
    const userContainer = document.getElementById("user-container");

    const mobileMenuBtn = document.getElementById("btn-mobile-menu");
    const mobileMenu = document.getElementById("mobile-menu");
    const iconMenu = document.getElementById("icon-menu");
    const iconClose = document.getElementById("icon-close");


    const closeMobileMenu = () => {
        mobileMenu?.classList.add("hidden");
        iconMenu?.classList.remove("hidden");
        iconClose?.classList.add("hidden");
    };


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


    mobileMenuBtn?.addEventListener("click", (event) => {
        event.stopPropagation();
        mobileMenu?.classList.toggle("hidden");
        iconMenu?.classList.toggle("hidden");
        iconClose?.classList.toggle("hidden");
    });


    document.querySelectorAll(".nav-mobile-btn").forEach(button => {
        button.addEventListener("click", () => {
            if (button.dataset.target) {
                closeMobileMenu();
            }
        });
    });


    document.addEventListener("click", (event) => {
        const targetElement = event.target;


        if (notifContainer && !notifContainer.contains(targetElement)) {
            notifMenu?.classList.add("hidden");
        }


        if (userContainer && !userContainer.contains(targetElement)) {
            userMenu?.classList.add("hidden");
        }


        const isClickInsideMobileMenu = mobileMenu?.contains(targetElement);
        const isClickOnMobileBtn = mobileMenuBtn?.contains(targetElement);

        if (!isClickInsideMobileMenu && !isClickOnMobileBtn) {
            closeMobileMenu();
        }
    });
});
