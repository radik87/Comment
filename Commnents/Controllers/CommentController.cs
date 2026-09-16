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
            return Json(await _commentService.Get());
        }

        [HttpPost]
        public async Task<IActionResult> Post(Comment comment)
        {
            string cleanHTML = _htmlSanitizerService.Clean(comment.Text);

            //string? filePath = null;
            //string? fileType = null;
            //if (file != null)
            //{
            //    filePath = await _fileService.ProcessUploadedFileAsync(file.OpenReadStream(), file.FileName, file.Length);
            //    fileType = Path.GetExtension(file.FileName).ToLower() == ".txt" ? "text" : "image";
            //}

            if (_htmlSanitizerService.IsValidXhtml(cleanHTML))
            {
                comment.Text = cleanHTML;
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
