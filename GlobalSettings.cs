using System.Collections.Generic;

namespace TheRealTransitionRando {
    public class GlobalSettings {
        public bool Enabled;
    }

    public class LocalSettings {
        public Dictionary<string, TransitionData> finalLocationData = new();
        public Dictionary<string, TransitionData> finalItemData = new();
    }
}
