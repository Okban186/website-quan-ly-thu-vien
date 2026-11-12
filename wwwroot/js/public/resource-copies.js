document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('resourceCopies');

    if (!container) return;

    const resourceId = container.dataset.resourceId;

    if (!resourceId) return;

    let currentPage = 1;
    let isShellInitialized = false;

    // Chuyển mã trạng thái từ API thành nhãn và màu hiển thị.
    const statusMap = {
        AVAILABLE: {
            label: 'Sẵn sàng',
            className: 'bg-emerald-50 text-emerald-700 border-emerald-200'
        },
        RESERVED: {
            label: 'Đang mượn',
            className: 'bg-amber-50 text-amber-700 border-amber-200'
        },
        BORROWED: {
            label: 'Đang mượn',
            className: 'bg-amber-50 text-amber-700 border-amber-200'
        },
        RETURNED_PENDING_CHECK: {
            label: 'Đang xử lý',
            className: 'bg-blue-50 text-blue-700 border-blue-200'
        },
        LOST: {
            label: 'Đang xử lý',
            className: 'bg-blue-50 text-blue-700 border-blue-200'
        },
        DAMAGED: {
            label: 'Đang xử lý',
            className: 'bg-blue-50 text-blue-700 border-blue-200'
        },
        MAINTENANCE: {
            label: 'Đang xử lý',
            className: 'bg-blue-50 text-blue-700 border-blue-200'
        },

        0: { label: 'Sẵn sàng', className: 'bg-emerald-50 text-emerald-700 border-emerald-200' },
        1: { label: 'Đang mượn', className: 'bg-amber-50 text-amber-700 border-amber-200' },
        2: { label: 'Đang mượn', className: 'bg-amber-50 text-amber-700 border-amber-200' },
        3: { label: 'Đang xử lý', className: 'bg-blue-50 text-blue-700 border-blue-200' },
        4: { label: 'Đang xử lý', className: 'bg-blue-50 text-blue-700 border-blue-200' },
        5: { label: 'Đang xử lý', className: 'bg-blue-50 text-blue-700 border-blue-200' },
        6: { label: 'Đang xử lý', className: 'bg-blue-50 text-blue-700 border-blue-200' }
    };

    function escapeHtml(value) {
        return String(value ?? '').replace(/[&<>"']/g, char => ({
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        })[char]);
    }

    function getStatus(status) {
        return statusMap[status] ?? {
            label: 'Không xác định',
            className: 'bg-slate-50 text-slate-600 border-slate-200'
        };
    }

    function initCopiesShell() {
        container.classList.remove('hidden');
        container.innerHTML = `
            <section class="bg-white rounded-xl border border-slate-200 p-5 space-y-4 shadow-sm">
                <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2 border-b border-slate-100 pb-3">
                    <div>
                        <h2 class="text-base font-bold text-slate-900">
                            Thông tin tài liệu vật lý
                        </h2>
                        <p class="text-xs text-slate-500 mt-0.5">
                            Danh sách từng bản vật lý của tài liệu
                        </p>
                    </div>

                    <div class="text-xs text-slate-500 font-medium">
                        Tổng số bản:
                        <span id="totalItemsCount" class="font-bold text-slate-900">0</span>
                    </div>
                </div>

                <!-- Thống kê -->
                <div class="bg-slate-50 border border-slate-200/80 rounded-lg p-3 flex flex-wrap items-center gap-x-6 gap-y-2 text-xs">
                    <div class="flex items-center gap-1.5 font-medium text-slate-600">
                        <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
                        Sẵn sàng:
                        <span id="statAvailable" class="font-bold text-emerald-700">0</span>
                    </div>

                    <div class="flex items-center gap-1.5 font-medium text-slate-600">
                        <span class="w-2 h-2 rounded-full bg-amber-500"></span>
                        Đang mượn:
                        <span id="statBorrowing" class="font-bold text-amber-700">0</span>
                    </div>

                    <div class="flex items-center gap-1.5 font-medium text-slate-600">
                        <span class="w-2 h-2 rounded-full bg-blue-500"></span>
                        Đang xử lý:
                        <span id="statProcessing" class="font-bold text-blue-700">0</span>
                    </div>
                </div>

                <!-- Bảng bản sao -->
                <div class="overflow-x-auto border border-slate-200 rounded-lg">
                    <table class="w-full text-left text-xs text-slate-700">
                        <thead class="bg-slate-100/80 border-b border-slate-200 font-bold text-slate-800 uppercase tracking-wider text-[11px]">
                            <tr>
                                <th class="px-4 py-2.5">Mã tài liệu</th>
                                <th class="px-4 py-2.5">Tình trạng</th>
                                <th class="px-4 py-2.5">Vị trí</th>
                            </tr>
                        </thead>

                        <tbody id="copiesTableBody" class="divide-y divide-slate-200 bg-white">
                            <tr>
                                <td colspan="3" class="px-4 py-6 text-center text-slate-400">Đang tải dữ liệu...</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <!-- Phân trang -->
                <div class="flex flex-col sm:flex-row items-center justify-between gap-3 text-xs">
                    <span id="copiesPageInfo" class="text-slate-500"></span>

                    <div class="flex items-center gap-2">
                        <button id="previousPage"
                                class="px-3 py-1.5 rounded-md border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer">
                            Trang trước
                        </button>

                        <span id="currentPageInfo" class="font-semibold text-slate-700"></span>

                        <button id="nextPage"
                                class="px-3 py-1.5 rounded-md border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer">
                            Trang sau
                        </button>
                    </div>
                </div>
            </section>
        `;

        document.getElementById('previousPage').addEventListener('click', () => {
            if (currentPage > 1) loadCopies(currentPage - 1);
        });

        document.getElementById('nextPage').addEventListener('click', () => {
            loadCopies(currentPage + 1);
        });

        isShellInitialized = true;
    }

    async function loadCopies(page = 1) {
        try {

            if (!isShellInitialized) {
                initCopiesShell();
            } else {

                const tbody = document.getElementById('copiesTableBody');
                if (tbody) {
                    tbody.innerHTML = `<tr><td colspan="3" class="px-4 py-6 text-center text-slate-400">Đang tải...</td></tr>`;
                }
            }

            const url = API.public.resources.copies(resourceId);

            const response = await apiClient.public.get(url, {
                params: { page }
            });

            if (!response.ok) {
                throw new Error('Không thể tải danh sách bản sao.');
            }

            const result = await response.json();

            currentPage = result.copies.page ?? page;
            renderCopiesData(result);

        } catch (error) {
            console.error('Load resource copies error:', error);

            const tbody = document.getElementById('copiesTableBody');
            if (tbody) {
                tbody.innerHTML = `
                    <tr>
                        <td colspan="3" class="px-4 py-6 text-center text-red-600">
                            Không thể tải thông tin bản sao. 
                            <button id="retryCopies" class="ml-2 font-semibold underline cursor-pointer">Thử lại</button>
                        </td>
                    </tr>
                `;
                document.getElementById('retryCopies')?.addEventListener('click', () => loadCopies(currentPage));
            }
        }
    }

    function renderCopiesData(result) {
        const copies = result.copies;
        const items = copies.items ?? [];

        document.getElementById('totalItemsCount').textContent = copies.totalItems ?? 0;
        document.getElementById('statAvailable').textContent = result.availableCopies ?? 0;
        document.getElementById('statBorrowing').textContent = result.borrowingCopies ?? 0;
        document.getElementById('statProcessing').textContent = result.processingCopies ?? 0;


        const tbody = document.getElementById('copiesTableBody');
        if (items.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="px-4 py-6 text-center text-slate-500">
                        Chưa có bản sao nào.
                    </td>
                </tr>
            `;
        } else {
            tbody.innerHTML = items.map(item => {
                const status = getStatus(item.status);

                return `
                    <tr class="hover:bg-slate-50 transition-colors">
                        <td class="px-4 py-2.5 font-mono font-bold text-slate-900">
                            ${escapeHtml(item.barcode)}
                        </td>

                        <td class="px-4 py-2.5">
                            <span class="px-2.5 py-0.5 rounded-full text-[11px] font-semibold border ${status.className}">
                                ${status.label}
                            </span>
                        </td>

                        <td class="px-4 py-2.5 text-slate-600 font-medium">
                            ${escapeHtml(item.locationName ?? 'Chưa xác định')}
                        </td>
                    </tr>
                `;
            }).join('');
        }


        updatePagination(copies);
    }

    function updatePagination(copies) {
        const p = copies.page ?? 1;
        const pageSize = copies.pageSize ?? 5;
        const totalItems = copies.totalItems ?? 0;
        const totalPages = copies.totalPages ?? 1;

        const start = totalItems === 0 ? 0 : (p - 1) * pageSize + 1;
        const end = Math.min(p * pageSize, totalItems);

        document.getElementById('copiesPageInfo').textContent =
            `Hiển thị ${start} - ${end} / ${totalItems} bản sao`;

        document.getElementById('currentPageInfo').textContent =
            `${p} / ${totalPages}`;

        document.getElementById('previousPage').disabled = p <= 1;
        document.getElementById('nextPage').disabled = p >= totalPages;
    }


    loadCopies(1);
});