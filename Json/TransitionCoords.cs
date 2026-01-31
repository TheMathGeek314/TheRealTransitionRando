using System.Collections.Generic;

namespace TheRealTransitionRando {
    public class TransitionCoords {
        public static Dictionary<(string, string), TransitionData> locationData = new();
        public static Dictionary<string, TransitionData> itemData = new();
        public static Dictionary<string, string> logicReplaceData = new();
    }

    public class TransitionData {
        public string myScene;
        public string objectName;
        public float x;
        public float y;
        public bool isDoor;
        public string targetScene;
        public string entryPoint;
        public float delay;

        public void translate() {
            TransitionCoords.locationData.Add((myScene, objectName), this);
            TransitionCoords.itemData.Add($"Transition-{targetScene}[{entryPoint}]", this);
            TransitionCoords.logicReplaceData.Add($"{targetScene}[{entryPoint}]", $"Transition-{targetScene}[{entryPoint}]");
        }
    }
}
