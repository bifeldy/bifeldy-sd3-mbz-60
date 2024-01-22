/**
 * 
 * Author       :: Basilius Bias Astho Christyono
 * Phone        :: (+62) 889 236 6466
 * 
 * Department   :: IT SD 03
 * Mail         :: bias@indomaret.co.id
 * 
 * Catatan      :: Bifeldy's Entry Point
 * 
 */

using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpOverrides;

using MudBlazor.Services;

using bifeldy_sd3_lib_60;
using bifeldy_sd3_lib_60.Extensions;

using bifeldy_sd3_mbz_60.Services;
using bifeldy_sd3_mbz_60.Models;
// using bifeldy_sd3_mbz_60.JobSchedulers;

string apiUrlPrefix = "api";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Services.Configure<ENV>(builder.Configuration.GetSection("ENV"));

Bifeldy.InitBuilder(builder);
Bifeldy.SetupSerilog();
Bifeldy.LoadConfig();
Bifeldy.AddSwagger(
    apiUrlPrefix,
    "Portal Database API",
    "Tempat Lempar Query :: Oracle / Postgre / MsSQL",
    true,
    false
);
Bifeldy.AddJobScheduler();
Bifeldy.SetupDI();

// Background Hosted Service Long Run Task Di Sini --
// Bifeldy.AddKafkaConsumerBackground("172.31.2.122:9092", "web_approval", _suffixKodeDc: true);

// Job Scheduler Di Sini -- https://www.freeformatter.com/cron-expression-generator-quartz.html
// Bifeldy.CreateJobSchedule<JobExample>("0 * * ? * *");
// Bifeldy.CreateJobSchedules("0 * * ? * *", typeof(JobExample), typeof(JobExample), typeof(JobExample));

builder.Services.AddCors();
builder.Services.AddControllers(x => {
    x.UseRoutePrefix(apiUrlPrefix);
}).AddXmlSerializerFormatters();
builder.Services.AddRazorPages(options => {
    options.RootDirectory = "/Templates";
});
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();

// Tambah Dependency Injection Di Sini --
builder.Services.AddSingleton<WeatherForecastService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
else {
    app.UseExceptionHandler("/Error");
    // app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(x =>
    x.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
);
// app.UseAuthentication();
// app.UseAuthorization();
app.UseForwardedHeaders(
    new ForwardedHeadersOptions {
        ForwardedHeaders = ForwardedHeaders.All
    }
);

Bifeldy.InitApp(app);
Bifeldy.UseSwagger(apiUrlPrefix);
await Bifeldy.StartJobScheduler();
Bifeldy.UseHelmet();
Bifeldy.UseNginxProxyPathSegment();
Bifeldy.UseErrorHandlerMiddleware();
Bifeldy.UseApiKeyMiddleware();
Bifeldy.UseJwtMiddleware();

app.UseEndpoints(x => {
    x.MapControllers();
    x.Map("/api/{**slug}", Bifeldy.Handle404ApiNotFound);
    x.MapBlazorHub();
    x.MapRazorPages();
    x.MapFallbackToPage("/{**slug}", "/_Host");
});
await app.RunAsync();
