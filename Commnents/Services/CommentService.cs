using Commnents.Models;
using Commnents.Repository;

namespace Commnents.Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepository;
        public CommentService(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<List<Comment>> Get()
        {
            return await _commentRepository.Get();
        }

        public async Task<Comment> Create(Comment comment)
        {
            return await _commentRepository.Create(comment);
        }

        // method for mock data
        public async Task<List<Comment>> CreateMany(List<Comment> comments)
        {
            return await _commentRepository.CreateMany(comments);
        }
    }
}
