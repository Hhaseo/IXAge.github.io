namespace IXAge_IHM.Shared.Pairing
{
    public class Fight
    {
        public List<(int, int)> fight = new List<(int, int)>();

        public List<Fight> PotentialsChilds = new List<Fight>();


        public (int, int)? Evaluated = null;

        public bool IsSameFight(List<(int, int)> f)
        {
            if (f.Count != fight.Count)
                return false;
            foreach (var elem in fight)
            {
                if (!f.Any(t => t.Item1 == elem.Item1 && t.Item2 == elem.Item2))
                    return false;
            }
            return true;
        }
        public bool IsStartingBy(List<(int, int)> f)
        {
            for (int i = 0; i < fight.Count() && i < f.Count(); i++)
            {
                if (f[i] != fight[i])
                    return false;
            }
            return true;
        }
        public bool IsStartingByWithNext(List<(int, int)> f, int? key, int? ennemy)
        {
            int i = 0;
            for (i = 0; i < fight.Count() && i < f.Count(); i++)
            {
                if (f[i] != fight[i])
                    return false;
            }
            if (key != null && fight[i].Item1 != key)
                return false;
            if (ennemy != null && fight[i].Item2 != ennemy)
                return false;
            return true;
        }
        public bool IsStartingByWithNotEnnemy(List<(int, int)> f, int? key, int? ennemy)
        {
            int i = 0;
            for (i = 0; i < fight.Count() && i < f.Count(); i++)
            {
                if (f[i] != fight[i])
                    return false;
            }
            if (key != null && fight[i].Item1 != key)
                return false;
            if (ennemy != null && fight[i].Item2 == ennemy)
                return false;
            return true;
        }
        public int GetOpponent(List<(int, int)> f, int? key, int? ennemy)
        {
            int i = 0;
            for (i = 0; i < fight.Count() && i < f.Count(); i++)
            {
                if (f[i] != fight[i])
                    return -1;
            }
            if (key != null && fight[i].Item1 == key)
                return fight[i].Item2;
            if (ennemy != null && fight[i].Item2 == ennemy)
                return fight[i].Item1;
            return -1;
        }
    }

    public class Tree_V2
    {
        public List<(Fight, int)> Evaluations = new List<(Fight, int)>();

        public List<Fight> NodeFights = new List<Fight>();
        public Fight InitialFight = new Fight();

        public int GetEvaluation(Fight f) => Evaluations.Where(t => t.Item1.IsSameFight(f.fight))?.FirstOrDefault().Item2 ?? 0;

        public void BuildEvaluation(PairingScenario simu, Fight parent, List<(int, int)> res, int currentValue, HashSet<int> team, HashSet<int> team2)
        {
            if (team.Any() && team2.Any())
            {
                foreach (var t1 in team)
                {
                    foreach (var t2 in team2)
                    {
                        var stand = new List<(int, int)>(res);
                        stand.Add((t1, t2));
                        var f = new Fight() { fight = stand };
                            NodeFights.Add(f);
                            if (f.fight.Count() == 1)
                                InitialFight.PotentialsChilds.Add(f);
                            if (parent != null)
                                parent.PotentialsChilds.Add(f);
                            var te = new HashSet<int>(team);
                            var te2 = new HashSet<int>(team2);
                            te.Remove(t1);
                            te2.Remove(t2);
                            BuildEvaluation(simu, f, stand, currentValue + simu.Team[t1].Evals[t2], te, te2);
                    }
                }
            }
            else
            {
                var f = new Fight() { fight=res };

                    f.Evaluated = (currentValue, 0);
                    NodeFights.Add(f);
                    Evaluations.Add((f, currentValue));

                // Evaluations.Add((new Fight() { fight=res }, currentValue));
            }
        }

        public void ComputeEvaluation(PairingScenario simu)
        {
            var res = new List<(int, int)>();
            HashSet<int> team = new HashSet<int>();
            HashSet<int> team2 = new HashSet<int>();
            for (int i = 0; i < simu.Team.Count; i++)
                team.Add(i);
            for (int i = 0; i < simu.Opponents.Count; i++)
                team2.Add(i);
            BuildEvaluation(simu, null, res, 0, team, team2);
        }
        public List<string> ToStringEvaluation()
        {
            List<string> txt = new List<string>();
            foreach (var tree in Evaluations)
            {
                txt.Add(string.Join(", ", tree.Item1.fight.Select(t => t.Item1+ " vs "+t.Item2)) + " => "+tree.Item2);
            }
            return txt;
        }
        public List<string> ToStringNodes()
        {
            List<string> txt = new List<string>();
            foreach (var tree in NodeFights)
            {
                txt.Add(string.Join(", ", tree.fight.Select(t => t.Item1+ " vs "+t.Item2))+ " => "+(tree.Evaluated?.Item1 ??-1));
            }
            return txt;
        }
    }
    public class Tree
    {
        public List<(Fight, int)> Evaluations = new List<(Fight, int)>();

        public List<Fight> NodeFights = new List<Fight>();
        public Fight InitialFight = new Fight();

        public int GetEvaluation(Fight f) => Evaluations.Where(t => t.Item1.IsSameFight(f.fight))?.FirstOrDefault().Item2 ?? 0;

        public void BuildEvaluation(PairingScenario simu, Fight parent, List<(int, int)> res, int currentValue, HashSet<int> team, HashSet<int> team2)
        {
            if (team.Any() && team2.Any())
            {
                foreach (var t1 in team)
                {
                    foreach (var t2 in team2)
                    {
                        var stand = new List<(int, int)>(res);
                        stand.Add((t1, t2));
                        var f = new Fight() { fight = stand };
                        var sf = NodeFights.Where(t => t.IsSameFight(f.fight)).FirstOrDefault();
                        if (sf == null)
                        {
                            NodeFights.Add(f);
                            if (f.fight.Count() == 1)
                                InitialFight.PotentialsChilds.Add(f);
                            if (parent != null)
                                parent.PotentialsChilds.Add(f);
                            var te = new HashSet<int>(team);
                            var te2 = new HashSet<int>(team2);
                            te.Remove(t1);
                            te2.Remove(t2);
                            BuildEvaluation(simu, f, stand, currentValue + simu.Team[t1].Evals[t2], te, te2);
                        }
                        else
                        {
                            parent.PotentialsChilds.Add(sf);
                        }

                    }
                }
            }
            else if (!Evaluations.Any(t => t.Item1.IsSameFight(res)))
            {
                var f = new Fight() { fight=res };
                var sf = NodeFights.Where(t => t.IsSameFight(f.fight)).FirstOrDefault();
                if (sf == null)
                {
                    f.Evaluated = (currentValue, 0);
                    NodeFights.Add(f);
                    Evaluations.Add((f, currentValue));
                }
                else
                {
                    f = sf;
                    sf.Evaluated = (currentValue, 0);
                    Evaluations.Add((sf, currentValue));
                }

               // Evaluations.Add((new Fight() { fight=res }, currentValue));
            }
        }

        public void ComputeEvaluation(PairingScenario simu)
        {
            var res = new List<(int, int)>();
            HashSet<int> team = new HashSet<int>();
            HashSet<int> team2 = new HashSet<int>();
            for (int i = 0; i < simu.Team.Count; i++)
                team.Add(i);
            for (int i = 0; i < simu.Opponents.Count; i++)
                team2.Add(i);
            BuildEvaluation(simu, null, res, 0, team, team2);
        }
        public List<string> ToStringEvaluation()
        {
            List<string> txt = new List<string>();
            foreach (var tree in Evaluations)
            {
                txt.Add(string.Join(", ", tree.Item1.fight.Select(t => t.Item1+ " vs "+t.Item2)) + " => "+tree.Item2);
            }
            return txt;
        }
        public List<string> ToStringNodes()
        {
            List<string> txt = new List<string>();
            foreach (var tree in NodeFights)
            {
                txt.Add(string.Join(", ", tree.fight.Select(t => t.Item1+ " vs "+t.Item2))+ " => "+(tree.Evaluated?.Item1 ??-1));
            }
            return txt;
        }
    }


        public class TreeNode
    {
        public void Explode(List<(int, int)> current, List<(List<(int, int)>, int)> res)
        {
            if (!this.Childs.Any())
            {
                res.Add((current, Value));
            }
            else
            {
                foreach (var elem in Childs)
                {
                    var newList = new List<(int, int)>(current);
                    newList.Add(elem.Key);
                    elem.Value.Explode(newList, res);
                }
            }
        }

        public List<string> ToString()
        {
            var res = new List<(List<(int, int)>, int)>();
            this.Explode(new List<(int, int)>(), res);
            List<string> txt = new List<string>();
            foreach (var tree in res)
            {
                txt.Add(string.Join(", ", tree.Item1.Select(t => t.Item1+ " vs "+t.Item2)) + " => "+tree.Item2);
            }
            return txt;
        }

        static double CalcStandardDeviation(IEnumerable<int> t)
        {
            if (t.Count() <=1) return 0;
            double average = t.Average();
            double res = 0.0;
            foreach (var v in t)
            {
                res += Math.Pow(v - average, 2);
            }
            return Math.Sqrt(res/(t.Count()-1));
        }

        public bool IsEnd { get; set; } = false;

        public int Value { get; set; } = 0;
        public IEnumerable<int> Values { get => IsEnd ? new List<int>() { Value } : Childs.SelectMany(t => t.Value.Values); }
        public double StandardDeviation { get => CalcStandardDeviation(Values); }
        public int BestValue { get => IsEnd ? Value : Childs.Max(t => t.Value.BestValue); }
        public int WorstValue { get => IsEnd ? Value : Childs.Min(t => t.Value.WorstValue); }
        public Dictionary<(int, int), TreeNode> Childs { get; set; } = new Dictionary<(int, int), TreeNode>();

        public static void BuildTree(PairingScenario simu, TreeNode parent, int currentValue, HashSet<int> team, HashSet<int> team2)
        {
            if (team.Any() && team2.Any())
            {
                foreach (var t1 in team)
                {
                    foreach (var t2 in team2)
                    {
                        var newElem = new TreeNode();
                        parent.Childs[(t1, t2)] = newElem;
                        var te = new HashSet<int>(team);
                        var te2 = new HashSet<int>(team2);
                        te.Remove(t1);
                        te2.Remove(t2);
                        BuildTree(simu, newElem, currentValue + simu.Team[t1].Evals[t2], te, te2);
                    }
                }
            }
            else
            {
                parent.IsEnd = true;
                parent.Value = currentValue;
            }
        }

        public static TreeNode Build(PairingScenario simu)
        {
            TreeNode res = new TreeNode();
            HashSet<int> team = new HashSet<int>(); 
            HashSet<int> team2 = new HashSet<int>();
            for (int i = 0; i < simu.Team.Count; i++)
                team.Add(i);
            for (int i = 0; i < simu.Opponents.Count; i++)
                team2.Add(i);
            BuildTree(simu, res, 0, team, team2);
            return res;
        }
    }

}
