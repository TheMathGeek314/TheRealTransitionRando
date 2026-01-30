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
            TransitionData td = TransitionCoords.itemData[name];
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
