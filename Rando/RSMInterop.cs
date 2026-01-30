using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace TheRealTransitionRando {
    internal static class RSMInterop {
        public static void Hook() {
            RandoSettingsManagerMod.Instance.RegisterConnection(new TrtrSettingsProxy());
        }
    }

    internal class TrtrSettingsProxy: RandoSettingsProxy<GlobalSettings, string> {
        public override string ModKey => TheRealTransitionRando.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; } = new EqualityVersioningPolicy<string>(TheRealTransitionRando.instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings) {
            settings ??= new();
        }

        public override bool TryProvideSettings(out GlobalSettings settings) {
            settings = TheRealTransitionRando.Settings;
            return settings.Enabled;
        }
    }
}
