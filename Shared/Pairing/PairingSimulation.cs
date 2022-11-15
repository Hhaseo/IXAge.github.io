using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IXAge_IHM.Shared.Pairing
{
    public enum PairingPhase
    {
        PhaseOne, // Envoie d'une personne
        PhaseOneWait, // Envoie d'une personne
        PhaseTwo, // Proposition de 2 résultat.
        PhaseTwoWait, // Proposition de 2 résultat.
        PhaseThree, // Acceptation d'un des 2 adversaire
        PhaseThreeWait, // Acceptation d'un des 2 adversaire
        End,
    }

    public class PlayerChoice
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }

    public class PairingSimulation
    {
        public PairingScenario Scenario { get; set; }
        public IPairingSimulationPlayer P1 { get; set; }
        public IPairingSimulationPlayer P2 { get; set; }

        public Dictionary<int, string> team1 = new Dictionary<int, string>();
        public Dictionary<int, string> team1Label = new Dictionary<int, string>();
        public Dictionary<int, string> team2 = new Dictionary<int, string>();
        public Dictionary<int, string> team2Label = new Dictionary<int, string>();
        public Dictionary<int, int> fights = new Dictionary<int, int>();

        public List<PlayerChoice> P1ChoiceAvailable = new List<PlayerChoice>();
        public List<PlayerChoice> P2ChoiceAvailable = new List<PlayerChoice>();

        public int p1Choice { get; set; }
        public int p2Choice { get; set; }
        public (int, int) p1Response { get; set; }
        public (int, int) p2Response { get; set; }
        public int p1ResponseChoice { get; set; }
        public int p2ResponsereChoice { get; set; }


        public List<string> P1Moves { get; set; } = new List<string>();
        public List<string> P2Moves { get; set; } = new List<string>();
        public List<string> Moves { get; set; } = new List<string>();
        public List<string> Fights { get; set; }  = new List<string>();

        public int TotalP1 { get; set; } = 0;
        public int TotalP2 { get; set; } = 0;

        public PairingPhase CurrentPhase { get; set; }

        public PairingSimulation()
        {

        }

        public void NextStep(int p1, int p2, (int, int) p1Resp, (int, int) p2Resp, int v1, int v2)
        {
            if (!P1.AutoChoice)
                p1Choice = p1;
            if (!P2.AutoChoice)
                p2Choice = p2;
            if (!P1.AutoChoice)
                p1Response = p1Resp;
            if (!P2.AutoChoice)
                p2Response = p2Resp;
            if (!P1.AutoChoice)
                p1ResponseChoice = v1;
            if (!P2.AutoChoice)
                p2ResponsereChoice = v2;
            switch (CurrentPhase)
            {
                case PairingPhase.PhaseOne: 
                case PairingPhase.PhaseOneWait:
                    P1Moves.Add("--------- ");
                    P2Moves.Add("--------- ");
                    Moves.Add("--------- ");

                    P1Moves.Add("P1 choose " + team1Label[p1Choice]);
                    P2Moves.Add("P2 choose " + team2Label[p2Choice]);
                    Moves.Add("P1 choose " + team1Label[p1Choice]);
                    Moves.Add("P2 choose " + team2Label[p2Choice]);
                    CurrentPhase = PairingPhase.PhaseTwo;
                    AutoNextStep();

                    break;
                case PairingPhase.PhaseTwo:
                case PairingPhase.PhaseTwoWait:

                    P1Moves.Add("P1 response to  " + team2Label[p2Choice] + " is " + team1Label[p1Response.Item1] + " and " + team1Label[p1Response.Item2]);
                    P2Moves.Add("P2 response to  " + team1Label[p1Choice] + " is " + team2Label[p2Response.Item1] + " and " + team2Label[p2Response.Item2]);
                    Moves.Add("P1 response to  " + team2Label[p2Choice] + " is " + team1Label[p1Response.Item1] + " and " + team1Label[p1Response.Item2]);
                    Moves.Add("P2 response to  " + team1Label[p1Choice] + " is " + team2Label[p2Response.Item1] + " and " + team2Label[p2Response.Item2]);
                    CurrentPhase = PairingPhase.PhaseThree;
                    AutoNextStep();
                    break;
                case PairingPhase.PhaseThree:
                case PairingPhase.PhaseThreeWait:

                    P1Moves.Add("P1 choose  " + team1Label[p1ResponseChoice] + " fight " + team2Label[p2Choice]);
                    P2Moves.Add("P2 choose  " + team2Label[p2ResponsereChoice] + " fight " + team1Label[p1Choice]);
                    Moves.Add("P1 choose  " + team1Label[p1ResponseChoice] + " fight " + team2Label[p2Choice]);
                    Moves.Add("P2 choose  " + team2Label[p2ResponsereChoice] + " fight " + team1Label[p1Choice]);
                    Fights.Add(team1Label[p1ResponseChoice] + " vs " + team2Label[p2ResponsereChoice] + ". Gain P1 : " + Scenario.Team[p1ResponseChoice].Evals[p2ResponsereChoice] + ", gain P2 :" + (20 - Scenario.Team[p1ResponseChoice].Evals[p2ResponsereChoice]));
                    Fights.Add(team1Label[p1ResponseChoice] + " vs " + team2Label[p2Choice] + ". Gain P1 : "+ Scenario.Team[p1ResponseChoice].Evals[p2Choice] + ", gain P2 :"+(20 - Scenario.Team[p1ResponseChoice].Evals[p2Choice]));

                    TotalP1 += Scenario.Team[p1ResponseChoice].Evals[p2ResponsereChoice];
                    TotalP2 += (20 - Scenario.Team[p1ResponseChoice].Evals[p2ResponsereChoice]);
                    CurrentPhase = PairingPhase.End;

                    P1Moves.Add("--------- ");
                    P2Moves.Add("--------- ");
                    Moves.Add("--------- ");
                    P1Moves.Add("Total : " + TotalP1.ToString());
                    P2Moves.Add("Total : " + TotalP2.ToString());
                    Moves.Add("Total P1 : " + TotalP1.ToString());
                    Moves.Add("Total P2 : " + TotalP2.ToString());

                    P1Moves.Add("--------- ");
                    P2Moves.Add("--------- ");
                    Moves.Add("--------- ");
                    AutoNextStep();

                    break;
                default: break;
            }
        }

        public void AutoNextStep()
        {
            switch (CurrentPhase)
            {
                case PairingPhase.PhaseOne:
                    if (P1.AutoChoice)
                        p1Choice = P1.ChooseOne(team1, team1.Keys.ToList(), team2.Keys.ToList());
                    else
                        P1ChoiceAvailable = team1.Select(t => new PlayerChoice() { Id = t.Key, Label = t.Value }).ToList();
                    if (P2.AutoChoice)
                        p2Choice = P2.ChooseOne(team2, team2.Keys.ToList(), team1.Keys.ToList());
                    else
                        P2ChoiceAvailable = team2.Select(t => new PlayerChoice() { Id = t.Key, Label = t.Value }).ToList();

                    break;
                case PairingPhase.PhaseTwo:
                    if (P1.AutoChoice)
                        p1Response = P1.ChooseResponse(p1Choice, team1Label[p1Choice], p2Choice, team2Label[p2Choice], team1, team1.Keys.ToList(), team2.Keys.ToList());
                    if (P2.AutoChoice)
                        p2Response = P2.ChooseResponse(p2Choice, team2Label[p2Choice], p1Choice, team1Label[p1Choice], team2, team2.Keys.ToList(), team1.Keys.ToList());
                    break;
                case PairingPhase.PhaseThree:
                    if (P1.AutoChoice)
                        p1ResponseChoice = P1.AdviceAcceptResponse(p2Choice, team2Label[p2Choice], p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]),
                                    p1Choice, team1Label[p1Choice], p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]), team1.Keys.ToList(), team2.Keys.ToList());
                    if (P2.AutoChoice)
                        p2ResponsereChoice = P2.AdviceAcceptResponse(p1Choice, team1Label[p1Choice], p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]),
                                    p2Choice, team2Label[p2Choice], p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]), team2.Keys.ToList(), team1.Keys.ToList());
                    break;
                default: break;
            }
            if (P1.AutoChoice && P2.AutoChoice && CurrentPhase != PairingPhase.End)
            {
                NextStep(0, 0, (0, 0), (0,0));
            }
            else
            {
                switch (CurrentPhase)
                {
                    case PairingPhase.PhaseOne:
                        CurrentPhase = PairingPhase.PhaseOneWait;
                        break;
                    case PairingPhase.PhaseTwo:
                        CurrentPhase = PairingPhase.PhaseTwoWait;
                        break;
                    case PairingPhase.PhaseThree:
                        CurrentPhase = PairingPhase.PhaseThreeWait;
                        break;
                }
            }
        }

        public string GetAdvice(int player)
        {
            string ChoiceLabel = "";
            int stand = 0;
            if (player == 1 ? P1.Advice : P2.Advice)
            {
                switch (CurrentPhase)
                {
                    case PairingPhase.PhaseOne:
                        stand = player == 1 ? P1.AdviceChooseOne(team1, team1.Keys.ToList(), team2.Keys.ToList()) : P2.AdviceChooseOne(team2, team2.Keys.ToList(), team1.Keys.ToList());
                         ChoiceLabel = player == 1 ? team1Label[stand] : team2Label[stand];
                        return "IA Recommend to send : " + ChoiceLabel;
                   case PairingPhase.PhaseTwo:
                        var stand2 = player == 1 ? P1.AdviceChooseResponse(p1Choice, team1Label[p1Choice], p2Choice, team2Label[p2Choice], team1, team1.Keys.ToList(), team2.Keys.ToList()) : 
                                               P2.AdviceChooseResponse(p2Choice, team2Label[p2Choice], p1Choice, team1Label[p1Choice], team2, team2.Keys.ToList(), team1.Keys.ToList());
                        ChoiceLabel = player == 1 ? team1Label[stand2.Item1] : team2Label[stand2.Item1];
                        var Choice2Label = player == 1 ? team1Label[stand2.Item2] : team2Label[stand2.Item2];
                        return "IA Recommend to respond : " + ChoiceLabel + " and " + Choice2Label;
                    case PairingPhase.PhaseThree:
                        stand = player == 1 ? P1.AdviceAcceptResponse(p2Choice, team2Label[p2Choice], p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]),
                                    p1Choice, team1Label[p1Choice], p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]), team1.Keys.ToList(), team2.Keys.ToList()) :
                                    P2.AdviceAcceptResponse(p1Choice, team1Label[p1Choice], p2Response, (team2[p2Response.Item1], team2[p2Response.Item2]),
                                    p2Choice, team2Label[p2Choice], p1Response, (team1[p1Response.Item1], team1[p1Response.Item2]), team2.Keys.ToList(), team1.Keys.ToList());
                        ChoiceLabel = player == 1 ? team1Label[stand] : team2Label[stand];
                        return "IA Recommend to take : " + ChoiceLabel;
                    default:break;
                }
            }
            return "";
        }

        public void Set(PairingScenario scenario, IPairingSimulationPlayer p1, IPairingSimulationPlayer p2)
        {
            CurrentPhase = PairingPhase.PhaseOne;
            Scenario = scenario;
            P1 = p1;
            P2 = p2;
            for (int i = 0; i < scenario.Team.Count; i++)
            {
                team1[i] = scenario.Team[i].MyLabel;
                team1Label[i] = scenario.Team[i].Army;
            }


            for (int i = 0; i < scenario.Opponents.Count; i++)
            {
                team2[i] = scenario.Opponents[i].Item1 + "\t-\t"+ scenario.Opponents[i].Item2;
                team2Label[i] = scenario.Opponents[i].Item2;
            }
            P1Moves = new List<string>();
            P2Moves = new List<string>();
            Moves = new List<string>();
            Fights = new List<string>();
            TotalP1 = 0;
            TotalP2 = 0;
            AutoNextStep();
        }
    }
}
