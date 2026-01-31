using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace TheRealTransitionRando {
    public class TheRealTransitionRando: Mod, IGlobalSettings<GlobalSettings> {
        new public string GetName() => "TheRealTransitionRando";
        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings Settings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => Settings = s;
        public GlobalSettings OnSaveGlobal() => Settings;

        internal static TheRealTransitionRando instance;

        public TheRealTransitionRando(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            RandoInterop.Hook();
        }
    }
}

//non-rando'd transitions should exist and behave as vanilla (TransitionLocation)
//map mod routing is wrong
//do any room rando settings need to be applied (or menu settings overridden)
//are any starts jail
//redesign door locations as AutoLocation over CoordinateLocation