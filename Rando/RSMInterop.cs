using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheRealTransitionRando {
    internal static class RSMInterop {
        public static void Hook() {
            RandoSettingsManagerMod.Instance.RegisterConnection(new TrtrSettingsProxy() {
                getter = () => TheRealTransitionRando.Settings,
                setter = gs => TheRealTransitionRando.Settings = gs
            });
        }
    }

    /*internal class TrtrSettingsProxy: RandoSettingsProxy<GlobalSettings, string> {
        public override string ModKey => TheRealTransitionRando.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; } = new EqualityVersioningPolicy<string>(TheRealTransitionRando.instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings) {
            settings ??= new();
        }

        public override bool TryProvideSettings(out GlobalSettings settings) {
            settings = TheRealTransitionRando.Settings;
            return settings.Enabled;
        }
    }*/

    internal class TrtrSettingsProxy: RandoSettingsProxy<GlobalSettings, Signature> {
        internal Func<GlobalSettings> getter;
        internal Action<GlobalSettings> setter;

        public override string ModKey => nameof(TheRealTransitionRando);

        public override VersioningPolicy<Signature> VersioningPolicy => new StructuralVersioningPolicy() { settingsGetter = getter };

        public override bool TryProvideSettings(out GlobalSettings settings) {
            settings = getter();
            return settings.Enabled;
        }

        public override void ReceiveSettings(GlobalSettings settings) {
            setter(settings ?? new());
        }
    }

    internal class StructuralVersioningPolicy: VersioningPolicy<Signature> {
        internal Func<GlobalSettings> settingsGetter;

        public override Signature Version => new() { FeatureSet = FeatureSetForSettings(settingsGetter()) };

        private static List<string> FeatureSetForSettings(GlobalSettings gs) => SupportedFeatures.Where(f => f.feature(gs)).Select(f => f.name).ToList();

        public override bool Allow(Signature s) => s.FeatureSet.All(name => SupportedFeatures.Any(sf => sf.name == name));

        private static List<(Predicate<GlobalSettings> feature, string name)> SupportedFeatures = new() {
            (gs => gs.Enabled, "TheRealTransitionRando")
        };
    }

    internal struct Signature {
        public List<string> FeatureSet;
    }
}
