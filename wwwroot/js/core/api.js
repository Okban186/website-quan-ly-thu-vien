const API = {
    public: {
        auth: {
            login: '/api/accounts/login'
        },
        resources: {
            search: '/api/resources/advance-search',
            copies: (id) => `/api/resources/${id}/copies`,
            suggestions: (id) => `/api/resources/${id}/suggestions`
        },

        authors: {
            search: '/api/authors/search'
        },

        publishers: {
            search: '/api/publishers/search'
        },

        categories: {
            search: '/api/categories/search'
        },

        documentTypes: {
            search: '/api/document-types/search'
        }
    },

    private: {
        auth: {
            logout: '/api/accounts/logout'
        }
    },

    system: {
        auth: {
            refresh: '/api/accounts/refresh'
        }
    }
};