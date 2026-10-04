
document.addEventListener('DOMContentLoaded', () => {

    const searchForm =
        document.getElementById('category-search-form');

    const searchInput =
        document.getElementById('search-input');

    const clearSearchButton =
        document.getElementById('btn-clear-search');

    const itemsPerPage =
        document.getElementById('itemsPerPage');

    const sortBy =
        document.getElementById('sortBy');


    /**
     * Lấy URL hiện tại.
     */
    function getUrl() {
        return new URL(window.location.href);
    }


    /**
     * Chuyển đến URL mới.
     */
    function navigate(url) {
        window.location.href = url.toString();
    }


    /**
     * Đồng bộ pageSize từ URL vào select.
     *
     * Ví dụ:
     * /Category?page=2&pageSize=12
     *
     * => select = 12
     */
    function syncPageSizeFromUrl() {

        if (!itemsPerPage) {
            return;
        }

        const url = getUrl();

        const pageSize =
            url.searchParams.get('pageSize');

        if (!pageSize) {
            return;
        }

        const optionExists =
            [...itemsPerPage.options]
                .some(option => option.value === pageSize);

        if (optionExists) {
            itemsPerPage.value = pageSize;
        }
    }


    /**
     * Đồng bộ sortBy từ URL vào select.
     *
     * Ví dụ:
     * /Category?page=1&sortBy=name-asc
     *
     * => select = name-asc
     */
    function syncSortFromUrl() {

        if (!sortBy) {
            return;
        }

        const url = getUrl();

        const sortValue =
            url.searchParams.get('sortBy');

        if (!sortValue) {
            // Không có sortBy => mặc định
            sortBy.value = 'default';
            return;
        }

        const optionExists =
            [...sortBy.options]
                .some(option => option.value === sortValue);

        if (optionExists) {
            sortBy.value = sortValue;
        }
    }


    /**
     * Search
     *
     * GET:
     * /Category?keyword=van
     */
    searchForm?.addEventListener('submit', (event) => {

        event.preventDefault();

        const url = getUrl();

        const keyword =
            searchInput.value.trim();

        if (keyword) {
            url.searchParams.set(
                'keyword',
                keyword
            );
        } else {
            url.searchParams.delete('keyword');
        }

        // Search mới luôn quay về trang 1.
        url.searchParams.set(
            'page',
            '1'
        );

        // Search mới reset sort.
        url.searchParams.delete(
            'sortBy'
        );

        navigate(url);
    });


    /**
     * Xóa tìm kiếm.
     *
     * /Category
     */
    clearSearchButton?.addEventListener('click', () => {

        const url = getUrl();

        url.searchParams.delete(
            'keyword'
        );

        url.searchParams.set(
            'page',
            '1'
        );

        // Reset sort.
        url.searchParams.delete(
            'sortBy'
        );

        navigate(url);
    });


    /**
     * Thay đổi số lượng item trên mỗi trang.
     *
     * Ví dụ:
     * /Category?page=1&pageSize=12
     */
    itemsPerPage?.addEventListener('change', () => {

        const url = getUrl();

        url.searchParams.set(
            'pageSize',
            itemsPerPage.value
        );

        // Khi đổi page size, quay lại trang 1.
        url.searchParams.set(
            'page',
            '1'
        );

        navigate(url);
    });


    /**
     * Thay đổi thứ tự sắp xếp.
     */
    sortBy?.addEventListener('change', () => {

        const url = getUrl();

        if (sortBy.value === 'default') {

            url.searchParams.delete(
                'sortBy'
            );

        } else {

            url.searchParams.set(
                'sortBy',
                sortBy.value
            );
        }

        // Đổi sort => quay về trang 1.
        url.searchParams.set(
            'page',
            '1'
        );

        navigate(url);
    });


    /**
     * Cho phép nhấn Enter trong ô tìm kiếm.
     */
    searchInput?.addEventListener('keydown', (event) => {

        if (event.key !== 'Enter') {
            return;
        }

        event.preventDefault();

        searchForm?.requestSubmit();
    });



    // Responsive pagination


    const paginationContainer =
        document.getElementById(
            'pagination-numbers'
        );

    if (paginationContainer) {

        const PAGINATION_BREAKPOINTS = [
            {
                maxWidth: 420,
                maxButtons: 3
            },
            {
                maxWidth: 640,
                maxButtons: 5
            },
            {
                maxWidth: 1024,
                maxButtons: 7
            },
            {
                maxWidth: Infinity,
                maxButtons: 10
            }
        ];

        const ELLIPSIS_CLASS = 'pagination-ellipsis min-w-[2.25rem] h-9 px-1 flex items-center justify-center text-xs text-slate-400 select-none';


        function getMaxPaginationButtons() {

            const width =
                window.innerWidth;

            const breakpoint =
                PAGINATION_BREAKPOINTS.find(
                    breakpoint =>
                        width <= breakpoint.maxWidth
                );

            return breakpoint
                ? breakpoint.maxButtons
                : 7;
        }


        function buildPageList(
            current,
            total,
            maxButtons
        ) {

            if (total <= maxButtons) {

                return Array.from(
                    { length: total },
                    (_, index) => index + 1
                );
            }

            const result = [];

            const sideCount =
                Math.max(
                    1,
                    Math.floor(
                        (maxButtons - 3) / 2
                    )
                );

            let start =
                Math.max(
                    2,
                    current - sideCount
                );

            let end =
                Math.min(
                    total - 1,
                    current + sideCount
                );

            const span =
                end - start;

            const desiredSpan =
                maxButtons - 4;

            if (span < desiredSpan) {

                if (
                    current - start <
                    end - current
                ) {

                    end =
                        Math.min(
                            total - 1,
                            end +
                            (desiredSpan - span)
                        );

                } else {

                    start =
                        Math.max(
                            2,
                            start -
                            (desiredSpan - span)
                        );
                }
            }

            result.push(1);

            if (start > 2) {
                result.push('...');
            }

            for (
                let page = start;
                page <= end;
                page++
            ) {
                result.push(page);
            }

            if (end < total - 1) {
                result.push('...');
            }

            result.push(total);

            return result;
        }


        function makeEllipsis() {

            const span =
                document.createElement('span');

            span.className =
                ELLIPSIS_CLASS;

            span.setAttribute(
                'aria-hidden',
                'true'
            );

            span.textContent = '...';

            return span;
        }


        function renderResponsivePagination() {

            const currentElement =
                paginationContainer.querySelector(
                    '[data-current="true"][data-page]'
                );

            if (!currentElement) {
                return;
            }

            const allButtons = [
                ...paginationContainer.querySelectorAll(
                    '[data-page]'
                )
            ];

            const total =
                allButtons.length;

            if (total <= 1) {
                return;
            }

            const current =
                parseInt(
                    currentElement.getAttribute(
                        'data-page'
                    ),
                    10
                );

            const maxButtons =
                getMaxPaginationButtons();

            const pageList =
                buildPageList(
                    current,
                    total,
                    maxButtons
                );


            // Xóa tất cả dấu "..." cũ.
            paginationContainer
                .querySelectorAll(
                    '.pagination-ellipsis'
                )
                .forEach(element => {
                    element.remove();
                });


            // Ẩn tất cả nút trước.
            allButtons.forEach(button => {
                button.classList.add('hidden');
            });


            // Hiện lại những trang cần thiết.
            pageList.forEach(item => {

                if (item === '...') {
                    return;
                }

                const targetButton =
                    allButtons.find(
                        button =>
                            parseInt(
                                button.getAttribute(
                                    'data-page'
                                ),
                                10
                            ) === item
                    );

                if (targetButton) {
                    targetButton.classList.remove(
                        'hidden'
                    );
                }
            });


            // Chèn "..." vào những khoảng trống.
            for (
                let i = 0;
                i < pageList.length;
                i++
            ) {

                if (pageList[i] !== '...') {
                    continue;
                }

                const nextPage =
                    pageList[i + 1];

                const nextButton =
                    allButtons.find(
                        button =>
                            parseInt(
                                button.getAttribute(
                                    'data-page'
                                ),
                                10
                            ) === nextPage
                    );

                if (nextButton) {

                    nextButton.before(
                        makeEllipsis()
                    );
                }
            }
        }


        function debounce(
            functionToExecute,
            milliseconds
        ) {

            let timer = null;

            return function (...args) {

                clearTimeout(timer);

                timer = setTimeout(
                    () => {
                        functionToExecute.apply(
                            this,
                            args
                        );
                    },
                    milliseconds
                );
            };
        }


        // Khởi chạy ngay.
        renderResponsivePagination();


        // Resize liên tục sẽ tính lại pagination.
        window.addEventListener(
            'resize',
            debounce(
                renderResponsivePagination,
                150
            )
        );
    }



    // Đồng bộ state khi trang được load


    syncPageSizeFromUrl();
    syncSortFromUrl();
});


/**
 * Browser Back / Forward.
 *
 * Browser có thể khôi phục lại giá trị
 * của <select> từ history/BFCache.
 *
 * Vì vậy phải lấy lại state từ URL.
 */
window.addEventListener('pageshow', () => {

    const itemsPerPage =
        document.getElementById('itemsPerPage');

    const sortBy =
        document.getElementById('sortBy');

    const url =
        new URL(window.location.href);


    // -------------------------
    // pageSize
    // -------------------------

    if (itemsPerPage) {

        const pageSize =
            url.searchParams.get('pageSize');

        if (pageSize) {

            const optionExists =
                [...itemsPerPage.options]
                    .some(
                        option =>
                            option.value === pageSize
                    );

            if (optionExists) {
                itemsPerPage.value = pageSize;
            }
        }
    }


    // -------------------------
    // sortBy
    // -------------------------

    if (sortBy) {

        const sortValue =
            url.searchParams.get('sortBy');

        if (!sortValue) {

            sortBy.value = 'default';

        } else {

            const optionExists =
                [...sortBy.options]
                    .some(
                        option =>
                            option.value === sortValue
                    );

            if (optionExists) {
                sortBy.value = sortValue;
            }
        }
    }
});
