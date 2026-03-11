using System.Collections.Generic;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.Randomization;
using RandomizerCore.Updater;

namespace TheRealTransitionRando {
    public record TrtrLogicItem(string Name, Term term): LogicItem(Name), ILocationDependentItem {
        public override void AddTo(ProgressionManager pm) {
            if(term.Type == TermType.State)
                pm.GiveMinimumState(term);
            else
                pm.Set(term, 1);
        }

        public override IEnumerable<Term> GetAffectedTerms() {
            yield return term;
        }

        public void Place(ProgressionManager pm, ILogicDef location) {
            if(location is IndeterminateLocation il) {
                const string key = "Transition" + nameof(GroupedStateTransmittingHook);
                GroupedStateTransmittingHook hook;
                if(!il.Shared.TryGetValue(key, out object obj)) {
                    il.Shared.Add(key, obj = hook = new GroupedStateTransmittingHook(il.Group.Label));
                    foreach(IRandoLocation rl in il.Group.Locations) {
                        if(rl is RandoTransition rt)
                            hook.AddSource(rt.lt.term);
                        pm.mu.AddPMHook(hook);
                    }
                }
            }
            else {
                pm.mu.LinkState(pm.lm.GetTermStrict("Trtr_Waypoint-" + location.Name), term);
            }
        }
    }
}
