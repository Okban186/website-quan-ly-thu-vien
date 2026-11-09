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

async function addToBorrowQueue(resourceId) {
    const btn = document.getElementById("btn-add-queue");

    if (btn) {
        btn.disabled = true;
        btn.style.opacity = "0.7";
    }

    try {
        const response = await apiClient.private.post(API.private.borrowCart.add(resourceId), {
            resourceId: resourceId
        });


        const text = await response.text();
        const result = text ? JSON.parse(text) : {};

        if (response.ok) {
            showToast("Đã thêm tài liệu vào danh sách chờ mượn thành công!", 'success');

            if (result.totalCount !== undefined) {
                updateQueueCountBadge(result.totalCount);
            }

            if (btn) {
                btn.disabled = false;
                btn.style.opacity = "1";
                btn.title = "Đã trong danh sách chờ mượn (Bấm để xóa)";
                btn.className = "px-4 py-3 rounded-xl font-bold text-sm shadow-md transition-all flex items-center justify-center shrink-0 bg-[#1e3a5f] text-white hover:bg-[#142944] cursor-pointer";
                btn.innerHTML = `<i class="fa-solid fa-bookmark text-amber-400 text-base"></i>`;
                btn.onclick = function () {
                    removeFromQueueAndResetButton(resourceId);
                };
            }
            return;
        } else {
            showToast(result.detail || "Không thể thêm tài liệu vào danh sách.");
        }
    } catch (err) {
        console.error("Lỗi:", err);
        showToast("Đã xảy ra lỗi kết nối, vui lòng thử lại sau.", 'error');
    }

    // Hồi phục lại trạng thái nút NẾU request bị lỗi / thất bại
    if (btn) {
        btn.disabled = false;
        btn.style.opacity = "1";
    }
}

async function removeFromQueueAndResetButton(resourceId) {
    try {

        const response = await apiClient.private.post(API.private.borrowCart.bulkDelete, [resourceId]);
        if (!response.ok) throw new Error("Không thể xóa tài liệu khỏi khay.");



    } catch (err) {
        console.error(err);
        showToast("Lỗi khi xóa tài liệu.");
    }

    showToast("Đã xóa tài liệu khỏi danh sách chờ.", 'info');


    const btn = document.getElementById("btn-add-queue");
    if (btn) {
        btn.title = "Thêm vào danh sách chờ mượn";
        btn.className = "px-4 py-3 rounded-xl font-bold text-sm shadow-sm transition-all flex items-center justify-center shrink-0 bg-white border border-slate-300 hover:bg-slate-50 text-slate-700 cursor-pointer";
        btn.innerHTML = `<i class="fa-regular fa-bookmark text-[#1e3a5f] text-base"></i>`;
        btn.onclick = function () {
            addToBorrowQueue(resourceId);
        };
    }
}

var selectedDeliveryMethod = 'RESERVATION'



function openBorrowModal(resourceId = "") {
    if (resourceId === "") {
        showToast("Vui lòng chọn ít nhất 1 tài liệu để đặt mượn.");
        return;
    }

    document.getElementById("borrow-flow-modal").classList.remove("hidden");
    goToModalStep(1);
}


function closeBorrowModal() {
    document.getElementById("borrow-flow-modal").classList.add("hidden");
}

