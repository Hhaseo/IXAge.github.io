using System.Collections.Concurrent;
using System.Text.Json;

namespace IXAge_IHM.Shared.Pairing
{

    public class PairingSimulationPlayer_AIMinMax : IPairingSimulationPlayer
    {
        public string Name { get => "AI Min Max"; }

        protected PairingScenario _simu;
        protected Fight _computed;
        protected Tree _tree;
        static protected int i = 0;

        public PairingSimulationPlayer_AIMinMax(PairingScenario simu)
        {
            _simu= simu;
            _tree = new Tree();
            _tree.ComputeEvaluation(simu);
            _computed = _tree.InitialFight;
            File.WriteAllLines("./TreeEvaluations.txt", _tree.ToStringEvaluation());
            File.WriteAllLines("./TreeData.txt", _tree.ToStringNodes());
            string jsonString =  JsonSerializer.Serialize(_tree);
            File.WriteAllText("./Tree.json", jsonString);
            var stand = SimuStep1_Ally(_computed);
            jsonString = JsonSerializer.Serialize(_tree);
            File.WriteAllText("./TreePost.json", jsonString);
        }

        (int, int) SimuStep2_Ennemy(Fight current, int? key)
        {
            int v1 = 1000;
            int k1 = 0;
            int v2 = 1000;
            int k2 = 0;
            //ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            foreach (var child in current.PotentialsChilds.Where(t => (!key.HasValue || t.fight.Last().Item1 == key)))
            {
                var vt1 = SimuStep1_Ally(child);
                if (TakeBest(false, v1, vt1.Item1))
                {
                    if (TakeBest(false, v2, v1))
                    {
                        v2 = vt1.Item1;
                        k2 = k1;
                    }
                    v1 = vt1.Item1;
                    k1 = vt1.Item2;
                }
                else if (TakeBest(false, v2, vt1.Item1))
                {
                    v2 = vt1.Item1;
                    k2 = vt1.Item2;
                }
            }
            //Parallel.ForEach(current.PotentialsChilds.Where(t => (!key.HasValue || t.fight.Last().Item1 == key)), elem =>
            //{
            //    data.Enqueue(SimuStep1_Ally(elem));
            //});
            //foreach (var vt1 in data)
            //    //foreach (var elem in current.Childs.Where(t => (!key.HasValue || t.Key.Item1 == key)))
            //{

            //    //var stand = SimuStep1_Ally(elem.Value);
            //    //var vt1 = stand.Item1;
            //    if (TakeBest(false, v1, vt1.Item1))
            //    {
            //        if (TakeBest(false, v2, v1))
            //        {
            //            v2 = vt1.Item1;
            //            k2 = k1;
            //        }
            //        v1 = vt1.Item1;
            //        k1 = vt1.Item2;
            //    }
            //    else if (TakeBest(false, v2, vt1.Item1))
            //    {
            //        v2 = vt1.Item1;
            //        k2 = vt1.Item2;
            //    }
            //}
            return (v2, k2);
        }

        (int, int) SimmuStep3_Ennemy(Fight current,  int ennemy, (int, int) possibilitéEnnemy)
        {
            int v1 = 1000;
            int k1 = 0;
            foreach (var elem in current.PotentialsChilds.Where(t => t.fight.Last().Item2 == ennemy &&
                             (t.fight.Last().Item1 == possibilitéEnnemy.Item1 || t.fight.Last().Item1 == possibilitéEnnemy.Item2)))
            {
                var vt1 = SimuStep1_Ally(elem, elem.fight.Last().Item1 == possibilitéEnnemy.Item1 ? possibilitéEnnemy.Item2 : possibilitéEnnemy.Item1);
                if (TakeBest(false, v1, vt1.Item1))
                {
                    v1 = vt1.Item1;
                    // On Cherche la réponse ami
                    k1 = elem.fight.Last().Item1;
                }
            }
            current.Evaluated = (v1, k1);
            File.WriteAllLines("./TreeData" + i.ToString() + ".txt", _tree.ToStringNodes());
            return ((v1, k1));
        }
        (int, int) SimmuStep3_Ally(Fight current, int key, int ennemy, (int, int) possibilityMoi, (int, int) possibilitéEnnemy)
        {
            int v1 = 0;
            int k1 = 0;
            foreach (var elem in current.PotentialsChilds.Where(t => t.fight.Last().Item1 == key && 
                             (t.fight.Last().Item2 == possibilityMoi.Item1 || t.fight.Last().Item2 == possibilityMoi.Item2)))
            {
                var vt1 = SimmuStep3_Ennemy(elem, ennemy, possibilitéEnnemy);
                if (TakeBest(true, v1, vt1.Item1))
                {
                    v1 = vt1.Item1;
                    // On cherche la réponse ennemi
                    k1 = elem.fight.Last().Item2;
                }
            }
            current.Evaluated = (v1, k1);
            File.WriteAllLines("./TreeData"+i.ToString()+".txt", _tree.ToStringNodes());
            return ((v1, k1));
        }

