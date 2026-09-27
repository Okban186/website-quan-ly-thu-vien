const toastContainer = document.getElementById('toast-container');

const toastIcons = {
    success: `
        <svg class="w-5 h-5 shrink-0" viewBox="0 0 24 24" fill="none"
             stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round"
                  d="M5 13l4 4L19 7" />
        </svg>
    `,

    error: `
        <svg class="w-5 h-5 shrink-0" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"> 
            <circle cx="12" cy="12" r="9" /> 
            <path stroke-linecap="round" d="M12 8v5" /> 
            <path stroke-linecap="round" d="M12 16h.01" /> 
        </svg>
    `,

    warning: `
        <svg class="w-5 h-5 shrink-0" viewBox="0 0 24 24" fill="none"
             stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round"
                  d="M12 9v4m0 4h.01M10.29 3.86l-7.82 13.5A2 2 0 004.2 20h15.6a2 2 0 001.73-2.64l-7.82-13.5a2 2 0 00-3.42 0z" />
        </svg>
    `
};

const toastStyles = {
    success: {
        container: 'bg-green-50 border-green-200 text-green-700',
        icon: 'bg-green-100 text-green-600'
    },

    error: {
        container: 'bg-red-50 border-red-200 text-red-700',
        icon: 'bg-red-100 text-red-600'
    },

    warning: {
        container: 'bg-amber-50 border-amber-200 text-amber-700',
        icon: 'bg-amber-100 text-amber-600'
    }
};

function showToast(message, type = 'error') {
    const style = toastStyles[type] ?? toastStyles.error;
    const icon = toastIcons[type] ?? toastIcons.error;

    const toast = document.createElement('div');

    toast.className = `
        pointer-events-auto
        flex items-center gap-3
        min-w-[320px] max-w-md
        px-4 py-3
        rounded-xl
        border
        shadow-lg
        text-sm font-medium
        ${style.container}
        opacity-0
        translate-x-8
        transition-all
        duration-300
        ease-out
    `;

    // Icon
    const iconContainer = document.createElement('div');

    iconContainer.className = `
        flex items-center justify-center
        w-8 h-8
        rounded-lg
        shrink-0
        ${style.icon}
    `;

    iconContainer.insertAdjacentHTML('afterbegin', icon);

    // Message
    const messageElement = document.createElement('span');

    messageElement.className = 'flex-1';
    messageElement.textContent = message;

    // Close button
    const closeButton = document.createElement('button');

    closeButton.type = 'button';
    closeButton.className = `
        shrink-0
        opacity-60
        hover:opacity-100
        transition-opacity
    `;

    closeButton.setAttribute('aria-label', 'Đóng');

    closeButton.insertAdjacentHTML(
        'afterbegin',
        `
        <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none"
             stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round"
                  d="M6 6l12 12M6 18L18 6" />
        </svg>
        `
    );

    toast.appendChild(iconContainer);
    toast.appendChild(messageElement);
    toast.appendChild(closeButton);

    toastContainer.appendChild(toast);

    // Slide in
    requestAnimationFrame(() => {
        toast.classList.remove(
            'opacity-0',
            'translate-x-8'
        );

        toast.classList.add(
            'opacity-100',
            'translate-x-0'
        );
    });

    let closeTimeout;

    const closeToast = () => {
        clearTimeout(closeTimeout);

        toast.classList.remove(
            'opacity-100',
            'translate-x-0'
        );

        toast.classList.add(
            'opacity-0',
            'translate-x-8'
        );

        setTimeout(() => {
            toast.remove();
        }, 300);
    };

    closeButton.addEventListener('click', closeToast);

    closeTimeout = setTimeout(closeToast, 3000);
}