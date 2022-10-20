using IXAge_IHM.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IXAge_IHM.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IXAgeController : ControllerBase
    {
        private readonly ILogger<IXAgeController> _logger;

        public static Dictionary<string, Dictionary<string, Book>> _versionsArmies = new Dictionary<string, Dictionary<string, Book>>();

        public IXAgeController(ILogger<IXAgeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetVersions")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/GetVersions")]
        public IEnumerable<string> GetVersions()
        {
            return _versionsArmies.Select(t => t.Key);
        }
        [HttpGet("GetArmies")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/GetArmies")]
        public IEnumerable<string> GetArmies(string version)
        {
            if (!_versionsArmies.ContainsKey(version)) return new List<string>();
            return _versionsArmies[version].Select(t => t.Key);
        }
    }
}