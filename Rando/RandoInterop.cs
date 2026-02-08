using Modding;
using System.IO;
using System.Linq;
using System.Reflection;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using MenuChanger;
using MenuChanger.MenuElements;


namespace TheRealTransitionRando {
    internal static class RandoInterop {
        public static void Hook() {
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(_ => {}, BuildConnectionMenuButton);
            RequestModifier.Hook();
            LogicAdder.Hook();

            DefineLocations();
            DefineItems();

            if(ModHooks.GetMod("RandoSettingsManager") is Mod) {
                RSMInterop.Hook();
            }
        }

        private static bool BuildConnectionMenuButton(MenuPage landingPage, out SmallButton settingsButton) {
            SmallButton button = new(landingPage, "TheRealTransitionRando");

            void UpdateButtonColor() {
                button.Text.color = TheRealTransitionRando.Settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }

            UpdateButtonColor();
            button.OnClick += () => {
                TheRealTransitionRando.Settings.Enabled = !TheRealTransitionRando.Settings.Enabled;
                UpdateButtonColor();
            };
            settingsButton = button;
            return true;
        }

        public static void DefineLocations() {
            static void DefineLoc(AbstractLocation loc, string scene, float x, float y) {
                InteropTag tag = AddTag(loc);
                tag.Properties["WorldMapLocation"] = (scene, x, y);
                Finder.DefineCustomLocation(loc);
            }
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            string tranCoords = assembly.GetManifestResourceNames().Single(str => str.EndsWith("transitionData.json"));
            using Stream tranStream = assembly.GetManifestResourceStream(tranCoords);

            foreach(TransitionData data in new ParseJson(tranStream).parseFile<TransitionData>())
                data.translate();

            foreach(TransitionData data in TransitionCoords.locationData.Values) {
                string name = $"Transition-{data.myScene}[{data.objectName}]";
                TransitionLocation tLoc = new() { name = name, sceneName = data.myScene };
                DefineLoc(tLoc, data.myScene, data.x, data.y);
            }
        }

        public static void DefineItems() {
            foreach(TransitionData data in TransitionCoords.itemData.Values) {
                TransitionItem item = new($"Transition-{data.targetScene}[{data.entryPoint}]");
                Finder.DefineCustomItem(item);
            }
        }

        public static InteropTag AddTag(TaggableObject obj) {
            InteropTag tag = obj.GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = TheRealTransitionRando.instance.GetName();
            return tag;
        }
    }
}
