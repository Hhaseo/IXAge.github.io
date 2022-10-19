using IXAge_IHM.Shared.Pairing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IXAge_Pairing_Parsing
{
    public class ParsWorsheet
    {

        public static void Pars(LinqToExcel.ExcelQueryFactory excelFile, string SheetName, string FolderName)
        {
            var result = from c in excelFile.WorksheetNoHeader(SheetName)
                         select c;
            var pairingSimu = new PairingScenario() { Label = result.ToList()[0][1] };
            Console.WriteLine("Team =  " + result.ToList()[0][1]);
            int id = 0;
            int nbPlayer = 0;
            pairingSimu.Team = new List<Team>();
            foreach (var line in result)
            {
                
                if (id > 1 && id < 4)
                {
                    int columnId = 0;
                    foreach (var column in line)
                    {
                        if (id == 2)
                        {
                            if (column != "")
                            {
                                pairingSimu.Opponents.Add((column, "NoArmy"));
                                Console.WriteLine("Ennemy Player = " + column);
                            }
                            else if (nbPlayer == 0 && columnId > 2)
                            {
                                nbPlayer = columnId - 2;
                                Console.WriteLine("Nb Player = " + nbPlayer);
                            }
                        }
                        else if (columnId <= (nbPlayer+1))
                        {
                            if (column != "")
                            {
                                pairingSimu.Opponents[columnId-2] = (pairingSimu.Opponents[columnId - 2].Item1, column);
                                Console.WriteLine("Ennemy Army = " + column);
                            }
                        }
                        columnId++;
                    }
                }
                else if (id >= 4 && id < (4+nbPlayer))
                {
                    var team = new Team();
                    int columnId = 0;
                    foreach (var column in line)
                    {
                        if (columnId == 0)
                        {
                            if (column != "")
                            {
                                team.Name = column;
                                Console.WriteLine("Player = " + column);
                            }
                        }
                        else if (columnId == 1)
                        {
                            if (column != "")
                            {
                                team.Army = column;
                                Console.WriteLine("Army = " + column);
                            }
                        }
                        else if (columnId < (nbPlayer + 2))
                        {
                            if (column != "" && int.TryParse(column, out var eval))
                            {
                                team.Evals.Add(eval);
                                Console.WriteLine("Estimation  = " + column);
                            } 
                        }
                        else if (columnId == (nbPlayer + 2))
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
            using (StreamWriter sw = new StreamWriter(FolderName+"Data_"+pairingSimu.Label+".json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, pairingSimu);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }
        }


    }
}
