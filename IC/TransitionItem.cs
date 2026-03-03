using System;
using System.Reflection;
using System.Threading.Tasks;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace TheRealTransitionRando {
    public class TransitionItem: AbstractItem {
        private static Type syncedTagType;
        private static FieldInfo wasObtainedLocally;

        public TransitionItem(string name) {
            this.name = name;
            InteropTag tag = RandoInterop.AddTag(this);
            AddTag<PersistentItemTag>().Persistence = Persistence.Persistent;
            UIDef = new MsgUIDef {
                name = new BoxedString(name),
                shopDesc = new BoxedString("Going somewhere?"),
                sprite = new ItemChangerSprite("ShopIcons.Marker_W")
            };
        }

        public static void SetupReflection() {
            syncedTagType = typeof(ItemSyncMod.GlobalSettings).Assembly.GetType("ItemSyncMod.Items.SyncedItemTag");
            wasObtainedLocally = syncedTagType.GetField("isLocalPickUp", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void GiveImmediate(GiveInfo info) {
            bool given = false;
            foreach(Tag tag in tags) {
                if(tag.GetType().Name == "SyncedItemTag") {
                    GiveWithItemSync(tag);
                    given = true;
                    break;
                }
            }
            if(!given)
                GiveAsync();
        }

        private void GiveWithItemSync(object tag) {
            if((bool)wasObtainedLocally.GetValue(tag))
                GiveAsync();
        }

        private async void GiveAsync() {
            if(HeroController.instance.cState.spellQuake) {
                HeroController.instance.gameObject.LocateMyFSM("Spell Control").SendEvent("HERO LANDED");
                while(HeroController.instance.cState.spellQuake) {
                    await Task.Yield();
                }
            }
            TransitionData td = TheRealTransitionRando.localSettings.finalItemData[name];
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo {
                SceneName = td.targetScene,
                EntryGateName = td.entryPoint,
                HeroLeaveDirection = GlobalEnums.GatePosition.unknown,
                EntryDelay = td.delay,
                WaitForSceneTransitionCameraFade = true,
                PreventCameraFadeOut = false,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false,
                forceWaitFetch = false
            });
        }
    }
}
