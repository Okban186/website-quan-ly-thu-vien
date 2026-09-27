CREATE TABLE storage_files (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    file_key   NVARCHAR (MAX)   NOT NULL,
    mime_type  NVARCHAR (100)  ,
    file_size  BIGINT          ,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL
);


GO
CREATE TABLE users (
    id             UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    username       NVARCHAR (100)   NOT NULL UNIQUE,
    email          NVARCHAR (255)   NOT NULL UNIQUE,
    password_hash  NVARCHAR (MAX)   NOT NULL,
    full_name      NVARCHAR (150)   NOT NULL,
    phone          NVARCHAR (20)   ,
    date_of_birth  DATE            ,
    avatar_file_id UNIQUEIDENTIFIER NULL,
    status         NVARCHAR (20)    DEFAULT 'ACTIVE' NOT NULL,
    last_login_at  DATETIME2 (7)   ,
    created_at     DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at     DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT chk_users_status CHECK (status IN ('ACTIVE', 'LOCKED', 'DISABLED')),
    CONSTRAINT fk_users_avatar_file FOREIGN KEY (avatar_file_id) REFERENCES storage_files (id)
);

CREATE TABLE roles (
    id          UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name        NVARCHAR (50)    NOT NULL UNIQUE,
    description NVARCHAR (MAX)  
);


GO
CREATE TABLE user_roles (
    user_id UNIQUEIDENTIFIER NOT NULL,
    role_id UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE
);


