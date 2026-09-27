--data seed
INSERT  INTO roles (
    name,
    description
)
VALUES            (N'ADMIN', N'Quản trị hệ thống'),
(N'LIBRARIAN', N'Thủ thư'),
(N'CATALOGER', N'Biên mục tài liệu'),
(N'STOREKEEPER', N'Thủ kho'),
(N'MEMBER', N'Bạn đọc');


GO
--Tạo Admin và gán roles
DECLARE @AdminId AS UNIQUEIDENTIFIER = NEWID();

DECLARE @AdminRoleId AS UNIQUEIDENTIFIER;

SELECT @AdminRoleId = id
FROM   roles
WHERE  name = N'ADMIN';

INSERT  INTO users (
    id,
    username,
    email,
    password_hash,
    full_name,
    status,
    created_at,
    updated_at
)
VALUES            (@AdminId, N'admin', N'admin@library.local', N'AQAAAAIAAYagAAAAEE7eEFl3OwbvdF/66TBEMu6eZoEb70ll74uxAf5x4O4nKws7j+C/j/myXHUXuX6TTA==', N'System Administrator', N'ACTIVE', SYSUTCDATETIME(), SYSUTCDATETIME());

INSERT  INTO user_roles (
    user_id,
    role_id
)
VALUES                 (@AdminId, @AdminRoleId);


GO
DECLARE @UserId1 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @UserId2 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @UserId3 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @MemberId1 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @MemberId2 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @MemberId3 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @CardId1 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @CardId2 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @CardId3 AS UNIQUEIDENTIFIER = NEWID();

DECLARE @MemberRoleId AS UNIQUEIDENTIFIER;

SELECT @MemberRoleId = id
FROM   roles
WHERE  name = N'MEMBER';

------------------------------------------------------------
-- USERS
------------------------------------------------------------
INSERT  INTO users (
    id,
    username,
    email,
    password_hash,
    full_name,
    phone,
    date_of_birth,
    status
)
VALUES            (@UserId1, N'nguyenvana', N'nguyenvana@gmail.com', N'AQAAAAIAAYagAAAAEGpY9oXW/cFM+ODqbPa/GSWa9ZSfUM+tZ+BNC5ZFiBQXh9RYSlVvWWPlw4zfjrfuCg==', N'Nguyễn Văn An', N'0901234567', '2000-05-15', N'ACTIVE'),
(@UserId2, N'tranthibinh', N'tranthibinh@gmail.com', N'AQAAAAIAAYagAAAAEMVjT3moaRR/qjDVHHcHpVcrdPZjTLPd1S00ejwy+IY2m50h+Rbx8iT1HC9BnkvCuw==', N'Trần Thị Bình', N'0912345678', '1999-08-20', N'ACTIVE'),
(@UserId3, N'leminhhoang', N'leminhhoang@gmail.com', N'AQAAAAIAAYagAAAAEERBHbsxBB3dFNC98s6aEdxoRI7mVKYBJSadipDWXeBhBbV9xEo3Gj5zHRdTXGkF7A==', N'Lê Minh Hoàng', N'0987654321', '2001-02-10', N'ACTIVE');

INSERT  INTO user_roles (
    user_id,
    role_id
)
VALUES                 (@UserId1, @MemberRoleId),
(@UserId2, @MemberRoleId),
(@UserId3, @MemberRoleId);

INSERT  INTO library_members (
    id,
    user_id,
    citizen_id
)
VALUES                      (@MemberId1, @UserId1, N'079200000001'),
(@MemberId2, @UserId2, N'079200000002'),
(@MemberId3, @UserId3, N'079200000003');

