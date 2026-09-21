using System.ComponentModel.DataAnnotations;

namespace Commnents.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        [Required]
        public string Text { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public string? FilePath { get; set; }
        public string? FileType { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        // Ограничение в два уровня (Self-Referencing)
        // Если ParentId == null -> это заглавный комментарий первого уровня
        // Если ParentId != null -> это ответ второго уровня
        public Guid? ParentId { get; set; }
        public Comment? Parent { get; set; }

        // Список ответов на этот комментарий (будет заполнен только для заглавных)
        public List<Comment> Replies { get; set; } = new();
    }
}
