
let appState = {
    activeTab: 'borrowed',
    borrowed: { items: [], page: 1, pageSize: 6, hasMore: true, totalItems: 0, loading: false },
    history: { items: [], page: 1, pageSize: 6, hasMore: true, totalItems: 0, loading: false }
};
let selectedCancelId = null;
let selectedRenewLoanItemId = null;

document.addEventListener("DOMContentLoaded", function () {
    loadNextPage('borrowed', true);
    loadNextPage('history', true);

    window.addEventListener('scroll', handleWindowScroll);

    const confirmBtn = document.getElementById('btn-confirm-cancel-action');
    if (confirmBtn) {
        confirmBtn.addEventListener('click', function () {
            if (selectedCancelId) {
                const idToCancel = selectedCancelId;
                closeCancelModal();
                cancelBorrowRequest(idToCancel);
            }
        });
    }

    const renewConfirmBtn = document.getElementById('btn-confirm-renew-action');
    if (renewConfirmBtn) {
        renewConfirmBtn.addEventListener('click', renewLoanItem);
    }
});

function handleWindowScroll() {
    const state = appState[appState.activeTab];
    if (!state.hasMore || state.loading) return;

    if ((window.innerHeight + window.scrollY) >= document.documentElement.scrollHeight - 150) {
        loadNextPage(appState.activeTab, false);
    }
}

function switchMainTab(tabName) {
    appState.activeTab = tabName;
    document.querySelectorAll('.main-tab-content').forEach(el => el.classList.add('hidden'));
    document.querySelectorAll('[id^="main-tab-btn-"]').forEach(el => {
        el.classList.remove('border-[#1e3a5f]', 'text-[#1e3a5f]');
        el.classList.add('border-transparent', 'text-slate-500');
    });

    document.getElementById(`main-tab-content-${tabName}`).classList.remove('hidden');
    const activeBtn = document.getElementById(`main-tab-btn-${tabName}`);
    activeBtn.classList.remove('border-transparent', 'text-slate-500');
    activeBtn.classList.add('border-[#1e3a5f]', 'text-[#1e3a5f]');

    handleWindowScroll();
}

async function fetchAllData() {
    window.removeEventListener('scroll', handleWindowScroll);
    await loadNextPage('borrowed', true);
    await loadNextPage('history', true);
    window.addEventListener('scroll', handleWindowScroll);
}

async function loadNextPage(type, isReset = false) {
    const state = appState[type];
    if (isReset) {
        state.page = 1;
        state.items = [];
        state.hasMore = true;
        state.loading = false;
    }

    if (!state.hasMore || state.loading) return;

    state.loading = true;
    const loader = document.getElementById(`loading-${type}`);
    if (loader) loader.classList.remove('hidden');

    const containerId = type === 'borrowed' ? 'container-borrowed-items' : 'container-history-items';
    const container = document.getElementById(containerId);
    const badge = document.getElementById(`badge-count-${type}`);

    if (isReset) {
        container.innerHTML = `<div class="text-center py-6 text-slate-400 text-xs">Đang cập nhật dữ liệu...</div>`;
    }

    try {
        const response = await apiClient.public.get(API.private.borrowRequest.getTransaction, {
            params: {
                type: type,
                page: state.page,
                pageSize: state.pageSize
            }
        });

        if (!response.ok) {
            showToast("Không thể tải dữ liệu.");
            return;
        }

        const result = await response.json();
        const fetchedItems = result.items || [];
        state.totalItems = result.totalItems || 0;

        state.items = state.items.concat(fetchedItems);
        badge.innerText = state.totalItems;

        state.hasMore = state.items.length < state.totalItems && fetchedItems.length > 0;
        if (state.hasMore) {
            state.page += 1;
        }

        renderPagedView(type, fetchedItems, isReset);

    } catch (err) {
        if (isReset) {
            container.innerHTML = `<p class="text-center text-rose-500 text-xs py-6">Lỗi tải danh mục dữ liệu.</p>`;
            badge.innerText = "0";
        }
    } finally {
        state.loading = false;
        if (loader) loader.classList.add('hidden');
        if (state.hasMore) {
            setTimeout(handleWindowScroll, 100);
        }
    }
}

