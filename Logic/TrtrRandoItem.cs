using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerMod.RC;

namespace TheRealTransitionRando {
    public class TrtrRandoItem: RandoModItem, ILocationDependentItem {
        public void Place(ProgressionManager pm, ILogicDef location) {
            ((TrtrLogicItem)item).Place(pm, location);
        }
    }
}