INSERT  INTO library_cards (
    id,
    member_id,
    card_number,
    status,
    issued_at,
    activated_at,
    expired_at
)
VALUES                    (@CardId1, @MemberId1, N'TV000001', N'ACTIVE', SYSUTCDATETIME(), SYSUTCDATETIME(), DATEADD(YEAR, 1, SYSUTCDATETIME())),
(@CardId2, @MemberId2, N'TV000002', N'ACTIVE', SYSUTCDATETIME(), SYSUTCDATETIME(), DATEADD(YEAR, 1, SYSUTCDATETIME())),
(@CardId3, @MemberId3, N'TV000003', N'EXPIRED', DATEADD(YEAR, -2, SYSUTCDATETIME()), DATEADD(YEAR, -2, SYSUTCDATETIME()), DATEADD(DAY, -1, SYSUTCDATETIME()));

INSERT  INTO categories (
    name,
    description,
    display_order
)
VALUES                 -- Văn học
(N'Văn học Việt Nam', N'Các tác phẩm văn học Việt Nam qua nhiều thời kỳ.', 1),
(N'Văn học nước ngoài', N'Các tác phẩm văn học của các quốc gia trên thế giới.', 2),
(N'Thơ', N'Các tuyển tập thơ và tác phẩm thơ.', 3),
(N'Truyện ngắn', N'Các tuyển tập và tác phẩm truyện ngắn.', 4),
(N'Tiểu thuyết', N'Các tác phẩm tiểu thuyết trong nước và quốc tế.', 5),
(N'Truyện trinh thám', N'Các tác phẩm trinh thám và điều tra.', 6),
(N'Truyện giả tưởng', N'Các tác phẩm fantasy và giả tưởng.', 7),
(N'Kịch', N'Các tác phẩm kịch và sân khấu.', 8),
-- Kinh tế
(N'Kinh tế', N'Kiến thức và tài liệu về kinh tế.', 9),
(N'Quản trị kinh doanh', N'Sách về quản trị và điều hành doanh nghiệp.', 10),
(N'Tài chính', N'Tài chính cá nhân và tài chính doanh nghiệp.', 11),
(N'Đầu tư', N'Kiến thức về đầu tư và thị trường tài chính.', 12),
(N'Marketing', N'Marketing, thương hiệu và truyền thông.', 13),
(N'Kế toán', N'Kế toán và kiểm toán.', 14),
-- Công nghệ
(N'Công nghệ thông tin', N'Kiến thức tổng quan về công nghệ thông tin.', 15),
(N'Lập trình', N'Các tài liệu về lập trình và phát triển phần mềm.', 16),
(N'Khoa học máy tính', N'Các tài liệu về khoa học máy tính và thuật toán.', 17),
(N'Trí tuệ nhân tạo', N'AI, machine learning và các lĩnh vực liên quan.', 18),
(N'Cơ sở dữ liệu', N'Các tài liệu về cơ sở dữ liệu và hệ quản trị cơ sở dữ liệu.', 19),
(N'Mạng máy tính', N'Mạng máy tính, Internet và các giao thức mạng.', 20),
(N'An toàn thông tin', N'Bảo mật hệ thống, mạng và dữ liệu.', 21),
-- Khoa học
(N'Khoa học tự nhiên', N'Kiến thức về các ngành khoa học tự nhiên.', 22),
(N'Toán học', N'Toán học cơ bản và nâng cao.', 23),
(N'Vật lý', N'Các tài liệu về vật lý.', 24),
(N'Hóa học', N'Các tài liệu về hóa học.', 25),
(N'Sinh học', N'Các tài liệu về sinh học và khoa học sự sống.', 26),
-- Xã hội
(N'Lịch sử', N'Lịch sử Việt Nam và lịch sử thế giới.', 27),
(N'Địa lý', N'Địa lý Việt Nam và thế giới.', 28),
(N'Chính trị - Xã hội', N'Các tài liệu về chính trị và đời sống xã hội.', 29),
(N'Văn hóa', N'Văn hóa Việt Nam và các nền văn hóa trên thế giới.', 30),
(N'Triết học', N'Các tác phẩm và tài liệu về triết học.', 31),
-- Tâm lý và kỹ năng
(N'Tâm lý học', N'Kiến thức về tâm lý và hành vi con người.', 32),
(N'Phát triển bản thân', N'Sách về phát triển bản thân và hoàn thiện kỹ năng cá nhân.', 33),
(N'Kỹ năng sống', N'Các kỹ năng cần thiết trong học tập và cuộc sống.', 34),
(N'Kỹ năng giao tiếp', N'Kỹ năng giao tiếp và xây dựng mối quan hệ.', 35),
(N'Lãnh đạo', N'Kỹ năng lãnh đạo và quản lý đội nhóm.', 36),
-- Giáo dục
(N'Giáo dục', N'Các tài liệu về giáo dục và phương pháp giảng dạy.', 37),
(N'Ngoại ngữ', N'Tài liệu học và sử dụng ngoại ngữ.', 38),
(N'Tiếng Anh', N'Sách học tiếng Anh và tài liệu tham khảo.', 39),
(N'Từ điển', N'Các loại từ điển và tài liệu tra cứu.', 40),
-- Trẻ em
(N'Thiếu nhi', N'Sách dành cho trẻ em.', 41),
(N'Truyện tranh', N'Truyện tranh dành cho nhiều độ tuổi.', 42),
(N'Sách giáo dục trẻ em', N'Sách hỗ trợ học tập và phát triển cho trẻ em.', 43),
-- Nghệ thuật
(N'Nghệ thuật', N'Các tài liệu về nghệ thuật và sáng tạo.', 44),
(N'Âm nhạc', N'Sách về âm nhạc và các nhạc sĩ.', 45),
(N'Nhiếp ảnh', N'Nhiếp ảnh và kỹ thuật chụp ảnh.', 46),
(N'Hội họa', N'Hội họa và nghệ thuật thị giác.', 47),
-- Đời sống
(N'Nấu ăn', N'Sách hướng dẫn nấu ăn và ẩm thực.', 48),
(N'Du lịch', N'Du lịch, khám phá và văn hóa các vùng miền.', 49),
(N'Sức khỏe', N'Kiến thức chăm sóc sức khỏe và đời sống.', 50),
(N'Thể thao', N'Thể thao, luyện tập và rèn luyện thể chất.', 51),
(N'Gia đình', N'Các chủ đề về gia đình và đời sống.', 52);

