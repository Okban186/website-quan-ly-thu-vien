/* =========================================================
   1. STORAGE FILES
   ========================================================= */

CREATE TABLE storage_files (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_storage_files PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    content_type NVARCHAR(100) NULL,
    file_size BIGINT NULL,
    object_key NVARCHAR(500) NOT NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_storage_files_created_at
        DEFAULT SYSUTCDATETIME()
);

CREATE UNIQUE INDEX UX_storage_files_object_key
ON storage_files(object_key);


/* =========================================================
   2. USERS
   ========================================================= */

CREATE TABLE users (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_users PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    username NVARCHAR(100) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    password_hash NVARCHAR(500) NOT NULL,

    full_name NVARCHAR(255) NOT NULL,
    phone NVARCHAR(20) NULL,
    date_of_birth DATE NULL,
    citizen_id NVARCHAR(20) NULL,
    gender NVARCHAR(20) NULL,
    address NVARCHAR(500) NULL,

    avatar_file_id UNIQUEIDENTIFIER NULL,

    status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_users_status DEFAULT N'ACTIVE',

    created_at DATETIME2 NOT NULL
        CONSTRAINT DF_users_created_at DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2 NOT NULL
        CONSTRAINT DF_users_updated_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_users_username UNIQUE (username),
    CONSTRAINT UQ_users_email UNIQUE (email),
    CONSTRAINT UQ_users_citizen_id UNIQUE (citizen_id),

    CONSTRAINT FK_users_avatar_file
        FOREIGN KEY (avatar_file_id)
        REFERENCES storage_files(id)
);

/* =========================================================
   3. ROLES
   ========================================================= */

CREATE TABLE roles (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_roles PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(500) NULL,

    CONSTRAINT UQ_roles_name UNIQUE (name)
);


/* =========================================================
   4. USER ROLES
   ========================================================= */

CREATE TABLE user_roles (
    user_id UNIQUEIDENTIFIER NOT NULL,
    role_id UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_user_roles
        PRIMARY KEY (user_id, role_id),

    CONSTRAINT FK_user_roles_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_user_roles_role
        FOREIGN KEY (role_id)
        REFERENCES roles(id)
        ON DELETE CASCADE
);


/* =========================================================
   5. STAFF
   ========================================================= */

CREATE TABLE staff (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_staff PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    user_id UNIQUEIDENTIFIER NOT NULL,

    employee_code NVARCHAR(50) NOT NULL,

    created_at DATETIME2 NOT NULL
        CONSTRAINT DF_staff_created_at DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2 NOT NULL
        CONSTRAINT DF_staff_updated_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_staff_user_id UNIQUE (user_id),
    CONSTRAINT UQ_staff_employee_code UNIQUE (employee_code),

    CONSTRAINT FK_staff_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
);


/* =========================================================
   6. LIBRARY MEMBERS
   ========================================================= */

CREATE TABLE library_members (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_library_members PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    user_id UNIQUEIDENTIFIER NOT NULL,

    membership_status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_library_members_membership_status
        DEFAULT N'ACTIVE',

    registered_at DATETIME2 NOT NULL
        CONSTRAINT DF_library_members_registered_at
        DEFAULT SYSUTCDATETIME(),

    expired_at DATETIME2 NULL,

    CONSTRAINT UQ_library_members_user_id UNIQUE (user_id),

    CONSTRAINT FK_library_members_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
);


/* =========================================================
   7. LIBRARY CARDS
   ========================================================= */

