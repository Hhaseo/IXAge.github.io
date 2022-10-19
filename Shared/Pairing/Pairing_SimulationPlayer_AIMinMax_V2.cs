using System.Collections.Concurrent;

namespace IXAge_IHM.Shared.Pairing
{

    public class PairingSimulationPlayer_AIMinMax_v2 : IPairingSimulationPlayer
    {
        public string Name { get => "AI Min Max"; }

        protected PairingScenario _simu;
        protected TreeNode _computed;
        public PairingSimulationPlayer_AIMinMax_v2(PairingScenario simu)
        {
            _simu= simu;
            _computed= TreeNode.Build(_simu);
        }


        (int, int) SimuStep2_Ennemy(TreeNode current, int? key)
        {
            int v1 = 1000;
            int k1 = 0;
            int v2 = 1000;
            int k2 = 0;
            ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            Parallel.ForEach(current.Childs.Where(t => (!key.HasValue || t.Key.Item1 == key)), elem =>
            {
                data.Enqueue(SimuStep1_Ally(elem.Value));
            });
            foreach (var vt1 in data)
            //foreach (var elem in current.Childs.Where(t => (!key.HasValue || t.Key.Item1 == key)))
            {

                //var stand = SimuStep1_Ally(elem.Value);
                //var vt1 = stand.Item1;
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
            return (v2, k2);
        }

        (int, int) SimmuStep3_Ennemy(TreeNode current, int ennemy, (int, int) possibilitéEnnemy)
        {
            int v1 = 1000;
            int k1 = 0;
            foreach (var elem in current.Childs.Where(t => t.Key.Item2 == ennemy &&
                             (t.Key.Item1 == possibilitéEnnemy.Item1 || t.Key.Item1 == possibilitéEnnemy.Item2)))
            {
                var vt1 = SimuStep1_Ally(elem.Value, elem.Key.Item1 == possibilitéEnnemy.Item1 ? possibilitéEnnemy.Item2 : possibilitéEnnemy.Item1);
                if (TakeBest(false, v1, vt1.Item1))
                {
                    v1 = vt1.Item1;
                    k1 = elem.Key.Item1;
                }
            }
            return ((v1, k1));
        }
        (int, int) SimmuStep3_Ally(TreeNode current, int key, int ennemy, (int, int) possibilityMoi, (int, int) possibilitéEnnemy)
        {
            int v1 = 0;
            int k1 = 0;
            foreach (var elem in current.Childs.Where(t => t.Key.Item1 == key &&
                             (t.Key.Item2 == possibilityMoi.Item1 || t.Key.Item2 == possibilityMoi.Item2)))
            {
                var vt1 = SimmuStep3_Ennemy(elem.Value, ennemy, possibilitéEnnemy);
                if (TakeBest(true, v1, vt1.Item1))
                {
                    v1 = vt1.Item1;
                    k1 = elem.Key.Item1;
                }
            }
            return ((v1, k1));
        }

        ((int, int), (int, int)) SimuStep2_Ally(TreeNode current, int? key, int? ennemy)
        {
            int v1 = 0;
            int k1 = 0;
            int v2 = 0;
            int k2 = 0;
            ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            Parallel.ForEach(current.Childs.Where(t => (!ennemy.HasValue || t.Key.Item2 == ennemy)), elem =>
            {
                data.Enqueue(SimuStep2_Ennemy(elem.Value, key));
            });
            //foreac
            foreach (var vt1 in data)
            {
                //foreach (var elem2 in current.Childs.Where(t => (!ennemy.HasValue || t.Key.Item2 == ennemy)))
                //{

                //    var vt1 = SimuStep2_Ennemy(elem2.Value, key);
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
            return ((v2, k2), (v1, k1));
        }

        (int, int) SimuStep1_Ennemy(TreeNode current, int key)
        {
            int v2 = 1000;
            int k2 = 0;
            ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            Parallel.ForEach(current.Childs, elem =>
            {
                var stand = SimuStep2_Ally(current, key, elem.Key.Item2);
                data.Enqueue((stand.Item1.Item1, elem.Key.Item2));
            });
            //foreach (var elem in current.Childs)
            //{
            //    var vt2 = SimuStep2_Ally(current, key, elem.Key.Item2);
            //    if (TakeBest(false, v2, vt2.Item1.Item1))
            //    {
            //        v2 = vt2.Item1.Item1;
            //        k2 = elem.Key.Item2;
            //    }
            //}
            foreach (var elem in data)
            {
                if (TakeBest(false, v2, elem.Item1))
                {
                    v2 = elem.Item1;
                    k2 = elem.Item2;
                }
            }
            return ((v2, k2));
        }
        (int, int) SimuStep1_Ally(TreeNode current, int? rejected = null)
        {
            if (current.Childs == null || !current.Childs.Any())
                return (current.Value, 0);
            int v1 = 0;
            int k1 = 0;
            ConcurrentQueue<(int, int)> data = new ConcurrentQueue<(int, int)>();

            Parallel.ForEach(current.Childs, elem =>
            {
                data.Enqueue(SimuStep1_Ennemy(current, elem.Key.Item1));
            });
            //foreach (var elem in current.Childs)
            //{
            //    var vt1 = SimuStep1_Ennemy(current, elem.Key.Item1);
            //    if (TakeBest(true, v1, vt1.Item1))
            //    {
            //        v1 = vt1.Item1;
            //        k1 = elem.Key.Item1;
            //    }
            //}
            foreach (var elem in data)
            {
                if (TakeBest(true, v1, elem.Item1))
                {
                    v1 = elem.Item1;
                    k1 = elem.Item2;
                }
            }
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
            if (_computed.Childs.ContainsKey((my, other)))
                _computed = _computed.Childs[(my, other)];
        }
    }

}
