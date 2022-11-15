using IXAge_IHM.Shared.Pairing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IXAge_Pairing
{
    public class DecisionTree
    {
        public Dictionary<ulong, MyChoice> fight = new Dictionary<ulong, MyChoice>();
    }


    public class MyChoice
    {
        public int myChoice = -1;
        public Dictionary<int, (int, int)> toSend = new Dictionary<int, (int, int)>();
        public Dictionary<(int, int), int> toAccept = new Dictionary<(int, int), int>();
    }

    public class BuildTree
    {
        DecisionTree tree = new DecisionTree();

        List<List<(int, int)>> Data = new List<List<(int, int)>>();
        public Dictionary<ulong, int> DataString = new Dictionary<ulong, int>();
        public void RunTestPhase3(PairingScenario elems, (ulong, ulong) fight1, (ulong, ulong) fight2, (ulong, ulong) fight3, ulong j, ulong prevJ, ulong lastJ, ref int cpt)
        {
            var cFight = fight1.Item1 * 100000000000 + fight1.Item2 * 10000000000 + fight2.Item1 * 1000000000 + fight2.Item2 * 100000000
                + fight3.Item1 * 10000000 + fight3.Item2 * 1000000;
            for (ulong i2 = 0; i2 < 6; i2++)
            {
                if (i2 != fight3.Item1 && i2 != fight1.Item1 && i2 != fight2.Item1)
                {
                    for (ulong i3 = i2 + 1; i3 < 6; i3++)
                    {
                        if (i3 != fight3.Item1 && i3 != fight1.Item1 && i3 != fight2.Item1)
                        {
                            ulong lastI = 0;
                            while (lastI == fight3.Item1 || lastI == i2 || lastI == i3 || lastI == fight1.Item1 || lastI == fight2.Item1)
                                lastI++;

                            DataString[cFight + i2 * 100000 +
                                j * 10000 + lastI * 1000 + prevJ * 100 + i3 * 10 + lastJ] = elems.Team[(int)fight1.Item1].Evals[(int)fight1.Item2] +
                                elems.Team[(int)fight2.Item1].Evals[(int)fight2.Item2] +
                                elems.Team[(int)fight3.Item1].Evals[(int)fight3.Item2] + elems.Team[(int)i2].Evals[(int)j] +
                                elems.Team[(int)lastI].Evals[(int)prevJ] + elems.Team[(int)i3].Evals[(int)lastJ];


                            DataString[cFight + i3 * 100000 +
                                j * 10000 + lastI * 1000 + prevJ * 100 + i2 * 10 + lastJ] = elems.Team[(int)fight1.Item1].Evals[(int)fight1.Item2] +
                                elems.Team[(int)fight2.Item1].Evals[(int)fight2.Item2] +
                                elems.Team[(int)fight3.Item1].Evals[(int)fight3.Item2] + elems.Team[(int)i3].Evals[(int)j] +
                                elems.Team[(int)lastI].Evals[(int)prevJ] + elems.Team[(int)i2].Evals[(int)lastJ];

                            cpt += 2;

                        }
                    }
                }
            }
        }
        public void RunTestPhase2(PairingScenario elems, (ulong, ulong) fight1, (ulong, ulong) fight2, ref int cpt)
        {
            var cFight = fight1.Item1 * 100000000000 + fight1.Item2 * 10000000000 + fight2.Item1 * 1000000000 + fight2.Item2 * 100000000;
            for (ulong i = 0; i < 6; i++)
            {
                if (i != fight1.Item1 && i != fight2.Item1)
                {
                    for (ulong j = 0; j < 6; j++)
                    {
                        if (j != fight1.Item2 && j != fight2.Item2)
                        {
                            for (ulong j2 = 0; j2 < 6; j2++)
                            {
                                if (j2 != fight1.Item2 && j2 != fight2.Item2 && j2 != j)
                                {
                                    for (ulong j3 = j2 + 1; j3 < 6; j3++)
                                    {
                                        if (j3 != fight1.Item2 && j3 != fight2.Item2 && j3 != j)
                                        {
                                            ulong lastJ = 0;
                                            while (lastJ == j || lastJ == j2 || lastJ == j3 || lastJ == fight1.Item2 || lastJ == fight2.Item2)
                                                lastJ++;

                                            RunTestPhase3(elems, fight1, fight2, (i, j2), j, j3, lastJ, ref cpt);
                                            RunTestPhase3(elems, fight1, fight2, (i, j3), j, j2, lastJ, ref cpt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void RunTest(PairingScenario elems)
        {
            int cpt = 0;
            for (ulong i = 0; i < 6; i++)
            {
                for (ulong j = 0; j < 6; j++)
                {
                    for (ulong i2 = 0; i2 < 6; i2++)
                    {
                        if (i2 != i)
                        {
                            for (ulong i3 = i2 + 1; i3 < 6; i3++)
                            {
                                if (i3 != i && i3 != i2)
                                {
                                    for (ulong j2 = 0; j2 < 6; j2++)
                                    {
                                        if (j2 != j)
                                        {
                                            for (ulong j3 = j2 + 1; j3 < 6; j3++)
                                            {
                                                if (j3 != j && j3 != j2)
                                                {
                                                    RunTestPhase2(elems, (i, j2), (i2, j), ref cpt);
                                                    RunTestPhase2(elems, (i, j3), (i2, j), ref cpt);
                                                    RunTestPhase2(elems, (i, j2), (i3, j), ref cpt);
                                                    RunTestPhase2(elems, (i, j3), (i3, j), ref cpt);
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
            //Console.WriteLine("CPT = "+cpt);
            //Console.WriteLine("CPT = "+DataString.Count);
        }
    }
}