let page = 1;
const pageSize = 5;
let isLoading = false;
let hasMore = true;
let selectedResourceIds = [];
let observer = null;
let currentModalStep = 1;
let selectedDeliveryMethod = 'RESERVATION';
let searchQuery = '';
let searchTimeout = null;

document.addEventListener("DOMContentLoaded", () => {
    // Tải trang đầu tiên từ server khi vào trang hoặc refresh
    loadQueuePage(page);
    initInfiniteScroll();
    initSearchListener();
});

function initSearchListener() {
    const searchInput = document.getElementById("queue-search-input");
    if (!searchInput) return;

    searchInput.addEventListener("input", (e) => {
        searchQuery = e.target.value.trim();

        // Hủy timeout trước đó nếu người dùng vẫn đang gõ tiếp
        if (searchTimeout) clearTimeout(searchTimeout);

        // Đợi 300ms sau khi người dùng ngừng gõ mới tiến hành gọi API
        searchTimeout = setTimeout(() => {
            page = 1;
            hasMore = true;

            // Xóa sạch các item kết quả cũ trên giao diện
            const container = document.getElementById("queue-items-container");
            if (container) {
                container.querySelectorAll(".queue-item-row").forEach(el => el.remove());
            }

            // Ẩn trạng thái empty nếu có
            const emptyState = document.getElementById("empty-state");
            if (emptyState) emptyState.classList.add("hidden");

            loadQueuePage(page);
        }, 300);
    });
}

async function loadQueuePage(pageNum) {
    if (isLoading || !hasMore) return;
    isLoading = true;

    if (pageNum === 1) {
        showInitialSkeleton();
    } else {
        showBottomLoader();
    }

    try {
        const response = await apiClient.private.get(API.private.borrowCart.getqueue, {
            params: { page: pageNum, pageSize: pageSize, search: searchQuery }
        });

        if (!response.ok) throw new Error("Không thể tải danh sách chờ mượn.");

        const data = await response.json();
        const itemsArray = data.items || data.Items || [];
        const totalItems = data.totalItems || data.TotalItems || 0;

        if (pageNum === 1) {
            updateQueueBadge(totalItems);
        }

        if (pageNum === 1 && itemsArray.length === 0) {
            showEmptyState();
            hasMore = false;
            return;
        }

        if (itemsArray.length > 0) {
            renderItems(itemsArray);
            page++;

            if ((pageNum * pageSize) >= totalItems || itemsArray.length < pageSize) {
                hasMore = false;
            } else {
                setTimeout(() => {
                    if (hasMore && !isLoading && document.documentElement.scrollHeight <= window.innerHeight) {
                        loadQueuePage(page);
                    }
                }, 100);
            }
        } else {
            hasMore = false;
            if (observer) observer.disconnect();
        }

    } catch (error) {
        if (pageNum === 1) showEmptyState();
    } finally {
        isLoading = false;
        hideInitialSkeleton();
        hideBottomLoader();
    }
}

function renderItems(items) {
    const container = document.getElementById("queue-items-container");
    const bottomSkeleton = document.getElementById("bottom-skeleton");
    if (!container) return;

    items.forEach(item => {
        const itemEl = document.createElement("div");

        const cardStyle = item.isAvailable
            ? "bg-white border-slate-200 hover:border-slate-300"
            : "bg-slate-50 border-slate-200 relative overflow-hidden";

        itemEl.className = `queue-item-row p-4 border rounded-lg flex gap-4 items-center justify-between transition-colors ${cardStyle}`;
        itemEl.id = `queue-item-${item.resourceId}`;

        const overlayHtml = !item.isAvailable
            ? `<div class="absolute inset-0 bg-slate-200/50 backdrop-grayscale-[0.3] pointer-events-none z-10"></div>`
            : '';

        let availabilityBadge = '';
        if (item.isAvailable) {
            availabilityBadge = `<span class="inline-flex items-center px-2 py-0.5 rounded text-[10px] font-medium bg-emerald-50 text-emerald-700 border border-emerald-200">Khả dụng trong kho</span>`;
        } else {
            const reason = item.unavailableReason || "Không khả dụng";
            availabilityBadge = `<span class="inline-flex items-center px-2 py-0.5 rounded text-[10px] font-medium bg-rose-50 text-rose-700 border border-rose-200 shadow-sm">${reason}</span>`;
        }

        const checkboxDisabled = !item.isAvailable ? 'disabled' : '';
        const checkboxStyle = !item.isAvailable ? 'opacity-30 cursor-not-allowed' : 'cursor-pointer text-indigo-600';
        const isChecked = selectedResourceIds.includes(String(item.resourceId)) ? 'checked' : '';
        const detailUrl = `/Resource/Detail/${item.resourceId}`;

        itemEl.innerHTML = `
            ${overlayHtml}
            <div class="flex gap-4 items-center flex-1 min-w-0 relative z-20">
                <input type="checkbox" value="${item.resourceId}" ${checkboxDisabled} ${isChecked}
                    class="queue-item-checkbox w-4 h-4 rounded border-slate-300 ${checkboxStyle}"
                    onchange="handleCheckboxChange(this, '${item.resourceId}')">
                
                <a href="${detailUrl}" class="flex-shrink-0 group">
                    <img src="${item.coverImage || '/images/default-book.png'}" class="w-14 h-20 object-cover rounded border border-slate-100 group-hover:opacity-90 transition-opacity ${!item.isAvailable ? 'grayscale opacity-75' : ''}" alt="Cover">
                </a>
                
                <div class="min-w-0 flex-1">
                    <a href="${detailUrl}" class="block">
                        <h3 class="text-sm font-semibold text-slate-800 line-clamp-1 hover:text-indigo-600 transition-colors ${!item.isAvailable ? 'text-slate-500' : ''}">${item.title}</h3>
                    </a>
                    <p class="text-xs text-slate-400 mt-0.5 truncate">Tác giả: ${item.authors || 'Nhiều tác giả'}</p>
                    <div class="flex items-center gap-2 mt-2">
                        ${availabilityBadge}
                    </div>
                </div>
            </div>
            
            <button onclick="removeFromQueueServer('${item.resourceId}')" class="p-1.5 text-slate-400 hover:text-rose-500 transition-colors flex-shrink-0 ml-2 relative z-30" title="Xóa khỏi khay">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
            </button>
        `;

        // Chèn item mới ngay trước bottom-skeleton
        if (bottomSkeleton && bottomSkeleton.parentNode === container) {
            container.insertBefore(itemEl, bottomSkeleton);
        } else {
            container.appendChild(itemEl);
        }
    });

    updateFloatingBarState();
}

