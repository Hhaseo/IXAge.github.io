namespace IXAge_IHM.Shared.Pairing
{
    public class Team
    {
        public string Name { get; set; } = "";
        public string Army { get; set; } = "";

        public string MyLabel { get => Name + "\t-\t" + Army; }
        public List<int> Evals { get; set; } = new List<int>();
        public string Comment { get; set; } = "";
    }
    public class TeamEvals
    {
        public string Player { get; set; } = "";
        public string Ennemy { get; set; } = "";

        public int Eval { get; set; } = 0;
    }

}
