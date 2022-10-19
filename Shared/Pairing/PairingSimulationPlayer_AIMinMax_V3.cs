using System.Collections.Concurrent;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IXAge_IHM.Shared.Pairing
{

    public class PairingSimulationPlayer_AIMinMax_V3 : IPairingSimulationPlayer
    {
        public string Name { get => "AI_MinMax_V3"; }

        protected PairingScenario _simu;
        protected Fight _computed;
        protected Tree_V2 _tree;
        static protected int i = 0;
        protected bool FirstP;
        public (int, int) expectedAdv = (0, 0);

        public class FirstChoice
        {
            public int MyChoice { get; set; }
            public (int, int) ExpectedAdv { get; set; }

        }

        public PairingSimulationPlayer_AIMinMax_V3(bool firstP, PairingScenario simu)
        {
            FirstP = firstP;
            _simu = simu;
            _tree = new Tree_V2();
            _tree.ComputeEvaluation(simu);
            _computed = _tree.InitialFight;
            File.WriteAllLines("./TreeEvaluations.txt", _tree.ToStringEvaluation());
            File.WriteAllLines("./TreeData.txt", _tree.ToStringNodes());
            string jsonString =  JsonSerializer.Serialize(_tree);
            File.WriteAllText("./Tree.json", jsonString);
            //var stand = SimuStep1_Ally(_computed);
            //jsonString = JsonSerializer.Serialize(_tree);
            //File.WriteAllText("./TreePost.json", jsonString);
        }
        bool TakeBest(bool best, int v1, int v2)
        {
            if (best)
                return v2 > v1;
            return v2 < v1;
        }


        public (int, int, int, int) ImaginateMyTurn(List<(int, int)> fight,List<int> restTeam1, List<int> restTeam2)
        {
            int vB = 0;
            int kB = 0;
            int kBPrim = 0;
            int B = 0;
            foreach (var op in restTeam1)
            {
                int v1 = 2000;
                int k1 = 0;
                int v2 = 2000;
                int k2 = 0;
                // On prend les 2 plus mauvais résultat (= choix ennemi)
                foreach (var ennemy in restTeam2)
                {
                    if (restTeam1.Count > 4)
                    {
                        var stTeam1 = new List<int>(restTeam1);
                        var stTeam2 = new List<int>(restTeam2);

                        stTeam1.Remove(op);
                        stTeam2.Remove(ennemy);

                        var stFight = new List<(int, int)>(fight);
                        stFight.Add((op, ennemy));
                        var nextEvals = ImaginateEnnemyTurn(stFight, stTeam1, stTeam2);
                        if (TakeBest(false, v1, nextEvals.Item4))
                        {
                            if (TakeBest(false, v2, v1))
                            {
                                v2 = v1;
                                k2 = k1;
                            }
                            v1 = nextEvals.Item4;
                            k1 = ennemy;
                        }
                        else if (TakeBest(false, v2, nextEvals.Item4))
                        {
                            v2 = nextEvals.Item4;
                            k2 = ennemy;
                        }
                    }
                    else
                    {
                        var nextEvals = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(fight, op, null)).Select(t => t.Item2).Min();
                        if (TakeBest(false, v1, nextEvals))
                        {
                            if (TakeBest(false, v2, v1))
                            {
                                v2 = v1;
                                k2 = k1;
                            }
                            v1 = nextEvals;
                            k1 = ennemy;
                        }
                        else if (TakeBest(false, v2, nextEvals))
                        {
                            v2 = nextEvals;
                            k2 = ennemy;
                        }
                    }
                }
                if (TakeBest(true, vB, v2))
                {
                    vB = v2;
                    kB = k2;
                    kBPrim = k1;
                    B = op;
                }
            }
            return (B, kB, kBPrim, vB);
        }
        public (int, int, int, int) ImaginateEnnemyTurn(List<(int, int)> fight, List<int> restTeam1, List<int> restTeam2)
        {
            int vB = 2000;
            int kB = 0;
            int kBPrim = 0;
            int B = 0;
            foreach (var op in restTeam1)
            {
                int v1 = 0;
                int k1 = 0;
                int v2 = 0;
                int k2 = 0;
                // On prend les 2 plus mauvais résultat (= choix ennemi)
                foreach (var ennemy in restTeam2)
                {
                    if (restTeam1.Count > 3)
                    {
                        var stTeam1 = new List<int>(restTeam1);
                        var stTeam2 = new List<int>(restTeam2);

                        stTeam1.Remove(op);
                        stTeam2.Remove(ennemy);

                        var stFight = new List<(int, int)>(fight);
                        stFight.Add((op, ennemy));
                        var nextEvals = ImaginateMyTurn(stFight, stTeam1, stTeam2);
                        if (TakeBest(true, v1, nextEvals.Item4))
                        {
                            if (TakeBest(true, v2, v1))
                            {
                                v2 = v1;
                                k2 = k1;
                            }
                            v1 = nextEvals.Item4;
                            k1 = ennemy;
                        }
                        else if (TakeBest(true, v2, nextEvals.Item4))
                        {
                            v2 = nextEvals.Item4;
                            k2 = ennemy;
                        }
                    }
                    else
                    {
                        var nextEvals = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(fight, op, null)).Select(t => t.Item2).Max();
                        if (TakeBest(true, v1, nextEvals))
                        {
                            if (TakeBest(true, v2, v1))
                            {
                                v2 = v1;
                                k2 = k1;
                            }
                            v1 = nextEvals;
                            k1 = ennemy;
                        }
                        else if (TakeBest(true, v2, nextEvals))
                        {
                            v2 = nextEvals;
                            k2 = ennemy;
                        }
                    }
                }
                if (TakeBest(false, vB, v2))
                {
                    vB = v2;
                    kB = k2;
                    kBPrim = k1;
                    B = op;
                }
            }
            return (B, kB, kBPrim, vB);
        }

        public string LabelFile { get => Name + "_" + _simu.Label + "_" + (FirstP ? "FirstPlayer" : "SecondPlayer")+".json"; } 

        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            int vB = 0;
            int kB = 0;
            int kBPrim = 0;
            int B = 0;
            if (options.Count == 6)
            {
                try
                {
                    if (File.Exists(LabelFile))
                    {

                        string text = File.ReadAllText(LabelFile);
                        var firstChoice = JsonConvert.DeserializeObject<FirstChoice>(text);
                        expectedAdv = firstChoice.ExpectedAdv;
                        return firstChoice.MyChoice;
                    }
                }
                catch (Exception ex)
                {

                }
                var nextEvals = ImaginateMyTurn(_computed.fight, restTeam1, restTeam2);
                //Console.WriteLine("Expecting ennemy : " + nextEvals.Item2 + ", " + nextEvals.Item3);
                //Console.WriteLine("Expecting Win : " + nextEvals.Item4);
                expectedAdv = (nextEvals.Item2, nextEvals.Item3);
                var toSave = new FirstChoice()
                {
                    ExpectedAdv = expectedAdv,
                    MyChoice = nextEvals.Item1
                };
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                using (StreamWriter sw = new StreamWriter(LabelFile))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, toSave);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }
                return nextEvals.Item1;
            }
            else
            {
                var nextEvals = ImaginateMyTurn(_computed.fight, restTeam1, restTeam2);
                //Console.WriteLine("Expecting ennemy : " + nextEvals.Item2 + ", " + nextEvals.Item3);
                //Console.WriteLine("Expecting Win : " + nextEvals.Item4);
                expectedAdv = (nextEvals.Item2, nextEvals.Item3);
                return nextEvals.Item1;
            }
        }

        public (int, int) possibilityResponse(int myChoice, int ennemy)
        {
            int vB = 0;
            int kB = 0;
                var nextEvals = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNotEnnemy(_computed.fight, myChoice, ennemy));
                int v1 = 2000;
                int k1 = 0;
                int v2 = 2000;
            int kBPrim = 0;
            int k2 = 0;
                // On prend les 2 plus mauvais résultat (= choix ennemi)
                foreach (var f in nextEvals)
                {
                    if (TakeBest(false, v1, f.Item2))
                    {
                        if (TakeBest(false, v2, v1))
                        {
                            v2 = v1;
                            k2 = k1;
                        }
                        v1 = f.Item2;
                        k1 = f.Item1.GetOpponent(_computed.fight, myChoice, null);
                    }
                    else if (TakeBest(false, v2, f.Item2))
                    {
                        v2 = f.Item2;
                        k2 = f.Item1.GetOpponent(_computed.fight, myChoice, null);
                    }
                }
                // On prend le meilleur parmi eux.
                if (TakeBest(true, vB, v2))
                {
                    vB = v2;
                    kB = k2;
                    kBPrim = k1;
            }
            return (kB, kBPrim);
        }

        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            ;
            restTeam1.Remove(myChoice);
            restTeam2.Remove(ennemy);
            int v1 = 0;
            int k1 = 0;
            int v2 = 0;
            int k2 = 0;
            if (ennemy == expectedAdv.Item1 || ennemy == expectedAdv.Item2)
            {
                expectedAdv = possibilityResponse(myChoice, ennemy);
            }
            if (restTeam1.Count() == 3)
            {
                foreach (var op in options)
                {

                    // Ennemi joue 1er combo
                    var c1 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[1], restTeam1[0], restTeam1[1]);
                    var c2 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[0], restTeam1[1], restTeam1[0]);

                    var c3 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[1], restTeam1[0], restTeam1[2]);
                    var c4 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[0], restTeam1[2], restTeam1[0]);

                    var c5 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[1], restTeam1[2], restTeam1[1]);
                    var c6 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[0], restTeam1[1], restTeam1[2]);

                    // Ennemi joue 2nd combo
                    var c10 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[2], restTeam1[0], restTeam1[1]);
                    var c11 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[0], restTeam1[1], restTeam1[0]);

                    var c12 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[2], restTeam1[0], restTeam1[2]);
                    var c13 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[0], restTeam1[2], restTeam1[0]);

                    var c14 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[0], restTeam2[2], restTeam1[1], restTeam1[2]);
                    var c15 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[0], restTeam1[2], restTeam1[1]);

                    // Ennemi joue 3eme combo
                    var c20 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[2], restTeam1[0], restTeam1[1]);
                    var c21 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[1], restTeam1[1], restTeam1[0]);

                    var c22 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[2], restTeam1[0], restTeam1[2]);
                    var c23 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[1], restTeam1[2], restTeam1[0]);

                    var c24 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[1], restTeam2[2], restTeam1[1], restTeam1[2]);
                    var c25 = SimulateCombi(restTeam1, restTeam2, myChoice, ennemy, restTeam2[2], restTeam2[1], restTeam1[2], restTeam1[1]);

                    int m1 = new List<int>() { c1, c2, c3, c4, c5, c6 }.Min();
                    int m2 = new List<int>() { c10, c11, c12, c13, c14, c15}.Min();
                    int m3 = new List<int>() { c20, c21, c22, c23, c24, c25 }.Min();
                    int min = Math.Min(Math.Min(m1, m2), m3);
                    Console.WriteLine("Test : "+m1 + " - " + m2 + " - " + m3 + " => " + min);
                    if (min == m1) // Expecting ennemi 0 = 1
                    {
                        var m11 = Math.Min(c1, c2);
                        var m12 = Math.Min(c3, c4);
                        var m13 = Math.Min(c5, c6);
                        var m14 = Math.Max(Math.Max(m11, m12), m13);
                        return m14 == m11 ? (restTeam1[0], restTeam1[1]) : (m14 == 12 ? (restTeam1[0], restTeam1[2]) : (restTeam1[1], restTeam1[2]));
                    }
                    if (min == m2) // Expecting ennemi 0 - 2
                    {
                        var m21 = Math.Min(c10, c11);
                        var m22 = Math.Min(c12, c13);
                        var m23 = Math.Min(c14, c15);
                        var m24 = Math.Max(Math.Max(m21, m22), m23);
                        return m24 == m21 ? (restTeam1[0], restTeam1[1]) : (m22 == 12 ? (restTeam1[0], restTeam1[2]) : (restTeam1[1], restTeam1[2]));
                    }
                    var m31 = Math.Min(c20, c21);
                    var m32 = Math.Min(c22, c23);
                    var m33 = Math.Min(c24, c25);
                    var m34 = Math.Max(Math.Max(m31, m32), m33);
                    return m34 == m31 ? (restTeam1[0], restTeam1[1]) : (m32 == 12 ? (restTeam1[0], restTeam1[2]) : (restTeam1[1], restTeam1[2]));
                }
            }
            else
            {
                foreach (var op in options)
                {
                    var state = new List<(int, int)>(_computed.fight);
                    state.Add((myChoice, expectedAdv.Item1));
                    state.Add((op.Key, ennemy));
                    var nextEvals = _tree.Evaluations.Where(t => t.Item1.IsStartingBy(state)).Max(t => t.Item2);
                    if (TakeBest(true, v1, nextEvals))
                    {
                        if (TakeBest(true, v2, v1))
                        {
                            v2 = v1;
                            k2 = k1;
                        }
                        v1 = nextEvals;
                        k1 = op.Key;
                    }
                    else if (TakeBest(true, v2, nextEvals))
                    {
                        v2 = nextEvals;
                        k2 = op.Key;
                    }
                }
            }
            return (k1, k2);
        }

        // Ma team, Team ennemi, affrontement pris Ennemi, restant Ennemi, affrontement pris par l'autre, restant moi
        public int SimulateCombi(List<int> restTeam1, List<int> restTeam2, int you, int ennemy, int Team2P1, int Team2P2, int team1P1, int Team1P2)
        {
            var sTeam1 = new List<int>(restTeam1);
            var sTeam2 = new List<int>(restTeam2);
            sTeam1.Remove(team1P1);
            sTeam1.Remove(Team1P2);
            sTeam2.Remove(Team2P1);
            sTeam2.Remove(Team2P2);

            var state = new List<(int, int)>(_computed.fight);
            state.Add((you, Team2P1));
            state.Add((team1P1, ennemy));

            state.Add((sTeam1.First(), Team2P2));
            state.Add((Team1P2, sTeam2.First()));

            return _tree.Evaluations.Where(t => t.Item1.IsStartingBy(state)).Min(t => t.Item2);
        }

        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            restTeam1.Remove(you);
            restTeam2.Remove(ennemy);
            if (restTeam1.Count() == 3)
            {
                var c1 = SimulateCombi(restTeam1, restTeam2, you, ennemy, possibility.Item1, possibility.Item2, ennemyPossibility.Item1, ennemyPossibility.Item2);
                var c2 = SimulateCombi(restTeam1, restTeam2, you, ennemy, possibility.Item1, possibility.Item2, ennemyPossibility.Item2, ennemyPossibility.Item1);

                var c3 = SimulateCombi(restTeam1, restTeam2, you, ennemy, possibility.Item2, possibility.Item1, ennemyPossibility.Item1, ennemyPossibility.Item2);
                var c4 = SimulateCombi(restTeam1, restTeam2, you, ennemy, possibility.Item2, possibility.Item1, ennemyPossibility.Item2, ennemyPossibility.Item1);

                int min1 = Math.Min(c1, c2);
                int min2 = Math.Min(c3, c4);
                return min1 > min2 ? possibility.Item1 : possibility.Item2;
            }
            else
            {

                var state = new List<(int, int)>(_computed.fight);
                state.Add((you, possibility.Item1));
                var c1 = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(state, ennemyPossibility.Item1, ennemy)).Min(t => t.Item2);
                var c2 = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(state, ennemyPossibility.Item2, ennemy)).Min(t => t.Item2);
                int min1 = Math.Min(c1, c2);

                state = new List<(int, int)>(_computed.fight);
                state.Add((you, possibility.Item2));
                var c3 = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(state, ennemyPossibility.Item1, ennemy)).Min(t => t.Item2);
                var c4 = _tree.Evaluations.Where(t => t.Item1.IsStartingByWithNext(state, ennemyPossibility.Item2, ennemy)).Min(t => t.Item2);
                int min2 = Math.Min(c3, c4);
                return min1 > min2 ? possibility.Item1 : possibility.Item2;
            }
        }

        public void FightValidate(int my, int other)
        {
            var stand = new List<(int, int)>(_computed.fight);
            stand.Add((my, other));
            var f = _computed.PotentialsChilds.Where(t => t.IsSameFight(stand))?.FirstOrDefault();
            if (f != null)
            {
                _computed = f;
            }
        }
    }

}