GO
CREATE TABLE staff (
    id            UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    user_id       UNIQUEIDENTIFIER NOT NULL UNIQUE,
    employee_code NVARCHAR (50)    NOT NULL UNIQUE,
    citizen_id    NVARCHAR (20)    UNIQUE,
    created_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_staff_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE library_members (
    id                        UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    user_id                   UNIQUEIDENTIFIER NOT NULL UNIQUE,
    citizen_id                NVARCHAR (20)    NOT NULL UNIQUE,
    id_document_front_file_id UNIQUEIDENTIFIER NULL,
    id_document_back_file_id  UNIQUEIDENTIFIER NULL,
    created_at                DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at                DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_members_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_member_id_document_front FOREIGN KEY (id_document_front_file_id) REFERENCES storage_files (id),
    CONSTRAINT fk_member_id_document_back FOREIGN KEY (id_document_back_file_id) REFERENCES storage_files (id)
);




GO
CREATE TABLE library_cards (
    id             UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    member_id      UNIQUEIDENTIFIER NOT NULL UNIQUE,
    card_number    NVARCHAR (50)    NOT NULL UNIQUE,
    status         NVARCHAR (20)    DEFAULT 'ACTIVE' NOT NULL,
    issued_at      DATETIME2 (7)   ,
    activated_at   DATETIME2 (7)   ,
    expired_at     DATETIME2 (7)   ,
    blocked_at     DATETIME2 (7)   ,
    blocked_reason NVARCHAR (MAX)  ,
    created_at     DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at     DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_library_card_patron FOREIGN KEY (member_id) REFERENCES library_members (id),
    CONSTRAINT chk_library_card_status CHECK (status IN ('PENDING', 'ACTIVE', 'BLOCKED', 'EXPIRED', 'CANCELLED'))
);


GO
CREATE TABLE publishers (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name       NVARCHAR (255)   NOT NULL UNIQUE,
    address    NVARCHAR (MAX)  ,
    phone      NVARCHAR (20)   ,
    email      NVARCHAR (255)  ,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL
);


GO
CREATE TABLE locations (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    code       NVARCHAR (50)    NOT NULL UNIQUE,
    name       NVARCHAR (150)   NOT NULL,
    floor      INT              NOT NULL,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT chk_locations_floor CHECK (floor > 0)
);


GO
CREATE TABLE document_types (
    id          UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    name        NVARCHAR (100)   NOT NULL,
    description NVARCHAR (MAX)   NULL,
    created_at  DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at  DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT PK_document_types PRIMARY KEY (id),
    CONSTRAINT UQ_document_types_name UNIQUE (name)
);


GO
CREATE TABLE books (
    id               UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    isbn             NVARCHAR (20)    UNIQUE,
    title            NVARCHAR (500)   NOT NULL,
    description      NVARCHAR (MAX)  ,
    publication_year INT             ,
    language         NVARCHAR (50)   ,
    page_count       INT             ,
    price            DECIMAL (12, 2) ,
    publisher_id     UNIQUEIDENTIFIER,
    document_type_id UNIQUEIDENTIFIER,
    status           NVARCHAR (20)    DEFAULT 'AVAILABLE' NOT NULL,
    created_at       DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at       DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_books_publisher FOREIGN KEY (publisher_id) REFERENCES publishers (id) ON DELETE SET NULL,
    CONSTRAINT fk_books_document_type FOREIGN KEY (document_type_id) REFERENCES document_types (id) ON DELETE SET NULL,
    CONSTRAINT chk_books_page_count CHECK (page_count IS NULL
                                           OR page_count > 0),
    CONSTRAINT chk_books_price CHECK (price IS NULL
                                      OR price >= 0),
    CONSTRAINT chk_books_publication_year CHECK (publication_year IS NULL
                                                 OR publication_year BETWEEN 1000 AND 9999)
);


GO
CREATE TABLE book_copies (
    id                UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    book_id           UNIQUEIDENTIFIER NOT NULL,
    location_id       UNIQUEIDENTIFIER,
    barcode           NVARCHAR (100)   NOT NULL UNIQUE,
    status            NVARCHAR (20)    DEFAULT 'AVAILABLE' NOT NULL,
    condition         NVARCHAR (20)    DEFAULT 'GOOD' NOT NULL,
    acquired_at       DATE            ,
    acquisition_price DECIMAL (12, 2) ,
    created_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_book_copies_book FOREIGN KEY (book_id) REFERENCES books (id),
    CONSTRAINT fk_book_copies_location FOREIGN KEY (location_id) REFERENCES locations (id) ON DELETE SET NULL,
    CONSTRAINT chk_book_copy_status CHECK (status IN ('AVAILABLE', 'RESERVED', 'BORROWED', 'LOST', 'DAMAGED', 'MAINTENANCE', 'REMOVED')),
    CONSTRAINT chk_book_copy_condition CHECK (condition IN ('NEW', 'GOOD', 'WORN', 'DAMAGED')),
    CONSTRAINT chk_acquisition_price CHECK (acquisition_price IS NULL
                                            OR acquisition_price >= 0)
);


GO
CREATE TABLE categories (
    id            UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name          NVARCHAR (150)   NOT NULL UNIQUE,
    description   NVARCHAR (MAX)  ,
    display_order INT              DEFAULT 0 NOT NULL,
    created_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL
);


GO
CREATE TABLE book_categories (
    book_id     UNIQUEIDENTIFIER NOT NULL,
    category_id UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (book_id, category_id),
    CONSTRAINT fk_book_categories_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,
    CONSTRAINT fk_book_categories_category FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE
);


GO
CREATE TABLE authors (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    name       NVARCHAR (255)   NOT NULL,
    biography  NVARCHAR (MAX)  ,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL
);


GO
CREATE TABLE book_authors (
    book_id   UNIQUEIDENTIFIER NOT NULL,
    author_id UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (book_id, author_id),
    CONSTRAINT fk_book_authors_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,
    CONSTRAINT fk_book_authors_author FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE
);


GO
CREATE TABLE borrow_requests (
    id           UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    member_id    UNIQUEIDENTIFIER NOT NULL,
    request_type NVARCHAR (20)    NOT NULL,
    status       NVARCHAR (30)    DEFAULT 'PENDING' NOT NULL,
    requested_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    approved_at  DATETIME2 (7)   ,
    processed_by UNIQUEIDENTIFIER,
    expires_at   DATETIME2 (7)   ,
    created_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_borrow_request_patron FOREIGN KEY (member_id) REFERENCES library_members (id),
    CONSTRAINT fk_borrow_request_staff FOREIGN KEY (processed_by) REFERENCES users (id),
    CONSTRAINT chk_borrow_request_type CHECK (request_type IN ('RESERVATION', 'DELIVERY')),
    CONSTRAINT chk_borrow_request_status CHECK (status IN ('PENDING', 'APPROVED', 'READY_FOR_PICKUP', 'DELIVERING', 'COMPLETED', 'REJECTED', 'CANCELLED', 'EXPIRED'))
);


GO
CREATE TABLE borrow_request_items (
    id                UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    borrow_request_id UNIQUEIDENTIFIER NOT NULL,
    book_id           UNIQUEIDENTIFIER NOT NULL,
    book_copy_id      UNIQUEIDENTIFIER,
    status            NVARCHAR (20)    DEFAULT 'PENDING' NOT NULL,
    reserved_at       DATETIME2 (7)   ,
    reserved_until    DATETIME2 (7)   ,
    created_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_request_item_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE CASCADE,
    CONSTRAINT fk_request_item_book FOREIGN KEY (book_id) REFERENCES books (id),
    CONSTRAINT fk_request_item_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),
    CONSTRAINT chk_request_item_status CHECK (status IN ('PENDING', 'RESERVED', 'FULFILLED', 'CANCELLED', 'EXPIRED'))
);