function renderPagedView(type, itemsToRender, isReset) {
    const containerId = type === 'borrowed' ? 'container-borrowed-items' : 'container-history-items';
    const container = document.getElementById(containerId);

    if (isReset) container.innerHTML = '';

    if (appState[type].items.length === 0) {
        if (type === 'borrowed') {
            container.innerHTML = `
                                                <div class="bg-white rounded-2xl border border-slate-200 p-10 text-center max-w-md mx-auto shadow-xs my-6">
                                                    <div class="w-14 h-14 rounded-full bg-slate-100 flex items-center justify-center mx-auto text-slate-400 mb-4">
                                                        <i class="fa-solid fa-book-bookmark text-xl"></i>
                                                    </div>
                                                    <h3 class="text-base font-bold text-slate-800 mb-1">Không có tài liệu hay yêu cầu nào</h3>
                                                    <p class="text-xs text-slate-500 max-w-sm mx-auto mb-6 leading-relaxed">
                                                        Bạn chưa mượn hoặc chưa có yêu cầu mượn/đặt trước nào đang xử lý.
                                                    </p>
                                                </div>`;
        } else {
            container.innerHTML = `
                                                <div class="bg-white rounded-2xl border border-slate-200 p-10 text-center max-w-md mx-auto shadow-xs my-6">
                                                    <div class="w-14 h-14 rounded-full bg-slate-100 flex items-center justify-center mx-auto text-slate-400 mb-4">
                                                        <i class="fa-solid fa-clock-rotate-left text-xl"></i>
                                                    </div>
                                                    <h3 class="text-base font-bold text-slate-800 mb-1">Chưa có lịch sử giao dịch</h3>
                                                    <p class="text-xs text-slate-500 max-w-sm mx-auto mb-2 leading-relaxed">
                                                        Nơi này sẽ lưu lại các đơn yêu cầu đã hoàn tất, đơn đã hủy hoặc tài liệu bạn đã trả trong quá khứ.
                                                    </p>
                                                </div>`;
        }
        return;
    }

    let html = '';
    itemsToRender.forEach(item => {
        let statusBadge = getStatusBadge(item.borrowItemStatus);
        let infoHtml = '';
        let actionButtons = '';

        const itemKey = item.loanItemId && item.loanItemId !== '00000000-0000-0000-0000-000000000000'
            ? item.loanItemId
            : item.borrowItemId;

        if (item.daysLeft !== null && item.daysLeft !== undefined && item.borrowDate) {
            infoHtml = `
                                                <span>Ngày mượn: ${formatToVietnamTime(item.borrowDate)}</span>
                                                <span>Hạn trả: ${formatToVietnamTime(item.dueDate)}</span>
                                                <span class="font-semibold ${item.daysLeft < 0 ? 'text-rose-600' : (item.daysLeft <= 3 ? 'text-amber-600' : 'text-emerald-600')}">
                                                    ${item.daysLeft < 0 ? `Quá hạn ${Math.abs(item.daysLeft)} ngày` : `Còn ${item.daysLeft} ngày`}
                                                </span>
                                                <span>Gia hạn: ${item.renewCount || 0} lần</span>`;

            if (item.borrowItemStatus.toUpperCase() !== 'OVERDUE' && item.daysLeft >= 0 && item.daysLeft <= 3) {
                actionButtons += `
                                                    <button type="button" onclick="confirmRenewLoanItem('${item.loanItemId}')" ${(item.renewCount >= 1) ? 'disabled' : ''}
                                                            class="px-3 py-1.5 rounded-lg border text-xs font-semibold flex items-center gap-1.5 transition-colors ${item.renewCount >= 1 ? 'border-slate-200 text-slate-400 bg-slate-50 cursor-not-allowed' : 'border-slate-300 text-slate-700 hover:bg-slate-50 cursor-pointer'}">
                                                        <i class="fa-solid fa-rotate-right"></i> ${item.renewCount >= 1 ? 'Đã gia hạn' : 'Gia hạn'}
                                                    </button>`;
            }
        } else {
            infoHtml = `
                                                <span>Yêu cầu lúc: ${formatToVietnamTime(item.requestDate)}</span>
                                                <span>Phương thức: ${item.method || 'N/A'}</span>
                                                ${item.requestCode ? `<span class="font-mono bg-slate-100 px-1.5 py-0.5 rounded text-slate-700">Mã: ${item.requestCode}</span>` : ''}`;

            if (item.canCancel) {
                actionButtons += `
                                                    <button type="button" onclick="confirmCancelRequest('${item.borrowItemId}')"
                                                            class="px-3 py-1.5 rounded-lg border border-rose-200 text-rose-600 hover:bg-rose-50 text-xs font-semibold flex items-center gap-1.5 transition-colors cursor-pointer">
                                                        <i class="fa-regular fa-circle-xmark"></i> Hủy yêu cầu
                                                    </button>`;
            }
        }

        actionButtons += `
                                            <button type="button" onclick="openDetailModal('${itemKey}')"
                                               class="px-3 py-1.5 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-50 text-xs font-semibold flex items-center gap-1.5 transition-colors cursor-pointer">
                                                <i class="fa-regular fa-eye"></i> Chi tiết
                                            </button>`;

        html += `
                                            <div id="item-card-${itemKey}" class="bg-white rounded-xl border border-slate-200 p-4 sm:p-5 flex flex-col sm:flex-row sm:items-center justify-between gap-4 shadow-xs">
                                                <div class="flex items-start gap-4">
                                                    <img src="${item.coverUrl}" alt="${item.bookTitle}" class="w-20 h-28 object-cover rounded-lg shadow-xs flex-shrink-0 bg-slate-100" />
                                                    <div class="space-y-1.5">
                                                        <div class="flex flex-wrap items-center gap-2">
                                                            <h3 class="text-sm font-bold text-slate-900">${item.bookTitle}</h3>
                                                            ${statusBadge}
                                                        </div>
                                                        <p class="text-xs text-slate-500 font-medium">${item.author}</p>
                                                        <div class="flex flex-wrap items-center gap-4 text-xs text-slate-500 pt-1">
                                                            ${infoHtml}
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="flex items-center gap-2 self-end sm:self-center">
                                                    ${actionButtons}
                                                </div>
                                            </div>`;
    });

    if (isReset) {
        container.innerHTML = html;
    } else {
        container.insertAdjacentHTML('beforeend', html);
    }
}

