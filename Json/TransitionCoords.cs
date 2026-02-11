using System.Collections.Generic;

namespace TheRealTransitionRando {
    public class TransitionCoords {
        public static Dictionary<(string, string), TransitionData> locationData = new();
        public static Dictionary<(string, string), TransitionData> fakeLocationData = new();
        public static Dictionary<(string, string), TransitionData> finalLocationData = new();
        public static Dictionary<string, TransitionData> itemData = new();
        public static Dictionary<string, TransitionData> fakeItemData = new();
        public static Dictionary<string, TransitionData> finalItemData = new();
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
        public bool fakeInterop;

        public void translate() {
            string targetSceneEntryPoint = $"{targetScene}[{entryPoint}]";
            (fakeInterop ? TransitionCoords.fakeLocationData : TransitionCoords.locationData).Add((myScene, objectName), this);
            (fakeInterop ? TransitionCoords.fakeItemData : TransitionCoords.itemData).Add("Transition-" + targetSceneEntryPoint, this);
            TransitionCoords.logicReplaceData.Add(targetSceneEntryPoint, "Transition-" + targetSceneEntryPoint);
        }
    }
}
