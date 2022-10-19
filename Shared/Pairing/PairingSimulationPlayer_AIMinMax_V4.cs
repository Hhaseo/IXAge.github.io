using System.Collections.Concurrent;
using System.Text.Json;
using IXAge_Pairing;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IXAge_IHM.Shared.Pairing
{

    public class PairingSimulationPlayer_AIMinMax_V4 : IPairingSimulationPlayer
    {
        public string Name { get => "AI_MinMax_V4"; }

        protected PairingScenario _simu;
        protected Fight _computed = new Fight();
        protected Tree_V2 _tree;
        static protected int i = 0;
        protected bool FirstP;
        public (int, int) expectedAdv = (0, 0);

        public class FirstChoice
        {
            public int MyChoice { get; set; }
            public (int, int) ExpectedAdv { get; set; }

        }

        Dictionary<ulong, int> Data;

        public PairingSimulationPlayer_AIMinMax_V4(bool firstP, PairingScenario simu)
        {
            FirstP = firstP;
            _simu = simu;

            BuildTree.RunTest(simu);
            Data = BuildTree.DataString;


            //_tree = new Tree_V2();
            //_tree.ComputeEvaluation(simu);
            //_computed = _tree.InitialFight;
            //File.WriteAllLines("./TreeEvaluations.txt", _tree.ToStringEvaluation());
            //File.WriteAllLines("./TreeData.txt", _tree.ToStringNodes());
            //string jsonString =  JsonSerializer.Serialize(_tree);
            //File.WriteAllText("./Tree.json", jsonString);
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

        // j'ai lancer i, il a mis j2, j3 en face
        // Il a envoyé j, j'ai mis i2, i3 en face
        public (int, int) MonChoix(List<(int, int)> fight)
        {
            int maxV = 0;
            int maxI = -1;
            for (int i = 0; i < 6; i++)
            {
                if (!fight.Any(t => t.Item1 == i))
                {
                    var standV = 2000;
                    for (int j = 0; j < 6; j++)
                    {
                        if (!fight.Any(t => t.Item2 == j))
                        {
                            var response = maReponse(fight, i, j);
                            standV = response.Item2 < standV ? response.Item2 : standV;
                        }
                    }
                    if (standV > maxV)
                    {
                        maxV = standV;
                        maxI = i;
                    }
                }
            }
            return (maxI, maxV);
        }

        // j'ai lancer i, il a mis j2, j3 en face
        // Il a envoyé j, j'ai mis i2, i3 en face
        public ((int, int), int) maReponse(List<(int, int)> fight, int i, int j)
        {
            int maxV = 0;
            (int, int) maxI = (0, 0);
            for (int i2 = 0; i2< 6; i2++)
            {
                if (!fight.Any(t => t.Item1 == i2) && i != i2)
                {
                    for (int i3 = 0; i3< 6; i3++)
                    {

                        if (!fight.Any(t => t.Item1 == i3) && i != i3 && i3 != i2)
                        {
                            var standV = 2000;

                            // max 
                            for (int j2 = 0; j2< 6; j2++)
                            {
                                if (!fight.Any(t => t.Item2 == j2) && j != j2)
                                {
                                    for (int j3 = 0; j3< 6; j3++)
                                    {
                                        if (!fight.Any(t => t.Item2 == j3) && j != j3 && j3 != j2)
                                        {
                                            var thisFight = TakeResponse(fight, i, j, i2, i3, j2, j3);
                                             // min
                                            if (thisFight.Item2 < standV)
                                            {
                                                standV = thisFight.Item2;
                                            }
                                        }
                                    }
                                }
                            }
                            if (standV > maxV)
                            {
                                maxI = (i2, i3);
                                maxV = standV;
                            }
                        }
                    }
                }
            }
            return (maxI, maxV);
        }

        // j'ai lancer i, il a mis j2, j3 en face
        // Il a envoyé j, j'ai mis i2, i3 en face
        public (int, int) TakeResponse(List<(int, int)> fight, int i, int j, int i2, int i3, int j2, int j3)
        {
            if (fight.Count == 2) // notre acceptation entrainera la fin de la partie.
            {
                int lastI = 0;
                while (lastI == fight[0].Item1 || lastI==i || lastI==i2|| lastI==fight[1].Item1 || lastI == i3)
                    lastI++;

                int lastJ = 0;
                while (lastJ == fight[0].Item2 || lastJ==j||lastJ==j2||lastJ==fight[1].Item2 || lastJ == j3)
                    lastJ++;

                ulong cFight = (ulong)fight[0].Item1 * 100000000000 +(ulong)fight[0].Item2 * 10000000000 +
                    (ulong)fight[1].Item1 * 1000000000 + (ulong)fight[1].Item2 * 100000000;
                // choix estimé ennemi


                // mon choix
                ulong cfight2 = cFight+ (ulong)i * 10000000 + (ulong)j2 * 1000000 + (ulong)lastI* 1000 + (ulong)j3 * 100;

                var cfight2_2 = BuildTree.DataString[cfight2 + (ulong)i2 * 100000 + (ulong)j * 10000 + (ulong)i3* 10 + (ulong)lastJ * 1];
                var cfight2_3 = BuildTree.DataString[cfight2 + (ulong)i3 * 100000 + (ulong)j * 10000 + (ulong)i2* 10 + (ulong)lastJ * 1];
                //choix ennemi
                var min1 = cfight2_2 > cfight2_3 ? cfight2_3 : cfight2_2;

                ulong cfight3 = cFight+ (ulong)i * 10000000 + (ulong)j3 * 1000000 + (ulong)lastI* 1000 + (ulong)j2 * 100;
                var cfight3_2 = BuildTree.DataString[cfight3 + (ulong)i2 * 100000 + (ulong)j * 10000 + (ulong)i3* 10 + (ulong)lastJ * 1];
                var cfight3_3 = BuildTree.DataString[cfight3 + (ulong)i3 * 100000 + (ulong)j * 10000 + (ulong)i2* 10 + (ulong)lastJ * 1];
                //choix ennemi
                var min2 = cfight3_2 > cfight3_3 ? cfight3_3 : cfight3_2;

                // on s'attend à ce que j'ennemi choississe celui qui minimise les _2 et _3 et moi je veux celui qui me maximise dans ce cas la.
                // mon choix
                return (min1 > min2 ? j2 : j3, min1 > min2 ? min1 : min2);
            }
            else
            {
                ulong cFight = 0;
                // choix estimé ennemi

                // Evaluation nouveau tour ur chaque.
                // min1 = min f1/f2
                // min2 = min f3/f4
                // Result max des 2 min.

                var f1 = MonChoix(new List<(int, int)>(fight) { (i, j2), (i2, j) });
                var f2 = MonChoix(new List<(int, int)>(fight) { (i, j2), (i3, j) });
                //choix ennemi
                var min1 = f1.Item2 > f2.Item2 ? f2.Item2 : f1.Item2;

                var f3 = MonChoix(new List<(int, int)>(fight) { (i, j3), (i2, j) });
                var f4 = MonChoix(new List<(int, int)>(fight) { (i, j3), (i3, j) });
                //choix ennemi
                var min2 = f3.Item2 > f4.Item2 ? f4.Item2 : f3.Item2;

                // mon choix
                return (min1 > min2 ? j2 : j3, min1 > min2 ? min1 : min2);

            }
        }

        public string LabelFile { get => Name + "_" + _simu.Label + "_" + (FirstP ? "FirstPlayer" : "SecondPlayer")+".json"; }

        public class DecisionTree
        {
            public Dictionary<ulong, int> Decision1 = new Dictionary<ulong, int>();
            public Dictionary<ulong, int> Decision2 = new Dictionary<ulong, int>();
            public Dictionary<ulong, int> Decision3 = new Dictionary<ulong, int>();
        }

        DecisionTree tree = null;

        public void CreateDecisionTree()
        {
            if (File.Exists(LabelFile))
            {
                string text = File.ReadAllText(LabelFile);
                tree = JsonConvert.DeserializeObject<DecisionTree>(text);
            }
            else
            {

                tree = new DecisionTree();
                List<(int, int)> fights = new List<(int, int)>();
                List<int> rest1 = new List<int>();
                List<int> rest2 = new List<int>();
                _computed.fight = fights;
                var  c = ChooseOne_flat(null, null, null);
                tree.Decision1[CountFlight()] = c;
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6;j++)
                    {
                        _computed.fight = fights;
                        var response = ChooseResponse_flat(i, "", j, "", null, null, null);
                        tree.Decision2[CountFlight(i, j)] = response.Item1 * 10 + response.Item2;

                        for (int i5 = 0; i5 < 6; i5++)
                        {
                            for (int i6 = 0; i6 < 6; i6++)
                            {
                                if (i5 != i && i6 != i && i5 != i6)
                                {
                                    for (int j5 = 0; j5 < 6; j5++)
                                    {
                                        for (int j6 = 0; j6 < 6; j6++)
                                        {
                                            if (j5 != j && j6 != j && j5 != j6)
                                            {

                                                var response2 = AcceptResponse_flat(j, null, (i5, i6), ("",""), i, null, (j5, j6), ("", ""), null, null);
                                                tree.Decision3[CountFlight(i, j, (i5, i6), (j5, j6))] = response2;


                                            }
                                        }
                                    }
                                }
                            }
                        }

                        for (int i2 = 0; i2 < 6; i2++)
                        {
                            if (i2 != i)
                            {
                                for (int j2 = 0; j2 < 6; j2++)
                                {
                                    if (j2 != j)
                                    {
                                        var sF = new List<(int, int)>();
                                        sF.Add((i, j));
                                        sF.Add((i2, j2));
                                        _computed.fight = sF;
                                        c = ChooseOne_flat(null, null, null);
                                        tree.Decision1[CountFlight()] = c;
                                        for (int i3 = 0; i3 < 6; i3++)
                                        {
                                            if (i3 != i && i3 != i2)
                                            {
                                                for (int j3 = 0; j3 < 6; j3++)
                                                {
                                                    if (j3 != j && j3 != j2)
                                                    {
                                                        response = ChooseResponse_flat(i3, "", j3, "", null, null, null);
                                                        tree.Decision2[CountFlight(i3, j3)] = response.Item1 * 10 + response.Item2;

                                                        for (int i5 = 0; i5 < 6; i5++)
                                                        {
                                                            for (int i6 = 0; i6 < 6; i6++)
                                                            {
                                                                if (i5 != i && i6 != i && i5 != i3 && i6 != i3 && i5 != i2 && i6 != i2 && i5 != i6)
                                                                {
                                                                    for (int j5 = 0; j5 < 6; j5++)
                                                                    {
                                                                        for (int j6 = 0; j6 < 6; j6++)
                                                                        {
                                                                            if (j5 != j && j6 != j && j5 != j3 && j6 != j3 && j5 != j2 && j6 != j2 && j5 != j6)
                                                                            {

                                                                                var response2 = AcceptResponse_flat(j3, null, (i5, i6), ("", ""), i3, null, (j5, j6), ("", ""), null, null);
                                                                                tree.Decision3[CountFlight(i3, j3, (i5, i6), (j5, j6))] = response2;


                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                _computed.fight = fights;
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                using (StreamWriter sw = new StreamWriter(LabelFile))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, tree);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }
            }
        }

        public ulong CountFlight()
        {
            ulong f = 0;
            int i = 5;
            foreach (var data in _computed.fight)
            {
                f += ((ulong)Math.Pow(10, i*2+1) * (ulong)data.Item1 + (ulong)Math.Pow(10, i*2) * (ulong)data.Item2);
                i--;
            }
            return f;
        }
        public ulong CountFlight(int myChoice, int ennemy)
        {
            ulong f = 0;
            int i = 5;
            foreach (var data in _computed.fight)
            {
                f += ((ulong)Math.Pow(10, i*2+1) * (ulong)data.Item1 + (ulong)Math.Pow(10, i*2) * (ulong)data.Item2);
                i--;
            }
            f += ((ulong)Math.Pow(10, i*2+1) * (ulong)myChoice);
            f += ((ulong)Math.Pow(10, (i-1)*2) * (ulong)ennemy);

            return f;
        }
        public ulong CountFlight(int myChoice, int ennemy, (int, int) ennemyPossibility, (int, int) possibility)
        {
            ulong f = 0;
            int i = 5;
            foreach (var data in _computed.fight)
            {
                f += ((ulong)Math.Pow(10, i*2+1) * (ulong)data.Item1 + (ulong)Math.Pow(10, i*2) * (ulong)data.Item2);
                i--;
            }
            f += ((ulong)Math.Pow(10, i*2+1) * (ulong)myChoice);

            f += ((ulong)Math.Pow(10, i*2) * (ulong)ennemyPossibility.Item1);
            f += ((ulong)Math.Pow(10, (i-1)*2+1) * (ulong)possibility.Item1);

            f += ((ulong)Math.Pow(10, (i-1)*2) * (ulong)ennemy);
            f += ((ulong)Math.Pow(10, (i-2)*2) * (ulong)ennemyPossibility.Item2);
            f += ((ulong)Math.Pow(10, (i-2)*2+1) * (ulong)possibility.Item2);
            return f;
        }

        public int ChooseOne_flat(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var s = MonChoix(_computed.fight);
            return s.Item1;
        }


        public (int, int) ChooseResponse_flat(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var s = maReponse(_computed.fight, myChoice, ennemy);
            return s.Item1;
        }

        public int AcceptResponse_flat(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            var s = TakeResponse(_computed.fight, you, ennemy, ennemyPossibility.Item1, ennemyPossibility.Item2, possibility.Item1, possibility.Item2);
            return s.Item1;
        }
        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            if (tree == null)
            {
                CreateDecisionTree();
            }
            return tree.Decision1[CountFlight()];
        }


        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            if (tree == null)
            {
                CreateDecisionTree();
            }
            var stand = tree.Decision2[CountFlight(myChoice, ennemy)];
            return (((int)(stand/10)), (stand %10));
        }

        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            if (tree == null)
            {
                CreateDecisionTree();
            }
            return tree.Decision3[CountFlight(you, ennemy,  ennemyPossibility, possibility)];
        }

        public void FightValidate(int my, int other)
        {
            _computed.fight.Add((my, other));
            var f = _computed.PotentialsChilds.Where(t => t.IsSameFight(_computed.fight))?.FirstOrDefault();
            if (f != null)
            {
                _computed = f;
            }
        }
    }

}
