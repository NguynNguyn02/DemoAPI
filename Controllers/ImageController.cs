using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public ImageController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET api/html?url
        [HttpGet]
        public async Task<IActionResult> GetAllImage([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("NULL");
            }
            try
            {

                HttpResponseMessage responseMessage = await _httpClient.GetAsync(url);

                responseMessage.EnsureSuccessStatusCode();

                string pageSource = await responseMessage.Content.ReadAsStringAsync();


                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.LoadHtml(pageSource);

                var divNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='col-865051549']");


                var imgNodes = divNode.SelectNodes(".//img");
                List<string> imageUrls = new List<string>();

                foreach (var imgNode in imgNodes)
                {
                    string dataSrc = imgNode.GetAttributeValue("data-src", null);
                    if (!string.IsNullOrEmpty(dataSrc))
                    {
                        imageUrls.Add(dataSrc);
                    }
                }


                var response = new List<Image>();
                foreach (var imageUrl in imageUrls)
                {
                    response.Add(new Image
                    {
                        ImageURL = imageUrl
                    });
                }
                return Ok(response);

            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
