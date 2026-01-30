using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringLogic;
using RandomizerCore.StringParsing;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace TheRealTransitionRando {
    public static class LogicAdder {
        public static void Hook() {
            RCData.RuntimeLogicOverride.Subscribe(50, ApplyLogic);
            RCData.RuntimeLogicOverride.Subscribe(float.MaxValue, ReplaceLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            JsonLogicFormat fmt = new();
            using Stream s = typeof(LogicAdder).Assembly.GetManifestResourceStream("TheRealTransitionRando.Resources.logic.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, s);

            /*using Stream st = typeof(LogicAdder).Assembly.GetManifestResourceStream("TheRealTransitionRando.Resources.logicSubstitutions.json");
            lmb.DeserializeFile(LogicFileType.LogicSubst, fmt, st);*/

            DefineTermsAndItems(lmb, fmt);
        }

        private static void DefineTermsAndItems(LogicManagerBuilder lmb, JsonLogicFormat fmt) {
            using Stream t = typeof(LogicAdder).Assembly.GetManifestResourceStream("TheRealTransitionRando.Resources.terms.json");
            lmb.DeserializeFile(LogicFileType.Terms, fmt, t);

            foreach(string item in TransitionCoords.itemData.Keys) {
                //lmb.AddItem(new SingleItem(item, new TermValue(lmb.GetTerm(item), 1)));
                lmb.AddItem(new LogicRealTransition(item, new TermValue(lmb.GetTerm(item), 1)));
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
                    if(key != term && lmb.LogicLookup[key].ToInfix().Contains(term)) {
                        lmb.DoSubst(new(key, term, TransitionCoords.logicReplaceData[term]));
                    }
                }
            }

            foreach(string key in keys) {
                /*if(!lmb.Waypoints.Contains(key) && !lmb.Transitions.Contains(key)) {
                    lmb.AddWaypoint(new(key, lmb.LogicLookup[key].ToInfix()));
                }*/
                if(key.StartsWith("Transition-") || lmb.Transitions.Contains(key))
                    continue;
                lmb.AddWaypoint(new($"TRTR_Waypoint-{key}", lmb.LogicLookup[key].ToInfix()));
                mlog($"Added waypoint TRTR_Waypoint-{key} : {lmb.LogicLookup[key].ToInfix()}");
            }
        }

        public static void mlog(string msg) {
            Modding.Logger.Log($"[TheRealTransitionRando] - {msg}");
        }
    }
}
