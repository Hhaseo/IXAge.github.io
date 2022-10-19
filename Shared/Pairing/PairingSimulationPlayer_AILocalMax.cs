namespace IXAge_IHM.Shared.Pairing
{
    public class PairingSimulationPlayer_AILocalMax : IPairingSimulationPlayer
    {
        public string Name { get => "AI Max"; }

        protected PairingScenario _simu;
        protected TreeNode _computed;
        public PairingSimulationPlayer_AILocalMax(PairingScenario simu)
        {
            _simu= simu;
            _computed= TreeNode.Build(_simu);
        }

        public int Get2ndMinValue(int key, List<Team> Team)
        {
            var min1 = 50;
            var min2 = 50;
            for (int i = 0; i < Team[key].Evals.Count; i++)
            {
                if (Team[key].Evals[i] < min1)
                {
                    if (min2 > min1)
                    {
                        min2 = min1;
                    }
                    min1 = Team[key].Evals[i];
                }
                else if (Team[key].Evals[i]  < min2)
                    min2 =Team[key].Evals[i];
            }
            return min2;
        }

        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            int maxK = 0;
            int maxV = -1;
            foreach (var key in options.Keys)
            {
                int v = Get2ndMinValue(key, _simu.Team);// _computed.Childs.Where(t => t.Key.Item1 == key).Max(t => t.Value.BestValue);
                if (v > maxV)
                {
                    maxK = key;
                    maxV = v;
                }
            }
            return maxK;
        }

        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, 
                int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            int maxK = 0;
            int maxV = -1;
            int maxK2 = 0;
            int maxV2 = -1;
            foreach (var key in options.Keys)
            {
                    int v = _simu.Team[key].Evals[ennemy];
                    if (v > maxV)
                    {
                        if (maxV > maxV2)
                        {
                            maxK2 = maxK;
                            maxV2 = maxV;
                        }
                        maxK = key;
                        maxV = v;
                    }
                    else if (v > maxV2)
                    {
                        maxK2 = key;
                        maxV2 = v;
                    }
            }
            return (maxK, maxK2);
        }

        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel, 
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            int v1 = _simu.Team[you].Evals[possibility.Item1];
            int v2 = _simu.Team[you].Evals[possibility.Item2];
            if (v1 >= v2)
            {
                Console.WriteLine(possibilityLabel.Item1);
                return possibility.Item1;
            }
            return possibility.Item2;
        }

        public void FightValidate(int my, int other)
        {
            if (_computed.Childs.ContainsKey((my, other)))
                _computed = _computed.Childs[(my, other)];
        }
    }

}
