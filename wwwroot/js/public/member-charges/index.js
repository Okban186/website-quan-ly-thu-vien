
const chargeState = {
    // Tách riêng state và mảng dữ liệu cho 2 trạng thái
    tabs: {
        'UNPAID': { items: [], page: 1, hasMore: true },
        'PAID': { items: [], page: 1, hasMore: true }
    },
    summary: { totalUnpaid: 0, unpaidCount: 0, totalPaid: 0, paidCount: 0 },
    activeTab: 'UNPAID', // Mặc định 'UNPAID'
    pageSize: 10,
    isLoading: false,
    selectedIds: new Set(),
    countdownTimer: null,
    secondsLeft: 900
};

function formatToVietnamTime(utcDateString) {
    if (!utcDateString) return 'N/A';
    const normalizedStr = utcDateString.endsWith('Z') || utcDateString.includes('+')
        ? utcDateString
        : utcDateString + 'Z';
    try {
        return new Date(normalizedStr).toLocaleString('vi-VN', {
            timeZone: 'Asia/Ho_Chi_Minh',
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        });
    } catch {
        return utcDateString;
    }
}

function formatCurrency(amount) {
    return Number(amount || 0).toLocaleString('vi-VN') + 'đ';
}

function isUnpaidStatus(status) {
    return (status || '').toUpperCase() === 'UNPAID';
}

// Tải dữ liệu phân trang cho tab hiện tại (hỗ trợ Infinite Scroll)
async function loadCharges(reset = false) {
    const currentTabState = chargeState.tabs[chargeState.activeTab];
    if (chargeState.isLoading) return;
    if (!reset && !currentTabState.hasMore) return;

    if (reset) {
        currentTabState.page = 1;
        currentTabState.items = []; // Reset mảng riêng của tab đó
        currentTabState.hasMore = true;
        if (chargeState.activeTab === 'UNPAID') chargeState.selectedIds.clear();
        document.getElementById('charges-list-container').innerHTML = '';
        document.getElementById('end-of-list-msg').classList.add('hidden');
        document.getElementById('empty-state').classList.add('hidden');
    }

    chargeState.isLoading = true;
    document.getElementById('loading-spinner').classList.remove('hidden');

    try {
        const params = new URLSearchParams({
            status: chargeState.activeTab,
            page: currentTabState.page,
            pageSize: chargeState.pageSize
        });

        const response = await fetch(`/api/charges/mine?${params.toString()}`, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) throw new Error('Không thể tải danh sách phí phạt');

        const data = await response.json();

        if (data.summary) {
            chargeState.summary = data.summary;
            updateSummaryUI();
        }

        const paged = data.charges || {};
        const newItems = paged.items || paged.data || [];
        const totalCount = paged.totalCount ?? paged.totalItems ?? 0;

        // Nối dồn vào mảng riêng của tab hiện tại (Infinite Scroll)
        currentTabState.items.push(...newItems);

        if (chargeState.activeTab === 'UNPAID') {
            newItems.forEach(item => {
                if (isUnpaidStatus(item.status)) {
                    chargeState.selectedIds.add(item.id);
                }
            });
        }

        renderNewItems(newItems);

        if (newItems.length < chargeState.pageSize || (totalCount > 0 && currentTabState.items.length >= totalCount)) {
            currentTabState.hasMore = false;
            if (currentTabState.items.length > 0) {
                document.getElementById('end-of-list-msg').classList.remove('hidden');
            }
        } else {
            currentTabState.page += 1;
        }

        if (currentTabState.items.length === 0) {
            document.getElementById('empty-state').classList.remove('hidden');
        }

        updateSelectionToolbar();
    } catch (err) {
        console.error(err);
    } finally {
        chargeState.isLoading = false;
        document.getElementById('loading-spinner').classList.add('hidden');
    }
}

function updateSummaryUI() {
    const s = chargeState.summary;
    const totalUnpaid = s.totalUnpaid || 0;
    const unpaidCount = s.unpaidCount || 0;
    const totalPaid = s.totalPaid || 0;
    const paidCount = s.paidCount || 0;

    document.getElementById('stat-total-unpaid').innerText = formatCurrency(totalUnpaid);
    document.getElementById('stat-unpaid-count').innerText = `(${unpaidCount} khoản)`;
    document.getElementById('stat-total-paid').innerText = formatCurrency(totalPaid);
    document.getElementById('stat-paid-count').innerText = `(${paidCount} khoản)`;

    document.getElementById('badge-unpaid-count').innerText = unpaidCount;
    document.getElementById('badge-paid-count').innerText = paidCount;

    const banner = document.getElementById('unpaid-warning-banner');
    if (totalUnpaid > 0) {
        banner.classList.remove('hidden');
        banner.classList.add('flex');
        document.getElementById('banner-unpaid-count').innerText = unpaidCount;
        document.getElementById('banner-total-unpaid').innerText = formatCurrency(totalUnpaid);
    } else {
        banner.classList.add('hidden');
        banner.classList.remove('flex');
    }
}