// Xử lý sự kiện checkbox trực tiếp O(1)
function handleCheckboxChange(checkbox, resourceId) {
    const idStr = String(resourceId);

    if (checkbox.checked) {
        if (!selectedResourceIds.includes(idStr)) {
            selectedResourceIds.push(idStr);
        }
    } else {
        selectedResourceIds = selectedResourceIds.filter(id => id !== idStr);
    }

    updateFloatingBarState();
}

// Hàm cập nhật trạng thái hiển thị của Nút Đặt Mượn Nổi ở góc
function updateFloatingBarState() {
    const floatingBar = document.getElementById("global-floating-bar");
    const selectedCountBadge = document.getElementById("floating-selected-count");

    if (selectedCountBadge) {
        selectedCountBadge.textContent = selectedResourceIds.length;
    }

    if (selectedResourceIds.length === 0) {
        if (floatingBar) {
            floatingBar.classList.add("translate-y-20", "opacity-0", "pointer-events-none");
            floatingBar.classList.remove("translate-y-0", "opacity-100");
        }
    } else {
        if (floatingBar) {
            floatingBar.classList.remove("translate-y-20", "opacity-0", "pointer-events-none");
            floatingBar.classList.add("translate-y-0", "opacity-100");
        }
    }
}

function initInfiniteScroll() {
    window.addEventListener('scroll', () => {
        if (isLoading || !hasMore) return;

        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.documentElement.scrollHeight - 250;

        if (scrollPosition >= threshold) {
            loadQueuePage(page);
        }
    });
}

function showInitialSkeleton() {
    const loader = document.getElementById("initial-skeleton");
    if (loader) loader.classList.remove("hidden");
}

function hideInitialSkeleton() {
    const loader = document.getElementById("initial-skeleton");
    if (loader) loader.classList.add("hidden");
}

function showBottomLoader() {
    const loader = document.getElementById("bottom-skeleton");
    if (loader) loader.classList.remove("hidden");
}

function hideBottomLoader() {
    const loader = document.getElementById("bottom-skeleton");
    if (loader) loader.classList.add("hidden");
}

function showEmptyState() {
    hideInitialSkeleton();
    hideBottomLoader();
    if (observer) observer.disconnect();

    const emptyState = document.getElementById("empty-state");
    if (emptyState) emptyState.classList.remove("hidden");

    updateQueueBadge(0);
}

function updateQueueBadge(count) {
    const badge = document.getElementById("queue-count-badge");
    if (badge) badge.textContent = `${count} tài liệu`;
}

async function removeFromQueueServer(resourceId) {
    try {
        // Truyền mảng chứa [resourceId] vì API backend nhận List<Guid> qua [FromBody]
        const response = await apiClient.private.post(API.private.borrowCart.bulkDelete, [resourceId]);
        if (!response.ok) throw new Error("Không thể xóa tài liệu khỏi khay.");

        const itemNode = document.getElementById(`queue-item-${resourceId}`);
        if (itemNode) itemNode.remove();

        const idStr = String(resourceId);
        selectedResourceIds = selectedResourceIds.filter(id => id !== idStr);

        // Cập nhật lại badge tổng số lượng
        const badge = document.getElementById("queue-count-badge");
        if (badge) {
            const currentCount = parseInt(badge.textContent) || 1;
            const newCount = Math.max(0, currentCount - 1);
            updateQueueBadge(newCount);
            if (newCount === 0) showEmptyState();
        }

        updateFloatingBarState();

        // Nếu modal đang mở thì làm tươi lại modal
        const modal = document.getElementById("borrow-flow-modal");
        if (modal && !modal.classList.contains("hidden")) {
            if (selectedResourceIds.length === 0) {
                closeBorrowModal();
            } else {
                renderModalSelectedBooks();
            }
        }

    } catch (err) {
        console.error(err);
        showToast("Lỗi khi xóa tài liệu.");
    }
}


