namespace IXAge_IHM.Shared
{

    public class PairingTeams
    {
        public List<string> TeamOne { get; set; } = new List<string>();
        public List<string> TeamTwo { get; set; } = new List<string>();
        public List<List<int>> Evaluations { get; set; } = new List<List<int>>();
    }
    public class GlobalStats
    {
        public bool HasRandomMvt { get => GroundAdv.Contains("D"); }
        public bool HasRandomFlyMvt { get => FlyAdv.Contains("D"); }
        public bool CanFly { get => FlyAdv != ""; }
        public string GroundAdv { get; set; } = "";
        public int GroundAdvV { get => int.TryParse(GroundAdv, out int v) ? v : -1; }
        public string GroundMar { get; set; } = "";
        public int GroundMarV { get => int.TryParse(GroundMar, out int v) ? v : -1; }
        public string FlyAdv { get; set; } = "";
        public int FlyAdvV { get => int.TryParse(FlyAdv, out int v) ? v : -1; }
        public string FlyMar { get; set; } = "";
        public int FlyMarV { get => int.TryParse(FlyMar, out int v) ? v : -1; }
        public string Dis { get; set; } = "";
        public int DisV { get => int.TryParse(Dis, out int v) ? v : -1; }
        public string Rea { get; set; } = "";
        public string ModelRules { get; set; } = "";
    }
    public class DefensiveStats
    {
        public string HP { get; set; } = "";
        public int HPV { get => int.TryParse(HP, out int v) ? v : -1; }
        public string Def { get; set; } = "";
        public int DefV { get => int.TryParse(Def, out int v) ? v : -1; }
        public string Res { get; set; } = "";
        public int ResV { get => int.TryParse(Res, out int v) ? v : -1; }
        public string Arm { get; set; } = "";

        public int ArmV { get => int.TryParse(Arm, out int v) ? v : -1; }
        public string Options { get; set; } = "";
    }
    

    public class OffensiveStats
    {
        public string Att { get; set; } = "";
        public int AttV { get => int.TryParse(Att, out int v) ? v : -1; }
        public string Off { get; set; } = "";
        public int OffV { get => int.TryParse(Off, out int v) ? v : -1; }
        public string Str { get; set; } = "";
        public int StrV { get => int.TryParse(Str, out int v) ? v : -1; }
        public string AP { get; set; } = "";
        public int APV { get => int.TryParse(AP, out int v) ? v : -1; }
    }

    public class Unit
    {
        public string Name { get; set; } = "";
        public string Height { get; set; } = "";
        public string Type { get; set; } = "";
        public string Base { get; set; } = "";
        public int Cost { get; set; } = -1;
        public int CostExtraModel { get; set; } = -1;
        public bool isSingleModel { get; set; } = false;
        public (int, int)? unitsModels { get; set; } = null;
        public (int, int)? LimitUnits { get; set; } = null;
        public (int, int)?LimitMount { get; set; } = null;
        public (int, int)? LimitModels { get; set; } = null;
        public List<string> data = new List<string>();
        
        public GlobalStats gbStats { get; set; } = null;
        public DefensiveStats deffStats { get; set; } = null;
        public Dictionary<string, OffensiveStats> offStats { get; set; } = new Dictionary<string, OffensiveStats>();
    }

    public class Section
    {
        public string Limit { get; set;  } = "";
        public List<string> data = new List<string>();
        public List<Unit> Units { get; set; } = new List<Unit>();
    }

    public class EndingRecap
    {
        public List<string> data = new List<string>();
    }
    public class Book
    {
        public string ArmyName { get; set; } = "";
        public Dictionary<string, Section> sections = new Dictionary<string, Section>();
        public EndingRecap endingRecap = new EndingRecap();
    }
}