using System.Collections.Generic;

namespace TheRealTransitionRando {
    public class GlobalSettings {
        public bool Enabled = false;
        [MenuChanger.Attributes.MenuRange(-1, 99)]
        public int Group = -1;
    }

    public class LocalSettings {
        public Dictionary<string, TransitionData> finalLocationData = new();
        public Dictionary<string, TransitionData> finalItemData = new();
    }
}
