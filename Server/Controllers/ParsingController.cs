using IXAge_IHM.Shared;
using IXAge_IHM.Shared.Parsing;
using Microsoft.AspNetCore.Mvc;

namespace IXAge_IHM.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParsingController : ControllerBase
    {
        private readonly ILogger<ParsingController> _logger;

        //public static Dictionary<string, Dictionary<string, Book>> _versionsArmies = new Dictionary<string, Dictionary<string, Book>>();

        public ParsingController(ILogger<ParsingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Run")]
        //[Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Run")]
        public IEnumerable<string> Get()
        {
            //DirectoryInfo diTop = new DirectoryInfo(@"../Files/");
            DirectoryInfo diTop = new DirectoryInfo(@"../Files/Source/");
            //List<string> dirs = new List<string>(Directory.EnumerateDirectories(docPath));
            List<string> res = new List<string>();
            if (!IXAgeController._versionsArmies.ContainsKey("2022"))
            {
                IXAgeController._versionsArmies["2022"] = new Dictionary<string, Book>();
            }
            foreach (var dir in diTop.EnumerateFiles())
            {
                Console.WriteLine("Load File : " + dir.Name);
                var pg = new Parsing();
                pg.Load(dir.FullName);
                IXAgeController._versionsArmies["2022"][dir.Name] = pg.book;
                res.Add("Load File : " + dir.Name);
                res = res.Concat(pg.ToListString()).ToList();

            }
            return res;
        }

        //[HttpGet("GetVersions")]
        //[Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        //[Route("[controller]/GetVersions")]
        //public IEnumerable<string> GetVersions()
        //{
        //    return _versionsArmies.Select(t => t.Key);
        //}
        //[HttpGet("GetArmies/{version}")]
        //[Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        //[Route("[controller]/GetArmies/{version}")]
        //public IEnumerable<string> GetArmies(string version)
        //{
        //    if (!_versionsArmies.ContainsKey(version)) return new List<string>();
        //    return _versionsArmies[version].Select(t => t.Key);
        //}
    }
}