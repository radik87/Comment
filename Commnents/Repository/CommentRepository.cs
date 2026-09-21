using Commnents.Models;
using Microsoft.EntityFrameworkCore;

namespace Commnents.Repository
{
    public class CommentRepository
    {
        private readonly CommentContext _commentContext;

        public CommentRepository(CommentContext commentContext)
        {
            _commentContext = commentContext;
        }

        public async Task<List<Comment>> Get()
        {
            return await _commentContext.Comments
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Take(25)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Comment> Create(Comment comment)
        {
            _commentContext.Comments.Add(comment);
            _commentContext.Users.Add(comment.User);
            await _commentContext.SaveChangesAsync();

            return comment;
        }


        public async Task<List<Comment>> CreateMany(List<Comment> comments)
        {
            foreach (Comment comment in comments)
            {
                _commentContext.Comments.Add(comment);
                _commentContext.Users.Add(comment.User);
            }

            await _commentContext.SaveChangesAsync();

            return comments;
        }
    }
}