GO
CREATE TABLE deliveries (
    id                UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    borrow_request_id UNIQUEIDENTIFIER NOT NULL,
    recipient_name    NVARCHAR (150)   NOT NULL,
    phone             NVARCHAR (20)    NOT NULL,
    address           NVARCHAR (MAX)   NOT NULL,
    delivered_at      DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    created_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_deliveries_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE CASCADE
);


GO
CREATE TABLE loans (
    id                UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    member_id         UNIQUEIDENTIFIER NOT NULL,
    borrow_request_id UNIQUEIDENTIFIER NULL,
    borrowed_at       DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    status            NVARCHAR (20)    DEFAULT 'ACTIVE' NOT NULL,
    created_by        UNIQUEIDENTIFIER NOT NULL,
    created_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_loans_member FOREIGN KEY (member_id) REFERENCES library_members (id),
    CONSTRAINT fk_loans_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE SET NULL,
    CONSTRAINT fk_loans_created_by FOREIGN KEY (created_by) REFERENCES users (id),
    CONSTRAINT chk_loan_status CHECK (status IN ('ACTIVE', 'COMPLETED', 'CANCELLED'))
);


GO
CREATE TABLE loan_items (
    id                  UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    loan_id             UNIQUEIDENTIFIER NOT NULL,
    book_copy_id        UNIQUEIDENTIFIER NOT NULL,
    due_date            DATETIME2 (7)    NOT NULL,
    status              NVARCHAR (20)    DEFAULT 'BORROWED' NOT NULL,
    condition_at_loan   NVARCHAR (20)   ,
    returned_at         DATETIME2 (7)   ,
    condition_at_return NVARCHAR (20)   ,
    created_at          DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at          DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_loan_items_loan FOREIGN KEY (loan_id) REFERENCES loans (id) ON DELETE CASCADE,
    CONSTRAINT fk_loan_items_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),
    CONSTRAINT chk_loan_item_status CHECK (status IN ('BORROWED', 'OVERDUE', 'RETURNED', 'LOST')),
    CONSTRAINT chk_condition_at_loan CHECK (condition_at_loan IS NULL
                                            OR condition_at_loan IN ('NEW', 'GOOD', 'WORN', 'DAMAGED')),
    CONSTRAINT chk_condition_at_return CHECK (condition_at_return IS NULL
                                              OR condition_at_return IN ('NEW', 'GOOD', 'WORN', 'DAMAGED'))
);


