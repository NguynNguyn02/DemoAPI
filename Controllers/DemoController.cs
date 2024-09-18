using HtmlAgilityPack.CssSelectors.NetCore;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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

                //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                //requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
                //HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync(url);

                responseMessage.EnsureSuccessStatusCode();

                string pageSource = await responseMessage.Content.ReadAsStringAsync();


                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.LoadHtml(pageSource);

                //give name
                var name = htmlDoc.QuerySelector("div.product-title-small");


                //give price500/price1000
                var divNode = htmlDoc.QuerySelector("div.price-wrapper");

                var price = divNode.QuerySelectorAll("span.woocommerce-Price-amount");
                string price500 = price[0].InnerText.Trim().Replace(".", "").Replace("&#8363;", "").Trim();

                string price1000 = price[1].InnerText.Trim().Replace(".", "").Replace("&#8363;", "").Trim();


                decimal.TryParse(price500, out decimal price500Value);
                decimal.TryParse(price1000, out decimal price1000Value);



                //give list image
                var figureNode = htmlDoc.QuerySelector("figure.woocommerce-product-gallery__wrapper");


                var imgNodes = figureNode.QuerySelectorAll("img");
                List<string> imageUrls = new List<string>();

                foreach (var imgNode in imgNodes)
                {
                    string dataSrc = imgNode.GetAttributeValue("data-src", null);
                    if (!string.IsNullOrEmpty(dataSrc))
                    {
                        imageUrls.Add(dataSrc);
                    }
                }

                var imgList = new List<Image>();
                foreach (var imageUrl in imageUrls) {
                    imgList.Add(new Image
                    {
                        ImageURL = imageUrl,
                    });
                }

                var response = new Demo()
                {
                    name = name.InnerText.Trim(),
                    price500 = price500Value,
                    price1000 = price1000Value,
                    ImageURLList = imgList.ToList()
                };
                return Ok(response);

            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