function openBorrowModal() {
    if (selectedResourceIds.length === 0) {
        showToast("Vui lòng chọn ít nhất 1 tài liệu để đặt mượn.");
        return;
    }

    renderModalSelectedBooks();

    // Mở modal và hiển thị Bước 1
    document.getElementById("borrow-flow-modal").classList.remove("hidden");
    goToModalStep(1);
}

function renderModalSelectedBooks() {
    const container = document.getElementById("modal-selected-books-list");
    if (!container) return;

    container.innerHTML = "";

    selectedResourceIds.forEach(id => {
        const itemRow = document.getElementById(`queue-item-${id}`);
        if (itemRow) {
            const title = itemRow.querySelector("h3")?.textContent || "Tài liệu";
            const imgSrc = itemRow.querySelector("img")?.src || "/images/default-book.png";

            const div = document.createElement("div");
            div.className = "flex items-center justify-between gap-3 py-1.5 first:pt-0 last:pb-0";
            div.innerHTML = `
                <div class="flex items-center gap-3 min-w-0 flex-1">
                    <img src="${imgSrc}" class="w-8 h-10 object-cover rounded border shrink-0" alt="">
                    <span class="text-xs font-semibold text-slate-800 line-clamp-1">${title}</span>
                </div>
                <button onclick="uncheckModalItem('${id}')" class="text-slate-400 hover:text-rose-500 p-1 transition-colors cursor-pointer" title="Bỏ chọn">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                    </svg>
                </button>
            `;
            container.appendChild(div);
        }
    });

    const summaryCount = document.getElementById("modal-summary-count");
    if (summaryCount) summaryCount.textContent = selectedResourceIds.length;
}

function uncheckModalItem(resourceId) {
    const idStr = String(resourceId);

    // Uncheck ô checkbox ngoài trang chính
    const itemRow = document.getElementById(`queue-item-${idStr}`);
    if (itemRow) {
        const checkbox = itemRow.querySelector('.queue-item-checkbox');
        if (checkbox) checkbox.checked = false;
    }

    // Lọc loại khỏi mảng
    selectedResourceIds = selectedResourceIds.filter(id => id !== idStr);

    updateFloatingBarState();

    // Cập nhật lại UI modal
    if (selectedResourceIds.length === 0) {
        closeBorrowModal();
    } else {
        renderModalSelectedBooks();
    }
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
        document.getElementById("confirm-count").textContent = `${selectedResourceIds.length} tài liệu`;
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

async function submitModalBorrow() {
    const btnSubmit = document.getElementById("btn-modal-submit");
    btnSubmit.disabled = true;
    btnSubmit.textContent = "Đang xử lý...";

    const itemsToDelete = [...selectedResourceIds];

    const payload = {
        resourceIds: selectedResourceIds,
        requestType: selectedDeliveryMethod,
        recipientPhone: selectedDeliveryMethod === 'DELIVERY' ? document.getElementById("modal-phone-input").value : null,
        deliveryAddress: selectedDeliveryMethod === 'DELIVERY' ? document.getElementById("modal-address-input").value : null
    };

    try {
        const response = await apiClient.private.post(API.private.borrowRequest.create, payload);

        if (response.ok) {
            const result = await response.json();

            if (itemsToDelete.length > 0) {
                try {
                    // Truyền trực tiếp mảng itemsToDelete thay vì bọc object
                    await apiClient.private.post(API.private.borrowCart.bulkDelete, itemsToDelete);

                    itemsToDelete.forEach(id => {
                        const itemNode = document.getElementById(`queue-item-${id}`);
                        if (itemNode) itemNode.remove();
                    });
                } catch (delErr) {
                    showToast("Lỗi khi xóa tài liệu khỏi khay sau khi đặt mượn:" + delErr);
                }
            }

            selectedResourceIds = [];
            updateFloatingBarState();

            const modalBooksContainer = document.getElementById("modal-selected-books-list")?.closest('.bg-slate-50');
            if (modalBooksContainer) modalBooksContainer.classList.add("hidden");

            const stepperContainer = document.getElementById("step-badge-1")?.closest('.flex.items-center.justify-center.gap-3');
            if (stepperContainer) stepperContainer.classList.add("hidden");

            const requestCode = result.requestCode || result.RequestCode || "";
            document.getElementById("modal-request-code").textContent = requestCode;



            goToModalStep(3);

            const remainingItems = document.querySelectorAll(".queue-item-row");
            if (remainingItems.length === 0) {
                showEmptyState();
            }
        } else {
            const errJson = await response.json().catch(() => ({}));
            showToast(errJson.detail || "Không thể gửi yêu cầu đặt mượn.");
        }
    } catch (err) {
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