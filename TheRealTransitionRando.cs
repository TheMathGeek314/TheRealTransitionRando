using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace TheRealTransitionRando {
    public class TheRealTransitionRando: Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings> {
        new public string GetName() => "TheRealTransitionRando";
        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings Settings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => Settings = s;
        public GlobalSettings OnSaveGlobal() => Settings;

        public static LocalSettings localSettings { get; set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;

        internal static TheRealTransitionRando instance;

        public TheRealTransitionRando(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            RandoInterop.Hook();
        }
    }
}

//check with vanilla rando and trandoplus
//incompatibilitize MilliGolf and maybe BugPrince choices
//write ReadMe