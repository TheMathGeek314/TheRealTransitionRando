using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach(Term locTerm in location.GetTerms())
                pm.mu.LinkState(locTerm, term);
        }
    }
}