INSERT  INTO publishers (
    name,
    address,
    phone,
    email
)
VALUES                 (N'Nhà xuất bản Kim Đồng', N'55 Quang Trung, Hai Bà Trưng, Hà Nội', N'02439434730', N'info@nxbkimdong.com.vn'),
(N'Nhà xuất bản Trẻ', N'161B Lý Chính Thắng, Phường Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh', N'02839316289', N'info@nxbtre.com.vn'),
(N'Nhà xuất bản Giáo dục Việt Nam', N'81 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội', N'02438220801', N'info@nxbgd.vn'),
(N'Nhà xuất bản Khoa học và Kỹ thuật', N'70 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội', N'02439422062', N'nxbkhkt@nxbkhkt.com.vn'),
(N'Nhà xuất bản Văn học', N'18 Nguyễn Trường Tộ, Ba Đình, Hà Nội', N'02437152186', N'nxbvanhoc@nxbvanhoc.com.vn'),
(N'Nhà xuất bản Lao động', N'175 Giảng Võ, Đống Đa, Hà Nội', N'02438515380', N'nxbladong@nxblaodong.com.vn'),
(N'Nhà xuất bản Tổng hợp Thành phố Hồ Chí Minh', N'62 Nguyễn Thị Minh Khai, Quận 1, TP. Hồ Chí Minh', N'02838253043', N'tonghop@nxbtrẻ.vn'),
(N'Nhà xuất bản Đại học Quốc gia Hà Nội', N'16 Hàng Chuối, Hai Bà Trưng, Hà Nội', N'02439714899', N'nxb@vnu.edu.vn');

