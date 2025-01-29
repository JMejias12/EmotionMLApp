using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FaceDetectionController : ControllerBase
{
    private readonly FaceDetectionService _faceDetectionService;

    public FaceDetectionController(FaceDetectionService faceDetectionService)
    {
        _faceDetectionService = faceDetectionService;
    }

    [HttpPost("DetectFaces")]
    public async Task<IActionResult> DetectFaces([FromBody] ImageData imageData)
    {
        var bytes = Convert.FromBase64String(imageData.Base64ImageData.Split(',')[1]);
        using var ms = new MemoryStream(bytes);
        using var bitmap = new Bitmap(ms);

        var boxes = _faceDetectionService.DetectFaces(bitmap);
        return Ok(new { boxes });
    }
}

public class ImageData
{
    public string Base64ImageData { get; set; }
}
