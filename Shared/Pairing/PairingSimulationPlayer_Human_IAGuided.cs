namespace IXAge_IHM.Shared.Pairing
{
    public class PairingSimulationPlayer_Human_IAGuided : IPairingSimulationPlayer
    {
        public string Name { get => "Human"; }
        IPairingSimulationPlayer _IA;

        public bool AutoChoice { get => false; }
        public bool Advice(int phase) => true;

        public PairingSimulationPlayer_Human_IAGuided(IPairingSimulationPlayer IA)
        {
            _IA = IA;
        }
        public int AdviceChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            return _IA.ChooseOne(options, restTeam1, restTeam2);
        }

        public (int, int) AdviceChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            return _IA.ChooseResponse(myChoice, myChoiceLabel, ennemy, ennemyLabel, options, restTeam1, restTeam2);
        }

        public int AdviceAcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
                        int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            return _IA.AcceptResponse(ennemy, ennemyLabel, ennemyPossibility, ennemyPossibilityLabel,
                you, youLabel, possibility, possibilityLabel, restTeam1, restTeam2);
        }

        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var advise = _IA.ChooseOne(options, restTeam1, restTeam2);
            Console.Write("Choose your player ("+String.Join(", ", options.Select(t => t.Key + ": "+t.Value))+") IA recommande ("+advise+"):");
            while (true) {
                if (int.TryParse(Console.ReadLine(), out var v) &&  options.ContainsKey(v))
                    return v;
                Console.Write("\nInvalid, Try Again :" );
            }
        }

        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var advise = _IA.ChooseResponse(myChoice, myChoiceLabel, ennemy, ennemyLabel, options, restTeam1, restTeam2);
            Console.WriteLine("Enemy choose "+ennemyLabel+", choose your response  : ("+String.Join(", ", options.Select(t => t.Key + ": "+t.Value))+") IA recommend ("+advise.Item1+" and "+advise.Item2+"):");
            while (true)
            {
                Console.Write("Response one :");
                var s1 = Console.ReadLine();
                Console.Write("Response two :");
                var s2 = Console.ReadLine();
                if (int.TryParse(s1, out var v1) && int.TryParse(s2, out var v2) && v1 != v2 && options.ContainsKey(v1)&& options.ContainsKey(v2))
                    return (v1, v2);
                Console.WriteLine("\nInvalid, Try Again :");
            }
        }

        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            var advise = _IA.AcceptResponse(ennemy, ennemyLabel, ennemyPossibility, ennemyPossibilityLabel,
                you, youLabel, possibility, possibilityLabel, restTeam1,  restTeam2);
            Console.Write("Choose opponent for "+youLabel+" : ("+possibility.Item1+": "+possibilityLabel.Item1+", "+possibility.Item2+": "+possibilityLabel.Item2+") IA Recommend ("+advise+") :");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out var v) && (possibility.Item1==v || possibility.Item2==v))
                    return v;
                Console.Write("\nInvalid, Try Again :");
            }
        }

        public void FightValidate(int p1, int p2)
        {
        }
    }

}