CREATE TABLE library_cards (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_library_cards PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    member_id UNIQUEIDENTIFIER NOT NULL,

    card_number NVARCHAR(50) NOT NULL,

    issued_at DATETIME2(7) NULL,
    expired_at DATETIME2(7) NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_library_cards_status DEFAULT 'PENDING',

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_library_cards_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_library_cards_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_library_cards_member UNIQUE (member_id),
    CONSTRAINT UQ_library_cards_number UNIQUE (card_number),

    CONSTRAINT FK_library_cards_member
        FOREIGN KEY (member_id)
        REFERENCES library_members(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_library_cards_status
        CHECK (
            status IN (
                'PENDING',
                'ACTIVE',
                'BLOCKED',
                'EXPIRED',
                'CANCELLED'
            )
        )
);


/* =========================================================
   8. PUBLISHERS
   ========================================================= */

CREATE TABLE publishers (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_publishers PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(255) NOT NULL,
    address NVARCHAR(500) NULL,
    phone NVARCHAR(30) NULL,
    email NVARCHAR(255) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_publishers_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_publishers_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_publishers_name UNIQUE (name)
);


/* =========================================================
   9. LOCATIONS
   ========================================================= */

CREATE TABLE locations (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_locations PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(500) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_locations_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_locations_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_locations_name UNIQUE (name)
);


/* =========================================================
   10. DOCUMENT TYPES
   ========================================================= */

CREATE TABLE document_types (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_document_types PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(150) NOT NULL,
    description NVARCHAR(500) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_document_types_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_document_types_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_document_types_name UNIQUE (name)
);


/* =========================================================
   11. RESOURCES
   ========================================================= */

CREATE TABLE resources (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_resources PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    isbn NVARCHAR(20) NULL,

    title NVARCHAR(500) NOT NULL,
    description NVARCHAR(MAX) NULL,

    physical_description NVARCHAR(1000),

    publication_year INT NULL,
    language NVARCHAR(50) NULL,
    page_count INT NULL,

    price DECIMAL(12, 2) NULL,

    publisher_id UNIQUEIDENTIFIER NULL,
    document_type_id UNIQUEIDENTIFIER NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_resources_status DEFAULT 'ACTIVE',

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_resources_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_resources_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_resources_isbn UNIQUE (isbn),

    CONSTRAINT FK_resources_publisher
        FOREIGN KEY (publisher_id)
        REFERENCES publishers(id)
        ON DELETE SET NULL,

    CONSTRAINT FK_resources_document_type
        FOREIGN KEY (document_type_id)
        REFERENCES document_types(id)
        ON DELETE SET NULL,

    CONSTRAINT CK_resources_status
        CHECK (status IN ('ACTIVE', 'INACTIVE'))
);


/* =========================================================
   12. LIBRARY ITEMS
   ========================================================= */

CREATE TABLE library_items (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_library_items PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    resource_id UNIQUEIDENTIFIER NOT NULL,
    location_id UNIQUEIDENTIFIER NULL,

    barcode NVARCHAR(100) NOT NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_library_items_status DEFAULT 'AVAILABLE',

    item_condition NVARCHAR(20) NOT NULL
        CONSTRAINT DF_library_items_condition DEFAULT 'GOOD',

    acquired_at DATETIME2(7) NULL,
    acquisition_price DECIMAL(12, 2) NULL,
    cover_price DECIMAL(12, 2) NULL,
    edition_number INT NULL,
    publication_year INT NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_library_items_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_library_items_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_library_items_barcode UNIQUE (barcode),

    CONSTRAINT FK_library_items_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_library_items_location
        FOREIGN KEY (location_id)
        REFERENCES locations(id)
        ON DELETE SET NULL,

    CONSTRAINT CK_library_items_status
        CHECK (
            status IN (
                'AVAILABLE',
                'RESERVED',
                'BORROWED',
                'RETURNED_PENDING_CHECK',
                'LOST',
                'DAMAGED',
                'MAINTENANCE',
                'REMOVED'
            )
        ),
    CONSTRAINT CK_library_items_price CHECK (acquisition_price >= 0 AND cover_price >= 0),

    CONSTRAINT CK_library_items_condition
        CHECK (
            item_condition IN (
                'NEW',
                'GOOD',
                'WORN',
                'DAMAGED'
            )
        )
)


/* =========================================================
   13. CATEGORIES
   ========================================================= */

CREATE TABLE categories (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_categories PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(150) NOT NULL,
    description NVARCHAR(500) NULL,

    display_order INT NOT NULL
        CONSTRAINT DF_categories_display_order DEFAULT 0,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_categories_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_categories_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_categories_name UNIQUE (name)
);


/* =========================================================
   14. RESOURCE CATEGORIES
   ========================================================= */

CREATE TABLE resource_categories (
    resource_id UNIQUEIDENTIFIER NOT NULL,
    category_id UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_resource_categories
        PRIMARY KEY (resource_id, category_id),

    CONSTRAINT FK_resource_categories_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_resource_categories_category
        FOREIGN KEY (category_id)
        REFERENCES categories(id)
        ON DELETE CASCADE
);


/* =========================================================
   15. AUTHORS
   ========================================================= */

CREATE TABLE authors (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_authors PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    name NVARCHAR(255) NOT NULL,
    biography NVARCHAR(MAX) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_authors_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_authors_updated_at
        DEFAULT SYSUTCDATETIME()
);


/* =========================================================
   16. RESOURCE AUTHORS
   ========================================================= */

CREATE TABLE resource_authors (
    resource_id UNIQUEIDENTIFIER NOT NULL,
    author_id UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_resource_authors
        PRIMARY KEY (resource_id, author_id),

    CONSTRAINT FK_resource_authors_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_resource_authors_author
        FOREIGN KEY (author_id)
        REFERENCES authors(id)
        ON DELETE CASCADE
);


/* =========================================================
   17. BORROW REQUESTS
   ========================================================= */

CREATE TABLE borrow_requests (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_borrow_requests PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    member_id UNIQUEIDENTIFIER NOT NULL,

    request_type NVARCHAR(20) NOT NULL,
    status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_borrow_requests_status DEFAULT 'PENDING',

    processed_by UNIQUEIDENTIFIER NULL,

    note NVARCHAR(1000) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_borrow_requests_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_borrow_requests_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_borrow_requests_member
        FOREIGN KEY (member_id)
        REFERENCES library_members(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_borrow_requests_processed_by
        FOREIGN KEY (processed_by)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_borrow_requests_type
        CHECK (
            request_type IN (
                'RESERVATION',
                'DELIVERY'
            )
        ),

    CONSTRAINT CK_borrow_requests_status
        CHECK (
            status IN (
                'PENDING',
                'APPROVED',
                'READY_FOR_PICKUP',
                'DELIVERING',
                'COMPLETED',
                'REJECTED',
                'CANCELLED',
                'EXPIRED'
            )
        )
);


/* =========================================================
   18. BORROW REQUEST ITEMS
   ========================================================= */

CREATE TABLE borrow_request_items (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_borrow_request_items PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    borrow_request_id UNIQUEIDENTIFIER NOT NULL,

    resource_id UNIQUEIDENTIFIER NOT NULL,

    /*
        NULL khi người dùng tạo request.
        Thủ thư mới chọn library_item cụ thể.
    */
    library_item_id UNIQUEIDENTIFIER NULL,

    status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_borrow_request_items_status DEFAULT 'PENDING',

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_borrow_request_items_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_borrow_request_items_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_borrow_request_items_request
        FOREIGN KEY (borrow_request_id)
        REFERENCES borrow_requests(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_borrow_request_items_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_borrow_request_items_library_item
        FOREIGN KEY (library_item_id)
        REFERENCES library_items(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_borrow_request_items_status
        CHECK (
            status IN (
                'PENDING',
                'APPROVED',
                'COMPLETED',
                'REJECTED',
                'CANCELLED'
            )
        )
);


/* =========================================================
   19. DELIVERIES
   ========================================================= */

CREATE TABLE deliveries (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_deliveries PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    borrow_request_id UNIQUEIDENTIFIER NOT NULL,

    address NVARCHAR(500) NOT NULL,
    recipient_name NVARCHAR(255) NOT NULL,
    recipient_phone NVARCHAR(30) NOT NULL,

    status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_deliveries_status DEFAULT 'PENDING',

    delivered_at DATETIME2(7) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_deliveries_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_deliveries_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_deliveries_borrow_request
        UNIQUE (borrow_request_id),

    CONSTRAINT FK_deliveries_borrow_request
        FOREIGN KEY (borrow_request_id)
        REFERENCES borrow_requests(id)
        ON DELETE NO ACTION
);


/* =========================================================
   20. LOANS
   ========================================================= */

CREATE TABLE loans (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_loans PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    member_id UNIQUEIDENTIFIER NOT NULL,

    borrow_request_id UNIQUEIDENTIFIER NULL,

    created_by UNIQUEIDENTIFIER NOT NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_loans_status DEFAULT 'ACTIVE',

    borrowed_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loans_borrowed_at
        DEFAULT SYSUTCDATETIME(),

    completed_at DATETIME2(7) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loans_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loans_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_loans_member
        FOREIGN KEY (member_id)
        REFERENCES library_members(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_loans_borrow_request
        FOREIGN KEY (borrow_request_id)
        REFERENCES borrow_requests(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_loans_created_by
        FOREIGN KEY (created_by)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_loans_status
        CHECK (
            status IN (
                'ACTIVE',
                'COMPLETED',
                'CANCELLED'
            )
        )
);


/* =========================================================
   21. LOAN ITEMS
   ========================================================= */

CREATE TABLE loan_items (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_loan_items PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    loan_id UNIQUEIDENTIFIER NOT NULL,

    library_item_id UNIQUEIDENTIFIER NOT NULL,

    due_at DATETIME2(7) NOT NULL,
    returned_at DATETIME2(7) NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_loan_items_status DEFAULT 'BORROWED',

    condition_at_loan NVARCHAR(20) NULL,
    condition_at_return NVARCHAR(20) NULL,
    condition_note_at_loan NVARCHAR(MAX) NULL,
    condition_note_at_return NVARCHAR(MAX) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loan_items_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loan_items_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_loan_items_loan
        FOREIGN KEY (loan_id)
        REFERENCES loans(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_loan_items_library_item
        FOREIGN KEY (library_item_id)
        REFERENCES library_items(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_loan_items_status
        CHECK (
            status IN (
                'BORROWED',
                'OVERDUE',
                'RETURNED',
                'LOST'
            )
        ),

    CONSTRAINT CK_loan_items_condition_at_loan
CHECK (
    condition_at_loan IN (
        'NEW',
        'GOOD',
        'WORN',
        'DAMAGED'
    )
),

CONSTRAINT CK_loan_items_condition_at_return
CHECK (
    condition_at_return IS NULL
    OR condition_at_return IN (
        'NEW',
        'GOOD',
        'WORN',
        'DAMAGED'
    )
)
);


/* =========================================================
   22. LOAN RENEWALS
   ========================================================= */

CREATE TABLE loan_renewals (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_loan_renewals PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    loan_item_id UNIQUEIDENTIFIER NOT NULL,

    old_due_at DATETIME2(7) NOT NULL,
    new_due_at DATETIME2(7) NOT NULL,

    renewed_by UNIQUEIDENTIFIER NOT NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_loan_renewals_created_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_loan_renewals_loan_item
        FOREIGN KEY (loan_item_id)
        REFERENCES loan_items(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_loan_renewals_renewed_by
        FOREIGN KEY (renewed_by)
        REFERENCES users(id)
        ON DELETE NO ACTION
);


/* =========================================================
   23. CHARGES
   ========================================================= */

CREATE TABLE charges (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_charges PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    member_id UNIQUEIDENTIFIER NOT NULL,

    loan_item_id UNIQUEIDENTIFIER NULL,

    charge_type NVARCHAR(20) NOT NULL,

    amount DECIMAL(12, 2) NOT NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_charges_status DEFAULT 'UNPAID',

    description NVARCHAR(1000) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_charges_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_charges_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_charges_member
        FOREIGN KEY (member_id)
        REFERENCES library_members(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_charges_loan_item
        FOREIGN KEY (loan_item_id)
        REFERENCES loan_items(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_charges_type
        CHECK (
            charge_type IN (
                'CARD_FEE',
                'OVERDUE',
                'DAMAGED',
                'LOST'
            )
        ),

    CONSTRAINT CK_charges_status
        CHECK (
            status IN (
                'UNPAID',
                'PAID',
                'WAIVED'
            )
        ),

    CONSTRAINT CK_charges_amount
        CHECK (amount >= 0)
);


/* =========================================================
   24. PAYMENT TRANSACTIONS
   ========================================================= */

CREATE TABLE payment_transactions (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_payment_transactions PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    member_id UNIQUEIDENTIFIER NOT NULL,

    transaction_code NVARCHAR(100) NOT NULL,

    payment_method NVARCHAR(30) NOT NULL
        CONSTRAINT DF_payment_transactions_method
        DEFAULT 'ONLINE_PAYMENT',

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_payment_transactions_status
        DEFAULT 'PENDING',

    total_amount DECIMAL(12, 2) NOT NULL,

    expired_at DATETIME2(7) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_payment_transactions_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_payment_transactions_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_payment_transactions_code
        UNIQUE (transaction_code),

    CONSTRAINT FK_payment_transactions_member
        FOREIGN KEY (member_id)
        REFERENCES library_members(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_payment_transactions_method
        CHECK (
            payment_method IN ('ONLINE_PAYMENT')
        ),

    CONSTRAINT CK_payment_transactions_status
        CHECK (
            status IN (
                'PENDING',
                'PROCESSING',
                'PAID',
                'FAILED',
                'CANCELLED'
            )
        ),

    CONSTRAINT CK_payment_transactions_amount
        CHECK (total_amount >= 0)
);


/* =========================================================
   25. PAYMENT ITEMS
   ========================================================= */

CREATE TABLE payment_items (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_payment_items PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    payment_id UNIQUEIDENTIFIER NOT NULL,
    charge_id UNIQUEIDENTIFIER NOT NULL,

    amount DECIMAL(12, 2) NOT NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_payment_items_created_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_payment_items_payment_charge
        UNIQUE (payment_id, charge_id),

    CONSTRAINT FK_payment_items_payment
        FOREIGN KEY (payment_id)
        REFERENCES payment_transactions(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_payment_items_charge
        FOREIGN KEY (charge_id)
        REFERENCES charges(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_payment_items_amount
        CHECK (amount >= 0)
);


/* =========================================================
   26. CARD REGISTRATIONS
   ========================================================= */

CREATE TABLE card_registrations (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_card_registrations PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    user_id UNIQUEIDENTIFIER NOT NULL,

    payment_transaction_id UNIQUEIDENTIFIER NULL,

    full_name NVARCHAR(255) NULL,
    email NVARCHAR(255) NULL,
    phone NVARCHAR(30) NULL,
    id_document_number NVARCHAR(100) NULL,

    id_document_front_file_id UNIQUEIDENTIFIER NULL,
    id_document_back_file_id UNIQUEIDENTIFIER NULL,
    avatar_file_id UNIQUEIDENTIFIER NULL,

    status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_card_registrations_status
        DEFAULT 'WAITING_FOR_INFORMATION',

    registration_token_hash NVARCHAR(64) NULL,
    registration_token_expired_at DATETIME2(7) NULL,
    registration_token_revoked_at DATETIME2(7) NULL,

    submitted_at DATETIME2(7) NULL,

    reviewed_by UNIQUEIDENTIFIER NULL,
    reviewed_at DATETIME2(7) NULL,
    rejection_reason NVARCHAR(1000) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_card_registrations_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_card_registrations_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_card_registrations_payment
        UNIQUE (payment_transaction_id),

    CONSTRAINT UQ_card_registrations_token
        UNIQUE (registration_token_hash),

    CONSTRAINT FK_card_registrations_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_card_registrations_payment
        FOREIGN KEY (payment_transaction_id)
        REFERENCES payment_transactions(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_card_registrations_front_file
        FOREIGN KEY (id_document_front_file_id)
        REFERENCES storage_files(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_card_registrations_back_file
        FOREIGN KEY (id_document_back_file_id)
        REFERENCES storage_files(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_card_registrations_avatar_file
        FOREIGN KEY (avatar_file_id)
        REFERENCES storage_files(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_card_registrations_reviewer
        FOREIGN KEY (reviewed_by)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_card_registrations_status
        CHECK (
            status IN (
                'WAITING_FOR_INFORMATION',
                'PENDING',
                'APPROVED',
                'REJECTED',
                'CANCELLED',
                'EXPIRED'
            )
        )
);


/* =========================================================
   27. RESOURCE IMAGES
   ========================================================= */

CREATE TABLE resource_images (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_resource_images PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    resource_id UNIQUEIDENTIFIER NOT NULL,
    file_id UNIQUEIDENTIFIER NOT NULL,

    is_primary BIT NOT NULL
        CONSTRAINT DF_resource_images_is_primary DEFAULT 0,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_resource_images_created_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_resource_images_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_resource_images_file
        FOREIGN KEY (file_id)
        REFERENCES storage_files(id)
        ON DELETE NO ACTION
);


/*
    Mỗi Resource chỉ có tối đa một ảnh primary.
*/
CREATE UNIQUE INDEX UX_resource_images_primary
ON resource_images(resource_id)
WHERE is_primary = 1;


/* =========================================================
   28. DIGITAL RESOURCES
   ========================================================= */

CREATE TABLE digital_resources (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_digital_resources PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    resource_id UNIQUEIDENTIFIER NOT NULL,
    file_id UNIQUEIDENTIFIER NOT NULL,
    display_order INT NOT NULL DEFAULT 1,

    resource_type NVARCHAR(30) NOT NULL,

    access_level NVARCHAR(20) NOT NULL
        CONSTRAINT DF_digital_resources_access_level
        DEFAULT 'MEMBER',

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_digital_resources_status
        DEFAULT 'ACTIVE',

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_digital_resources_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_digital_resources_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_digital_resources_resource
        FOREIGN KEY (resource_id)
        REFERENCES resources(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_digital_resources_file
        FOREIGN KEY (file_id)
        REFERENCES storage_files(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_digital_resources_type
        CHECK (
            resource_type IN (
                'EBOOK',
                'DOCUMENT',
                'AUDIOBOOK',
                'OTHER'
            )
        ),

    CONSTRAINT CK_digital_resources_access_level
        CHECK (
            access_level IN (
                'PUBLIC',
                'MEMBER',
                'STAFF',
                'HIDDEN'
            )
        ),

    CONSTRAINT CK_digital_resources_status
        CHECK (
            status IN (
                'ACTIVE',
                'INACTIVE'
            )
        )

    CONSTRAINT UQ_digital_resources_order
        UNIQUE (
        resource_id,
        resource_type,
        display_order
)
);


/* =========================================================
   29. NOTIFICATIONS
   ========================================================= */

CREATE TABLE notifications (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_notifications PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    title NVARCHAR(255) NOT NULL,
    message NVARCHAR(1000) NOT NULL,

    notification_type NVARCHAR(50) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_notifications_created_at
        DEFAULT SYSUTCDATETIME()
);


/* =========================================================
   30. NOTIFICATION RECIPIENTS
   ========================================================= */

CREATE TABLE notification_recipients (
    notification_id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NOT NULL,

    is_read BIT NOT NULL
        CONSTRAINT DF_notification_recipients_is_read DEFAULT 0,

    read_at DATETIME2(7) NULL,

    CONSTRAINT PK_notification_recipients
        PRIMARY KEY (notification_id, user_id),

    CONSTRAINT FK_notification_recipients_notification
        FOREIGN KEY (notification_id)
        REFERENCES notifications(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_notification_recipients_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);


/* =========================================================
   31. COPY ISSUES
   ========================================================= */

CREATE TABLE copy_issues (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_copy_issues PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    library_item_id UNIQUEIDENTIFIER NOT NULL,

    loan_item_id UNIQUEIDENTIFIER NULL,

    reported_by UNIQUEIDENTIFIER NOT NULL,

    issue_type NVARCHAR(30) NOT NULL,
    description NVARCHAR(MAX) NULL,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_copy_issues_status DEFAULT 'OPEN',

    resolved_at DATETIME2(7) NULL,
    resolved_by UNIQUEIDENTIFIER NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_copy_issues_created_at
        DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_copy_issues_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_copy_issues_library_item
        FOREIGN KEY (library_item_id)
        REFERENCES library_items(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_copy_issues_reported_by
        FOREIGN KEY (reported_by)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_copy_issues_resolved_by
        FOREIGN KEY (resolved_by)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_copy_issues_status
        CHECK (
            status IN (
                'OPEN',
                'IN_PROGRESS',
                'RESOLVED',
                'CANCELLED'
            )
        )
);


/* =========================================================
   32. LIBRARY ITEM STATUS HISTORY
   ========================================================= */

CREATE TABLE library_item_status_history (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_library_item_status_history PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    library_item_id UNIQUEIDENTIFIER NOT NULL,

    old_status NVARCHAR(20) NULL,
    new_status NVARCHAR(20) NOT NULL,

    changed_by UNIQUEIDENTIFIER NULL,
    reason NVARCHAR(1000) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_library_item_status_history_created_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_library_item_status_history_item
        FOREIGN KEY (library_item_id)
        REFERENCES library_items(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_library_item_status_history_user
        FOREIGN KEY (changed_by)
        REFERENCES users(id)
        ON DELETE NO ACTION
);


/* =========================================================
   33. UPLOAD SESSIONS
   ========================================================= */

CREATE TABLE upload_sessions (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_upload_sessions PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    user_id UNIQUEIDENTIFIER NULL,
    card_registration_id UNIQUEIDENTIFIER NULL,

    object_key NVARCHAR(500) NOT NULL,

    upload_type NVARCHAR(20) ,

    status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_upload_sessions_status DEFAULT 'PENDING',

    expires_at DATETIME2(7) NULL,

    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_upload_sessions_created_at
        DEFAULT SYSUTCDATETIME(),

    completed_at DATETIME2(7) NULL,

    updated_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_upload_sessions_updated_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_upload_sessions_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_upload_sessions_card_registration
        FOREIGN KEY (card_registration_id)
        REFERENCES card_registrations(id)
        ON DELETE NO ACTION,

    CONSTRAINT CK_upload_sessions_status
        CHECK (
            status IN (
                'PENDING',
                'COMPLETED',
                'EXPIRED',
                'FAILED'
            )
        )
);


/* =========================================================
   34. REFRESH TOKENS
   ========================================================= */

CREATE TABLE refresh_tokens (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_refresh_tokens PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    user_id UNIQUEIDENTIFIER NOT NULL,

    token_hash NVARCHAR(64) NOT NULL,

    family_id UNIQUEIDENTIFIER NOT NULL,

    expires_at DATETIME2(7) NOT NULL,
    created_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_refresh_tokens_created_at
        DEFAULT SYSUTCDATETIME(),

    revoked_at DATETIME2(7) NULL,

    replaced_by UNIQUEIDENTIFIER NULL,

    CONSTRAINT UQ_refresh_tokens_token_hash
        UNIQUE (token_hash),

    CONSTRAINT FK_refresh_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT FK_refresh_tokens_replaced_by
        FOREIGN KEY (replaced_by)
        REFERENCES refresh_tokens(id)
        ON DELETE NO ACTION
);


/* =========================================================
   35. REVOKED TOKENS
   ========================================================= */

CREATE TABLE revoked_tokens (
    id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_revoked_tokens PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    jti UNIQUEIDENTIFIER NOT NULL,

    user_id UNIQUEIDENTIFIER NOT NULL,

    expires_at DATETIME2(7) NOT NULL,

    revoked_at DATETIME2(7) NOT NULL
        CONSTRAINT DF_revoked_tokens_revoked_at
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_revoked_tokens_jti UNIQUE (jti),

    CONSTRAINT FK_revoked_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE NO ACTION
);


CREATE TABLE import_batches (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

    storage_file_id UNIQUEIDENTIFIER NOT NULL,

    import_type VARCHAR(50) NOT NULL DEFAULT 'LIBRARY_ITEM',

    status VARCHAR(30) NOT NULL DEFAULT 'PENDING',

    total_rows INTEGER NOT NULL DEFAULT 0,

    success_rows INTEGER NOT NULL DEFAULT 0,

    failed_rows INTEGER NOT NULL DEFAULT 0,

    created_by UNIQUEIDENTIFIER NOT NULL,

    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    started_at DATETIME2(7),

    completed_at DATETIME2(7),

    error_message TEXT,

    CONSTRAINT fk_import_batches_storage_file
        FOREIGN KEY (storage_file_id)
        REFERENCES storage_files(id),

    CONSTRAINT fk_import_batches_created_by
        FOREIGN KEY (created_by)
        REFERENCES users(id),

    CONSTRAINT chk_import_batches_type
        CHECK (
            import_type IN (
                'LIBRARY_ITEM'
            )
        ),

    CONSTRAINT chk_import_batches_status
        CHECK (
            status IN (
                'PENDING',
                'PROCESSING',
                'COMPLETED',
                'COMPLETED_WITH_ERRORS',
                'FAILED',
                'CANCELLED'
            )
        ),

    CONSTRAINT chk_import_batches_total_rows
        CHECK (total_rows >= 0),

    CONSTRAINT chk_import_batches_success_rows
        CHECK (success_rows >= 0),

    CONSTRAINT chk_import_batches_failed_rows
        CHECK (failed_rows >= 0),

    CONSTRAINT chk_import_batches_row_count
        CHECK (
            success_rows + failed_rows <= total_rows
        ),

    CONSTRAINT chk_import_batches_completed_at
        CHECK (
            completed_at IS NULL
            OR started_at IS NULL
            OR completed_at >= started_at
        )
);

CREATE INDEX ix_import_batches_storage_file_id
    ON import_batches(storage_file_id);

CREATE INDEX ix_import_batches_created_by
    ON import_batches(created_by);

CREATE INDEX ix_import_batches_created_at
    ON import_batches(created_at DESC);

CREATE INDEX ix_import_batches_status
    ON import_batches(status);




/* =========================================================
   IMPORTANT BUSINESS INDEXES
   ========================================================= */


/* =========================================================
   RESOURCES
   ========================================================= */

/*
    Tìm kiếm tài liệu theo tiêu đề.
*/
CREATE INDEX IX_resources_title
ON resources(title);


/*
    Lọc danh sách tài liệu theo loại.
*/
CREATE INDEX IX_resources_document_type_id
ON resources(document_type_id);


/*
    Lọc tài liệu theo nhà xuất bản.
*/
CREATE INDEX IX_resources_publisher_id
ON resources(publisher_id);


/*
    Lấy danh sách tài liệu theo trạng thái.
*/
CREATE INDEX IX_resources_status
ON resources(status);


/* =========================================================
   LIBRARY ITEMS
   ========================================================= */

/*
    Lấy các bản vật lý của một Resource.
*/
CREATE INDEX IX_library_items_resource_id
ON library_items(resource_id);


/*
    Tìm các bản AVAILABLE của một Resource.
    
    Đây là query rất quan trọng khi thủ thư
    phân bổ bản sách cho borrow request.
*/
CREATE INDEX IX_library_items_resource_status
ON library_items(resource_id, status);


/*
    Tìm các bản đang nằm tại một location.
*/
CREATE INDEX IX_library_items_location_id
ON library_items(location_id);


/* =========================================================
   RESOURCE CATEGORIES
   ========================================================= */

/*
    PK(resource_id, category_id) đã hỗ trợ:
        Resource -> Categories

    Index này hỗ trợ chiều ngược:
        Category -> Resources
*/
CREATE INDEX IX_resource_categories_category_id
ON resource_categories(category_id);


/* =========================================================
   RESOURCE AUTHORS
   ========================================================= */

/*
    PK(resource_id, author_id) đã hỗ trợ:
        Resource -> Authors

    Index này hỗ trợ:
        Author -> Resources
*/
CREATE INDEX IX_resource_authors_author_id
ON resource_authors(author_id);


/* =========================================================
   RESOURCE IMAGES
   ========================================================= */

/*
    Lấy toàn bộ hình ảnh của Resource.
*/
CREATE INDEX IX_resource_images_resource_id
ON resource_images(resource_id);


/* =========================================================
   DIGITAL RESOURCES
   ========================================================= */

/*
    Lấy ebook/audiobook/document của một Resource.
*/
CREATE INDEX IX_digital_resources_resource_id
ON digital_resources(resource_id);


/*
    Lấy Resource theo loại tài nguyên số.
    
    Ví dụ:
        EBOOK
        AUDIOBOOK
*/
CREATE INDEX IX_digital_resources_resource_type
ON digital_resources(resource_type);


/* =========================================================
   BORROW REQUESTS
   ========================================================= */

/*
    Lịch sử request của một member.
*/
CREATE INDEX IX_borrow_requests_member_id
ON borrow_requests(member_id);


/*
    Queue request cho thủ thư xử lý.
    
    Ví dụ:
        WHERE status = 'PENDING'
*/
CREATE INDEX IX_borrow_requests_status
ON borrow_requests(status);


/* =========================================================
   BORROW REQUEST ITEMS
   ========================================================= */

/*
    Lấy các item thuộc một request.
*/
CREATE INDEX IX_borrow_request_items_request_id
ON borrow_request_items(borrow_request_id);


/*
    Tìm các request đang yêu cầu một Resource.
*/
CREATE INDEX IX_borrow_request_items_resource_id
ON borrow_request_items(resource_id);


/*
    Tìm request đã được phân bổ cho LibraryItem.
*/
CREATE INDEX IX_borrow_request_items_library_item_id
ON borrow_request_items(library_item_id);


/* =========================================================
   LOANS
   ========================================================= */

/*
    Lịch sử mượn của member.
*/
CREATE INDEX IX_loans_member_id
ON loans(member_id);


/*
    Queue các loan đang ACTIVE.
*/
CREATE INDEX IX_loans_status
ON loans(status);


/* =========================================================
   LOAN ITEMS
   ========================================================= */

/*
    Lấy các item của một loan.
*/
CREATE INDEX IX_loan_items_loan_id
ON loan_items(loan_id);


/*
    Kiểm tra lịch sử mượn của một LibraryItem.
    
    Quan trọng vì một LibraryItem có thể được
    mượn nhiều lần trong lịch sử.
*/
CREATE INDEX IX_loan_items_library_item_id
ON loan_items(library_item_id);


/*
    Tìm các sách đang BORROWED / OVERDUE.
*/
CREATE INDEX IX_loan_items_status
ON loan_items(status);


/*
    Tìm các khoản sắp quá hạn / đã quá hạn.
*/
CREATE INDEX IX_loan_items_due_at
ON loan_items(due_at);


/* =========================================================
   CHARGES
   ========================================================= */

/*
    Lấy các khoản phí của member.
*/
CREATE INDEX IX_charges_member_id
ON charges(member_id);


/*
    Tìm các khoản UNPAID / PAID / WAIVED.
*/
CREATE INDEX IX_charges_status
ON charges(status);


/* =========================================================
   PAYMENT TRANSACTIONS
   ========================================================= */

/*
    Lịch sử thanh toán của member.
*/
CREATE INDEX IX_payment_transactions_member_id
ON payment_transactions(member_id);


/*
    Tìm payment theo trạng thái.
    
    Đặc biệt hữu ích cho:
        PENDING
        PROCESSING
*/
CREATE INDEX IX_payment_transactions_status
ON payment_transactions(status);


/* =========================================================
   PAYMENT ITEMS
   ========================================================= */

/*
    Tìm các payment liên quan đến một charge.
    
    Không UNIQUE vì một charge có thể xuất hiện
    trong nhiều lần retry payment.
*/
CREATE INDEX IX_payment_items_charge_id
ON payment_items(charge_id);


/* =========================================================
   CARD REGISTRATIONS
   ========================================================= */

/*
    Lịch sử đăng ký thẻ của user.
*/
CREATE INDEX IX_card_registrations_user_id
ON card_registrations(user_id);


/*
    Danh sách registration theo trạng thái.
    
    Ví dụ:
        PENDING
        WAITING_FOR_INFORMATION
*/
CREATE INDEX IX_card_registrations_status
ON card_registrations(status);


/* =========================================================
   NOTIFICATIONS
   ========================================================= */

/*
    Lấy notification của một user.
*/
CREATE INDEX IX_notification_recipients_user_id
ON notification_recipients(user_id);


/*
    Query rất thường gặp:
        Lấy notification chưa đọc của user.
*/
CREATE INDEX IX_notification_recipients_user_unread
ON notification_recipients(user_id, is_read);


/* =========================================================
   COPY ISSUES
   ========================================================= */

/*
    Lấy các issue của một LibraryItem.
*/
CREATE INDEX IX_copy_issues_library_item_id
ON copy_issues(library_item_id);


/*
    Tìm các issue đang OPEN.
*/
CREATE INDEX IX_copy_issues_status
ON copy_issues(status);


/* =========================================================
   LIBRARY ITEM STATUS HISTORY
   ========================================================= */

/*
    Lấy lịch sử trạng thái của một LibraryItem.
*/
CREATE INDEX IX_library_item_status_history_item_created_at
ON library_item_status_history(
    library_item_id,
    created_at
);


/* =========================================================
   UPLOAD SESSIONS
   ========================================================= */

/*
    Lấy upload session của user.
*/
CREATE INDEX IX_upload_sessions_user_id
ON upload_sessions(user_id);


/*
    Tìm session theo trạng thái + thời gian hết hạn.
*/
CREATE INDEX IX_upload_sessions_status_expired_at
ON upload_sessions(status, expires_at);


/* =========================================================
   REFRESH TOKENS
   ========================================================= */

/*
    Lấy refresh tokens của user.
*/
CREATE INDEX IX_refresh_tokens_user_id
ON refresh_tokens(user_id);


/*
    Quản lý token theo family.
*/
CREATE INDEX IX_refresh_tokens_family_id
ON refresh_tokens(family_id);


/*
    Cleanup token hết hạn.
*/
CREATE INDEX IX_refresh_tokens_expires_at
ON refresh_tokens(expires_at);


/* =========================================================
   REVOKED TOKENS
   ========================================================= */

/*
    Cleanup revoked JWT đã hết hạn.
*/
CREATE INDEX IX_revoked_tokens_expires_at
ON revoked_tokens(expires_at);
