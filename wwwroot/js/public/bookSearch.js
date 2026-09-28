document.addEventListener('DOMContentLoaded', () => {
    /**
     * CONFIG
     */
    const DEFAULT_PAGE_SIZE = 12;

    const API = {
        searchBooks: '/api/resources/search',
        searchAuthors: '/api/authors/search',
        searchPublishers: '/api/publishers/search',
        searchCategories: '/api/categories/search',
        searchDocumentTypes: '/api/document-types/search'
    };

    /**
     * Operator của group hiện tại được dùng để nối
     * group hiện tại với group tiếp theo.
     */
    const state = {
        keyword: '',
        filterExpression: '',
        publicationYear: null,
        status: 'all',
        sort: 'relevance',
        page: 1,
        pageSize: DEFAULT_PAGE_SIZE,
        view: 'list',
        lastSearchKeyword: '',
        lastSearchFilterExpression: ''
    };

    let currentBookSearchResult = null;

    /**
     * FILTER CACHE
     */
    const filterCache = {
        documentType: new Map(),
        author: new Map(),
        publisher: new Map(),
        category: new Map()
    };

    /**
     * DOM ELEMENTS
     */
    const elements = {
        keyword: document.getElementById('catalog-keyword-search'),
        documentTypeSearch: document.getElementById('catalog-doc-type-search'),
        documentTypeList: document.getElementById('doc-type-list'),
        authorSearch: document.getElementById('catalog-author-search'),
        authorList: document.getElementById('author-list'),
        publisherSearch: document.getElementById('catalog-publisher-search'),
        publisherList: document.getElementById('publisher-list'),
        categorySearch: document.getElementById('catalog-category-search'),
        categoryList: document.getElementById('category-list'),
        year: document.getElementById('catalog-year-filter'),
        status: document.getElementById('catalog-status-filter'),
        sort: document.getElementById('catalog-sort'),
        results: document.getElementById('book-results'),
        resultCount: document.getElementById('result-count'),
        pagination: document.getElementById('pagination'),
        activeFilters: document.getElementById('active-filters'),
        activeFilterList: document.getElementById('active-filter-list'),
        filterExpression: document.getElementById('filter-expression'),
        filterExpressionSuggestions: document.getElementById('filter-expression-suggestions'),
        filterSidebar: document.getElementById('filter-sidebar')
    };

    /**
     * COMMON HELPERS
     */
    function escapeHtml(value) {
        if (value === null || value === undefined) {
            return '';
        }

        return String(value)
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#039;');
    }

    function debounce(callback, delay = 300) {
        let timer = null;

        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => {
                callback(...args);
            }, delay);
        };
    }

    async function getJson(url) {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                Accept: 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`Request failed: ${response.status} `);
        }

        return await response.json();
    }

    function extractItems(data) {
        if (Array.isArray(data)) {
            return data;
        }

        if (data && Array.isArray(data.items)) {
            return data.items;
        }

        return [];
    }

    function normalizeSelectionItem(item) {
        return {
            id: String(item.id),
            name: item.name ?? item.title ?? ''
        };
    }

    function getFilterKeyword(input) {
        return input?.value.trim() ?? '';
    }

    /**
     * SEARCH BOOKS
     */
    async function searchBooks() {
        syncFilterExpression();

        const request = {
            keyword: state.keyword || null,
            filterExpression: state.filterExpression || null,
            publicationYear: state.publicationYear,
            status: state.status,
            sort: state.sort,
            page: state.page,
            pageSize: state.pageSize
        };

        showLoading();

        try {
            const response = await fetch(API.searchBooks, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json'
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) {
                throw new Error(`Book search failed: ${response.status} `);
            }

            const data = await response.json();

            state.lastSearchKeyword = state.keyword;
            state.lastSearchFilterExpression = state.filterExpression;
            currentBookSearchResult = data;

            renderBooks(data);
            renderResultCount(data);
            renderPagination(data);
        } catch (error) {
            console.error('Không thể tải danh sách tài liệu:', error);
            showSearchError();
        }
    }

    function showLoading() {
        if (!elements.results) {
            return;
        }

        elements.results.innerHTML = `
    <div class="bg-white border border-slate-200 rounded-xl p-8 text-center" >
        <p class="text-xs text-slate-500">
            Đang tải danh sách tài liệu...
        </p>
            </div >
    `;
    }

    function showSearchError() {
        if (!elements.results) {
            return;
        }

        elements.results.innerHTML = `
    <div class="bg-white border border-red-200 rounded-xl p-8 text-center" >
        <p class="text-xs text-red-500">
            Không thể tải danh sách tài liệu.
        </p>
            </div >
    `;

        if (elements.pagination) {
            elements.pagination.innerHTML = '';
            elements.pagination.classList.add('hidden');
        }
    }

    /**
     * BOOK RESULT
     */
    function renderBooks(data) {
        if (!elements.results) {
            return;
        }

        const books = data.items ?? [];

        // Layout mặc định của danh sách
        elements.results.classList.add(
            'grid',
            'grid-cols-1',
            'gap-4',
            'w-full'
        );

        const isGrid = state.view === 'grid';

        // Chỉ thêm nhiều cột khi ở grid view
        elements.results.classList.toggle('sm:grid-cols-2', isGrid);
        elements.results.classList.toggle('lg:grid-cols-3', isGrid);

        if (books.length === 0) {
            elements.results.innerHTML = `
    <div class="bg-white border border-slate-200 rounded-xl p-8 text-center" >
                    <p class="text-sm font-semibold text-slate-700">
                        Không tìm thấy tài liệu
                    </p>
                    <p class="text-xs text-slate-400 mt-1">
                        Hãy thử thay đổi điều kiện tìm kiếm.
                    </p>
                </div >
    `;

            return;
        }

        elements.results.innerHTML = books.map(createBookCard).join('');
    }

    function createBookCard(book) {
        const authors = Array.isArray(book.authors) && book.authors.length > 0
            ? book.authors.join(', ')
            : 'Chưa cập nhật';

        const categories = Array.isArray(book.categories)
            ? book.categories
            : [];

        const availableCopies = Number(book.availableCopies ?? 0);
        const totalCopies = Number(book.totalCopies ?? 0);
        const isAvailable = availableCopies > 0;

        /**
         * GRID
         */
        if (state.view === 'grid') {
            return `
    <article
class="bg-white border border-slate-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition cursor-pointer"
data-book-id="${escapeHtml(book.id)}"
    >
                    <div class="aspect-[3/4] bg-slate-100 overflow-hidden">
                        ${book.coverUrl
                    ? `
                                <img
                                    src="${escapeHtml(book.coverUrl)}"
                                    alt="${escapeHtml(book.title)}"
                                    class="w-full h-full object-cover"
                                >
                            `
                    : `
                                <div class="w-full h-full flex items-center justify-center text-xs text-slate-400">
                                    Không có ảnh
                                </div>
                            `}
                    </div>

                    <div class="p-4">
                        <div class="flex items-start justify-between gap-2">
                            <h3 class="text-sm font-bold text-slate-800 line-clamp-2">
                                ${escapeHtml(book.title)}
                            </h3>

                            ${book.documentTypeName
                    ? `
                                    <span class="shrink-0 text-[10px] px-2 py-1 rounded-full bg-slate-100 text-slate-600">
                                        ${escapeHtml(book.documentTypeName)}
                                    </span>
                                `
                    : ''}
                        </div>

                        <p class="text-xs text-slate-500 mt-2">
                            ${escapeHtml(authors)}
                        </p>

                        ${book.publisherName
                    ? `
                                <p class="text-[11px] text-slate-400 mt-1">
                                    ${escapeHtml(book.publisherName)}
                                </p>
                            `
                    : ''}

                        ${book.publicationYear
                    ? `
                                <p class="text-[11px] text-slate-400 mt-1">
                                    ${book.publicationYear}
                                </p>
                            `
                    : ''}

                        ${categories.length > 0
                    ? `
                                <div class="flex flex-wrap gap-1 mt-3">
                                    ${categories
                        .slice(0, 3)
                        .map(category => `
                                            <span class="text-[10px] px-2 py-1 bg-slate-50 text-slate-500 rounded-md">
                                                ${escapeHtml(category)}
                                            </span>
                                        `)
                        .join('')}
                                </div>
                            `
                    : ''}

                        <div class="mt-3 text-[11px] font-medium ${isAvailable
                    ? 'text-emerald-600'
                    : 'text-red-500'}">
                            ${isAvailable
                    ? `${availableCopies}/${totalCopies} bản có sẵn`
                    : 'Hết sách'}
                        </div>
                    </div>
                </article >
    `;
        }

        /**
         * LIST
         */
        return `
    <article
class="bg-white border border-slate-200 rounded-xl p-4 shadow-sm hover:shadow-md transition cursor-pointer"
data-book-id="${escapeHtml(book.id)}"
    >
    <div class="flex gap-4">
        <div class="w-20 sm:w-24 shrink-0">
            <div class="aspect-[3/4] bg-slate-100 rounded-lg overflow-hidden">
                ${book.coverUrl
                ? `
                                    <img
                                        src="${escapeHtml(book.coverUrl)}"
                                        alt="${escapeHtml(book.title)}"
                                        class="w-full h-full object-cover"
                                    >
                                `
                : `
                                    <div class="w-full h-full flex items-center justify-center text-[10px] text-slate-400">
                                        Không có ảnh
                                    </div>
                                `}
            </div>
        </div>

        <div class="min-w-0 flex-1">
            <div class="flex flex-wrap items-start gap-2">
                <h3 class="text-sm font-bold text-slate-800">
                    ${escapeHtml(book.title)}
                </h3>

                ${book.documentTypeName
                ? `
                                    <span class="text-[10px] px-2 py-1 rounded-full bg-slate-100 text-slate-600">
                                        ${escapeHtml(book.documentTypeName)}
                                    </span>
                                `
                : ''}
            </div>

            <p class="text-xs text-slate-500 mt-2">
                <span class="font-medium text-slate-600">
                    Tác giả:
                </span>
                ${escapeHtml(authors)}
            </p>

            ${book.publisherName
                ? `
                                <p class="text-xs text-slate-500 mt-1">
                                    <span class="font-medium text-slate-600">
                                        Nhà xuất bản:
                                    </span>
                                    ${escapeHtml(book.publisherName)}
                                </p>
                            `
                : ''}

            ${book.publicationYear
                ? `
                                <p class="text-xs text-slate-500 mt-1">
                                    <span class="font-medium text-slate-600">
                                        Năm:
                                    </span>
                                    ${book.publicationYear}
                                </p>
                            `
                : ''}

            ${categories.length > 0
                ? `
                                <div class="flex flex-wrap gap-1.5 mt-3">
                                    ${categories
                    .map(category => `
                                            <span class="text-[10px] px-2 py-1 bg-slate-50 border border-slate-100 text-slate-500 rounded-md">
                                                ${escapeHtml(category)}
                                            </span>
                                        `)
                    .join('')}
                                </div>
                            `
                : ''}

            <div class="mt-3">
                <span class="text-[11px] font-semibold ${isAvailable
                ? 'text-emerald-600'
                : 'text-red-500'}">
                    ${isAvailable
                ? `${availableCopies}/${totalCopies} bản có sẵn`
                : 'Hết sách'}
                </span>
            </div>
        </div>
    </div>
            </article >
    `;
    }

    function renderResultCount(data) {
        if (!elements.resultCount) {
            return;
        }

        const totalItems = Number(data.totalItems ?? 0);
        elements.resultCount.textContent = `${totalItems.toLocaleString('vi-VN')} kết quả`;
    }

    /**
     * PAGINATION
     */
    function renderPagination(data) {
        if (!elements.pagination) {
            return;
        }

        const totalPages = Number(data.totalPages ?? 0);
        const currentPage = Number(data.page ?? 1);

        if (totalPages <= 1) {
            elements.pagination.innerHTML = '';
            elements.pagination.classList.add('hidden');
            return;
        }

        elements.pagination.classList.remove('hidden');

        const pageList = buildPageList(currentPage, totalPages);

        let html = `
    <div class="flex items-center justify-center gap-1.5 flex-wrap" >
        `;

        html += `
        <button
type = "button"
data-page="${currentPage - 1}"
                ${currentPage <= 1 ? 'disabled' : ''}
class="
min-w-[2.25rem]
h-9
px-2
rounded-lg
border
border-slate-200
bg-white
text-xs
                    ${currentPage <= 1
                ? 'text-slate-300 cursor-not-allowed'
                : 'text-slate-600 hover:bg-slate-50'
            }
"
    >
                ‹
            </button >
    `;

        for (const page of pageList) {
            if (page === '...') {
                html += `
    <span class="min-w-[2.25rem] h-9 flex items-center justify-center text-xs text-slate-400" >
                        ...
                    </span >
    `;

                continue;
            }

            const active = page === currentPage;

            html += `
    <button
type = "button"
data-page="${page}"
class="
min-w-[2.25rem]
h-9
px-2
rounded-lg
border
text-xs
                        ${active
                    ? 'bg-[#1e3a5f] border-[#1e3a5f] text-white font-semibold'
                    : 'bg-white border-slate-200 text-slate-600 hover:bg-slate-50'
                }
"
    >
    ${page}
                </button >
    `;
        }

        html += `
    <button
type = "button"
data-page="${currentPage + 1}"
                ${currentPage >= totalPages ? 'disabled' : ''}
class="
min-w-[2.25rem]
h-9
px-2
rounded-lg
border
border-slate-200
bg-white
text-xs
                    ${currentPage >= totalPages
                ? 'text-slate-300 cursor-not-allowed'
                : 'text-slate-600 hover:bg-slate-50'
            }
"
    >
                ›
            </button >
    `;

        html += `
            </div >
    <span class="text-xs text-slate-400">
        Trang ${currentPage} / ${totalPages}
    </span>
`;

        elements.pagination.innerHTML = html;
    }

    function buildPageList(current, total) {
        const maxButtons = getMaxPaginationButtons();

        if (total <= maxButtons) {
            return Array.from(
                { length: total },
                (_, index) => index + 1
            );
        }

        const pages = [1];
        const sideCount = Math.max(1, Math.floor((maxButtons - 3) / 2));
        const start = Math.max(2, current - sideCount);
        const end = Math.min(total - 1, current + sideCount);

        if (start > 2) {
            pages.push('...');
        }

        for (let page = start; page <= end; page++) {
            pages.push(page);
        }

        if (end < total - 1) {
            pages.push('...');
        }

        pages.push(total);

        return pages;
    }

    function getMaxPaginationButtons() {
        const width = window.innerWidth;

        if (width <= 420) {
            return 3;
        }

        if (width <= 640) {
            return 5;
        }

        if (width <= 1024) {
            return 7;
        }

        return 10;
    }

    elements.pagination?.addEventListener('click', event => {
        const button = event.target.closest('[data-page]');

        if (!button || button.disabled) {
            return;
        }

        const page = Number(button.dataset.page);

        if (!Number.isInteger(page) || page < 1) {
            return;
        }

        state.page = page;
        searchBooks();
    });

    /**
     * FILTER DROPDOWN
     */
    function renderFilterItems(container, items) {
        if (!container) {
            return;
        }

        container.innerHTML = '';

        if (items.length === 0) {
            container.innerHTML = `
    <div class="px-3 py-2 text-xs text-slate-400" >
        Không tìm thấy dữ liệu
                </div >
    `;

            return;
        }

        items.forEach(item => {
            const button = document.createElement('button');

            button.type = 'button';
            button.dataset.id = item.id;
            button.dataset.name = item.name;
            button.className = 'w-full text-left px-3 py-2 text-sm text-slate-700 hover:bg-slate-50 rounded-lg';

            button.innerHTML = `
    <span class="flex items-center justify-between gap-2" >
        <span>
            ${escapeHtml(item.name)}
        </span>
                </span >
    `;

            container.appendChild(button);
        });
    }

    async function loadFilterItems(type, url, keyword, container) {
        const cache = filterCache[type];

        if (!cache || !container) {
            return;
        }

        const normalizedKeyword = keyword.trim();

        if (cache.has(normalizedKeyword)) {
            renderFilterItems(container, cache.get(normalizedKeyword));
            showFilterList(container);
            return;
        }

        try {
            const params = new URLSearchParams();

            if (normalizedKeyword) {
                params.set('name', normalizedKeyword);
            }

            const query = params.toString();
            const requestUrl = query ? `${url}?${query} ` : url;
            const data = await getJson(requestUrl);

            const items = extractItems(data)
                .map(normalizeSelectionItem)
                .filter(item => item.id && item.name);

            cache.set(normalizedKeyword, items);

            renderFilterItems(container, items);
            showFilterList(container);
        } catch (error) {
            console.error(`Không thể tải dữ liệu filter "${type}": `, error);

            container.innerHTML = `
    <div class="px-3 py-2 text-xs text-red-400" >
        Không thể tải dữ liệu
                </div >
    `;

            showFilterList(container);
        }
    }

    function hideAllFilterLists() {
        elements.documentTypeList?.classList.add('hidden');
        elements.authorList?.classList.add('hidden');
        elements.publisherList?.classList.add('hidden');
        elements.categoryList?.classList.add('hidden');
    }

    function showFilterList(list) {
        hideAllFilterLists();
        list?.classList.remove('hidden');
    }

    /**
     * FILTER CONFIG
     */
    const filterConfigs = {
        documentType: {
            field: 'document_types',
            input: elements.documentTypeSearch,
            list: elements.documentTypeList,
            url: API.searchDocumentTypes,
            operator: '|'
        },
        author: {
            field: 'authors',
            input: elements.authorSearch,
            list: elements.authorList,
            url: API.searchAuthors,
            operator: '&'
        },
        publisher: {
            field: 'publishers',
            input: elements.publisherSearch,
            list: elements.publisherList,
            url: API.searchPublishers,
            operator: '|'
        },
        category: {
            field: 'categories',
            input: elements.categorySearch,
            list: elements.categoryList,
            url: API.searchCategories,
            operator: '&'
        }
    };

    /**
     * FILTER INPUT
     */
    Object.entries(filterConfigs).forEach(([type, config]) => {
        if (!config.input || !config.list) {
            return;
        }

        const search = debounce(() => {
            loadFilterItems(
                type,
                config.url,
                getFilterKeyword(config.input),
                config.list
            );
        }, 300);

        config.input.addEventListener('focus', () => {
            loadFilterItems(
                type,
                config.url,
                getFilterKeyword(config.input),
                config.list
            );
        });

        config.input.addEventListener('input', search);

        config.list.addEventListener('click', event => {
            const button = event.target.closest('button[data-id]');

            if (!button) {
                return;
            }

            selectFilterFromDropdown(
                config.field,
                button.dataset.name,
                config.operator
            );
        });
    });

    document.addEventListener('click', event => {
        const target = event.target;

        const isInsideFilter =
            elements.documentTypeSearch?.contains(target) ||
            elements.documentTypeList?.contains(target) ||
            elements.authorSearch?.contains(target) ||
            elements.authorList?.contains(target) ||
            elements.publisherSearch?.contains(target) ||
            elements.publisherList?.contains(target) ||
            elements.categorySearch?.contains(target) ||
            elements.categoryList?.contains(target);

        if (!isInsideFilter) {
            hideAllFilterLists();
        }
    });

    document.addEventListener('keydown', event => {
        if (event.key === 'Escape') {
            hideAllFilterLists();
        }
    });

    /**
     * CLEAR ACTIONS
     */
    document.addEventListener('click', event => {
        const actionElement = event.target.closest('[data-action]');

        if (!actionElement) {
            return;
        }

        const action = actionElement.dataset.action;

        switch (action) {
            case 'year-clear':
                state.publicationYear = null;

                if (elements.year) {
                    elements.year.value = '';
                }

                state.page = 1;
                searchBooks();
                break;

            case 'clear-all':
                clearAllFilters();
                break;

            case 'reset-breadcrumb':
                clearAllFilters();
                break;

            case 'home':
                window.location.href = '/';
                break;

            case 'mobile-filter':
                toggleMobileFilter();
                break;
        }
    });

    /**
     * CLEAR ALL
     */
    function clearAllFilters() {
        state.keyword = '';
        state.filterExpression = '';
        state.publicationYear = null;
        state.status = 'all';
        state.sort = 'relevance';
        state.page = 1;

        if (elements.keyword) {
            elements.keyword.value = '';
        }

        if (elements.filterExpression) {
            elements.filterExpression.value = '';
        }

        if (elements.documentTypeSearch) {
            elements.documentTypeSearch.value = '';
        }

        if (elements.authorSearch) {
            elements.authorSearch.value = '';
        }

        if (elements.publisherSearch) {
            elements.publisherSearch.value = '';
        }

        if (elements.categorySearch) {
            elements.categorySearch.value = '';
        }

        if (elements.year) {
            elements.year.value = '';
        }

        if (elements.status) {
            elements.status.value = 'all';
        }

        if (elements.sort) {
            elements.sort.value = 'relevance';
        }

        hideAllFilterLists();
        renderFilterExpression();
        searchBooks();
    }

    /**
     * ACTIVE FILTERS
     */
    function renderActiveFilters() {
        if (!elements.activeFilters) {
            return;
        }

        syncFilterExpression();

        const hasExpression = state.filterExpression.length > 0;
        const hasOtherFilters =
            state.keyword ||
            state.publicationYear !== null ||
            state.status !== 'all';

        if (!hasExpression && !hasOtherFilters) {
            elements.activeFilters.classList.add('hidden');
            return;
        }

        elements.activeFilters.classList.remove('hidden');
    }

    // elements.activeFilterList?.addEventListener('click', event => {
    //     const button = event.target.closest('[data-filter-key]');

    //     if (!button) {
    //         return;
    //     }

    //     clearFilter(button.dataset.filterKey);
    // });

    /**
     * CLEAR ONE FILTER
     */
    function clearFilter(key) {
        switch (key) {
            case 'keyword':
                state.keyword = '';

                if (elements.keyword) {
                    elements.keyword.value = '';
                }
                break;

            case 'documentType':
                state.documentTypeIds = [];
                state.documentTypeNames = [];

                if (elements.documentTypeSearch) {
                    elements.documentTypeSearch.value = '';
                }

                renderCachedFilter(
                    'documentType',
                    elements.documentTypeSearch,
                    elements.documentTypeList,
                    []
                );
                break;

            case 'author':
                state.authorGroups = [];

                if (elements.authorSearch) {
                    elements.authorSearch.value = '';
                }

                renderCachedFilter(
                    'author',
                    elements.authorSearch,
                    elements.authorList,
                    []
                );
                break;

            case 'publisher':
                state.publisherIds = [];
                state.publisherNames = [];

                if (elements.publisherSearch) {
                    elements.publisherSearch.value = '';
                }

                renderCachedFilter(
                    'publisher',
                    elements.publisherSearch,
                    elements.publisherList,
                    []
                );
                break;

            case 'category':
                state.categoryGroups = [];

                if (elements.categorySearch) {
                    elements.categorySearch.value = '';
                }

                renderCachedFilter(
                    'category',
                    elements.categorySearch,
                    elements.categoryList,
                    []
                );
                break;

            case 'year':
                state.publicationYear = null;

                if (elements.year) {
                    elements.year.value = '';
                }
                break;

            case 'status':
                state.status = 'all';

                if (elements.status) {
                    elements.status.value = 'all';
                }
                break;
        }

        state.page = 1;
        searchBooks();
    }

    /**
     * LIST / GRID
     */
    const viewButtons = document.querySelectorAll('[data-view]');

    viewButtons.forEach(button => {
        button.addEventListener('click', () => {
            const view = button.dataset.view;

            if (view !== 'list' && view !== 'grid') {
                return;
            }

            state.view = view;
            updateViewButtons();

            if (currentBookSearchResult) {
                renderBooks(currentBookSearchResult);
            }
        });
    });

    function updateViewButtons() {
        viewButtons.forEach(button => {
            const active = button.dataset.view === state.view;

            button.classList.toggle('bg-[#1e3a5f]', active);
            button.classList.toggle('text-white', active);
            button.classList.toggle('shadow-sm', active);
            button.classList.toggle('text-slate-500', !active);
        });
    }

    /**
     * KEYWORD
     */
    elements.filterExpression?.addEventListener('keydown', event => {
        if (event.key !== 'Enter') {
            return;
        }

        event.preventDefault();

        const filterExpression = elements.filterExpression.value.trim();

        if (filterExpression === state.lastSearchFilterExpression) {
            return;
        }

        syncFilterExpression();
        state.page = 1;
        searchBooks();
    });

    /**
     * YEAR
     */
    elements.year?.addEventListener('input', event => {
        event.target.value = event.target.value
            .replace(/\D/g, '')
            .slice(0, 4);
    });

    elements.year?.addEventListener('change', () => {
        const value = elements.year.value.trim();

        if (!value) {
            state.publicationYear = null;
        } else {
            const year = Number(value);
            state.publicationYear = Number.isInteger(year) ? year : null;
        }

        state.page = 1;
        searchBooks();
    });

    /**
     * STATUS
     */
    elements.status?.addEventListener('change', () => {
        state.status = elements.status.value || 'all';
        state.page = 1;
        searchBooks();
    });

    /**
     * SORT
     */
    elements.sort?.addEventListener('change', () => {
        state.sort = elements.sort.value || 'relevance';
        state.page = 1;
        searchBooks();
    });

    /**
     * MOBILE FILTER
     */
    function toggleMobileFilter() {
        if (!elements.filterSidebar) {
            return;
        }

        elements.filterSidebar.classList.toggle('hidden');
    }

    /**
     * RESPONSIVE
     *
     * Từ 1150px trở lên:
     * sidebar luôn hiện.
     */
    let resizeTimer = null;

    window.addEventListener('resize', () => {
        clearTimeout(resizeTimer);

        resizeTimer = setTimeout(() => {
            if (elements.filterSidebar && window.innerWidth >= 1150) {
                elements.filterSidebar.classList.remove('hidden');
            }

            if (elements.pagination && currentBookSearchResult) {
                renderPagination(currentBookSearchResult);
            }
        }, 150);
    });

    /**
     * Đồng bộ text từ input vào state.
     */
    function syncFilterExpression() {
        if (!elements.filterExpression) {
            return;
        }

        state.filterExpression = elements.filterExpression.value.trim();
    }

    elements.filterExpression?.addEventListener('input', () => {
        syncFilterExpression();
        state.page = 1;
    });

    /**
     * Đưa expression lên input.
     */
    function renderFilterExpression() {
        if (!elements.filterExpression) {
            return;
        }

        elements.filterExpression.value = state.filterExpression;
    }

    /**
     * Escape tên để đưa vào expression.
     *
     * Ví dụ:
     *
     * Văn học Việt Nam
     *
     * =>
     *
     * "Văn học Việt Nam"
     */
    function quoteFilterValue(value) {
        return `"${String(value)
            .replaceAll('\\', '\\\\')
            .replaceAll('"', '\\"')}"`;
    }

    function findFilterGroup(expression, field) {
        const prefix = `${field}:(`;
        const start = expression.indexOf(prefix);

        if (start === -1) {
            return null;
        }

        const contentStart = start + prefix.length;
        let depth = 1;
        let inQuotes = false;
        let escaped = false;

        for (let i = contentStart; i < expression.length; i++) {
            const char = expression[i];

            if (escaped) {
                escaped = false;
                continue;
            }

            if (char === '\\' && inQuotes) {
                escaped = true;
                continue;
            }

            if (char === '"') {
                inQuotes = !inQuotes;
                continue;
            }

            if (inQuotes) {
                continue;
            }

            if (char === '(') {
                depth++;
            }

            if (char === ')') {
                depth--;

                if (depth === 0) {
                    return {
                        start,
                        contentStart,
                        end: i
                    };
                }
            }
        }

        return null;
    }

    /**
     * Thêm một giá trị vào filter expression.
     *
     * Ví dụ:
     *
     * categories:"Văn học Việt Nam"
     *
     * thêm:
     *
     * "Công nghệ thông tin"
     *
     * =>
     *
     * categories:"Văn học Việt Nam" & "Công nghệ thông tin"
     */
    function appendFilterValue(field, value, operator = '|') {
        if (!value) {
            return;
        }

        const expression = state.filterExpression.trim();
        const quotedValue = quoteFilterValue(value);
        const group = findFilterGroup(expression, field);

        /**
         * Chưa có group:
         *
         * categories:("Âm nhạc")
         */
        if (!group) {
            const newGroup = `${field}:(${quotedValue})`;

            if (!expression) {
                state.filterExpression = newGroup;
            } else {
                /**
                 * Các field khác nhau mặc định AND.
                 *
                 * categories:(...)
                 * &
                 * authors:(...)
                 */
                state.filterExpression = `${expression} & ${newGroup}`;
            }

            renderFilterExpression();
            return;
        }

        /**
         * Đã có group:
         *
         * categories:("Âm nhạc")
         *
         * =>
         *
         * categories:("Âm nhạc" & "CNTT")
         */
        const currentContent = expression
            .slice(group.contentStart, group.end)
            .trim();

        /**
         * Tránh thêm trùng.
         */
        const existingValues = extractQuotedValues(currentContent);
        const normalizedValue = String(value).trim().toLowerCase();

        if (existingValues.some(existing =>
            existing.trim().toLowerCase() === normalizedValue
        )) {
            return;
        }

        const newContent = currentContent
            ? `${currentContent} ${operator} ${quotedValue}`
            : quotedValue;

        state.filterExpression =
            expression.slice(0, group.contentStart) +
            newContent +
            expression.slice(group.end);

        renderFilterExpression();
    }

    function extractQuotedValues(text) {
        const values = [];
        let current = '';
        let inQuotes = false;
        let escaped = false;

        for (let i = 0; i < text.length; i++) {
            const char = text[i];

            if (escaped) {
                current += char;
                escaped = false;
                continue;
            }

            if (char === '\\' && inQuotes) {
                escaped = true;
                continue;
            }

            if (char === '"') {
                if (inQuotes) {
                    values.push(current);
                    current = '';
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (inQuotes) {
                current += char;
            }
        }

        return values;
    }

    function selectFilterFromDropdown(field, name, operator) {
        appendFilterValue(field, name, operator);
        state.page = 1;
        searchBooks();
    }

    elements.keyword?.addEventListener('keydown', event => {
        if (event.key !== 'Enter') {
            return;
        }

        event.preventDefault();

        const keyword = elements.keyword.value.trim();

        if (keyword === state.lastSearchKeyword) {
            return;
        }

        state.keyword = keyword;
        state.page = 1;
        searchBooks();
    });

    /**
     * INITIALIZE
     */
    updateViewButtons();
    hideAllFilterLists();
    searchBooks();
});
