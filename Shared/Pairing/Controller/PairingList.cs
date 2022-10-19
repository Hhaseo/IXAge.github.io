using IXAge_IHM.Shared.Pairing;

namespace IXAge_IHM.Server.Controllers
{
    public class PairingList
    {
        public string Label { get; set; } = "";
        public List<PairingScenario> PairingSimulations { get; set; } = new List<PairingScenario>();
    }
}