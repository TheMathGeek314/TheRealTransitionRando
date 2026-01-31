using System.Collections.Generic;
using RandomizerCore;
using RandomizerCore.Logic;

namespace TheRealTransitionRando {
    public record TrtrLogicItem: LogicItem, ILocationDependentItem {
        public readonly Term term;

        public TrtrLogicItem(string name, TermValue tv): base(name) {
            Name = name;
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
            pm.mu.LinkState(pm.lm.GetTermStrict(location.Name), term);
        }
    }
}
