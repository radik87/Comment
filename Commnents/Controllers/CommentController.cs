using CommentsApp.Core.Services;
using Commnents.Constans;
using Commnents.Models;
using Commnents.Services;
using Microsoft.AspNetCore.Mvc;
using Philiprehberger.HtmlSanitizer;

namespace Commnents.Controllers
{
    [Route(RouteConst.Default)]
    [ApiController]
    public class CommentController : Controller
    {
      private readonly CommentService _commentService;
        private readonly HtmlSanitizerService _htmlSanitizerService;

        public CommentController(CommentService commentService, HtmlSanitizerService htmlSanitizerService)
        {
            _commentService = commentService;
            _htmlSanitizerService = htmlSanitizerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Json(await  _commentService.Get());
        }

        [HttpPost]
        public async Task<IActionResult> Post(Comment comment)
        {
            string cleanHTML = _htmlSanitizerService.Clean(comment.Text);

            if (_htmlSanitizerService.IsValidXhtml(cleanHTML))
            {
                return Json(await _commentService.Create(comment));
            }
            else
            {
                return BadRequest("invalid XTML check text your message");
            }
        }

        // method for mock data
        [HttpPost]
        [Route("many")]
        public async Task<IActionResult> PostMany(List<Comment> comments)
        {
            return Json(await _commentService.CreateMany(comments));
        }
    }
}