function renderNewItems(items) {
    const container = document.getElementById('charges-list-container');
    const fragment = document.createDocumentFragment();

    items.forEach(item => {
        const unpaid = isUnpaidStatus(item.status);
        const checked = unpaid && chargeState.selectedIds.has(item.id);
        const title = item.resourceTitle || item.chargeTypeName || 'Khoản phí thư viện';
        const cover = item.coverUrl || '/images/default-cover.jpg';

        const card = document.createElement('div');
        card.id = `charge-card-${item.id}`;
        card.className = `bg-white rounded-xl border p-4 sm:p-5 flex flex-col sm:flex-row sm:items-center justify-between gap-4 shadow-xs transition-colors ${checked ? 'border-[#1e3a5f]/40 bg-blue-50/15' : 'border-slate-200'
            }`;

        card.innerHTML = `
                <div class="flex items-start gap-3.5 sm:gap-4 flex-1">
                    ${unpaid ? `
                        <div class="pt-2 sm:pt-1 flex items-center flex-shrink-0">
                            <input type="checkbox"
                                   id="checkbox-charge-${item.id}"
                                   ${checked ? 'checked' : ''}
                                   onchange="handleToggleSelect('${item.id}', this.checked)"
                                   class="w-4 h-4 rounded border-slate-300 text-[#1e3a5f] focus:ring-[#1e3a5f] cursor-pointer accent-[#1e3a5f]" />
                        </div>
                    ` : ''}

                    <img src="${cover}" alt="${title}" onerror="this.src='/images/default-cover.jpg'"
                         class="w-14 h-20 object-cover rounded-lg shadow-xs bg-slate-100 flex-shrink-0" />

                    <div class="space-y-1.5 flex-1 min-w-0">
                        <div class="flex flex-wrap items-center gap-2">
                            <h3 class="text-sm font-bold text-slate-900">${title}</h3>
                            <span class="text-xs font-bold text-rose-600 bg-rose-50 border border-rose-200 px-2 py-0.5 rounded-full">
                                ${formatCurrency(item.amount)}
                            </span>
                            <span class="text-[10px] font-semibold text-slate-500 bg-slate-100 px-2 py-0.5 rounded">
                                ${item.chargeTypeName || item.chargeTypeCode}
                            </span>
                        </div>

                        <p class="text-xs text-slate-600 leading-relaxed max-w-xl">
                            ${item.description || 'Phí quy định theo chính sách thư viện.'}
                        </p>

                        <div class="text-[11px] text-slate-400 pt-1">
                            ${unpaid
                ? `<span>Ngày ghi nhận: ${formatToVietnamTime(item.createdAt)}</span>`
                : `<span class="text-emerald-600 font-semibold">Đã thanh toán vào ${formatToVietnamTime(item.paidAt || item.createdAt)}</span>`
            }
                        </div>
                    </div>
                </div>

                <div class="self-end sm:self-center flex-shrink-0">
                    ${unpaid
                ? `<button type="button" onclick="openPaySingle('${item.id}')" class="bg-rose-600 hover:bg-rose-700 text-white px-4 py-2 rounded-lg text-xs font-semibold shadow-xs transition-colors cursor-pointer">
                               Thanh toán
                           </button>`
                : `<span class="inline-flex items-center gap-1.5 text-xs font-semibold text-emerald-600 bg-emerald-50 border border-emerald-200 px-3 py-1.5 rounded-lg">
                               <i class="fa-regular fa-circle-check"></i>
                               <span>Đã thanh toán</span>
                           </span>`
            }
                </div>
            `;

        fragment.appendChild(card);
    });

    container.appendChild(fragment);
}

// Chuyển tab: Kiểm tra mảng, nếu có rồi thì render cache, nếu rỗng mới gọi API
function switchTab(tab) {
    if (chargeState.activeTab === tab) return;
    chargeState.activeTab = tab;

    const tabs = {
        'UNPAID': document.getElementById('tab-unpaid'),
        'PAID': document.getElementById('tab-paid')
    };

    Object.entries(tabs).forEach(([key, el]) => {
        if (key === tab) {
            el.classList.add('border-[#1e3a5f]', 'text-[#1e3a5f]');
            el.classList.remove('border-transparent', 'text-slate-500');
        } else {
            el.classList.remove('border-[#1e3a5f]', 'text-[#1e3a5f]');
            el.classList.add('border-transparent', 'text-slate-500');
        }
    });

    const currentTabState = chargeState.tabs[tab];

    // Kiểm tra xem mảng của tab này đã có dữ liệu chưa
    if (currentTabState.items.length > 0) {
        // Đã có -> Render lại từ mảng cache, không gọi API
        document.getElementById('charges-list-container').innerHTML = '';
        renderNewItems(currentTabState.items);
        document.getElementById('empty-state').classList.toggle('hidden', currentTabState.items.length > 0);
        document.getElementById('end-of-list-msg').classList.toggle('hidden', currentTabState.hasMore);
        updateSelectionToolbar();
    } else {
        // Mảng rỗng -> Gọi API tải mới
        loadCharges(true);
    }
}