function selectModalDelivery(method) {
    selectedDeliveryMethod = method; // Sẽ nhận 'RESERVATION' hoặc 'DELIVERY'
    const optionPickup = document.getElementById("option-pickup");
    const optionDelivery = document.getElementById("option-delivery");
    const inputWrapper = document.getElementById("modal-input-wrapper");

    const dotPickup = document.getElementById("dot-pickup");
    const dotDelivery = document.getElementById("dot-delivery");

    if (method === 'RESERVATION') {
        optionPickup.className = "p-4 rounded-xl border border-[#1e3a5f] bg-blue-50/30 ring-1 ring-[#1e3a5f] transition-all cursor-pointer flex items-start gap-3.5";
        optionDelivery.className = "p-4 rounded-xl border border-slate-200 hover:border-slate-300 transition-all cursor-pointer flex items-start gap-3.5";

        if (dotPickup) {
            dotPickup.className = "w-4 h-4 rounded-full mt-0.5 border border-[#1e3a5f] bg-[#1e3a5f] flex items-center justify-center transition-colors";
            dotPickup.innerHTML = '<div class="w-1.5 h-1.5 rounded-full bg-white"></div>';
        }

        if (dotDelivery) {
            dotDelivery.className = "w-4 h-4 rounded-full mt-0.5 border border-slate-300 bg-white flex items-center justify-center transition-colors";
            dotDelivery.innerHTML = '';
        }

        if (inputWrapper) inputWrapper.classList.add("hidden");
    } else {
        optionDelivery.className = "p-4 rounded-xl border border-[#1e3a5f] bg-blue-50/30 ring-1 ring-[#1e3a5f] transition-all cursor-pointer flex items-start gap-3.5";
        optionPickup.className = "p-4 rounded-xl border border-slate-200 hover:border-slate-300 transition-all cursor-pointer flex items-start gap-3.5";

        if (dotDelivery) {
            dotDelivery.className = "w-4 h-4 rounded-full mt-0.5 border border-[#1e3a5f] bg-[#1e3a5f] flex items-center justify-center transition-colors";
            dotDelivery.innerHTML = '<div class="w-1.5 h-1.5 rounded-full bg-white"></div>';
        }

        if (dotPickup) {
            dotPickup.className = "w-4 h-4 rounded-full mt-0.5 border border-slate-300 bg-white flex items-center justify-center transition-colors";
            dotPickup.innerHTML = '';
        }

        if (inputWrapper) inputWrapper.classList.remove("hidden");
    }
}

function goToModalStep(step) {
    if (step === 2 && selectedDeliveryMethod === 'DELIVERY') {
        const phoneInput = document.getElementById("modal-phone-input")?.value.trim() || "";
        const addressInput = document.getElementById("modal-address-input")?.value.trim() || "";

        const phoneRegex = /(84|0[3|5|7|8|9])+([0-9]{8})\b/;

        if (!phoneInput) {
            showFieldError("modal-phone-input", "phone-error-msg", "Vui lòng nhập số điện thoại nhận sách.");
            return;
        }
        if (!phoneRegex.test(phoneInput)) {
            showFieldError("modal-phone-input", "phone-error-msg", "Số điện thoại không đúng định dạng (phải gồm 10 chữ số hợp lệ).");
            return;
        }

        if (!addressInput) {
            showFieldError("modal-address-input", "address-error-msg", "Vui lòng nhập địa chỉ giao nhận chi tiết.");
            return;
        }
    }

    currentModalStep = step;

    document.getElementById("modal-step-1").classList.add("hidden");
    document.getElementById("modal-step-2").classList.add("hidden");
    document.getElementById("modal-step-3").classList.add("hidden");

    // Cập nhật giao diện Stepper
    const b1 = document.getElementById("step-badge-1");
    const b2 = document.getElementById("step-badge-2");
    const b3 = document.getElementById("step-badge-3");
    const l1 = document.getElementById("step-line-1");
    const l2 = document.getElementById("step-line-2");

    b1.className = "w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-colors bg-[#1e3a5f] text-white";
    b2.className = step >= 2 ? "w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-colors bg-[#1e3a5f] text-white" : "w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-colors bg-slate-200 text-slate-500";
    b3.className = step === 3 ? "w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-colors bg-[#1e3a5f] text-white" : "w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-colors bg-slate-200 text-slate-500";
    l1.className = step >= 2 ? "w-12 sm:w-20 h-0.5 bg-[#1e3a5f]" : "w-12 sm:w-20 h-0.5 bg-slate-200";
    l2.className = step === 3 ? "w-12 sm:w-20 h-0.5 bg-[#1e3a5f]" : "w-12 sm:w-20 h-0.5 bg-slate-200";

    if (step === 1) {
        document.getElementById("modal-step-1").classList.remove("hidden");
    } else if (step === 2) {
        document.getElementById("confirm-count").textContent = `1 tài liệu`;
        document.getElementById("confirm-method").textContent = selectedDeliveryMethod === 'RESERVATION' ? 'Nhận tại thư viện' : 'Giao tận nơi';

        const phoneRow = document.getElementById("confirm-phone-row");
        const addrRow = document.getElementById("confirm-address-row");

        if (selectedDeliveryMethod === 'DELIVERY') {
            if (phoneRow) phoneRow.classList.remove("hidden");
            if (addrRow) addrRow.classList.remove("hidden");
            document.getElementById("confirm-phone").textContent = document.getElementById("modal-phone-input").value;
            document.getElementById("confirm-address").textContent = document.getElementById("modal-address-input").value;
        } else {
            if (phoneRow) phoneRow.classList.add("hidden");
            if (addrRow) addrRow.classList.add("hidden");
        }

        document.getElementById("modal-step-2").classList.remove("hidden");
    } else if (step === 3) {
        const modalBooksContainer = document.getElementById("modal-selected-books-list")?.closest('.bg-slate-50');
        if (modalBooksContainer) modalBooksContainer.classList.add("hidden");

        document.getElementById("modal-step-3").classList.remove("hidden");
    }
}

