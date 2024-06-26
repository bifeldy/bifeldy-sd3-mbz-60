﻿using Microsoft.Extensions.Options;

using bifeldy_sd3_lib_60.Abstractions;
using bifeldy_sd3_lib_60.Databases;
using bifeldy_sd3_lib_60.Services;

using bifeldy_sd3_mbz_60.Models;
using bifeldy_sd3_mbz_60.Services;

namespace bifeldy_sd3_mbz_60.Repositories {

    public interface IWeatherForecastRepository {
        Task<WeatherForecast[]> GetForecastAsync();
    }

    public sealed class CWeatherForecastRepository : CRepository, IWeatherForecastRepository {

        private readonly ENV _env;

        private readonly IOraPg _orapg;
        private readonly IWeatherForecastService _wfs;

        public CWeatherForecastRepository(
            IOptions<ENV> env,
            IApplicationService @as,
            IOraPg orapg,
            IMsSQL mssql,
            IWeatherForecastService wfs
        ) : base(env, @as, orapg, mssql) {
            _orapg = orapg;
            _env = env.Value;
            _orapg = orapg;
            _wfs = wfs;
        }

        public async Task<WeatherForecast[]> GetForecastAsync() {
            DateTime dt = await _orapg.ExecScalarAsync<DateTime>($@"
                SELECT {(_env.IS_USING_POSTGRES ? "CURRENT_TIMESTAMP::DATE" : "TRUNC(SYSDATE)")} + 7
                {(_env.IS_USING_POSTGRES ? "" : "FROM DUAL")}
            ");
            Random rng = new Random();
            WeatherForecast[] wf = Enumerable.Range(1, 5).Select(index => {
                string[] summaries = _wfs.Summaries;
                return new WeatherForecast {
                    Date = dt,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = summaries[rng.Next(summaries.Length)]
                };
            }).ToArray();
            return wf;
        }

    }

}
