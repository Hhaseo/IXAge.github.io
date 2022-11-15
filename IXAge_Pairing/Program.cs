// See https://aka.ms/new-console-template for more information
using IXAge_IHM.Shared.Pairing;
using IXAge_Pairing;
using Newtonsoft.Json;

bool tournament = false;
//"data.json", "Data_Desquiloutz & co.json", 
foreach (var fileName in new List<string>() { "Data_Data_IR_2022/Data_Pays Loire 2.json" }) {
    Console.WriteLine("Current File = "+fileName);
    string text = File.ReadAllText(fileName);
    var elems = JsonConvert.DeserializeObject<PairingScenario>(text);
    
    if (elems != null)
    {
        if (tournament)
        {
            List<int> data = new List<int>() { 8, 3, 4 };
            foreach (int a in data)
            {
                foreach (int b in data)
                {

                    var p1 = IPairingSimulationPlayer.CreatePlayer(true, a, elems);
                    var p2 = IPairingSimulationPlayer.CreatePlayer(false, b, elems.Switch());

                    var team1 = new Dictionary<int, string>();
                    var team1Label = new Dictionary<int, string>();
                    for (int i = 0; i < elems.Team.Count; i++)
                    {
                        team1[i] = elems.Team[i].Name;
                        team1Label[i] = elems.Team[i].Name;
                    }
                    var team2 = new Dictionary<int, string>();
                    var team2Label = new Dictionary<int, string>();
                    for (int i = 0; i < elems.Opponents.Count; i++)
                    {
                        team2[i] = elems.Opponents[i].Item1;
                        team2Label[i] = elems.Opponents[i].Item2;
                    }
                    var fights = new Dictionary<int, int>();

                    Console.WriteLine("Game : "+a +" versus " + b);
                    int phase = 0;
                    while (team1.Count > 1 && team2.Count > 1)
                    {
                        var p1Choice = p1.ChooseOne(team1, team1.Keys.ToList(), team2.Keys.ToList());
                        var p2Choice = p2.ChooseOne(team2, team2.Keys.ToList(), team1.Keys.ToList());
                        var p1ChoiceLabel = team1Label[p1Choice];
                        var p2ChoiceLabel = team2Label[p2Choice];
                        team1.Remove(p1Choice);
                        team2.Remove(p2Choice);

                        var p1Response = p1.ChooseResponse(p1Choice, p1ChoiceLabel, p2Choice, p2ChoiceLabel, team1, team1.Keys.ToList(), team2.Keys.ToList());
                        var p2Response = p2.ChooseResponse(p2Choice, p2ChoiceLabel, p1Choice, p1ChoiceLabel, team2, team2.Keys.ToList(), team1.Keys.ToList());

                        var p1ValidFight = p1.AcceptResponse(p2Choice, p2ChoiceLabel, p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]),
                            p1Choice, p1ChoiceLabel, p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]), team1.Keys.ToList(), team2.Keys.ToList());
                        var p2ValidFight = p2.AcceptResponse(p1Choice, p1ChoiceLabel, p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]),
                            p2Choice, p2ChoiceLabel, p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]), team2.Keys.ToList(), team1.Keys.ToList());

                        fights[p1Choice] = p1ValidFight;
                        fights[p2ValidFight] = p2Choice;
                        p1.FightValidate(p1Choice, p1ValidFight);
                        p2.FightValidate(p1ValidFight, p1Choice);
                        p1.FightValidate(p2ValidFight, p2Choice);
                        p2.FightValidate(p2Choice, p2ValidFight);
                        team1.Remove(p2ValidFight);
                        team2.Remove(p1ValidFight);
                        if (team1.Count == 2 && team2.Count == 2)
                        {
                            var p1Reject = p2Response.Item1 == p1ValidFight ? p2Response.Item2 : p2Response.Item1;
                            var p1RejectLabel = team2Label[p1Reject];
                            var p2Reject = p1Response.Item1 == p2ValidFight ? p1Response.Item2 : p1Response.Item1;
                            var p2RejectLabel = team1Label[p2Reject];
                            team2.Remove(p1Reject);
                            team1.Remove(p2Reject);
                            fights[team1.First().Key] = p1Reject;
                            fights[p2Reject] = team2.First().Key;

                            p1.FightValidate(team1.First().Key, p1Reject);
                            p2.FightValidate(p1Reject, team1.First().Key);

                            p1.FightValidate(p2Reject, team2.First().Key);
                            p2.FightValidate(team2.First().Key, p2Reject);

                            team1.Remove(team1.First().Key);
                            team2.Remove(team2.First().Key);
                        }
                    }
                    if (team1.Count == 1 && team2.Count == 1)
                    {
                        fights[team1.First().Key] = team2.First().Key;
                        p1.FightValidate(team1.First().Key, team2.First().Key);
                        p2.FightValidate(team2.First().Key, team1.First().Key);
                    }
                    int score = 0;
                    int score2 = 0;
                    foreach (var f in fights)
                    {
                        score += elems.Team[f.Key].Evals[f.Value];
                        score2 += (20 - elems.Team[f.Key].Evals[f.Value]);
                    }
                    Console.WriteLine("Player one made :" + score);
                    Console.WriteLine("Player two made :" + score2);
                    Console.WriteLine();
                    Console.WriteLine();

                }
            }

            Console.ReadLine();
        }
        else
        {
            //Console.WriteLine("----- Team 1  : ");
            Console.WriteLine(elems?.ToString());

            //Console.WriteLine("----- Team 2  : ");
            //Console.WriteLine(elems?.Switch().ToString());

            new BuildTree().RunTest(elems);

            var data = TreeNode.Build(elems);
            Console.WriteLine("Max : "+data.BestValue);
            Console.WriteLine("Min : "+data.WorstValue);
            Console.WriteLine("Ecart Type : "+data.StandardDeviation);

            Console.Write(@"Choose Player One :
                    \n\t 0 = Human (default) 
                    \n\t 1 : Humain conseillé par IA max  
                    \n\t 2 : Humain conseillé par IA v3  (default) 
                    \n\t 3 = Max IA 
                    \n\t 4 = Max Min IA v3 
                    \n\t 5 = Max Min IA 
                    \n\t 6 = Local Max 
                    \n\t 7 = Max Min IA v2 
                    \n\t 8 = Max Min IA v4 ");
            var p1 = IPairingSimulationPlayer.CreatePlayer(true, int.TryParse(Console.ReadLine(), out var id1) ? id1 : 2, elems);
            //bool humanFirst = p1 is PairingSimulationPlayer_Human;

            Console.Write(@"Choose Player Two :
                    \n\t 0 = Human     
                    \n\t 1 : Humain conseillé par IA max  
                    \n\t 2 : Humain conseillé par IA v3 
                    \n\t 3 = Max IA 
                    \n\t 4 = Max Min IA v3  (default) 
                    \n\t 5 = Max Min IA 
                    \n\t 6 = Local Max 
                    \n\t 7 = Max Min IA v2
                    \n\t 8 = Max Min IA v4 ");
            var p2 = IPairingSimulationPlayer.CreatePlayer(false, int.TryParse(Console.ReadLine(), out var id2) ? id2 : 4, elems.Switch());
            //bool humanSecond = p2 is PairingSimulationPlayer_Human;

            var team1 = new Dictionary<int, string>();
            var team1Label = new Dictionary<int, string>();
            for (int i = 0; i < elems.Team.Count; i++)
            {
                team1[i] = elems.Team[i].Name;
                team1Label[i] = elems.Team[i].Name;
            }
            var team2 = new Dictionary<int, string>();
            var team2Label = new Dictionary<int, string>();
            for (int i = 0; i < elems.Opponents.Count; i++)
            {
                team2[i] = elems.Opponents[i].Item1;
                team2Label[i] = elems.Opponents[i].Item2;
            }
            var fights = new Dictionary<int, int>();

            Console.WriteLine("\n\n");
            Console.WriteLine("Starting Game : "+p1.Name +" versus " + p2.Name);
            int phase = 0;
            while (team1.Count > 1 && team2.Count > 1)
            {
                Console.WriteLine("----- Phase n*"+phase++ + " -----");
                var p1Choice = p1.ChooseOne(team1, team1.Keys.ToList(), team2.Keys.ToList());
                var p2Choice = p2.ChooseOne(team2, team2.Keys.ToList(), team1.Keys.ToList());
                var p1ChoiceLabel = team1Label[p1Choice];
                var p2ChoiceLabel = team2Label[p2Choice];
                Console.WriteLine("-- Player one Choose :  "+p1ChoiceLabel);
                Console.WriteLine("-- Player two Choose :  "+p2ChoiceLabel);
                team1.Remove(p1Choice);
                team2.Remove(p2Choice);

                var p1Response = p1.ChooseResponse(p1Choice, p1ChoiceLabel, p2Choice, p2ChoiceLabel, team1, team1.Keys.ToList(), team2.Keys.ToList());
                var p2Response = p2.ChooseResponse(p2Choice, p2ChoiceLabel, p1Choice, p1ChoiceLabel, team2, team2.Keys.ToList(), team1.Keys.ToList());
                Console.WriteLine("-- Player one Response to  "+p2ChoiceLabel+" is : "+ team1Label[p1Response.Item1]+" And "+ team1Label[p1Response.Item2]);
                Console.WriteLine("-- Player two Response to  "+p1ChoiceLabel+" is : "+ team2Label[p2Response.Item1]+" And "+ team2Label[p2Response.Item2]);

                var p1ValidFight = p1.AcceptResponse(p2Choice, p2ChoiceLabel, p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]),
                    p1Choice, p1ChoiceLabel, p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]), team1.Keys.ToList(), team2.Keys.ToList());
                var p2ValidFight = p2.AcceptResponse(p1Choice, p1ChoiceLabel, p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]),
                    p2Choice, p2ChoiceLabel, p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]), team2.Keys.ToList(), team1.Keys.ToList());

                Console.WriteLine("Fight : " + p1ChoiceLabel + " vs " + team2Label[p1ValidFight]);
                Console.WriteLine("Fight : " + team1Label[p2ValidFight] + " vs " + p2ChoiceLabel);
                fights[p1Choice] = p1ValidFight;
                fights[p2ValidFight] = p2Choice;
                p1.FightValidate(p1Choice, p1ValidFight);
                p2.FightValidate(p1ValidFight, p1Choice);
                p1.FightValidate(p2ValidFight, p2Choice);
                p2.FightValidate(p2Choice, p2ValidFight);
                team1.Remove(p2ValidFight);
                team2.Remove(p1ValidFight);
                //if (humanFirst)
                Console.WriteLine(elems?.ToString(team1, team2));
                //if (humanSecond)
                //    Console.WriteLine(elems?.Switch().ToString(team1, team2));
                if (team1.Count == 2 && team2.Count == 2)
                {
                    Console.WriteLine("----- Last Phase -----");
                    var p1Reject = p2Response.Item1 == p1ValidFight ? p2Response.Item2 : p2Response.Item1;
                    var p1RejectLabel = team2Label[p1Reject];
                    var p2Reject = p1Response.Item1 == p2ValidFight ? p1Response.Item2 : p1Response.Item1;
                    var p2RejectLabel = team1Label[p2Reject];
                    team2.Remove(p1Reject);
                    team1.Remove(p2Reject);
                    fights[team1.First().Key] = p1Reject;
                    fights[p2Reject] = team2.First().Key;

                    p1.FightValidate(team1.First().Key, p1Reject);
                    p2.FightValidate(p1Reject, team1.First().Key);

                    p1.FightValidate(p2Reject, team2.First().Key);
                    p2.FightValidate(team2.First().Key, p2Reject);

                    Console.WriteLine("Last fight : " + team1Label[team1.First().Key] + " vs " + p1RejectLabel);
                    Console.WriteLine("Last fight : " + p2RejectLabel + " vs " + team2Label[team2.First().Key]);
                    team1.Remove(team1.First().Key);
                    team2.Remove(team2.First().Key);
                }
            }
            if (team1.Count == 1 && team2.Count == 1)
            {
                fights[team1.First().Key] = team2.First().Key;
                p1.FightValidate(team1.First().Key, team2.First().Key);
                p2.FightValidate(team2.First().Key, team1.First().Key);
                Console.WriteLine("Last fight : " + team1.First().Value + " vs " + team2.First().Value);
            }
            int score = 0;
            int score2 = 0;
            foreach (var f in fights)
            {
                score += elems.Team[f.Key].Evals[f.Value];
                score2 += (20 - elems.Team[f.Key].Evals[f.Value]);
            }
            Console.WriteLine("Player one made :" + score);
            Console.WriteLine("Player two made :" + score2);
            Console.WriteLine("End of game");
            Console.ReadLine();
        }
    }
}