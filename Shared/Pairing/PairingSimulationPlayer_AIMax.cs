namespace IXAge_IHM.Shared.Pairing
{
    public class PairingSimulationPlayer_AIMax : IPairingSimulationPlayer
    {
        public string Name { get => "AI Max"; }

        protected PairingScenario _simu;
        protected TreeNode _computed;
        public PairingSimulationPlayer_AIMax(PairingScenario simu)
        {
            _simu= simu;
            _computed= TreeNode.Build(_simu);
        }

        public int Get2ndMinValue(int key)
        {
            var min1 = 50;
            var min2 = 50;
            foreach (var elem in _computed.Childs.Where(t => t.Key.Item1 == key))
            {
                if (elem.Value.BestValue < min1)
                {
                    if (min2 > min1)
                    {
                        min2 = min1;
                    }
                    min1 = elem.Value.BestValue;
                }
                else if (elem.Value.BestValue < min2)
                    min2 =elem.Value.BestValue;
            }
            return min2;
        }

        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            int maxK = 0;
            int maxV = -1;
            foreach (var key in options.Keys)
            {
                int v = Get2ndMinValue(key);// _computed.Childs.Where(t => t.Key.Item1 == key).Max(t => t.Value.BestValue);
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
                    int v = _computed.Childs.Where(t => t.Key.Item1 == key && t.Key.Item2 == ennemy).Max(t => t.Value.BestValue);
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
            int v1 = _computed.Childs.Where(t => t.Key.Item1 == you && t.Key.Item2 == possibility.Item1).Max(t => t.Value.BestValue);
            int v2 = _computed.Childs.Where(t => t.Key.Item1 == you && t.Key.Item2 == possibility.Item2).Max(t => t.Value.BestValue);
            if (v1 >= v2)
            {
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