INSERT  INTO authors (
    name,
    biography
)
VALUES              (N'Nguyễn Nhật Ánh', N'Nhà văn Việt Nam nổi tiếng với nhiều tác phẩm viết về tuổi thơ, tuổi mới lớn và cuộc sống học đường.'),
(N'Nam Cao', N'Nhà văn hiện thực xuất sắc của văn học Việt Nam, nổi tiếng với các tác phẩm viết về người nông dân và trí thức trước Cách mạng tháng Tám.'),
(N'Vũ Trọng Phụng', N'Nhà văn, nhà báo Việt Nam nổi bật với các tác phẩm hiện thực và trào phúng.'),
(N'Ngô Tất Tố', N'Nhà văn, nhà báo Việt Nam, được biết đến với những tác phẩm phản ánh đời sống người nông dân Việt Nam.'),
(N'Tô Hoài', N'Nhà văn Việt Nam có nhiều tác phẩm dành cho thiếu nhi và các tác phẩm viết về đời sống, văn hóa của nhiều vùng miền.'),
(N'Xuân Quỳnh', N'Nhà thơ Việt Nam nổi tiếng với nhiều tác phẩm thơ viết về tình yêu, gia đình và cuộc sống.'),
(N'Hồ Chí Minh', N'Nhà hoạt động cách mạng, nhà văn và nhà thơ Việt Nam, có nhiều tác phẩm văn học và chính luận.'),
(N'Nguyễn Du', N'Đại thi hào dân tộc Việt Nam, tác giả của Truyện Kiều.'),
(N'William Shakespeare', N'Nhà viết kịch và nhà thơ người Anh, được biết đến với nhiều tác phẩm kinh điển của văn học thế giới.'),
(N'George Orwell', N'Tác giả người Anh nổi tiếng với các tiểu thuyết và tác phẩm chính luận, trong đó có 1984 và Animal Farm.'),
(N'J. K. Rowling', N'Nhà văn người Anh, nổi tiếng với loạt tiểu thuyết Harry Potter.'),
(N'Antoine de Saint-Exupéry', N'Nhà văn và phi công người Pháp, nổi tiếng với tác phẩm Hoàng tử bé.'),
(N'Haruki Murakami', N'Nhà văn Nhật Bản nổi tiếng với các tiểu thuyết mang màu sắc hiện thực pha trộn yếu tố siêu thực.'),
(N'Dale Carnegie', N'Tác giả người Mỹ nổi tiếng với các sách về kỹ năng giao tiếp và phát triển bản thân.'),
(N'Stephen Hawking', N'Nhà vật lý lý thuyết người Anh, nổi tiếng với các công trình và sách phổ biến khoa học về vũ trụ.'),
(N'Yuval Noah Harari', N'Nhà sử học và tác giả người Israel, nổi tiếng với các sách về lịch sử loài người và tương lai của nhân loại.');

INSERT  INTO document_types (
    name,
    description
)
VALUES                     (N'Sách', N'Sách in và các ấn phẩm dạng sách.'),
(N'Tạp chí', N'Tạp chí định kỳ.'),
(N'Báo', N'Các loại báo.'),
(N'Luận văn', N'Luận văn, luận án và công trình nghiên cứu.'),
(N'Tài liệu tham khảo', N'Tài liệu phục vụ tra cứu và nghiên cứu.'),
(N'Truyện tranh', N'Tài liệu dạng truyện tranh.'),
(N'Bản đồ', N'Tài liệu bản đồ và địa lý.'),
(N'Tài liệu nghe nhìn', N'Tài liệu âm thanh, hình ảnh và video.');

