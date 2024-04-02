using Microsoft.Extensions.Options;

using Quartz;

using bifeldy_sd3_lib_60.Databases;
using bifeldy_sd3_lib_60.Models;
using bifeldy_sd3_lib_60.Services;

namespace bifeldy_sd3_mbz_60.JobSchedulers {

    public sealed class JobFolderCleaner : IJob {

        private readonly ILogger<JobFolderCleaner> _logger;
        private readonly EnvVar _envVar;
        private readonly IOraPg _orapg;
        private readonly IBerkasService _berkas;

        public JobFolderCleaner(
            ILogger<JobFolderCleaner> logger,
            IOptions<EnvVar> envVar,
            IOraPg orapg,
            IBerkasService berkas
        ) {
            _logger = logger;
            _envVar = envVar.Value;
            _orapg = orapg;
            _berkas = berkas;
        }

        public async Task Execute(IJobExecutionContext _context) {
            try {
                DateTime dt = await _orapg.ExecScalarAsync<DateTime>($@"SELECT {(_envVar.IS_USING_POSTGRES ? "NOW()" : "SYSDATE FROM DUAL")}");
                _logger.LogInformation($"Tanggal & Waktu Database => {dt}");
                _berkas.CleanUp(false);
            }
            catch (Exception ex) {
                _logger.LogError($"{_context.Scheduler.SchedulerName} {ex.Message}");
            }
        }

    }

}