function handleToggleSelect(id, isChecked) {
    if (isChecked) {
        chargeState.selectedIds.add(id);
    } else {
        chargeState.selectedIds.delete(id);
    }

    const card = document.getElementById(`charge-card-${id}`);
    if (card) {
        card.classList.toggle('border-[#1e3a5f]/40', isChecked);
        card.classList.toggle('bg-blue-50/15', isChecked);
        card.classList.toggle('border-slate-200', !isChecked);
    }

    updateSelectionToolbar();
}

function handleToggleSelectAll(isChecked) {
    const unpaidItems = chargeState.tabs['UNPAID'].items.filter(i => isUnpaidStatus(i.status));
    unpaidItems.forEach(item => {
        if (isChecked) {
            chargeState.selectedIds.add(item.id);
        } else {
            chargeState.selectedIds.delete(item.id);
        }
        const cb = document.getElementById(`checkbox-charge-${item.id}`);
        if (cb) cb.checked = isChecked;

        const card = document.getElementById(`charge-card-${item.id}`);
        if (card) {
            card.classList.toggle('border-[#1e3a5f]/40', isChecked);
            card.classList.toggle('bg-blue-50/15', isChecked);
            card.classList.toggle('border-slate-200', !isChecked);
        }
    });

    updateSelectionToolbar();
}

function updateSelectionToolbar() {
    const toolbar = document.getElementById('batch-toolbar');
    const unpaidItems = chargeState.tabs['UNPAID'].items.filter(i => isUnpaidStatus(i.status));

    if (unpaidItems.length === 0 || chargeState.activeTab === 'PAID') {
        toolbar.classList.add('hidden');
        toolbar.classList.remove('flex');
        return;
    }

    toolbar.classList.remove('hidden');
    toolbar.classList.add('flex');

    const selectedItems = unpaidItems.filter(i => chargeState.selectedIds.has(i.id));
    const selectedCount = selectedItems.length;
    const totalAmount = selectedItems.reduce((sum, i) => sum + Number(i.amount || 0), 0);

    document.getElementById('checkbox-select-all').checked = selectedCount > 0 && selectedCount === unpaidItems.length;
    document.getElementById('selected-count-label').innerText = selectedCount;
    document.getElementById('loaded-unpaid-count-label').innerText = unpaidItems.length;
    document.getElementById('selected-total-amount').innerText = formatCurrency(totalAmount);

    const btnPay = document.getElementById('btn-pay-selected');
    const badge = document.getElementById('btn-pay-selected-badge');

    if (selectedCount > 0) {
        btnPay.disabled = false;
        btnPay.className = 'px-4 py-2 rounded-lg text-xs font-semibold shadow-xs transition-colors flex items-center gap-2 bg-[#1e3a5f] hover:bg-[#142944] text-white cursor-pointer';
        badge.classList.remove('hidden');
        badge.innerText = selectedCount;
    } else {
        btnPay.disabled = true;
        btnPay.className = 'px-4 py-2 rounded-lg text-xs font-semibold shadow-xs transition-colors flex items-center gap-2 bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed';
        badge.classList.add('hidden');
    }
}

function openPaySingle(id) {
    const item = chargeState.tabs['UNPAID'].items.find(x => x.id === id);
    if (item) openPaymentModal([item]);
}

function openPaySelected() {
    const selectedItems = chargeState.tabs['UNPAID'].items.filter(i => isUnpaidStatus(i.status) && chargeState.selectedIds.has(i.id));
    if (selectedItems.length > 0) openPaymentModal(selectedItems);
}

function openPayAllLoadedUnpaid() {
    const unpaidItems = chargeState.tabs['UNPAID'].items.filter(i => isUnpaidStatus(i.status));
    if (unpaidItems.length > 0) openPaymentModal(unpaidItems);
}

