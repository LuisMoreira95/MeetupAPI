using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MMeetupAPI.Controllers
{
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfigController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("reload")]
        public ActionResult Reload()
        {
            try
            {
                ((IConfigurationRoot)this.configuration).Reload();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}
