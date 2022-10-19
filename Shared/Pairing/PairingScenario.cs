using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IXAge_IHM.Shared.Pairing
{

    public class PairingScenario
    {

        public class EnnemyData
        {
            public string Item1 { get; set; }
            public string Item2 { get; set; }
        }

        public List<TeamEvals> ToIHMList()
        {
            var res = new List<TeamEvals>();
            foreach (var player in Team)
            {
                for (var i = 0; i < player.Evals.Count(); i++)
                {
                    res.Add(new TeamEvals()
                    {
                        Player = player.MyLabel,
                        Ennemy = Opponents[i].Item1 + "\t-\t" + Opponents[i].Item2,
                        Eval = player.Evals[i]
                    });
                }
            }
            return res;
        }

        public string Label { get; set; } = "";
        public List<Team> Team { get; set; } = new List<Team>();
        // Player -> Army
        public List<EnnemyData> Opponents { get; set; } = new List<EnnemyData>();

        public override string ToString()
        {
            if (Team == null || Opponents == null) return String.Empty;
            string res = "\n----------\n\t\t" + String.Join("\t", Opponents);
            foreach (var player in Team)
            {
                res += "\n" + (player.Name ?? "P") + "\t" + String.Join("\t", player.Evals ?? new List<int>() { });
            }
            res += "\n----------\n";
            return res;
        }
        public string ToString(Dictionary<int, string> team1, Dictionary<int, string> team2)
        {
            if (Team == null || Opponents == null) return String.Empty;
            string res = "\n----------\n\t\t" + String.Join("\t", team2.Select(t => Opponents[t.Key].Item1));
            res += "\n----------\n\t\t" + String.Join("\t", team2.Select(t => Opponents[t.Key].Item2));
            foreach (var player in Team)
            {
                if (team1.ContainsValue(player.Name))
                {
                    res += "\n" + (player.Name ?? "P") + "\t" + String.Join("\t", team2.Select(t => player.Evals[t.Key]) ?? new List<int>() { });
                }
            }
            res += "\n----------\n";
            return res;
        }

        public PairingScenario Switch()
        {
            var res = new PairingScenario() { Label = this.Label };

            res.Opponents = Team.Select(t => new EnnemyData() { Item1 = t.Name, Item2 = t.Army }).ToList();
            res.Team = new List<Team>();
            for (int i = 0; i < Opponents.Count; i++)
            {
                var eval = new List<int>(0);
                for (int j = 0; j < Team.Count; j++)
                {
                    eval.Add(20 - Team[j].Evals[i]);
                }
                res.Team.Add(new Pairing.Team()
                {
                    Name=Opponents[i].Item1,
                    Army=Opponents[i].Item2,
                    Evals = eval
                }); ;
            }

            return res;
        }
    }

}
