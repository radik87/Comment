using CommentsApp.Core.Services;
using Commnents.Constans;
using Commnents.Models;
using Commnents.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commnents.Controllers
{
    [Route(RouteConst.Default)]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;
        private readonly HtmlSanitizerService _htmlSanitizerService;
        private readonly FileService _fileService;

        public CommentController(CommentService commentService, HtmlSanitizerService htmlSanitizerService, FileService fileService)
        {
            _commentService = commentService;
            _htmlSanitizerService = htmlSanitizerService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Comment> comments = await _commentService.Get();
            return Json(await _commentService.Get());
        }

        //[HttpGet]
        //public async Task<IActionResult> GetPages([FromQuery]int page = 1)
        //{
        //    int pageSize = 25;
        //    List<Comment> comments = await _commentService.Get();
        //    int count = comments.Count();
        //    dynamic items = comments.Skip((page - 1) * pageSize).Take(pageSize);

        //    PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

        //    IndexViewModel viewModel = new IndexViewModel
        //    {
        //        PageViewModel = pageViewModel,
        //        Comments = items
        //    };

        //    return Json(viewModel);
        //}

        [HttpPost]
        public async Task<IActionResult> Post(Comment comment, [FromForm] IFormFile? file)
        {
            if (file != null)
            {
                try
                {
                    comment = await _fileService.SaveFile(comment, file);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            comment.Text = _htmlSanitizerService.Clean(comment.Text);

            if (_htmlSanitizerService.IsValidXhtml(comment.Text))
            {
                return Json(await _commentService.Create(comment));
            }
            else
            {
                return BadRequest("invalid XHTML check text your message");
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
