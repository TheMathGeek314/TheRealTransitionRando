using System.Collections.Generic;
using System.Linq;
using RandomizerCore;
using RandomizerCore.Logic;

namespace TheRealTransitionRando {
    public record LogicRealTransition: LogicItem, ILocationDependentItem {
        public string name;
        public Term term;

        public LogicRealTransition(string Name, TermValue tv) : base(Name) {
            name = Name;
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
