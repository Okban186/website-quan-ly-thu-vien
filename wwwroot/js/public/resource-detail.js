document.addEventListener("DOMContentLoaded", function () {
    checkDescriptionHeight();

    const desc = document.getElementById('descText');
    const btn = document.getElementById('toggleDescBtn');

    var thumbsSwiper = new Swiper(".resource-thumbs-swiper", {
        spaceBetween: 8,
        slidesPerView: 3, // Giới hạn đúng 3 ảnh xuất hiện cùng lúc
        freeMode: true,
        watchSlidesProgress: true,
    });

    // Khởi tạo Swiper chính và liên kết với thumbs ở trên
    var mainSwiper = new Swiper(".resource-main-swiper", {
        spaceBetween: 10,
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
        thumbs: {
            swiper: thumbsSwiper,
        },
    });

    if (!desc || !btn) return;

    if (desc.scrollHeight > desc.clientHeight) {
        btn.classList.remove('hidden');
    }
    if (document.querySelector('.resource-swiper')) {
        new Swiper('.resource-swiper', {
            loop: true,
            pagination: {
                el: '.swiper-pagination',
                clickable: true,
            },
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
            },
        });
    }
});

// Hàm mở Lightbox xem ảnh lớn
function openLightbox(imgSrc) {
    const lightbox = document.getElementById('imageLightbox');
    const lightboxImg = document.getElementById('lightboxImg');
    lightboxImg.src = imgSrc;
    lightbox.classList.remove('hidden');
    lightbox.classList.add('flex');
}

// Hàm đóng Lightbox
function closeLightbox() {
    const lightbox = document.getElementById('imageLightbox');
    lightbox.classList.remove('flex');
    lightbox.classList.add('hidden');
}


function checkDescriptionHeight() {
    const container = document.getElementById('descContainer');
    const btn = document.getElementById('toggleDescBtn');
    const gradient = document.getElementById('descGradient');

    if (!container) return;

    const COLLAPSED_HEIGHT = 128;

    if (container.scrollHeight > COLLAPSED_HEIGHT) {

        btn.classList.remove('hidden');
        gradient.classList.remove('hidden');
    } else {

        container.classList.remove('max-h-32', 'overflow-hidden');
        btn.classList.add('hidden');
        gradient.classList.add('hidden');
    }
}

function toggleDescription() {
    const container = document.getElementById('descContainer');
    const btn = document.getElementById('toggleDescBtn');
    const gradient = document.getElementById('descGradient');

    if (container.classList.contains('max-h-32')) {
        // Mở rộng ra
        container.classList.remove('max-h-32');
        container.classList.add('max-h-[1000px]');
        gradient.classList.add('hidden');
        btn.textContent = 'Thu gọn';
    } else {
        // Thu gọn lại
        container.classList.add('max-h-32');
        container.classList.remove('max-h-[1000px]');
        gradient.classList.remove('hidden');
        btn.textContent = 'Xem thêm';

        // Cuộn nhẹ lên đầu khung mô tả khi bấm thu gọn để người dùng không bị hụt tầm nhìn
        container.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
}