using Microsoft.AspNetCore.Mvc;

namespace ApiKeyAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    [ServiceFilter(typeof(ApiKeyFilter))]
    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        return Ok();
    }
}