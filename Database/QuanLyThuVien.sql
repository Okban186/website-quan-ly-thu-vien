
CREATE TABLE

    storage_files (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        file_key nvarchar(max) NOT NULL,

        file_name nvarchar(255) NOT NULL,

        file_url nvarchar(max),

        mime_type nvarchar(100) CHECK (

    mime_type IN (

        'image/jpeg',

        'image/png',

        'image/webp',

        'application/pdf',

        'application/epub+zip',

        'audio/mpeg',

        'audio/mp4',

        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'

    )),

        file_size BIGINT,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()

    );

GO

CREATE TABLE users (
    id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    username nvarchar(100) NOT NULL UNIQUE,
    email nvarchar(255) NOT NULL UNIQUE,        
    password_hash nvarchar(max) NOT NULL,
    full_name nvarchar(150) NOT NULL,          
    phone nvarchar(20),                       
    date_of_birth DATE,                      
    avatar_file_id uniqueidentifier NULL,      
    status nvarchar(20) NOT NULL DEFAULT 'ACTIVE',
    last_login_at DATETIME2(7),
    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT chk_users_status CHECK (status IN ('ACTIVE', 'LOCKED', 'DISABLED')),
    CONSTRAINT fk_users_avatar_file FOREIGN KEY (avatar_file_id) REFERENCES storage_files (id)
);

CREATE TABLE

    roles (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

name nvarchar(50) NOT NULL UNIQUE,

description nvarchar(max)

    );

GO

CREATE TABLE

    user_roles (

        user_id uniqueidentifier NOT NULL,

        role_id uniqueidentifier NOT NULL,

PRIMARY KEY (user_id, role_id),

CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,

CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE

    );

GO

