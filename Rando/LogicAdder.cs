using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace TheRealTransitionRando {
    public static class LogicAdder {
        public static void Hook() {
            RCData.RuntimeLogicOverride.Subscribe(float.MaxValue - 1, ApplyLogic);
            RCData.RuntimeLogicOverride.Subscribe(float.MaxValue, ReplaceLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            JsonLogicFormat fmt = new();
            using Stream s = typeof(LogicAdder).Assembly.GetManifestResourceStream("TheRealTransitionRando.Resources.logic.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, s);

            DefineTermsAndItems(lmb, fmt);
            CheckFakeData(lmb, fmt);
        }

        private static void DefineTermsAndItems(LogicManagerBuilder lmb, JsonLogicFormat fmt) {
            using Stream t = typeof(LogicAdder).Assembly.GetManifestResourceStream("TheRealTransitionRando.Resources.terms.json");
            lmb.DeserializeFile(LogicFileType.Terms, fmt, t);

            foreach(string item in TransitionCoords.itemData.Keys) {
                lmb.AddItem(new TrtrLogicItem(item, new TermValue(lmb.GetTerm(item), 1)));
            }
        }

        private static void CheckFakeData(LogicManagerBuilder lmb, JsonLogicFormat fmt) {
            TransitionCoords.finalLocationData.Clear();
            TransitionCoords.finalItemData.Clear();

            foreach((string, string) key in TransitionCoords.locationData.Keys)
                TransitionCoords.finalLocationData.Add(key, TransitionCoords.locationData[key]);
            foreach((string scene, string objName) in TransitionCoords.fakeLocationData.Keys) {
                string icName = $"{scene}[{objName}]";
                if(lmb.Transitions.Contains(icName)) {
                    lmb.AddLogicDef(new("Transition-" + icName, "*" + icName));
                    TransitionCoords.finalLocationData.Add((scene, objName), TransitionCoords.fakeLocationData[(scene, objName)]);
                }
            }

            foreach(string key in TransitionCoords.itemData.Keys)
                TransitionCoords.finalItemData.Add(key, TransitionCoords.itemData[key]);
            foreach(string item in TransitionCoords.fakeItemData.Keys) {
                string icName = item.Split('-')[1];
                if(lmb.Transitions.Contains(icName)) {
                    Term itemTerm = lmb.GetOrAddTerm(item, TermType.State);
                    lmb.AddItem(new TrtrLogicItem(item, new TermValue(itemTerm, 1)));
                    TransitionCoords.finalItemData.Add(item, TransitionCoords.fakeItemData[item]);
                }
            }
        }

        private static void ReplaceLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            List<string> keys = lmb.LogicLookup.Keys.ToList();
            foreach(string key in keys) {
                if(key.StartsWith("Transition-"))
                    continue;
                foreach(string term in TransitionCoords.logicReplaceData.Keys) {
                    if(term.Substring(0, 4) == "Town" && key.StartsWith("Deepnest_Spider_Town"))
                        continue;
                    if(lmb.LogicLookup[key].ToInfix().Contains(term)) {
                        lmb.DoSubst(new(key, term, TransitionCoords.logicReplaceData[term]));
                    }
                }
            }

            string waypointPrefix = "Trtr_Waypoint-";
            foreach(string key in keys) {
                if(lmb.Transitions.Contains(key))
                    continue;
                lmb.AddWaypoint(new(waypointPrefix + key, lmb.LogicLookup[key].ToInfix()));
            }
            foreach(string t in new string[] { "Tutorial_01[top1]", "Town[top1]", "Deepnest_01b[top2]", "Deepnest_East_03[top2]",
                                                "Fungus2_25[top2]", "RestingGrounds_02[top1]", "Mines_23[top1]", "Mines_13[top1]" })
                lmb.AddWaypoint(new("Transition-" + t, lmb.LogicLookup[t].ToInfix()));
        }
    }
}
