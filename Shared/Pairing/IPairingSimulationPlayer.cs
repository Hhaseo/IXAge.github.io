namespace IXAge_IHM.Shared.Pairing
{
    public interface IPairingSimulationPlayer
    {
        string Name { get; }
        public static List<string> IAAvailable => new List<string>() { "Human", "Human (IA v1 Guided)", "Human (IA v2 Guided)", "IA v1", "IA v2" };

        public static string DefaultP1 = "Human (IA v2 Guided)";

        public static string DefaultP2 = "IA v2";

        static IPairingSimulationPlayer CreatePlayer(bool firstP, string Name, PairingScenario simu)
        {
            switch (Name)
            {
                case "Human": return new PairingSimulationPlayer_Human();
                case "Human (IA v1 Guided)": return new PairingSimulationPlayer_Human_IAGuided(new PairingSimulationPlayer_AIMax(simu));
                case "Human (IA v2 Guided)": return new PairingSimulationPlayer_Human_IAGuided(new PairingSimulationPlayer_AIMinMax_V3(firstP, simu));
                case "IA v1": return new PairingSimulationPlayer_AIMax(simu);
                case "IA v2": return new PairingSimulationPlayer_AIMinMax_V3(firstP, simu);
            }
            return null;
        }

        static IPairingSimulationPlayer CreatePlayer(bool firstP, int i, PairingScenario simu)
        {
            switch (i)
            {
                case 0: return new PairingSimulationPlayer_Human();
                case 1: return new PairingSimulationPlayer_Human_IAGuided(new PairingSimulationPlayer_AIMax(simu));
                case 2: return new PairingSimulationPlayer_Human_IAGuided(new PairingSimulationPlayer_AIMinMax_V3(firstP, simu));
                case 3: return new PairingSimulationPlayer_AIMax(simu);
                case 4: return new PairingSimulationPlayer_AIMinMax_V3(firstP, simu);
                case 5: return new PairingSimulationPlayer_AIMinMax(simu);
                case 6: return new PairingSimulationPlayer_AILocalMax(simu);
                case 7: return new PairingSimulationPlayer_AIMinMax_v2(simu);
                case 8: return new PairingSimulationPlayer_AIMinMax_V4(firstP, simu);
            }
            return null;
        }

        public bool AutoChoice { get => true; }
        public bool Advice { get => false; }
        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2);
        public void FightValidate(int p1, int p2);
        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2);
        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
                            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2);

        public int AdviceChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2) => 0;
        public (int, int) AdviceChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2) => (0,0);
        public int AdviceAcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
                        int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2) => 0;
    }

}
