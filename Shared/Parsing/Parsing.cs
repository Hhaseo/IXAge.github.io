using IronPdf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IXAge_IHM.Shared.Parsing
{
    public class Parsing
    {
        public Book book = new Book();

        static HashSet<string> specificSection = new HashSet<string>()
        {
            // Vampire
            "The Suffering",
            "Swift Death"
        };
        public List<string> logString = new List<string>();
        public override string ToString() => String.Join("\n", ToListString());
        public IEnumerable<string> ToListString()
        {
            List<string> res = new List<string>();
            res.Add("Book :");
            foreach (var section in book.sections)
            {
                res.Add("-----------");
                res.Add(section.Key + " = " + section.Value.Limit);
                foreach (var line in section.Value.data)
                    res.Add(line);
            }
            res.Add("-----------");
            res.Add("Synthese");
            foreach (var line in book.endingRecap.data)
                res.Add(line);
            return logString;
        }

        public bool TryParseInterval(string data, out (int, int) res)
        {
            var pos = data.IndexOf("–");
            if (pos > 0 && int.TryParse(data.Substring(0, pos), out var start) && int.TryParse(data.Substring(pos+1, data.Length-pos-1), out var end))
            {
                res = (start, end);
                return true;
            }
            res = (-1, -1);
            return false;
        }

        public enum ParsingStep
        {
            EnTete,
            BeforeGlobalStat,
            GlobalStat,
            Defensive,
            Offensive,
        }

        public ParsingStep ParsingEnTete(string line, Unit newUnit, ParsingStep currentStep)
        {
            (int, int) intervals = (-1, -1);
            int value;
            var elems = line.ToLower().Split(' ').ToList();
            var id = elems.IndexOf("height");
            if (id >= 0 && id < (elems.Count - 1))
            {
                newUnit.Height = elems[id + 1];
            }
            id = elems.IndexOf("type");
            if (id >= 0 && id < (elems.Count - 1))
            {
                newUnit.Type = elems[id + 1];
            }
            id = elems.IndexOf("base");
            if (id >= 0 && id < (elems.Count - 1))
            {
                newUnit.Base = elems[id + 1] + " " + elems[id + 2];
                currentStep = ParsingStep.BeforeGlobalStat;
            }
            id = elems.IndexOf("pts");
            if (id > 0 && int.TryParse(elems[id-1], out value) && newUnit.Cost == -1)
            {
                newUnit.Cost = value;
            }
            id = elems.IndexOf("pts/extra");
            if (id > 0 && id+1 == elems.IndexOf("model") && int.TryParse(elems[id-1], out value) && newUnit.CostExtraModel == -1)
            {
                newUnit.CostExtraModel = value;
            }

            id = elems.IndexOf("models");
            if (id >= 0 && !newUnit.unitsModels.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.unitsModels = intervals;
            }
            id = elems.IndexOf("mounts/army");
            if (id >= 0 && !newUnit.LimitMount.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitMount = intervals;
            }
            id = elems.IndexOf("units/army");
            if (id >= 0 && !newUnit.LimitUnits.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitUnits = intervals;
            }
            id = elems.IndexOf("models/army");
            if (id >= 0 && !newUnit.LimitModels.HasValue &&  TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitModels = intervals;
            }
            id = elems.IndexOf("models*");
            if (id >= 0 && !newUnit.unitsModels.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.unitsModels = intervals;
            }
            id = elems.IndexOf("mounts/army*");
            if (id >= 0 && !newUnit.LimitMount.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitMount = intervals;
                logString.Add("Limit Mount : "+newUnit.LimitMount.Value.Item1 + " to " + newUnit.LimitMount.Value.Item2);
            }
            id = elems.IndexOf("units/army*");
            if (id >= 0 && !newUnit.LimitUnits.HasValue && TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitUnits = intervals;
                logString.Add("Limit Units : "+newUnit.LimitUnits.Value.Item1 + " to " + newUnit.LimitUnits.Value.Item2);
            }
            id = elems.IndexOf("models/army*");
            if (id >= 0 && !newUnit.LimitModels.HasValue &&  TryParseInterval(elems[id-1], out intervals))
            {
                newUnit.LimitModels = intervals;
                logString.Add("Limit Models : "+newUnit.LimitModels.Value.Item1 + " to " + newUnit.LimitModels.Value.Item2);
            }
            return currentStep;
        }

        public ParsingStep ParsingGlobalStats (string line, Unit newUnit, ParsingStep currentStep)
        {
            return currentStep;
        }
        public void Load(string fileName)
        {
            //Select the Desired PDF File
            PdfDocument PDF = PdfDocument.FromFile(fileName);
            
            //Using ExtractAllText() method, extract every single text from an pdf
            string AllText = PDF.ExtractAllText();
            //View text in an Label or textbox
            var texte = AllText.Split("\r\n");
            //Console.Write(@"Identify : ");
            bool firstSection = false;
            bool endSection = false;
            string currentSection = "";
            bool previousLineIsEmpty = true;
            bool inMountSession = false;
            Unit newUnit = null;
            bool hasRea = false;
            ParsingStep currentStep = ParsingStep.EnTete;
            for (int i = 0; i < texte.Length; i++)
            //foreach (var line in texte)
            {
                if (!firstSection && texte[i].Contains("Characters ("))
                {
                    firstSection = true;
                }
                if (firstSection && texte[i].Contains("Quick Reference Sheet"))
                {
                    endSection = true;
                }
                if (firstSection && !endSection)
                {
                    if (texte[i].Contains("Characters (") || texte[i].Contains("Core (") || texte[i].Contains("Special (")
                        || specificSection.Any(t => texte[i].Contains(t + " (")))
                    {
                        int n = texte[i].IndexOf(" (");
                        var section = texte[i].Substring(0, n);
                        var max = texte[i].Substring(n + 2, texte[i].Length - n - 3);
                        currentSection = section;
                        book.sections[section] = new Section() { Limit = max };
                        previousLineIsEmpty = true;
                        inMountSession = false;
                    }
                    else if (texte[i].Contains("Character Mounts"))
                    {
                        var section = texte[i];
                        currentSection = section;
                        previousLineIsEmpty = true;
                        book.sections[section] = new Section() { };
                        inMountSession = true;
                    }
                    else 
                    {
                        book.sections[currentSection].data.Add(texte[i]);
                        if (previousLineIsEmpty && texte[i] != "" && (i + 1) < texte.Length &&
                            ((!inMountSession && texte[i+1].Contains("pts")) || (inMountSession && texte[i].Contains("Height"))
                            || (inMountSession && texte[i+1].Contains("Mounts"))))
                        {
                            if (texte[i].Contains(" Height"))
                            {
                                logString.Add("\n\n New Unit : "+texte[i].Substring(0, texte[i].IndexOf(" Height")));
                            }
                            else
                            {
                                logString.Add("\n\n New Unit: "+texte[i]);
                            }
                            newUnit = new Unit() { Name = texte[i] };
                            currentStep = ParsingStep.EnTete;
                            book.sections[currentSection].Units.Add(newUnit);
                        }
                        if (newUnit != null) { 
                            if (texte[i].ToLower().Contains("models / army"))
                            {
                                newUnit.isSingleModel = true;
                            }
                            switch (currentStep)
                            {
                                case ParsingStep.EnTete:
                                    currentStep = ParsingEnTete(texte[i], newUnit, currentStep);
                                    break;
                                case ParsingStep.BeforeGlobalStat:
                                    currentStep = ParsingGlobalStats(texte[i], newUnit, currentStep);
                                    if (texte[i].Contains("Global Adv Mar Dis"))
                                    {
                                        hasRea =  texte[i].Contains("Rea");
                                        currentStep = ParsingStep.GlobalStat;
                                    }
                                    else
                                    {
                                        logString.Add("BeforeGlobalStat : "+texte[i]);
                                    }
                                    break;
                                case ParsingStep.GlobalStat:
                                    if (texte[i].Contains("Defensive HP Def Res Arm"))
                                    {
                                        currentStep = ParsingStep.Defensive;
                                    }
                                    else
                                    {
                                        logString.Add("GlobalStat : "+texte[i]);
                                        if (newUnit.gbStats == null)
                                        {
                                            if (texte[i] == "Ground")
                                            {
                                                newUnit.gbStats = new GlobalStats()
                                                {
                                                    GroundAdv = texte[i+2].Substring(0, texte[i+2].Length-1),
                                                    GroundMar =  texte[i+4].Substring(0, texte[i+4].Length-1),
                                                    FlyAdv=texte[i+3].Substring(0, texte[i+3].Length-1),
                                                    FlyMar=texte[i+5].Substring(0, texte[i+5].Length-1)
                                                };
                                                 i = i+6;
                                                var values = texte[i].Split(' ').ToList();
                                                 newUnit.gbStats.Dis = values[0];
                                                values.RemoveAt(0);
                                                if (hasRea)
                                                {
                                                    newUnit.gbStats.Rea = values[0];
                                                    values.RemoveAt(0);
                                                }
                                                newUnit.gbStats.ModelRules = String.Join(" ", values);
                                            }
                                            else
                                            {
                                                var values = texte[i].Split(' ').ToList();
                                                if (values.Count() > 1)
                                                {
                                                    if (values[0].Contains("D"))
                                                    {
                                                        newUnit.gbStats = new GlobalStats()
                                                        {
                                                            GroundAdv = values[0].Substring(0, values[0].Length-1),
                                                        };
                                                    }
                                                    else
                                                    {
                                                        newUnit.gbStats = new GlobalStats()
                                                        {
                                                            GroundAdv = values[0].Substring(0, values[0].Length-1),
                                                            GroundMar = values[1].Substring(0, values[1].Length-1),
                                                        };
                                                        values.RemoveAt(0);
                                                    }
                                                    values.RemoveAt(0);
                                                    newUnit.gbStats.Dis = values[0];
                                                    values.RemoveAt(0);
                                                    if (hasRea)
                                                    {
                                                        newUnit.gbStats.Rea = values[0];
                                                        values.RemoveAt(0);
                                                    }
                                                    newUnit.gbStats.ModelRules = String.Join(" ", values);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            newUnit.gbStats.ModelRules += " " + texte[i];
                                        }
                                    }
                                    break;
                                case ParsingStep.Defensive:
                                    if (texte[i].Contains("Offensive Att Off Str AP Agi"))
                                    {
                                        currentStep = ParsingStep.Offensive;
                                    }
                                    else
                                    {
                                        var values = texte[i].Split(' ');
                                        if (newUnit.deffStats == null)
                                        {
                                            newUnit.deffStats = new DefensiveStats()
                                            {
                                                HP = values[0],
                                                Def = values[1],
                                                Res = values[2],
                                                Arm = values[3],
                                                Options = values.Length > 4 ? values[4] : "",
                                            };
                                        }
                                        else
                                        {
                                            if (newUnit.deffStats != null)
                                            {
                                                newUnit.deffStats.Options += " " + texte[i];
                                            }
                                        }
                                    }
                                    break;
                                case ParsingStep.Offensive:
                                    logString.Add("Offensive : "+texte[i]);
                                    break;
                            }
                        }
                        else
                        {

                            logString.Add(texte[i]);
                        }
                        //if (inMountSession)
                        //    logString.Add("Test : "+texte[i]);
                        //logString.Add(line);
                        previousLineIsEmpty = texte[i]  == "" ||  texte[i] == "d";
                    }
                }
                else if (endSection)
                {
                    book.endingRecap.data.Add(texte[i]);
                }
            }
        }
    }
}