INSERT  INTO books (
    id,
    isbn,
    title,
    description,
    publication_year,
    language,
    page_count,
    price,
    publisher_id,
    document_type_id,
    created_at,
    updated_at
)
VALUES            ('A1000001-0000-0000-0000-000000000001', N'9786041234501', N'Mắt biếc', N'Tiểu thuyết nổi tiếng của Nguyễn Nhật Ánh, kể về tình yêu tuổi học trò và những ký ức trong trẻo của tuổi thơ.', 1990, N'Việt Nam', 300, 85000, (SELECT id
                                                                                                                                                                                                                                             FROM   publishers
                                                                                                                                                                                                                                             WHERE  name = N'Nhà xuất bản Trẻ'), (SELECT id
                                                                                                                                                                                                                                                                                  FROM   document_types
                                                                                                                                                                                                                                                                                  WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000002', N'9786041234502', N'Tôi thấy hoa vàng trên cỏ xanh', N'Tác phẩm viết về tuổi thơ, tình bạn và những rung động đầu đời của những đứa trẻ tại một vùng quê Việt Nam.', 2010, N'Việt Nam', 378, 110000, (SELECT id
                                                                                                                                                                                                                                               FROM   publishers
                                                                                                                                                                                                                                               WHERE  name = N'Nhà xuất bản Trẻ'), (SELECT id
                                                                                                                                                                                                                                                                                    FROM   document_types
                                                                                                                                                                                                                                                                                    WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000003', N'9786041234503', N'Cho tôi xin một vé đi tuổi thơ', N'Một câu chuyện nhẹ nhàng và hài hước về thế giới tuổi thơ qua góc nhìn của những đứa trẻ.', 2008, N'Việt Nam', 208, 90000, (SELECT id
                                                                                                                                                                                                                            FROM   publishers
                                                                                                                                                                                                                            WHERE  name = N'Nhà xuất bản Trẻ'), (SELECT id
                                                                                                                                                                                                                                                                 FROM   document_types
                                                                                                                                                                                                                                                                 WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000004', N'9786041234504', N'Dế Mèn phiêu lưu ký', N'Tác phẩm thiếu nhi kinh điển kể về những cuộc phiêu lưu của Dế Mèn và những bài học về tình bạn, trách nhiệm và cuộc sống.', 1941, N'Việt Nam', 144, 65000, (SELECT id
                                                                                                                                                                                                                                                  FROM   publishers
                                                                                                                                                                                                                                                  WHERE  name = N'Nhà xuất bản Kim Đồng'), (SELECT id
                                                                                                                                                                                                                                                                                            FROM   document_types
                                                                                                                                                                                                                                                                                            WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000005', N'9786041234505', N'Chí Phèo', N'Tác phẩm hiện thực nổi tiếng của Nam Cao, phản ánh số phận người nông dân bị tha hóa trong xã hội cũ.', 1941, N'Việt Nam', 120, 55000, (SELECT id
                                                                                                                                                                                                                  FROM   publishers
                                                                                                                                                                                                                  WHERE  name = N'Nhà xuất bản Văn học'), (SELECT id
                                                                                                                                                                                                                                                           FROM   document_types
                                                                                                                                                                                                                                                           WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000006', N'9786041234506', N'Số đỏ', N'Tiểu thuyết trào phúng nổi tiếng của Vũ Trọng Phụng, phản ánh xã hội Việt Nam thời kỳ thuộc địa.', 1936, N'Việt Nam', 280, 95000, (SELECT id
                                                                                                                                                                                                          FROM   publishers
                                                                                                                                                                                                          WHERE  name = N'Nhà xuất bản Văn học'), (SELECT id
                                                                                                                                                                                                                                                   FROM   document_types
                                                                                                                                                                                                                                                   WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000007', N'9786041234507', N'Tắt đèn', N'Tác phẩm hiện thực phản ánh cuộc sống khổ cực của người nông dân Việt Nam dưới chế độ thực dân phong kiến.', 1939, N'Việt Nam', 220, 80000, (SELECT id
                                                                                                                                                                                                                      FROM   publishers
                                                                                                                                                                                                                      WHERE  name = N'Nhà xuất bản Văn học'), (SELECT id
                                                                                                                                                                                                                                                               FROM   document_types
                                                                                                                                                                                                                                                               WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000008', N'9786041234508', N'Đắc nhân tâm', N'Cuốn sách nổi tiếng về nghệ thuật giao tiếp, ứng xử và xây dựng các mối quan hệ với người khác.', 1936, N'Tiếng Việt', 320, 86000, (SELECT id
                                                                                                                                                                                                                  FROM   publishers
                                                                                                                                                                                                                  WHERE  name = N'Nhà xuất bản Trẻ'), (SELECT id
                                                                                                                                                                                                                                                       FROM   document_types
                                                                                                                                                                                                                                                       WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000009', N'9786041234509', N'Lập trình C++', N'Tài liệu nhập môn lập trình C++ với các kiến thức về biến, kiểu dữ liệu, cấu trúc điều khiển, hàm, mảng và lập trình hướng đối tượng.', 2023, N'Tiếng Việt', 450, 145000, (SELECT id
                                                                                                                                                                                                                                                          FROM   publishers
                                                                                                                                                                                                                                                          WHERE  name = N'Nhà xuất bản Khoa học và Kỹ thuật'), (SELECT id
                                                                                                                                                                                                                                                                                                                FROM   document_types
                                                                                                                                                                                                                                                                                                                WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000010', N'9786041234510', N'1984', N'Tiểu thuyết nổi tiếng của George Orwell, mô tả một xã hội bị kiểm soát và giám sát toàn diện.', 1949, N'Tiếng Việt', 328, 120000, (SELECT id
                                                                                                                                                                                                         FROM   publishers
                                                                                                                                                                                                         WHERE  name = N'Nhà xuất bản Văn học'), (SELECT id
                                                                                                                                                                                                                                                  FROM   document_types
                                                                                                                                                                                                                                                  WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000011', N'9786041234511', N'Hoàng tử bé', N'Tác phẩm kinh điển của Antoine de Saint-Exupéry kể về hành trình của một hoàng tử nhỏ qua nhiều hành tinh.', 1943, N'Tiếng Việt', 120, 70000, (SELECT id
                                                                                                                                                                                                                            FROM   publishers
                                                                                                                                                                                                                            WHERE  name = N'Nhà xuất bản Kim Đồng'), (SELECT id
                                                                                                                                                                                                                                                                      FROM   document_types
                                                                                                                                                                                                                                                                      WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000012', N'9786041234512', N'Rừng Na Uy', N'Tiểu thuyết nổi tiếng của Haruki Murakami xoay quanh tình yêu, tuổi trẻ, mất mát và sự trưởng thành.', 1987, N'Tiếng Việt', 400, 135000, (SELECT id
                                                                                                                                                                                                                      FROM   publishers
                                                                                                                                                                                                                      WHERE  name = N'Nhà xuất bản Trẻ'), (SELECT id
                                                                                                                                                                                                                                                           FROM   document_types
                                                                                                                                                                                                                                                           WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000013', N'9786041234513', N'Lược sử loài người', N'Tác phẩm phổ biến khoa học và lịch sử trình bày quá trình phát triển của loài người từ thời tiền sử đến thời hiện đại.', 2011, N'Tiếng Việt', 560, 180000, (SELECT id
                                                                                                                                                                                                                                                FROM   publishers
                                                                                                                                                                                                                                                WHERE  name = N'Nhà xuất bản Tổng hợp Thành phố Hồ Chí Minh'), (SELECT id
                                                                                                                                                                                                                                                                                                                FROM   document_types
                                                                                                                                                                                                                                                                                                                WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000014', N'9786041234514', N'Lược sử thời gian', N'Cuốn sách phổ biến khoa học trình bày những vấn đề cơ bản về vũ trụ, không gian, thời gian và nguồn gốc của vũ trụ.', 1988, N'Tiếng Việt', 256, 125000, (SELECT id
                                                                                                                                                                                                                                            FROM   publishers
                                                                                                                                                                                                                                            WHERE  name = N'Nhà xuất bản Khoa học và Kỹ thuật'), (SELECT id
                                                                                                                                                                                                                                                                                                  FROM   document_types
                                                                                                                                                                                                                                                                                                  WHERE  name = N'Sách'), SYSUTCDATETIME(), SYSUTCDATETIME()),
