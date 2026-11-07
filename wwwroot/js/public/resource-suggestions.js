document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('resourceSuggestions');

    if (!container) return;

    const resourceId = container.dataset.resourceId;

    if (!resourceId) return;

    loadSuggestions();

    async function loadSuggestions() {
        container.classList.remove('hidden');
        renderSkeleton();

        try {
            const response = await apiClient.public.get(
                API.public.resources.suggestions(resourceId)
            );

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();

            const resources = result.items || result.data || [];

            if (!Array.isArray(resources) || resources.length === 0) {
                container.classList.add('hidden');
                return;
            }

            renderSuggestions(resources);

        } catch (error) {
            console.error('Load suggestions error:', error);
            renderError();
        }
    }

    // Skeleton hiển thị trong thời gian API thực sự đang tải.
    function renderSkeleton() {
        container.innerHTML = `
            <div class="bg-white rounded-2xl p-6 border border-slate-200 shadow-sm space-y-6">

                <div class="space-y-2">
                    <div class="h-6 w-56 bg-slate-200 rounded animate-pulse"></div>
                    <div class="h-3 w-40 bg-slate-100 rounded animate-pulse"></div>
                </div>

                <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
                    ${Array.from({ length: 5 }, () => `
                        <div class="rounded-xl border border-slate-200 overflow-hidden">
                            <div class="aspect-[3/4] bg-slate-200 animate-pulse"></div>

                            <div class="p-3 space-y-3">
                                <div class="h-2 w-16 bg-slate-200 rounded animate-pulse"></div>
                                <div class="h-3 w-full bg-slate-200 rounded animate-pulse"></div>
                                <div class="h-3 w-3/4 bg-slate-100 rounded animate-pulse"></div>
                                <div class="h-3 w-full bg-slate-100 rounded animate-pulse"></div>
                            </div>
                        </div>
                    `).join('')}
                </div>

            </div>
        `;
    }

    function renderSuggestions(books) {
        container.innerHTML = `
            <section class="bg-white rounded-2xl p-6 border border-slate-200 shadow-sm space-y-6">

                <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
                    <div>
                        <div class="flex items-center">
                            <i class="fa-solid fa-sparkles text-amber-500"></i>

                            <h2 class="text-lg sm:text-xl font-bold text-slate-900">
                                Sách có thể bạn sẽ thích
                            </h2>
                        </div>

                        <p class="text-xs text-slate-500 mt-0.5">
                            Gợi ý sách cùng thể loại
                        </p>
                    </div>

                    <a href="/Resource/Search"
                       class="text-xs font-semibold text-[#1e3a5f] hover:underline flex items-center gap-1 self-start sm:self-auto">
                        <span>Khám phá thêm trong kho sách</span>
                        <i class="fa-solid fa-chevron-right text-[10px]"></i>
                    </a>
                </div>

                <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
                    ${books.map(renderBookCard).join('')}
                </div>

            </section>
        `;
    }

    function renderBookCard(book) {
        const id = encodeURIComponent(book.id);
        const title = escapeHtml(book.title);
        const categories = Array.isArray(book.categories)
            ? book.categories.map(escapeHtml).join(', ')
            : 'Chưa phân loại';
        const authors = escapeHtml(
            Array.isArray(book.authors) ? book.authors.join(', ') : ''
        );

        const hasCopies = book.availableCopies > 0;

        // Chỉ chấp nhận URL ảnh HTTP/HTTPS.
        const coverUrl = safeImageUrl(book.coverUrl);

        const cover = coverUrl
            ? `
                <img src="${escapeHtml(coverUrl)}"
                     alt="${title}"
                     loading="lazy"
                     class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                     onerror="this.onerror=null;this.src='/images/no-image.png'" />
            `
            : `
                <div class="w-full h-full flex items-center justify-center bg-slate-100 text-slate-400">
                    <i class="fa-solid fa-book-open text-4xl"></i>
                </div>
            `;

        return `
            <a href="/Resource/Detail/${id}"
               class="bg-white rounded-xl border border-slate-200 overflow-hidden hover:shadow-md hover:border-slate-300 transition-all cursor-pointer flex flex-col group h-full">

                <div class="aspect-[3/4] overflow-hidden relative bg-slate-100">

                    ${cover}

                    <span class="absolute top-2 right-2 text-[10px] font-bold px-2 py-0.5 rounded text-white ${hasCopies ? 'bg-emerald-500' : 'bg-rose-500'}">
                        ${hasCopies ? 'Có sẵn' : 'Hết sách'}
                    </span>

                </div>

                <div class="p-3 flex-1 flex flex-col justify-between">

                    <div>
                      <span class="text-[9px] font-semibold text-slate-500 uppercase tracking-wider block mb-1">
                         ${categories}
                        </span>

                        <h4 class="font-bold text-slate-900 text-xs line-clamp-2 group-hover:text-[#1e3a5f] transition-colors leading-snug">
                            ${title}
                        </h4>

                        <p class="text-[11px] text-slate-500 mt-1 line-clamp-1">
                            ${authors}
                        </p>
                    </div>

                    <div class="mt-3 pt-2 border-t border-slate-100 flex items-center justify-between text-[10px] text-slate-400">
                        <span>Năm: ${escapeHtml(book.publishYear)}</span>

                        <span class="font-semibold text-[#1e3a5f] group-hover:underline">
                            Chi tiết
                        </span>
                    </div>

                </div>
            </a>
        `;
    }

    function renderError() {
        container.innerHTML = `
            <div class="bg-white rounded-2xl border border-slate-200 p-6 text-center">

                <i class="fa-solid fa-circle-exclamation text-2xl text-slate-400 mb-3"></i>

                <p class="text-sm text-slate-600 mb-3">
                    Không thể tải sách gợi ý.
                </p>

                <button type="button"
                        id="retrySuggestions"
                        class="text-sm font-semibold text-[#1e3a5f] hover:underline">
                    Thử lại
                </button>

            </div>
        `;

        document.getElementById('retrySuggestions')
            .addEventListener('click', loadSuggestions);
    }

    function safeImageUrl(value) {
        if (!value) return null;

        try {
            const url = new URL(value, window.location.origin);

            if (url.protocol !== 'http:' && url.protocol !== 'https:') {
                return null;
            }

            return url.href;
        } catch {
            return null;
        }
    }

    function escapeHtml(value) {
        return String(value ?? '').replace(/[&<>"']/g, char => ({
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        })[char]);
    }
});