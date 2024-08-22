using json.compare.BusinessLogic.Entities;
using json.compare.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace json.compare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JsonCompareController : ControllerBase
    {
        private readonly IJsonCompareService _jsonCompareService;

        public JsonCompareController(IJsonCompareService jsonCompareService)
        {
            _jsonCompareService = jsonCompareService;
        }

        [HttpPost("compare")]
        public IActionResult Compare([FromBody] JsonCompareRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Json1) || string.IsNullOrEmpty(request.Json2))
            {
                return BadRequest("Both JSON objects must be provided.");
            }

            JObject jObj1, jObj2;

            try
            {
                jObj1 = JObject.Parse(request.Json1);
                jObj2 = JObject.Parse(request.Json2);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON format.");
            }

            var jsonCompare = new JsonCompareRequest
            {
                Json1 = jObj1.ToString(), // Convert JObject to string
                Json2 = jObj2.ToString()  // Convert JObject to string
            };

            var differences = _jsonCompareService.CompareJson(jsonCompare);
            return Ok(differences);
        }
    }
}
