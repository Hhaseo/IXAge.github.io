using IXAge_IHM.Shared.Pairing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IXAge_IHM.Shared.Pairing.PairingScenario;

namespace IXAge_Pairing_Parsing
{
    public class ParsWorsheet
    {

        public static void Pars(LinqToExcel.ExcelQueryFactory excelFile, string SheetName, string FolderName)
        {
            try {
                var result = from c in excelFile.WorksheetNoHeader(SheetName)
                             select c;
                var pairingSimu = new PairingScenario() { Label = SheetName };
                Console.WriteLine("Team =  " + SheetName);
                int id = 0;
                int nbPlayer = 0;
                int idShift = 0;
                pairingSimu.Team = new List<Team>();
                Boolean bFirstLines = true;
                foreach (var line in result)
                {
                    if (bFirstLines && !line.Any(t => (t.Value).ToString().Trim() != ""))
                        idShift++;
                    else
                        bFirstLines = false;

                    if (id > (39 + idShift) && id < (41 + idShift))
                    {
                        int columnId = 0;
                        foreach (var column in line)
                        {
                            if (id == (40 + idShift))
                            {
                                if (column != "")
                                {
                                    string data = column;
                                    int idData = data.LastIndexOf(' ');
                                    pairingSimu.Opponents.Add(new EnnemyData() { Item1 = idData < 1 ? data : data.Substring(0, idData), Item2 = idData < 1 ? "" : data.Substring(idData+1, data.Length - idData-1) });
                                    Console.WriteLine("Ennemy Player = " + column);
                                }
                                else if (nbPlayer == 0 && columnId > 2)
                                {
                                    nbPlayer = columnId - 3;
                                    Console.WriteLine("Nb Player = " + nbPlayer);
                                }
                            }
                            else if (columnId <= (nbPlayer + 2))
                            {
                                if (column != "")
                                {
                                    pairingSimu.Opponents[columnId - 2] = new EnnemyData() { Item1 = pairingSimu.Opponents[columnId - 2].Item1, Item2 = column };
                                    Console.WriteLine("Ennemy Army = " + column);
                                }
                            }
                            columnId++;
                        }
                    }
                    // Mon equipe
                    else if (id >= (41 + idShift) && id < (41 + idShift + nbPlayer))
                    {
                        var team = new Team();
                        int columnId = 0;
                        foreach (var column in line)
                        {
                            if (columnId == 2)
                            {
                                if (column != "")
                                {
                                    string data = column;
                                    int idData = data.LastIndexOf(' ');
                                    team.Name = data.Substring(0, idData);
                                    Console.WriteLine("Player = " + team.Name +"$");
                                    team.Army = data.Substring(idData+1, data.Length - idData-1);
                                    Console.WriteLine("Army = " + team.Army + "$");
                                }
                            }
                            //else if (columnId == 3)
                            //{
                            //    if (column != "")
                            //    {
                            //        team.Army = column;
                            //        Console.WriteLine("Army = " + column);
                            //    }
                            //}
                            else if (columnId < (nbPlayer + 3))
                            {
                                if (column != "" && int.TryParse(column, out var eval))
                                {
                                    team.Evals.Add(eval);
                                    Console.WriteLine("Estimation  = " + column);
                                }
                                else
                                {
                                    team.Evals.Add(10);
                                    Console.WriteLine("Evaluation Manquante = " + column);
                                }
                            }
                            else if (columnId == (nbPlayer + 3))
                            {
                                if (column != "")
                                {
                                    team.Comment = column;
                                    Console.WriteLine("Comment  = " + column);
                                }
                            }
                            columnId++;
                        }
                        pairingSimu.Team.Add(team);
                    }
                    id++;
                }
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                using (StreamWriter sw = new StreamWriter(FolderName + "Data_" + pairingSimu.Label + ".json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, pairingSimu);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            }


    }
}
