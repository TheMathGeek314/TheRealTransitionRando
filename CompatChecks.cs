using Modding;
using System;

namespace TheRealTransitionRando {
    internal static class CompatChecks {
        private static bool golfWarned = false;

        public static void Run() {
            if(ModHooks.GetMod("BugPrince") is Mod)
                CheckBugPrince();
            if(ModHooks.GetMod("MilliGolf") is Mod) {
                if(!golfWarned) {
                    golfWarned = true;
                    throw new TrtrMilliGolfWarning();
                }
            }
            //todo: base rando
            //todo: TrandoPlus
        }

        private static void CheckBugPrince() {
            if(BugPrince.BugPrinceMod.RS.EnableTransitionChoices) {
                throw new TrtrBugPrinceException();
            }
        }
    }

    public class TrtrBugPrinceException: Exception {
        public override string ToString() => "TheRealTransitionRando does not support Transition Choices from BugPrince.";
    }

    public class TrtrMilliGolfWarning: Exception {
        public override string ToString() => "TheRealTransitionRando will not function correctly in MilliGolf scenes. (Trying again will ignore this warning)";
    }
}
