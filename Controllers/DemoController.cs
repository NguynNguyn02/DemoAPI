using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class DemoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public DemoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // GET api/html?url=https://example.com
        [HttpGet]
        public async Task<IActionResult> GetPageSource([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("NULL");
            }
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string pageSource = await response.Content.ReadAsStringAsync();
                return Ok(pageSource);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