function confirmCancelRequest(borrowItemId) {
    const item = appState.borrowed.items.find(x => x.borrowItemId === borrowItemId);
    if (!item) return;

    selectedCancelId = borrowItemId;
    document.getElementById('modal-book-title').innerText = `Bạn có chắc chắn muốn hủy yêu cầu mượn "${item.bookTitle}" không?`;

    const modal = document.getElementById('cancel-confirm-modal');
    const box = document.getElementById('modal-content-box');
    modal.classList.remove('hidden');
    setTimeout(() => {
        box.classList.remove('scale-95', 'opacity-0');
        box.classList.add('scale-100', 'opacity-100');
    }, 10);
}

function closeCancelModal() {
    const modal = document.getElementById('cancel-confirm-modal');
    const box = document.getElementById('modal-content-box');
    box.classList.remove('scale-100', 'opacity-100');
    box.classList.add('scale-95', 'opacity-0');
    setTimeout(() => {
        modal.classList.add('hidden');
        selectedCancelId = null;
    }, 200);
}

async function cancelBorrowRequest(borrowItemId) {
    const borrowedState = appState.borrowed;
    const itemIndex = borrowedState.items.findIndex(
        item => String(item.borrowItemId) === String(borrowItemId)
    );

    if (itemIndex < 0) {
        showToast("Không tìm thấy yêu cầu cần hủy.");
        return;
    }

    const confirmBtn = document.getElementById('btn-confirm-cancel-action');
    if (confirmBtn) confirmBtn.disabled = true;

    try {
        const response = await apiClient.private.delete(
            API.private.borrowRequest.cancleTransaction(borrowItemId)
        );

        if (!response.ok) {
            showToast("Không thể hủy yêu cầu này.");
            return;
        }

        borrowedState.items.splice(itemIndex, 1);
        borrowedState.totalItems = Math.max(0, borrowedState.totalItems - 1);

        const borrowedContainer = document.getElementById('container-borrowed-items');
        const borrowedCard = borrowedContainer?.querySelector(`#item-card-${borrowItemId}`);
        if (borrowedCard) borrowedCard.remove();

        if (borrowedState.items.length === 0) {
            renderPagedView('borrowed', borrowedState.items, true);
        }

        const borrowedBadge = document.getElementById('badge-count-borrowed');
        if (borrowedBadge) borrowedBadge.innerText = borrowedState.totalItems;

        await loadNextPage('history', true);

        showToast("Đã hủy yêu cầu thành công.");
    } catch (err) {
        console.error("Lỗi khi hủy yêu cầu:", err);
        showToast("Đã xảy ra lỗi kết nối.");
    } finally {
        if (confirmBtn) confirmBtn.disabled = false;
    }
}