CREATE TABLE staff (
    id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    user_id uniqueidentifier NOT NULL UNIQUE,
    employee_code nvarchar(50) NOT NULL UNIQUE,  
    citizen_id nvarchar(20) UNIQUE,             
    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_staff_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE library_members (
    id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    user_id uniqueidentifier NOT NULL UNIQUE,
    citizen_id nvarchar(20) NOT NULL UNIQUE,      
    id_document_front_file_id uniqueidentifier NULL,
    id_document_back_file_id uniqueidentifier NULL,  
    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_members_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_member_id_document_front FOREIGN KEY (id_document_front_file_id) REFERENCES storage_files (id),
    CONSTRAINT fk_member_id_document_back FOREIGN KEY (id_document_back_file_id) REFERENCES storage_files (id)
);

CREATE TABLE

    card_registrations (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        full_name nvarchar(150) NOT NULL,

        email nvarchar(255) NOT NULL,

        phone nvarchar(20) NOT NULL,

        id_document_number nvarchar(50) NOT NULL,

        id_document_front_file_id uniqueidentifier NULL,

        id_document_back_file_id uniqueidentifier NULL,

        avatar_file_id uniqueidentifier NULL,

status nvarchar(20) NOT NULL DEFAULT 'PENDING',

        submitted_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        reviewed_at DATETIME2(7),

        reviewed_by uniqueidentifier,

        rejection_reason nvarchar(max),

        expires_at DATETIME2(7),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_card_registration_reviewer FOREIGN KEY (reviewed_by) REFERENCES users (id) ON DELETE SET NULL,

CONSTRAINT fk_card_registration_avatar FOREIGN KEY (avatar_file_id) REFERENCES storage_files (id),

CONSTRAINT fk_card_registration_id_front FOREIGN KEY (id_document_front_file_id) REFERENCES storage_files (id),

CONSTRAINT fk_card_registration_id_back FOREIGN KEY (id_document_back_file_id) REFERENCES storage_files (id),

CONSTRAINT chk_card_registration_status CHECK (

status IN (

                'PENDING',

                'APPROVED',

                'REJECTED',

                'CANCELLED',

                'EXPIRED'

            )

        )

    );

GO

CREATE TABLE

    library_cards (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        member_id uniqueidentifier NOT NULL UNIQUE,

        card_number nvarchar(50) NOT NULL UNIQUE,

status nvarchar(20) NOT NULL DEFAULT 'ACTIVE',

        issued_at DATETIME2(7),

        activated_at DATETIME2(7),

        expired_at DATETIME2(7),

        blocked_at DATETIME2(7),

        blocked_reason nvarchar(max),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_library_card_patron FOREIGN KEY (member_id) REFERENCES library_members (id),

CONSTRAINT chk_library_card_status CHECK (

status IN (

                'PENDING',

                'ACTIVE',

                'BLOCKED',

                'EXPIRED',

                'CANCELLED'

            )

        )

    );

GO

CREATE TABLE

    publishers (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

name nvarchar(255) NOT NULL UNIQUE,

address nvarchar(max),

        phone nvarchar(20),

        email nvarchar(255),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()

    );

GO

CREATE TABLE

    locations (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        code nvarchar(50) NOT NULL UNIQUE,

name nvarchar(150) NOT NULL,

        floor int NOT NULL,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT chk_locations_floor CHECK (floor > 0)

    );

GO

CREATE TABLE document_types (

    id UNIQUEIDENTIFIER NOT NULL

DEFAULT NEWSEQUENTIALID(),

name NVARCHAR(100) NOT NULL,

description NVARCHAR(MAX) NULL,

    created_at DATETIME2(7) NOT NULL

DEFAULT SYSUTCDATETIME(),

CONSTRAINT PK_document_types

PRIMARY KEY (id),

CONSTRAINT UQ_document_types_name

UNIQUE (name)

);

GO
CREATE TABLE

    books (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        isbn nvarchar(20) UNIQUE,

        title nvarchar(500) NOT NULL,

description nvarchar(max),

        publication_year int,

language nvarchar(50),

        page_count int,

        price decimal(12,2),

        publisher_id uniqueidentifier,
        document_type_id uniqueidentifier,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_books_publisher FOREIGN KEY (publisher_id) REFERENCES publishers (id) ON DELETE SET NULL,
CONSTRAINT fk_books_document_type FOREIGN KEY (document_type_id) REFERENCES document_types (id) ON DELETE SET NULL,

CONSTRAINT chk_books_page_count CHECK (

            page_count IS NULL

OR page_count > 0

        ),

CONSTRAINT chk_books_price CHECK (

            price IS NULL

OR price >= 0

        ),

CONSTRAINT chk_books_publication_year CHECK (

            publication_year IS NULL

OR publication_year BETWEEN 1000 AND 9999

        )

    );

GO

CREATE TABLE

    book_copies (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        book_id uniqueidentifier NOT NULL,

        location_id uniqueidentifier,

        barcode nvarchar(100) NOT NULL UNIQUE,

status nvarchar(20) NOT NULL DEFAULT 'AVAILABLE',

        condition nvarchar(20) NOT NULL DEFAULT 'GOOD',

        acquired_at DATE,

        acquisition_price decimal(12,2),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_book_copies_book FOREIGN KEY (book_id) REFERENCES books (id),

CONSTRAINT fk_book_copies_location FOREIGN KEY (location_id) REFERENCES locations (id) ON DELETE SET NULL,

CONSTRAINT chk_book_copy_status CHECK (

status IN (

                'AVAILABLE',

                'RESERVED',

                'BORROWED',

                'LOST',

                'DAMAGED',

                'MAINTENANCE',

                'REMOVED'

            )

        ),

CONSTRAINT chk_book_copy_condition CHECK (condition IN ('NEW', 'GOOD', 'WORN', 'DAMAGED')),

CONSTRAINT chk_acquisition_price CHECK (

            acquisition_price IS NULL

OR acquisition_price >= 0

        )

    );

GO

CREATE TABLE

    categories (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        parent_id uniqueidentifier,

name nvarchar(150) NOT NULL,

description nvarchar(max),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_categories_parent FOREIGN KEY (parent_id) REFERENCES categories (id),

CONSTRAINT uq_categories_parent_name UNIQUE (parent_id, name)

    );

GO

CREATE TABLE

    book_categories (

        book_id uniqueidentifier NOT NULL,

        category_id uniqueidentifier NOT NULL,

PRIMARY KEY (book_id, category_id),

CONSTRAINT fk_book_categories_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,

CONSTRAINT fk_book_categories_category FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE

    );

GO

CREATE TABLE

    authors (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

name nvarchar(255) NOT NULL,

        biography nvarchar(max),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()

    );

GO

CREATE TABLE

    book_authors (

        book_id uniqueidentifier NOT NULL,

        author_id uniqueidentifier NOT NULL,

PRIMARY KEY (book_id, author_id),

CONSTRAINT fk_book_authors_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,

CONSTRAINT fk_book_authors_author FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE

    );

GO

CREATE TABLE

    borrow_requests (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        member_id uniqueidentifier NOT NULL,

        request_type nvarchar(20) NOT NULL,

status nvarchar(30) NOT NULL DEFAULT 'PENDING',

        requested_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        approved_at DATETIME2(7),

        processed_by uniqueidentifier,

        expires_at DATETIME2(7),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_borrow_request_patron FOREIGN KEY (member_id) REFERENCES library_members (id),

CONSTRAINT fk_borrow_request_staff FOREIGN KEY (processed_by) REFERENCES users (id),

CONSTRAINT chk_borrow_request_type CHECK (request_type IN ('RESERVATION', 'DELIVERY')),

CONSTRAINT chk_borrow_request_status CHECK (

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

GO

CREATE TABLE

    borrow_request_items (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        borrow_request_id uniqueidentifier NOT NULL,

        book_id uniqueidentifier NOT NULL,

        book_copy_id uniqueidentifier,

status nvarchar(20) NOT NULL DEFAULT 'PENDING',

        reserved_at DATETIME2(7),

        reserved_until DATETIME2(7),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_request_item_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE CASCADE,

CONSTRAINT fk_request_item_book FOREIGN KEY (book_id) REFERENCES books (id),

CONSTRAINT fk_request_item_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),

CONSTRAINT chk_request_item_status CHECK (

status IN (

                'PENDING',

                'RESERVED',

                'FULFILLED',

                'CANCELLED',

                'EXPIRED'

            )

        )

    );

GO

CREATE TABLE

    deliveries (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        borrow_request_id uniqueidentifier NOT NULL,

        recipient_name nvarchar(150) NOT NULL,

        phone nvarchar(20) NOT NULL,

address nvarchar(max) NOT NULL,

        delivered_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_deliveries_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE CASCADE

    );

GO

CREATE TABLE

    loans (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        member_id uniqueidentifier NOT NULL,

        borrow_request_id uniqueidentifier,

        loan_date DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        due_date DATETIME2(7) NOT NULL,

status nvarchar(20) NOT NULL DEFAULT 'ACTIVE',

        created_by uniqueidentifier NOT NULL,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_loans_patron FOREIGN KEY (member_id) REFERENCES library_members (id),

CONSTRAINT fk_loans_request FOREIGN KEY (borrow_request_id) REFERENCES borrow_requests (id) ON DELETE SET NULL,

CONSTRAINT fk_loans_created_by FOREIGN KEY (created_by) REFERENCES users (id),

CONSTRAINT chk_loan_status CHECK (

status IN (

                'ACTIVE',

                'RETURNED',

                'OVERDUE'

            )

        )

    );

GO

CREATE TABLE

    loan_items (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        loan_id uniqueidentifier NOT NULL,

        book_copy_id uniqueidentifier NOT NULL,

        condition_at_loan nvarchar(20),

        returned_at DATETIME2(7),

        condition_at_return nvarchar(20),

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_loan_items_loan FOREIGN KEY (loan_id) REFERENCES loans (id) ON DELETE CASCADE,

CONSTRAINT fk_loan_items_copy FOREIGN KEY (book_copy_id) REFERENCES book_copies (id),

CONSTRAINT chk_condition_at_loan CHECK (

            condition_at_loan IS NULL

OR condition_at_loan IN ('NEW', 'GOOD', 'WORN', 'DAMAGED')

        ),

CONSTRAINT chk_condition_at_return CHECK (

            condition_at_return IS NULL

OR condition_at_return IN ('NEW', 'GOOD', 'WORN', 'DAMAGED')

        )

    );

GO

CREATE TABLE

    loan_renewals (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        loan_id uniqueidentifier NOT NULL,

        old_due_date DATETIME2(7) NOT NULL,

        new_due_date DATETIME2(7) NOT NULL,

        renewed_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        renewed_by uniqueidentifier NOT NULL,

        reason nvarchar(max),

CONSTRAINT fk_loan_renewals_loan FOREIGN KEY (loan_id) REFERENCES loans (id) ON DELETE CASCADE,

CONSTRAINT fk_loan_renewals_user FOREIGN KEY (renewed_by) REFERENCES users (id),

CONSTRAINT chk_renewal_dates CHECK (new_due_date > old_due_date)

    );

GO

CREATE TABLE

    charges (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        member_id uniqueidentifier NOT NULL,

        charge_type nvarchar(30) NOT NULL,

description nvarchar(max) NOT NULL,

        amount decimal(12,2) NOT NULL,

status nvarchar(20) NOT NULL DEFAULT 'UNPAID',

        loan_item_id uniqueidentifier,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        paid_at DATETIME2(7),

CONSTRAINT fk_charges_member FOREIGN KEY (member_id) REFERENCES library_members (id),

CONSTRAINT fk_charges_loan_item FOREIGN KEY (loan_item_id) REFERENCES loan_items (id) ON DELETE SET NULL,

CONSTRAINT chk_charge_amount CHECK (amount > 0),

CONSTRAINT chk_charge_type CHECK (

            charge_type IN ('CARD_FEE', 'OVERDUE', 'DAMAGED', 'LOST')

        ),

CONSTRAINT chk_charge_status CHECK (status IN ('UNPAID', 'PAID', 'WAIVED'))

    );

GO

CREATE TABLE

    payment_transactions (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        member_id uniqueidentifier NOT NULL,

        transaction_code nvarchar(100) UNIQUE NOT NULL,

        payment_method nvarchar(30) NOT NULL,

status nvarchar(20) NOT NULL DEFAULT 'PENDING',

        total_amount decimal(12,2) NOT NULL,

        paid_at DATETIME2(7),

        expires_at DATETIME2(7) NOT NULL,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

FOREIGN KEY (member_id) REFERENCES library_members (id),

CHECK (total_amount > 0),

CHECK (

            payment_method IN ('ONLINE_PAYMENT')

        ),

CHECK (

status IN (

                'PENDING',

                'PROCESSING',

                'PAID',

                'FAILED',

                'CANCELLED'

            )

        )

    );

GO

CREATE TABLE

    payment_items (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        payment_id uniqueidentifier NOT NULL,

        charge_id uniqueidentifier NOT NULL,

        amount decimal(12,2) NOT NULL,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

FOREIGN KEY (payment_id) REFERENCES payment_transactions (id) ON DELETE CASCADE,

FOREIGN KEY (charge_id) REFERENCES charges (id),

CHECK (amount > 0),

UNIQUE (payment_id, charge_id)

    );

GO

CREATE TABLE

    book_images (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        book_id uniqueidentifier NOT NULL,

        file_id uniqueidentifier NOT NULL,

        is_primary bit NOT NULL DEFAULT 0,

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_book_images_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,

CONSTRAINT fk_book_images_file FOREIGN KEY (file_id) REFERENCES storage_files (id)

    );

GO

CREATE TABLE

    digital_resources (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        book_id uniqueidentifier NOT NULL,

        file_id uniqueidentifier NOT NULL,

        resource_type nvarchar(30) NOT NULL,

        title nvarchar(500),

        access_level NVARCHAR(20) NOT NULL DEFAULT 'MEMBER',

description nvarchar(max),

status nvarchar(20) NOT NULL DEFAULT 'ACTIVE',

        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

        updated_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT fk_digital_resource_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,

CONSTRAINT fk_digital_resource_file FOREIGN KEY (file_id) REFERENCES storage_files (id),

CONSTRAINT chk_digital_resource_type CHECK (resource_type IN ('EBOOK', 'DOCUMENT', 'OTHER','AUDIOBOOK')),

CONSTRAINT chk_digital_resource_status CHECK (status IN ('ACTIVE', 'INACTIVE')),

CONSTRAINT chk_digital_resource_access_level CHECK (access_level IN ('PUBLIC', 'MEMBER', 'STAFF', 'HIDDEN'))

    );

GO



CREATE TABLE saved_books (
    user_id UNIQUEIDENTIFIER NOT NULL,
    book_id UNIQUEIDENTIFIER NOT NULL,
    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_saved_books PRIMARY KEY (user_id, book_id),
    CONSTRAINT FK_saved_books_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT FK_saved_books_book FOREIGN KEY (book_id) REFERENCES books(id) ON DELETE CASCADE
);

GO

CREATE TABLE

    notifications (

        id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

        title nvarchar(255) NOT NULL,

message nvarchar(max) NOT NULL,

        notification_type nvarchar(30) NOT NULL,


        created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

CONSTRAINT chk_notifications_type CHECK (

            notification_type IN (

                'CARD',

                'BORROW',

                'RETURN',

                'DUE_DATE',

                'FINE',

                'PAYMENT',

                'SYSTEM'

            )

        )

    );

GO



CREATE TABLE copy_issues (

    id UNIQUEIDENTIFIER NOT NULL

DEFAULT NEWSEQUENTIALID(),

    book_copy_id UNIQUEIDENTIFIER NOT NULL,

    issue_type NVARCHAR(30) NOT NULL,

description NVARCHAR(MAX) NULL,

    reported_by UNIQUEIDENTIFIER NULL,

    reported_at DATETIME2(7) NOT NULL

DEFAULT SYSUTCDATETIME(),

    resolved_by UNIQUEIDENTIFIER NULL,

    resolved_at DATETIME2(7) NULL,

status NVARCHAR(20) NOT NULL

DEFAULT 'OPEN',

    created_at DATETIME2(7) NOT NULL

DEFAULT SYSUTCDATETIME(),

    updated_at DATETIME2(7) NOT NULL

DEFAULT SYSUTCDATETIME(),

CONSTRAINT PK_copy_issues

PRIMARY KEY (id),

CONSTRAINT FK_copy_issues_book_copy

FOREIGN KEY (book_copy_id)

REFERENCES book_copies(id),

CONSTRAINT FK_copy_issues_reported_by

FOREIGN KEY (reported_by)

REFERENCES users(id),

CONSTRAINT FK_copy_issues_resolved_by

FOREIGN KEY (resolved_by)

REFERENCES users(id),

CONSTRAINT CK_copy_issues_type

CHECK (

            issue_type IN (

                'LOST',

                'DAMAGED',

                'MAINTENANCE',

                'MISSING_LOCATION'

            )

        ),

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





CREATE TABLE book_copy_status_history (

    id UNIQUEIDENTIFIER NOT NULL

DEFAULT NEWSEQUENTIALID(),

    book_copy_id UNIQUEIDENTIFIER NOT NULL,

    old_status NVARCHAR(20) NULL,

    new_status NVARCHAR(20) NOT NULL,

    changed_by UNIQUEIDENTIFIER NULL,

    changed_at DATETIME2(7) NOT NULL

DEFAULT SYSUTCDATETIME(),

    reason NVARCHAR(500) NULL,

CONSTRAINT PK_book_copy_status_history

PRIMARY KEY (id),

CONSTRAINT FK_copy_status_history_copy

FOREIGN KEY (book_copy_id)

REFERENCES book_copies(id),

CONSTRAINT FK_copy_status_history_user

FOREIGN KEY (changed_by)

REFERENCES users(id)

);






CREATE TABLE notification_recipients (

    notification_id UNIQUEIDENTIFIER NOT NULL,

    user_id UNIQUEIDENTIFIER NOT NULL,

    read_at DATETIME2(7) NULL,

    created_at DATETIME2(7) DEFAULT SYSUTCDATETIME(),

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


CREATE TABLE upload_sessions (
    id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    user_id UNIQUEIDENTIFIER NOT NULL,

    object_key NVARCHAR(500) NOT NULL,

    original_file_name NVARCHAR(255) NOT NULL,
    expected_mime_type NVARCHAR(100) NOT NULL,
    max_file_size BIGINT NOT NULL,

    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',

    expires_at DATETIME2(7) NOT NULL,

    created_at DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    completed_at DATETIME2(7) NULL,

    CONSTRAINT PK_upload_sessions
        PRIMARY KEY (id),

    CONSTRAINT FK_upload_sessions_user
        FOREIGN KEY (user_id)
        REFERENCES users(id),

    CONSTRAINT CK_upload_sessions_status
        CHECK (
            status IN (
                'PENDING',
                'PROCESSING',
                'COMPLETED',
                'FAILED',
                'EXPIRED',
                'CANCELLED'
            )
        ),

    CONSTRAINT CK_upload_sessions_max_file_size
        CHECK (max_file_size > 0)
);


CREATE TABLE refresh_tokens (
    id uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

    user_id uniqueidentifier NOT NULL,

    token_hash varchar(64) NOT NULL UNIQUE,

    family_id uniqueidentifier NOT NULL,

    expires_at datetime2(7) NOT NULL,

    created_at datetime2(7) NOT NULL
        DEFAULT SYSUTCDATETIME(),

    revoked_at datetime2(7) NULL,

    replaced_by_token_id uniqueidentifier NULL,

    CONSTRAINT fk_refresh_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES users(id),

    CONSTRAINT fk_refresh_tokens_replaced_by
        FOREIGN KEY (replaced_by_token_id)
        REFERENCES refresh_tokens(id)
);

--luu cac jti da dang xuat
CREATE TABLE revoked_tokens (
    jti uniqueidentifier PRIMARY KEY,
    user_id uniqueidentifier NOT NULL,
    expires_at datetime2(7) NOT NULL,
    revoked_at datetime2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_revoked_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
);





--data seed

INSERT INTO roles (name, description)
VALUES
    (N'ADMIN', N'Quản trị hệ thống'),
    (N'LIBRARIAN', N'Thủ thư'),
    (N'CATALOGER', N'Biên mục tài liệu'),
    (N'STOREKEEPER', N'Thủ kho'),
    (N'MEMBER', N'Bạn đọc');



GO
--Tạo Admin và gán roles
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminRoleId UNIQUEIDENTIFIER;

SELECT @AdminRoleId = id
FROM roles
WHERE name = N'ADMIN';

INSERT INTO users (
    id,
    username,
    email,
    password_hash,
    full_name,
    status,
    created_at,
    updated_at
)
VALUES (
    @AdminId,
    N'admin',
    N'admin@library.local',
    N'AQAAAAIAAYagAAAAEE7eEFl3OwbvdF/66TBEMu6eZoEb70ll74uxAf5x4O4nKws7j+C/j/myXHUXuX6TTA==',
    N'System Administrator',
    N'ACTIVE',
    SYSUTCDATETIME(),
    SYSUTCDATETIME()
);

INSERT INTO user_roles (
    user_id,
    role_id
)
VALUES (
    @AdminId,
    @AdminRoleId
);


GO
