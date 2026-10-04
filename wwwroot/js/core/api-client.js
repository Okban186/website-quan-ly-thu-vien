//Hàm tạo cấu hình cho json và gộp header chuẩn xác
function createJsonOptions(method, body, options = {}) {
    const { params, headers, ...restOptions } = options;
    return {
        method,
        ...restOptions, // Đưa lên đầu để tránh ghi đè
        params,
        headers: {
            'Content-Type': 'application/json',
            ...headers
        },
        body: body !== undefined && body !== null
            ? JSON.stringify(body)
            : undefined
    };
}

// Helper hỗ trợ tự động nối query params (?page=1&limit=10) vào URL cho Fetch API
function appendQueryParams(url, options) {
    if (options?.params && Object.keys(options.params).length > 0) {
        const queryString = new URLSearchParams(options.params).toString();
        return `${url}?${queryString}`;
    }
    return url;
}

// các hàm lõi gửi request
async function requestPublic(url, options = {}) {
    const finalUrl = appendQueryParams(url, options);
    return fetch(finalUrl, options);
}

async function requestPrivate(url, options = {}) {
    const finalUrl = appendQueryParams(url, options);
    return fetchWithAuth(finalUrl, options);
}

const apiClient = {
    public: {
        get(url, options = {}) {
            return requestPublic(url, createJsonOptions('GET', null, options));
        },
        post(url, body = null, options = {}) {
            return requestPublic(url, createJsonOptions('POST', body, options));
        }
    },

    private: {
        get(url, options = {}) {
            return requestPrivate(url, createJsonOptions('GET', null, options));
        },
        post(url, body = null, options = {}) {
            return requestPrivate(url, createJsonOptions('POST', body, options));
        },
        put(url, body = null, options = {}) {
            return requestPrivate(url, createJsonOptions('PUT', body, options));
        },
        delete(url, options = {}) {
            return requestPrivate(url, createJsonOptions('DELETE', null, options));
        }
    }
};