        ((int, int), (int, int)) SimuStep2_Ally(Fight current, int? key, int? ennemy)
        {
            int v1 = 0;
            int k1 = 0;
            int v2 = 0;
            int k2 = 0;
            //ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            foreach (var child in current.PotentialsChilds.Where(t => (!ennemy.HasValue || t.fight.Last().Item2 == ennemy)))
            {
                var vt1 = SimuStep2_Ennemy(child, key);
                    if (TakeBest(true, v1, vt1.Item1))
                {
                    if (TakeBest(true, v2, v1))
                    {
                        v2 = v1;
                        k2 = k1;
                    }
                    v1 = vt1.Item1;
                    k1 = vt1.Item2;
                }
                else if (TakeBest(true, v2, vt1.Item1))
                {
                    v2 = vt1.Item1;
                    k2 = vt1.Item2;
                }
            }
            //Parallel.ForEach(current.PotentialsChilds.Where(t => (!ennemy.HasValue || t.fight.Last().Item2 == ennemy)), elem =>
            //{
            //    data.Enqueue(SimuStep2_Ennemy(elem, key));
            //});
            //foreac
            //foreach (var vt1 in data)
            //{ 
            //    //foreach (var elem2 in current.Childs.Where(t => (!ennemy.HasValue || t.Key.Item2 == ennemy)))
            //    //{

            //    //    var vt1 = SimuStep2_Ennemy(elem2.Value, key);
            //        if (TakeBest(true, v1, vt1.Item1))
            //        {
            //            if (TakeBest(true, v2, v1))
            //            {
            //                v2 = v1;
            //                k2 = k1;
            //            }
            //            v1 = vt1.Item1;
            //            k1 = vt1.Item2;
            //        }
            //        else if (TakeBest(true, v2, vt1.Item1))
            //        {
            //            v2 = vt1.Item1;
            //            k2 = vt1.Item2;
            //        }
            //    }
            return ((v2, k2), (v1, k1));
        }

        (int, int) SimuStep1_Ennemy(Fight current, int key)
        {
            int v2 = 1000;
            int k2 = 0;
            //ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            foreach (var child in current.PotentialsChilds)
            {
                var stand = SimuStep2_Ally(current, key, child.fight.Last().Item2);
                var elem = (stand.Item1.Item1, child.fight.Last().Item2);
                if (TakeBest(false, v2, elem.Item1))
                {
                    v2 = elem.Item1;
                    k2 = elem.Item2;
                }
            }

            //    Parallel.ForEach(current.PotentialsChilds, elem =>
            //{
            //    var stand = SimuStep2_Ally(current, key, elem.fight.Last().Item2);
            //    data.Enqueue((stand.Item1.Item1, elem.fight.Last().Item2));
            //});
            //foreach (var elem in current.Childs)
            //{
            //    var vt2 = SimuStep2_Ally(current, key, elem.Key.Item2);
            //    if (TakeBest(false, v2, vt2.Item1.Item1))
            //    {
            //        v2 = vt2.Item1.Item1;
            //        k2 = elem.Key.Item2;
            //    }
            //}
            //foreach (var elem in data)
            //{
            //    if (TakeBest(false, v2, elem.Item1))
            //    {
            //        v2 = elem.Item1;
            //        k2 = elem.Item2;
            //    }
            //}
            return ((v2, k2));
        }
        (int, int) SimuStep1_Ally(Fight current, int? rejected = null)
        {
            Console.WriteLine("Fight : "+String.Join(", ", current.fight.Select(t => t.Item1+ " vs "+t.Item2)));

            if (current.Evaluated.HasValue)
              return current.Evaluated.Value;
            int v1 = 0;
            int k1 = 0;
            //ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();
            
            foreach (var child in current.PotentialsChilds)
            {
                var elem = SimuStep1_Ennemy(current, child.fight.Last().Item1);
                if (TakeBest(true, v1, elem.Item1))
                {
                    v1 = elem.Item1;
                    k1 = elem.Item2;
                }
            }
            //Parallel.ForEach(current.PotentialsChilds, elem =>
            //{
            //    data.Enqueue(SimuStep1_Ennemy(current, elem.fight.Last().Item1));
            //});
            //foreach (var elem in current.Childs)
            //{
            //    var vt1 = SimuStep1_Ennemy(current, elem.Key.Item1);
            //    if (TakeBest(true, v1, vt1.Item1))
            //    {
            //        v1 = vt1.Item1;
            //        k1 = elem.Key.Item1;
            //    }
            //}
            //foreach (var elem in data)
            //{
            //    if (TakeBest(true, v1, elem.Item1))
            //    {
            //        v1 = elem.Item1;
            //        k1 = elem.Item2;
            //    }
            //}
            return ((v1, k1));
        }
        bool TakeBest(bool best, int v1, int v2)
        {
            if (best)
                return v2 > v1;
            return v2 < v1;
        }

        public int ChooseOne(Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var stand = SimuStep1_Ally(_computed);
            return stand.Item2;
        }

        public (int, int) ChooseResponse(int myChoice, string myChoiceLabel, int ennemy, string ennemyLabel, Dictionary<int, string> options, List<int> restTeam1, List<int> restTeam2)
        {
            var stand = SimuStep2_Ally(_computed, myChoice, ennemy);
            return (stand.Item2.Item2, stand.Item1.Item2);
        }

        public int AcceptResponse(int ennemy, string ennemyLabel, (int, int) ennemyPossibility, (string, string) ennemyPossibilityLabel,
            int you, string youLabel, (int, int) possibility, (string, string) possibilityLabel, List<int> restTeam1, List<int> restTeam2)
        {
            var stand = SimmuStep3_Ally(_computed, you, ennemy, possibility, ennemyPossibility);
            return stand.Item2;
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