('A1000001-0000-0000-0000-000000000015', N'9786041234515', N'Bản đồ Thành phố Hồ Chí Minh', N'Tài liệu bản đồ cung cấp thông tin địa lý và hệ thống địa danh chính trên địa bàn Thành phố Hồ Chí Minh.', 2024, N'Tiếng Việt', NULL, 95000, (SELECT id
                                                                                                                                                                                                                                            FROM   publishers
                                                                                                                                                                                                                                            WHERE  name = N'Nhà xuất bản Tổng hợp Thành phố Hồ Chí Minh'), (SELECT id
                                                                                                                                                                                                                                                                                                            FROM   document_types
                                                                                                                                                                                                                                                                                                            WHERE  name = N'Bản đồ'), SYSUTCDATETIME(), SYSUTCDATETIME());

INSERT  INTO book_authors (
    book_id,
    author_id
)
VALUES                   ('A1000001-0000-0000-0000-000000000001', (SELECT id
                                                                   FROM   authors
                                                                   WHERE  name = N'Nguyễn Nhật Ánh')),
('A1000001-0000-0000-0000-000000000002', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Nguyễn Nhật Ánh')),
('A1000001-0000-0000-0000-000000000003', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Nguyễn Nhật Ánh')),
('A1000001-0000-0000-0000-000000000004', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Tô Hoài')),
('A1000001-0000-0000-0000-000000000005', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Nam Cao')),
('A1000001-0000-0000-0000-000000000006', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Vũ Trọng Phụng')),
('A1000001-0000-0000-0000-000000000007', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Ngô Tất Tố')),
('A1000001-0000-0000-0000-000000000008', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Dale Carnegie')),
('A1000001-0000-0000-0000-000000000009', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Stephen Hawking')),
('A1000001-0000-0000-0000-000000000010', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'George Orwell')),
('A1000001-0000-0000-0000-000000000011', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Antoine de Saint-Exupéry')),
('A1000001-0000-0000-0000-000000000012', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Haruki Murakami')),
('A1000001-0000-0000-0000-000000000013', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Yuval Noah Harari')),
('A1000001-0000-0000-0000-000000000014', (SELECT id
                                          FROM   authors
                                          WHERE  name = N'Stephen Hawking'));

INSERT INTO book_categories (
    book_id,
    category_id
)
SELECT 'A1000001-0000-0000-0000-000000000001',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000002',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000003',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Thiếu nhi')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000004',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Thiếu nhi')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000005',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000006',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000007',
       id
FROM   categories
WHERE  name IN (N'Văn học Việt Nam', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000008',
       id
FROM   categories
WHERE  name IN (N'Phát triển bản thân', N'Kỹ năng giao tiếp')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000009',
       id
FROM   categories
WHERE  name IN (N'Công nghệ thông tin', N'Lập trình', N'Khoa học máy tính')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000010',
       id
FROM   categories
WHERE  name IN (N'Văn học nước ngoài', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000011',
       id
FROM   categories
WHERE  name IN (N'Văn học nước ngoài', N'Thiếu nhi')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000012',
       id
FROM   categories
WHERE  name IN (N'Văn học nước ngoài', N'Tiểu thuyết')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000013',
       id
FROM   categories
WHERE  name IN (N'Lịch sử', N'Văn hóa')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000014',
       id
FROM   categories
WHERE  name IN (N'Khoa học tự nhiên', N'Vật lý')
UNION ALL
SELECT 'A1000001-0000-0000-0000-000000000015',
       id
FROM   categories
WHERE  name = N'Địa lý';