GO
CREATE TABLE loan_renewals (
    id           UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    loan_item_id UNIQUEIDENTIFIER NOT NULL,
    old_due_date DATETIME2 (7)    NOT NULL,
    new_due_date DATETIME2 (7)    NOT NULL,
    renewed_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    renewed_by   UNIQUEIDENTIFIER NOT NULL,
    reason       NVARCHAR (MAX)  ,
    CONSTRAINT fk_loan_renewals_loan_item FOREIGN KEY (loan_item_id) REFERENCES loan_items (id) ON DELETE CASCADE,
    CONSTRAINT fk_loan_renewals_user FOREIGN KEY (renewed_by) REFERENCES users (id),
    CONSTRAINT chk_renewal_dates CHECK (new_due_date > old_due_date)
);


GO
CREATE TABLE charges (
    id           UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    member_id    UNIQUEIDENTIFIER NOT NULL,
    charge_type  NVARCHAR (30)    NOT NULL,
    description  NVARCHAR (MAX)   NOT NULL,
    amount       DECIMAL (12, 2)  NOT NULL,
    status       NVARCHAR (20)    DEFAULT 'UNPAID' NOT NULL,
    loan_item_id UNIQUEIDENTIFIER,
    created_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    paid_at      DATETIME2 (7)   ,
    CONSTRAINT fk_charges_member FOREIGN KEY (member_id) REFERENCES library_members (id),
    CONSTRAINT fk_charges_loan_item FOREIGN KEY (loan_item_id) REFERENCES loan_items (id) ON DELETE SET NULL,
    CONSTRAINT chk_charge_amount CHECK (amount > 0),
    CONSTRAINT chk_charge_type CHECK (charge_type IN ('CARD_FEE', 'OVERDUE', 'DAMAGED', 'LOST')),
    CONSTRAINT chk_charge_status CHECK (status IN ('UNPAID', 'PAID', 'WAIVED'))
);


GO
CREATE TABLE payment_transactions (
    id               UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    member_id        UNIQUEIDENTIFIER NOT NULL,
    transaction_code NVARCHAR (100)   UNIQUE NOT NULL,
    payment_method   NVARCHAR (30)    NOT NULL,
    status           NVARCHAR (20)    DEFAULT 'PENDING' NOT NULL,
    total_amount     DECIMAL (12, 2)  NOT NULL,
    paid_at          DATETIME2 (7)   ,
    expires_at       DATETIME2 (7)    NOT NULL,
    created_at       DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    FOREIGN KEY (member_id) REFERENCES library_members (id),
    CHECK (total_amount > 0),
    CHECK (payment_method IN ('ONLINE_PAYMENT')),
    CHECK (status IN ('PENDING', 'PROCESSING', 'PAID', 'FAILED', 'CANCELLED'))
);


GO
CREATE TABLE payment_items (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    payment_id UNIQUEIDENTIFIER NOT NULL,
    charge_id  UNIQUEIDENTIFIER NOT NULL,
    amount     DECIMAL (12, 2)  NOT NULL,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    FOREIGN KEY (payment_id) REFERENCES payment_transactions (id) ON DELETE CASCADE,
    FOREIGN KEY (charge_id) REFERENCES charges (id),
    CONSTRAINT UQ_payment_items_charge UNIQUE (charge_id),
    CHECK (amount > 0),
    UNIQUE (payment_id, charge_id)
);


