using ConnectionSettingsRando;

namespace TheRealTransitionRando {
    internal class CSRInterop {
        public static void Hook() {
            CSR.Register(
                TheRealTransitionRando.instance.GetName(),
                () => TheRealTransitionRando.Settings,
                s => SettingsRandomizer.CopyTo(s, TheRealTransitionRando.Settings)
            );
        }
    }
}