function getStatusBadge(status) {
    const s = status ? status.toUpperCase() : '';
    switch (s) {
        case 'PENDING':
            return `<span class="bg-amber-50 text-amber-700 border border-amber-200/60 text-[10px] font-semibold px-2.5 py-0.5 rounded-full">Chờ duyệt</span>`;
        case 'APPROVED':
        case 'READY_FOR_PICKUP':
            return `<span class="bg-sky-50 text-sky-700 border border-sky-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đã duyệt / Sẵn sàng</span>`;
        case 'DELIVERING':
            return `<span class="bg-indigo-50 text-indigo-700 border border-indigo-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đang giao</span>`;
        case 'ACTIVE':
            return `<span class="bg-emerald-50 text-emerald-700 border border-emerald-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đang mượn</span>`;
        case 'DUE_SOON':
            return `<span class="bg-orange-50 text-orange-700 border border-orange-200/60 text-[10px] font-semibold px-2.5 py-0.5 rounded-full">Sắp đến hạn</span>`;
        case 'OVERDUE':
            return `<span class="bg-red-50 text-red-700 border border-red-200/60 text-[10px] font-semibold px-2.5 py-0.5 rounded-full">Quá hạn</span>`;
        case 'COMPLETED':
            return `<span class="bg-teal-50 text-teal-700 border border-teal-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đã hoàn tất</span>`;
        case 'RETURNED':
            return `<span class="bg-slate-50 text-slate-500 border border-slate-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đã hoàn trả</span>`;
        case 'CANCELLED':
            return `<span class="bg-slate-100/70 text-slate-500 border border-slate-200 text-[10px] font-semibold px-2 py-0.5 rounded-full">Đã hủy</span>`;
        case 'REJECTED':
            return `<span class="bg-rose-50 text-rose-700 border border-rose-200/60 text-[10px] font-semibold px-2.5 py-0.5 rounded-full">Bị từ chối</span>`;
        case 'EXPIRED':
            return `<span class="bg-fuchsia-50 text-fuchsia-700 border border-fuchsia-200/60 text-[10px] font-semibold px-2.5 py-0.5 rounded-full">Đã hết hạn</span>`;
        case 'LOST':
            return `<span class="bg-stone-100 text-red-800 border border-red-300 text-[10px] font-bold px-2 py-0.5 rounded-full">Làm mất / Đã đền</span>`;
        default:
            return `<span class="bg-slate-50 text-slate-600 border border-slate-200/60 text-[10px] font-semibold px-2 py-0.5 rounded-full">${status || ''}</span>`;
    }
}

function formatToVietnamTime(dateString) {
    if (!dateString) return 'N/A';
    try {
        return new Date(dateString).toLocaleString('vi-VN', {
            timeZone: 'Asia/Ho_Chi_Minh',
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        });
    } catch (e) {
        return dateString;
    }
}