CREATE TABLE card_registrations (
    id                        UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,

    payment_transaction_id    UNIQUEIDENTIFIER NOT NULL,

    full_name                 NVARCHAR(150) NULL,
    email                     NVARCHAR(255) NULL,
    phone                     NVARCHAR(20) NULL,
    id_document_number        NVARCHAR(50) NULL,

    id_document_front_file_id UNIQUEIDENTIFIER NULL,
    id_document_back_file_id  UNIQUEIDENTIFIER NULL,
    avatar_file_id            UNIQUEIDENTIFIER NULL,

    status                    NVARCHAR(20) DEFAULT 'WAITING_FOR_INFORMATION' NOT NULL,

    registration_token_hash   NVARCHAR(255) NULL,
    token_expires_at          DATETIME2(7) NULL,
    token_revoked_at          DATETIME2(7) NULL,

    submitted_at              DATETIME2(7) NULL,
    reviewed_at               DATETIME2(7) NULL,
    reviewed_by               UNIQUEIDENTIFIER NULL,
    rejection_reason          NVARCHAR(MAX) NULL,

    expires_at                DATETIME2(7) NULL,

    created_at                DATETIME2(7) DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at                DATETIME2(7) DEFAULT SYSUTCDATETIME() NOT NULL,

    CONSTRAINT uq_card_registration_payment
        UNIQUE (payment_transaction_id),

    CONSTRAINT fk_card_registration_payment
        FOREIGN KEY (payment_transaction_id)
        REFERENCES payment_transactions (id),

    CONSTRAINT fk_card_registration_reviewer
        FOREIGN KEY (reviewed_by)
        REFERENCES users (id)
        ON DELETE SET NULL,

    CONSTRAINT fk_card_registration_avatar
        FOREIGN KEY (avatar_file_id)
        REFERENCES storage_files (id),

    CONSTRAINT fk_card_registration_id_front
        FOREIGN KEY (id_document_front_file_id)
        REFERENCES storage_files (id),

    CONSTRAINT fk_card_registration_id_back
        FOREIGN KEY (id_document_back_file_id)
        REFERENCES storage_files (id),

    CONSTRAINT chk_card_registration_status
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


GO
CREATE TABLE book_images (
    id         UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    book_id    UNIQUEIDENTIFIER NOT NULL,
    file_id    UNIQUEIDENTIFIER NOT NULL,
    is_primary BIT              DEFAULT 0 NOT NULL,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_book_images_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,
    CONSTRAINT fk_book_images_file FOREIGN KEY (file_id) REFERENCES storage_files (id)
);


GO
CREATE TABLE digital_resources (
    id            UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    book_id       UNIQUEIDENTIFIER NOT NULL,
    file_id       UNIQUEIDENTIFIER NOT NULL,
    resource_type NVARCHAR (30)    NOT NULL,
    title         NVARCHAR (500)  ,
    access_level  NVARCHAR (20)    DEFAULT 'MEMBER' NOT NULL,
    description   NVARCHAR (MAX)  ,
    status        NVARCHAR (20)    DEFAULT 'ACTIVE' NOT NULL,
    created_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at    DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_digital_resource_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,
    CONSTRAINT fk_digital_resource_file FOREIGN KEY (file_id) REFERENCES storage_files (id),
    CONSTRAINT chk_digital_resource_type CHECK (resource_type IN ('EBOOK', 'DOCUMENT', 'OTHER', 'AUDIOBOOK')),
    CONSTRAINT chk_digital_resource_status CHECK (status IN ('ACTIVE', 'INACTIVE')),
    CONSTRAINT chk_digital_resource_access_level CHECK (access_level IN ('PUBLIC', 'MEMBER', 'STAFF', 'HIDDEN'))
);


GO
CREATE TABLE saved_books (
    user_id    UNIQUEIDENTIFIER NOT NULL,
    book_id    UNIQUEIDENTIFIER NOT NULL,
    created_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT PK_saved_books PRIMARY KEY (user_id, book_id),
    CONSTRAINT FK_saved_books_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT FK_saved_books_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE
);


GO
CREATE TABLE notifications (
    id                UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    title             NVARCHAR (255)   NOT NULL,
    message           NVARCHAR (MAX)   NOT NULL,
    notification_type NVARCHAR (30)    NOT NULL,
    created_at        DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT chk_notifications_type CHECK (notification_type IN ('CARD', 'BORROW', 'RETURN', 'DUE_DATE', 'FINE', 'PAYMENT', 'SYSTEM'))
);


GO
CREATE TABLE copy_issues (
    id           UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    book_copy_id UNIQUEIDENTIFIER NOT NULL,
    issue_type   NVARCHAR (30)    NOT NULL,
    description  NVARCHAR (MAX)   NULL,
    reported_by  UNIQUEIDENTIFIER NULL,
    reported_at  DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    resolved_by  UNIQUEIDENTIFIER NULL,
    resolved_at  DATETIME2 (7)    NULL,
    status       NVARCHAR (20)    DEFAULT 'OPEN' NOT NULL,
    created_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    updated_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT PK_copy_issues PRIMARY KEY (id),
    CONSTRAINT FK_copy_issues_book_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),
    CONSTRAINT FK_copy_issues_reported_by FOREIGN KEY (reported_by) REFERENCES users (id),
    CONSTRAINT FK_copy_issues_resolved_by FOREIGN KEY (resolved_by) REFERENCES users (id),
    CONSTRAINT CK_copy_issues_type CHECK (issue_type IN ('LOST', 'DAMAGED', 'MAINTENANCE', 'MISSING_LOCATION')),
    CONSTRAINT CK_copy_issues_status CHECK (status IN ('OPEN', 'IN_PROGRESS', 'RESOLVED', 'CANCELLED'))
);