async function submitModalBorrow(resourceId = "") {
    const btnSubmit = document.getElementById("btn-modal-submit");
    btnSubmit.disabled = true;
    btnSubmit.textContent = "Đang xử lý...";

    const payload = {
        resourceIds: [resourceId],
        requestType: selectedDeliveryMethod,
        recipientPhone: selectedDeliveryMethod === 'DELIVERY' ? document.getElementById("modal-phone-input").value : null,
        deliveryAddress: selectedDeliveryMethod === 'DELIVERY' ? document.getElementById("modal-address-input").value : null
    };

    try {
        const response = await apiClient.private.post(API.private.borrowRequest.create, payload);

        if (response.ok) {
            const result = await response.json();

            const modalBooksContainer = document.getElementById("modal-selected-books-list")?.closest('.bg-slate-50');
            if (modalBooksContainer) modalBooksContainer.classList.add("hidden");

            const stepperContainer = document.getElementById("step-badge-1")?.closest('.flex.items-center.justify-center.gap-3');
            if (stepperContainer) stepperContainer.classList.add("hidden");

            const requestCode = result.requestCode || result.RequestCode || "";
            document.getElementById("modal-request-code").textContent = requestCode;

            goToModalStep(3);

            const directBorrowBtn = document.getElementById("btn-direct-borrow");
            if (directBorrowBtn) {
                directBorrowBtn.disabled = true;
                directBorrowBtn.title = "Bạn đang có yêu cầu hoặc đang mượn tài liệu này";
                directBorrowBtn.removeAttribute("onclick");
                directBorrowBtn.className = "flex-1 py-3 rounded-xl font-bold text-sm shadow-md transition-all flex items-center justify-center gap-2 bg-slate-200 text-slate-500 cursor-not-allowed shadow-none";
                directBorrowBtn.innerHTML = `
                    <span>Đang mượn / Chờ duyệt</span>
                `;
            }
        } else {
            const errJson = await response.json().catch(() => ({}));
            showToast(errJson.detail || "Không thể gửi yêu cầu đặt mượn.");
        }
    } catch (err) {
        console.error("LỖI CHI TIẾT:", err);
        showToast("Có lỗi xảy ra trong quá trình đặt mượn.");
    } finally {
        btnSubmit.disabled = false;
        btnSubmit.textContent = "Xác nhận đặt mượn";
    }
}


function showFieldError(inputId, errorId, message) {
    const inputEl = document.getElementById(inputId);
    const errorEl = document.getElementById(errorId);

    if (inputEl) {
        inputEl.classList.add("border-rose-500", "bg-rose-50/20", "focus:border-rose-500");
        inputEl.classList.remove("border-slate-300");
        inputEl.focus();
    }

    if (errorEl) {
        errorEl.textContent = message;
        errorEl.classList.remove("hidden");
    }
}

function clearFieldError(inputId, errorId) {
    const inputEl = document.getElementById(inputId);
    const errorEl = document.getElementById(errorId);

    if (inputEl) {
        inputEl.classList.remove("border-rose-500", "bg-rose-50/20", "focus:border-rose-500");
        inputEl.classList.add("border-slate-300");
    }

    if (errorEl) {
        errorEl.textContent = "";
        errorEl.classList.add("hidden");
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const phoneInput = document.getElementById("modal-phone-input");
    const addressInput = document.getElementById("modal-address-input");

    if (phoneInput) {
        phoneInput.addEventListener("input", () => clearFieldError("modal-phone-input", "phone-error-msg"));
    }
    if (addressInput) {
        addressInput.addEventListener("input", () => clearFieldError("modal-address-input", "address-error-msg"));
    }
});