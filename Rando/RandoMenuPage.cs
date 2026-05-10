using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace TheRealTransitionRando {
    public class RandoMenuPage {
        internal MenuPage TrtrRandoPage;
        internal MenuElementFactory<GlobalSettings> trtrMEF;
        internal VerticalItemPanel trtrVIP;

        internal SmallButton JumpToTRTRButton;

        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu() {
            Instance = null;
        }

        public static void Hook() {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button) {
            button = Instance.JumpToTRTRButton;
            return true;
        }

        private void SetTopLevelButtonColor() {
            if(JumpToTRTRButton != null) {
                JumpToTRTRButton.Text.color = TheRealTransitionRando.Settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }

        private static void ConstructMenu(MenuPage landingPage) => Instance = new(landingPage);

        private RandoMenuPage(MenuPage landingPage) {
            TrtrRandoPage = new MenuPage(Localize("TheRealTransitionRando"), landingPage);
            trtrMEF = new(TrtrRandoPage, TheRealTransitionRando.Settings);
            trtrVIP = new(TrtrRandoPage, new(0, 300), 75f, true, trtrMEF.Elements);
            Localize(trtrMEF);
            foreach(IValueElement e in trtrMEF.Elements) {
                e.SelfChanged += obj => SetTopLevelButtonColor();
            }

            JumpToTRTRButton = new(landingPage, Localize("TheRealTransitionRando"));
            JumpToTRTRButton.AddHideAndShowEvent(landingPage, TrtrRandoPage);
            SetTopLevelButtonColor();
        }
    }
}
