using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApi.Controllers
{
    [Route("api/")]
    [ApiExplorerSettings(GroupName = "О приложении")]
    [ApiController]
    public class MetaController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet("info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Startup).Assembly;

            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok($"Текущая версия: {version}, Последнее обновление: {lastUpdate}");
        }
    }
}
