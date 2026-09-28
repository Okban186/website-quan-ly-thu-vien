using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class LibraryMember
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public LibraryMembershipStatus MembershipStatus { get; set; }

    public DateTime RegisteredAt { get; set; }
    public DateTime? ExpiredAt { get; set; }

    public User User { get; set; } = null!;

    public LibraryCard? LibraryCard { get; set; }

    public ICollection<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}