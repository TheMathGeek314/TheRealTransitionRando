using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Internal;
using Modding;
using System;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

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
            foreach(TransitionPoint tp in transitions) {//this should be conditional to whether checks exist (palace exits aren't real at the moment)
                if(!tp.isADoor) {
                    tp.gameObject.layer = LayerMask.NameToLayer("Terrain");
                    tp.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
                    tp.gameObject.AddComponent<TransitionCollider>();
                }
                else {
                    tp.gameObject.SetActive(false);
                }
            }
        }

        private static void editFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.scene.name == "Tutorial_01" && self.FsmName == "Great Door") {
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
