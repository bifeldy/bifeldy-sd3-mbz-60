using System.Data;

using Microsoft.AspNetCore.Mvc;

using bifeldy_sd3_lib_60.Databases;
using bifeldy_sd3_mbz_60.Models;
using bifeldy_sd3_mbz_60.Services;

namespace bifeldy_sd3_wapi_31_new.Controllers {

    [ApiController]
    [Route("[controller]")]
    public sealed class WeatherForecastController : ControllerBase {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOraPg _orapg;
        private readonly WeatherForecastService _wfs;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IOraPg orapg,
            WeatherForecastService wfs
        ) {
            _logger = logger;
            _orapg = orapg;
            _wfs = wfs;
        }

        [HttpGet]
        public async Task<ObjectResult> GetAll() {
            _logger.LogInformation(GetType().Name);
            DateTime dt = await _orapg.ExecScalarAsync<DateTime>("SELECT CURRENT_TIMESTAMP::DATE + 7");
            Random rng = new Random();
            WeatherForecast[] wf = Enumerable.Range(1, 5).Select(index => {
                string[] summaries = _wfs.Summaries;
                return new WeatherForecast {
                    Date = dt,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = summaries[rng.Next(summaries.Length)]
                };
            }).ToArray();
            return Ok(new {
                info = $"😅 200 - {GetType().Name} :: List All 🤣",
                results = wf
            });
        }

    }

}