function openPaymentModal(items) {
    const totalAmount = items.reduce((sum, i) => sum + Number(i.amount || 0), 0);

    document.getElementById('modal-subtitle').innerText = items.length === 1
        ? `Khoản phạt: ${items[0].resourceTitle || items[0].chargeTypeName}`
        : `Thanh toán ${items.length} khoản phạt đã chọn`;

    document.getElementById('modal-pay-amount').innerText = `${Number(totalAmount).toLocaleString('vi-VN')} ₫`;

    const transferCode = items.length === 1
        ? `PHAT TV ${items[0].id.substring(0, 6).toUpperCase()}`
        : `PHAT TV ${items.length}KHOAN`;
    document.getElementById('modal-transfer-content').innerText = transferCode;

    const multiBox = document.getElementById('modal-multi-items-box');
    const multiList = document.getElementById('modal-multi-list');
    if (items.length > 1) {
        multiBox.classList.remove('hidden');
        document.getElementById('modal-multi-count').innerText = items.length;
        multiList.innerHTML = items.map(f => `
                <div class="flex items-center justify-between text-[11px] pt-1 first:pt-0">
                    <span class="text-slate-700 truncate max-w-[220px]">• ${f.resourceTitle || f.chargeTypeName}</span>
                    <span class="font-bold text-slate-900 ml-2 whitespace-nowrap">${Number(f.amount).toLocaleString('vi-VN')} ₫</span>
                </div>
            `).join('');
    } else {
        multiBox.classList.add('hidden');
    }

    document.getElementById('qr-active-box').classList.remove('hidden');
    document.getElementById('qr-expired-box').classList.add('hidden');
    startCountdown(900);

    const modal = document.getElementById('payment-modal');
    const box = document.getElementById('payment-modal-box');
    modal.classList.remove('hidden');
    setTimeout(() => {
        box.classList.remove('scale-95', 'opacity-0');
        box.classList.add('scale-100', 'opacity-100');
    }, 10);
}

function closePaymentModal() {
    if (chargeState.countdownTimer) clearInterval(chargeState.countdownTimer);
    const modal = document.getElementById('payment-modal');
    const box = document.getElementById('payment-modal-box');
    box.classList.remove('scale-100', 'opacity-100');
    box.classList.add('scale-95', 'opacity-0');
    setTimeout(() => modal.classList.add('hidden'), 150);
}

// CHỈ KHI THANH TOÁN THÀNH CÔNG MỚI RESET VÀ RELOAD LẠI DỮ LIỆU CẢ 2 TAB
function simulatePaymentSuccess() {
    closePaymentModal();
    alert("Thanh toán thành công!");
    // Reset sạch cache của cả 2 mảng để tải lại trạng thái mới nhất từ server
    chargeState.tabs['UNPAID'].items = [];
    chargeState.tabs['PAID'].items = [];
    loadCharges(true);
}

function startCountdown(seconds) {
    if (chargeState.countdownTimer) clearInterval(chargeState.countdownTimer);
    chargeState.secondsLeft = seconds;
    const el = document.getElementById('modal-countdown');

    const updateDisplay = () => {
        const m = Math.floor(chargeState.secondsLeft / 60).toString().padStart(2, '0');
        const s = (chargeState.secondsLeft % 60).toString().padStart(2, '0');
        el.innerText = `${m}:${s}`;
    };

    el.className = 'font-mono text-sm font-bold tabular-nums px-2 py-0.5 rounded text-amber-700 bg-amber-50 border border-amber-200';
    updateDisplay();

    chargeState.countdownTimer = setInterval(() => {
        chargeState.secondsLeft -= 1;
        if (chargeState.secondsLeft <= 0) {
            chargeState.secondsLeft = 0;
            updateDisplay();
            clearInterval(chargeState.countdownTimer);
            el.className = 'font-mono text-sm font-bold tabular-nums px-2 py-0.5 rounded text-rose-700 bg-rose-50 border border-rose-200';
            document.getElementById('qr-active-box').classList.add('hidden');
            document.getElementById('qr-expired-box').classList.remove('hidden');
        } else {
            updateDisplay();
        }
    }, 1000);
}

function copyField(text, msgId) {
    navigator.clipboard?.writeText(text);
    const msg = document.getElementById(msgId);
    if (msg) {
        msg.classList.remove('hidden');
        setTimeout(() => msg.classList.add('hidden'), 2000);
    }
}

function copyTransferContent() {
    const text = document.getElementById('modal-transfer-content').innerText;
    copyField(text, 'copy-content-msg');
}

// Khởi tạo Intersection Observer cho Infinite Scroll
document.addEventListener('DOMContentLoaded', () => {
    const sentinel = document.getElementById('infinite-scroll-sentinel');
    const observer = new IntersectionObserver((entries) => {
        const currentTabState = chargeState.tabs[chargeState.activeTab];
        if (entries[0].isIntersecting && currentTabState.hasMore && !chargeState.isLoading) {
            loadCharges(false); // Cuộn xuống tự động tải thêm và nối tiếp vào mảng hiện tại
        }
    }, {
        rootMargin: '200px'
    });

    observer.observe(sentinel);
    loadCharges(true); // Tải lần đầu cho tab UNPAID
});
