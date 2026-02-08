using Modding;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Locations;
using ItemChanger.Internal;

namespace TheRealTransitionRando {
    public class TransitionLocation: AutoLocation {
        private static readonly Dictionary<string, TransitionLocation> SubscribedLocations = new();

        protected override void OnLoad() {
            if(SubscribedLocations.Count == 0)
                HookTransitions();
            SubscribedLocations[UnsafeSceneName] = this;
        }

        protected override void OnUnload() {
            SubscribedLocations.Remove(UnsafeSceneName);
            if(SubscribedLocations.Count == 0)
                UnhookTransitions();
        }

        private static void HookTransitions() {
            On.GameManager.OnNextLevelReady += sceneLoad;
            On.PlayMakerFSM.OnEnable += editFsm;
            if(ModHooks.GetMod("RecentItems") is Mod) {
                HookRecentItems();
            }
        }

        private static void UnhookTransitions() {
            On.GameManager.OnNextLevelReady -= sceneLoad;
            On.PlayMakerFSM.OnEnable -= editFsm;
            if(ModHooks.GetMod("RecentItems") is Mod) {
                UnhookRecentItems();
            }
        }

        private static void HookRecentItems() {
            RecentItemsDisplay.Events.ModifyDisplayItem += DoRecentFilter;
        }

        private static void UnhookRecentItems() {
            RecentItemsDisplay.Events.ModifyDisplayItem -= DoRecentFilter;
        }

        private static void DoRecentFilter(RecentItemsDisplay.ItemDisplayArgs obj) {
            if(obj != null)
                if(obj.DisplayName.StartsWith("Transition-"))
                    obj.IgnoreItem = true;
        }

        private static void sceneLoad(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            TagTransitions();
        }

        private static async void TagTransitions() {
            if(HeroController.instance == null)
                return;
            while(HeroController.instance.cState.transitioning)
                await Task.Yield();
            TransitionPoint[] transitions = GameObject.FindObjectsOfType<TransitionPoint>();
            foreach(TransitionPoint tp in transitions) {
                if(!tp.isADoor) {
                    tp.gameObject.layer = LayerMask.NameToLayer("Terrain");
                    tp.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
                    tp.gameObject.AddComponent<TransitionCollider>();
                }
            }
        }

        private static void editFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            string sceneName = self.gameObject.scene.name;
            string objectName = self.gameObject.name;
            if(sceneName == "Tutorial_01" && self.FsmName == "Great Door") {
                self.GetState("Move").GetFirstActionOfType<BeginSceneTransition>().Enabled = false;
                self.GetState("Move").AddLastAction(new Lambda(() => {
                    GiveInfo giveInfo = new() {
                        Container = Container.Unknown,
                        FlingType = FlingType.DirectDeposit,
                        MessageType = MessageType.Corner,
                        Transform = self.transform
                    };
                    Ref.Settings.Placements["Transition-Tutorial_01[right1]"].GiveAll(giveInfo);
                }));
            }
            if(sceneName == "Crossroads_01" && objectName == "door1")
                objectName = "top1";
            if(self.FsmName == "Door Control" && TransitionCoords.locationData.ContainsKey((sceneName, objectName))) {
                FsmState grantState = self.AddState("Grant Check");
                FsmState canEnterState = self.GetState("Can Enter?");
                canEnterState.ClearTransitions();
                canEnterState.AddTransition("FINISHED", "Grant Check");
                grantState.AddFirstAction(new Lambda(() => {
                    if(Ref.Settings.Placements.TryGetValue($"Transition-{sceneName}[{objectName}]", out AbstractPlacement ap)) {
                        GiveInfo giveInfo = new GiveInfo {
                            Container = Container.Unknown,
                            FlingType = FlingType.DirectDeposit,
                            MessageType = MessageType.Corner,
                            Transform = self.transform
                        };
                        ap.GiveAll(giveInfo);
                    }
                }));
                grantState.AddLastAction(new HidePromptMarker() { storedObject = self.FsmVariables.GetFsmGameObject("Prompt") });
            }
        }
    }

    public class TransitionCollider: MonoBehaviour {
        private TransitionPoint tp;
        private string locationName;
        private bool isRandod;
        private static FieldInfo _activated = typeof(TransitionPoint).GetField("activated", BindingFlags.NonPublic | BindingFlags.Instance);

        public TransitionCollider() {
            tp = gameObject.GetComponent<TransitionPoint>();
            string scene = gameObject.scene.name;
            string objectName = gameObject.name;
            if(scene == "Deepnest_14" && objectName.StartsWith("left"))
                objectName = "left1";
            else if(scene == "Fungus2_14" && objectName.StartsWith("bot"))
                objectName = "bot3";
            else if(scene == "Fungus2_15" && objectName.StartsWith("top"))
                objectName = "top3";
            else if(scene == "Fungus2_25" && objectName.StartsWith("right"))
                objectName = "right1";
            else if(scene == "Crossroads_01" && objectName == "top2")
                objectName = "top1";
                isRandod = TransitionCoords.locationData.ContainsKey((scene, objectName));
            if(isRandod)
                locationName = $"Transition-{scene}[{objectName}]";
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if(!isRandod)
                return;
            if(!tp.isADoor && collision.collider.gameObject.layer == 9 && GameManager.instance.gameState == GlobalEnums.GameState.PLAYING) {
                _activated.SetValue(tp, true);
                if(Ref.Settings.Placements.TryGetValue(locationName, out AbstractPlacement ap)) {
                    GiveInfo giveInfo = new GiveInfo {
                        Container = Container.Unknown,
                        FlingType = FlingType.DirectDeposit,
                        MessageType = MessageType.Corner,
                        Transform = tp.transform
                    };
                    ap.GiveAll(giveInfo);
                }
            }
        }
    }
}
