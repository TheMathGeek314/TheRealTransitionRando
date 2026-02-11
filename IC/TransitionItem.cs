using Modding;
using System.Collections.Generic;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace TheRealTransitionRando {
    public class TransitionItem: AbstractItem {
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

        public override void GiveImmediate(GiveInfo info) {
            if(ModHooks.GetMod("ItemSyncMod") is Mod)
                DontItemSync(this);

            TransitionData td = TransitionCoords.finalItemData[name];
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

        private void DontItemSync(TransitionItem item) {
            List<Tag> nonSyncTags = new();
            foreach(IInteropTag tag in item.GetTags<IInteropTag>()) {
                if(tag.Message != "SyncedItemTag") {
                    nonSyncTags.Add((tag as Tag)!);
                }
            }
            item.RemoveTags<IInteropTag>();
            item.AddTags(nonSyncTags);
        }
    }
}