CREATE TABLE book_copy_status_history (
    id           UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    book_copy_id UNIQUEIDENTIFIER NOT NULL,
    old_status   NVARCHAR (20)    NULL,
    new_status   NVARCHAR (20)    NOT NULL,
    changed_by   UNIQUEIDENTIFIER NULL,
    changed_at   DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    reason       NVARCHAR (500)   NULL,
    CONSTRAINT PK_book_copy_status_history PRIMARY KEY (id),
    CONSTRAINT FK_copy_status_history_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),
    CONSTRAINT FK_copy_status_history_user FOREIGN KEY (changed_by) REFERENCES users (id)
);

CREATE TABLE notification_recipients (
    notification_id UNIQUEIDENTIFIER NOT NULL,
    user_id         UNIQUEIDENTIFIER NOT NULL,
    read_at         DATETIME2 (7)    NULL,
    created_at      DATETIME2 (7)    DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_notification_recipients PRIMARY KEY (notification_id, user_id),
    CONSTRAINT FK_notification_recipients_notification FOREIGN KEY (notification_id) REFERENCES notifications (id) ON DELETE CASCADE,
    CONSTRAINT FK_notification_recipients_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE upload_sessions (
    id                   UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
    card_registration_id UNIQUEIDENTIFIER NULL,
    user_id              UNIQUEIDENTIFIER NULL,
    object_key           NVARCHAR (500)   NOT NULL,
    upload_type          NVARCHAR (30)    NOT NULL,
    status               NVARCHAR (20)    DEFAULT 'PENDING' NOT NULL,
    expires_at           DATETIME2 (7)    NOT NULL,
    created_at           DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    completed_at         DATETIME2 (7)    NULL,
    CONSTRAINT PK_upload_sessions PRIMARY KEY (id),
    CONSTRAINT FK_upload_sessions_user FOREIGN KEY (user_id) REFERENCES users (id),
    CONSTRAINT fk_upload_session_registration FOREIGN KEY (card_registration_id) REFERENCES card_registrations (id),
    CONSTRAINT CK_upload_sessions_status CHECK (status IN ('PENDING', 'PROCESSING', 'COMPLETED', 'FAILED', 'EXPIRED', 'CANCELLED'))
);

CREATE TABLE refresh_tokens (
    id                   UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    user_id              UNIQUEIDENTIFIER NOT NULL,
    token_hash           VARCHAR (64)     NOT NULL UNIQUE,
    family_id            UNIQUEIDENTIFIER NOT NULL,
    expires_at           DATETIME2 (7)    NOT NULL,
    created_at           DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    revoked_at           DATETIME2 (7)    NULL,
    replaced_by_token_id UNIQUEIDENTIFIER NULL,
    CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES users (id),
    CONSTRAINT fk_refresh_tokens_replaced_by FOREIGN KEY (replaced_by_token_id) REFERENCES refresh_tokens (id)
);

--luu cac jti da dang xuat
CREATE TABLE revoked_tokens (
    jti        UNIQUEIDENTIFIER PRIMARY KEY,
    user_id    UNIQUEIDENTIFIER NOT NULL,
    expires_at DATETIME2 (7)    NOT NULL,
    revoked_at DATETIME2 (7)    DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT fk_revoked_tokens_user FOREIGN KEY (user_id) REFERENCES users (id)
);