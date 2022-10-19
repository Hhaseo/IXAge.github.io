using IXAge_IHM.Shared;
using IXAge_IHM.Shared.Pairing;
using IXAge_IHM.Shared.Parsing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IXAge_IHM.Server.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class PairingController : ControllerBase
    {
        private readonly ILogger<ParsingController> _logger;

        //public static Dictionary<string, Dictionary<string, Book>> _versionsArmies = new Dictionary<string, Dictionary<string, Book>>();

        public PairingController(ILogger<ParsingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("List")]
        //[Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/List")]
        public IEnumerable<PairingList> List()
        {
            var res = new List<PairingList>();
            // Make a reference to a directory.
            DirectoryInfo di = new DirectoryInfo("./");

            // Get a reference to each directory in that directory.
            DirectoryInfo[] diArr = di.GetDirectories();

            // Display the names of the directories.
            foreach (DirectoryInfo dri in diArr)
            {
                if (dri.Name.Count() > 5 && dri.Name.Substring(0, 5) == "Data_")
                {
                    var teams = Directory.GetFiles(dri.FullName).Where(
                         file => Path.GetExtension(file) == ".json"
                     ).ToList();
                    var newP = new PairingList() { Label = dri.Name.Substring(5, dri.Name.Length - 5) };
                    foreach (var team in teams)
                    {
                        string text = System.IO.File.ReadAllText(team);
                        var elems = JsonConvert.DeserializeObject<PairingScenario>(text);
                        if (elems != null)
                        {
                            newP.PairingSimulations.Add(elems);
                        }
                    }
                    if (newP.PairingSimulations.Any())
                        res.Add(newP);
                }
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