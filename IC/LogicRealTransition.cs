using System.Collections.Generic;
using System.Linq;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;

namespace TheRealTransitionRando {
    public record LogicRealTransition: LogicItem, ILocationDependentItem {
        public string name;
        public readonly Term term;

        public LogicRealTransition(string name, TermValue tv): base(name) {
            Name = this.name = name;
            term = tv.Term;
        }

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
            string prefixedName = $"TRTR_Waypoint-{location.Name}";
            LogicAdder.mlog($"calling LinkState({prefixedName}, {term.Name})");
            pm.mu.LinkState(pm.lm.GetTermStrict(prefixedName), term);
        }
    }
}