function openDetailModal(itemKey) {
    const item = [...appState.borrowed.items, ...appState.history.items]
        .find(x => x.loanItemId === itemKey || x.borrowItemId === itemKey);
    if (!item) return;

    const safeSetText = (id, text) => {
        const el = document.getElementById(id);
        if (el) el.innerText = text;
    };

    const safeSetHtml = (id, html) => {
        const el = document.getElementById(id);
        if (el) el.innerHTML = html;
    };

    const coverEl = document.getElementById('det-cover');
    if (coverEl) coverEl.src = item.coverUrl || '/images/default-cover.jpg';

    safeSetText('det-title', item.bookTitle || 'Không rõ');
    safeSetText('det-author', item.author || 'Chưa rõ tác giả');
    safeSetHtml('det-status-badge', getStatusBadge(item.borrowItemStatus));

    safeSetText('det-request-code', item.requestCode || 'Không có mã đơn');
    safeSetText('det-method', item.method || 'N/A');
    safeSetText('det-req-status', item.borrowItemStatus || 'N/A');
    safeSetText('det-carrier-code', item.carrierTrackingCode && item.carrierTrackingCode.trim() !== '' ? item.carrierTrackingCode : 'Chưa có mã vận đơn');
    safeSetText('det-borrow-date', formatToVietnamTime(item.borrowDate));
    safeSetText('det-return-date', formatToVietnamTime(item.returnedAt));
    safeSetText('det-due-date', formatToVietnamTime(item.dueDate));
    safeSetText('det-renew-count', `${item.renewCount || 0} lần (Tối đa 1 lần)`);
    safeSetText('det-request-date', formatToVietnamTime(item.requestDate));

    const boxBorrowDate = document.getElementById('box-borrow-date-detail');
    const boxDueDate = document.getElementById('box-due-date-detail');
    const boxDaysLeft = document.getElementById('box-days-left-detail');
    const boxRenewCount = document.getElementById('box-renew-count-detail');
    const boxRequestDate = document.getElementById('box-request-date');
    const boxReturnAt = document.getElementById('box-due-return-detail');

    if (item.borrowDate) {
        if (boxBorrowDate) boxBorrowDate.style.display = 'block';
        if (boxDueDate) boxDueDate.style.display = 'block';
        if (boxRequestDate) boxRequestDate.style.display = 'block';
        if (boxReturnAt) boxReturnAt.style.display = item.returnedAt ? 'block' : 'none';

        if (item.daysLeft !== null && item.daysLeft !== undefined) {
            if (boxDaysLeft) boxDaysLeft.style.display = 'block';
            if (boxRenewCount) boxRenewCount.style.display = 'block';

            const daysLeftEl = document.getElementById('det-days-left');
            if (daysLeftEl) {
                if (item.daysLeft < 0) {
                    daysLeftEl.className = "font-semibold text-rose-600";
                    daysLeftEl.innerText = `Quá hạn ${Math.abs(item.daysLeft)} ngày`;
                } else if (item.daysLeft <= 3) {
                    daysLeftEl.className = "font-semibold text-amber-600";
                    daysLeftEl.innerText = `Còn ${item.daysLeft} ngày (Sắp đến hạn)`;
                } else {
                    daysLeftEl.className = "font-semibold text-emerald-600";
                    daysLeftEl.innerText = `Còn ${item.daysLeft} ngày`;
                }
            }
        } else {
            if (boxDaysLeft) boxDaysLeft.style.display = 'none';
            if (boxRenewCount) boxRenewCount.style.display = 'none';
        }
    } else {
        if (boxBorrowDate) boxBorrowDate.style.display = 'none';
        if (boxDueDate) boxDueDate.style.display = 'none';
        if (boxDaysLeft) boxDaysLeft.style.display = 'none';
        if (boxRenewCount) boxRenewCount.style.display = 'none';
        if (boxReturnAt) boxReturnAt.style.display = 'none';
        if (boxRequestDate) boxRequestDate.style.display = 'block';
    }

    const modal = document.getElementById('detail-info-modal');
    const box = document.getElementById('detail-modal-box');
    if (modal && box) {
        modal.classList.remove('hidden');
        setTimeout(() => {
            box.classList.remove('scale-95', 'opacity-0');
            box.classList.add('scale-100', 'opacity-100');
        }, 10);
    }
}

function closeDetailModal() {
    const modal = document.getElementById('detail-info-modal');
    const box = document.getElementById('detail-modal-box');
    if (modal && box) {
        box.classList.remove('scale-100', 'opacity-100');
        box.classList.add('scale-95', 'opacity-0');
        setTimeout(() => {
            modal.classList.add('hidden');
        }, 200);
    }
}

function confirmRenewLoanItem(loanItemId) {
    const item = appState.borrowed.items.find(x => x.loanItemId == loanItemId);
    selectedRenewLoanItemId = loanItemId;
    if (item) {
        document.getElementById('modal-renew-book-title').innerText = `Bạn có chắc chắn muốn gia hạn thêm 7 ngày cho "${item.bookTitle}" không?`;
    }

    const modal = document.getElementById('renew-confirm-modal');
    const box = document.getElementById('renew-modal-content-box');
    modal.classList.remove('hidden');
    setTimeout(() => {
        box.classList.remove('scale-95', 'opacity-0');
        box.classList.add('scale-100', 'opacity-100');
    }, 10);
}

function closeRenewModal() {
    const modal = document.getElementById('renew-confirm-modal');
    const box = document.getElementById('renew-modal-content-box');
    box.classList.remove('scale-100', 'opacity-100');
    box.classList.add('scale-95', 'opacity-0');
    setTimeout(() => {
        modal.classList.add('hidden');
        selectedRenewLoanItemId = null;
    }, 200);
}

async function renewLoanItem() {
    if (!selectedRenewLoanItemId) {
        showToast("Không tìm thấy thông tin mã mượn tài liệu.");
        return;
    }

    closeRenewModal();
    try {
        const response = await apiClient.private.post(API.private.loanRenewal.renew(selectedRenewLoanItemId));
        if (response.ok) {
            showToast("Gia hạn thành công!");
            closeDetailModal();
            fetchAllData();
        } else {
            const result = await response.json();
            showToast(result.message || "Gia hạn thất bại.");
        }
    } catch (error) {
        showToast("Đã xảy ra lỗi kết nối đến máy chủ.");
    }
}
