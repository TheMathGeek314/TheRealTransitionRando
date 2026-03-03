using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemChanger;
using RandomizerCore.Exceptions;
using RandomizerCore.Randomization;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using rStartDef = RandomizerMod.RandomizerData.StartDef;

namespace TheRealTransitionRando {
    public class RequestModifier {
        public static void Hook() {
            RequestBuilder.OnUpdate.Subscribe(123, ApplyTransitionDefs);
            RequestBuilder.OnUpdate.Subscribe(-499, SetupItems);
            RequestBuilder.OnUpdate.Subscribe(101, RestrictPlacements);
            RequestBuilder.OnUpdate.Subscribe(-499.5f, DefinePools);
            RequestBuilder.OnSelectStart.Subscribe(0, StartStuff);
            RequestBuilder.OnUpdate.Subscribe(-1001, RemoveTransitions);

            RequestBuilder.OnUpdate.Subscribe(0, CheckCompatibilities);
        }

        public static void ApplyTransitionDefs(RequestBuilder rb) {
            if(TheRealTransitionRando.Settings.Enabled) {
                foreach(TransitionData data in TheRealTransitionRando.localSettings.finalLocationData.Values) {
                    string name = $"Transition-{data.myScene}[{data.objectName}]";
                    rb.AddLocationByName(name);
                    rb.EditLocationRequest(name, info => {
                        info.customPlacementFetch = (factory, placement) => {
                            if(factory.TryFetchPlacement(name, out AbstractPlacement ap))
                                return ap;
                            AbstractLocation absLoc = Finder.GetLocation(name);
                            absLoc.flingType = FlingType.DirectDeposit;
                            AbstractPlacement absPlace = absLoc.Wrap();
                            factory.AddPlacement(absPlace);
                            return absPlace;
                        };
                        info.getLocationDef = () => new() {
                            Name = name,
                            FlexibleCount = false,
                            AdditionalProgressionPenalty = false,
                            SceneName = data.myScene
                        };
                    });
                }
            }
        }

        private static void SetupItems(RequestBuilder rb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            foreach(TransitionData data in TheRealTransitionRando.localSettings.finalItemData.Values) {
                string name = $"Transition-{data.targetScene}[{data.entryPoint}]";
                rb.EditItemRequest(name, info => {
                    info.getItemDef = () => new ItemDef() {
                        Name = name,
                        Pool = "RealTransition",
                        MajorItem = false,
                        PriceCap = 1
                    };
                    info.randoItemCreator = factory => new TrtrRandoItem() { item = rb.lm.ItemLookup[name] };
                });
                rb.AddItemByName(name);
            }
        }

        private static void RestrictPlacements(RequestBuilder rb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            HashSet<string> bannedLocations = [LocationNames.Seer, LocationNames.Grubfather, LocationNames.Egg_Shop, LocationNames.World_Sense, LocationNames.Lore_Tablet_World_Sense];
            List<string> bannedPrefixes = ["Journal_Entry-", "Hunter's_Notes-", "Geo_Rock_Piece-", "Geo_Chest_Piece", "Colo_Geo_Piece-", "Geo_Piece_Grubfather-", "Switch-", "Fishing_Spot-", "Egg_Bomb-"];
            foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups()) {
                if(igb.strategy is DefaultGroupPlacementStrategy dgps) {
                    dgps.ConstraintList.Add(new DefaultGroupPlacementStrategy.Constraint(
                        (item, location) => !(item.Name.StartsWith("Transition-") && (bannedLocations.Contains(location.Name) || bannedPrefixes.Any(prefix => location.Name.StartsWith(prefix)))),
                        Label: "RealTransition Placement",
                        Fail: (item, location) => throw new OutOfLocationsException()
                    ));
                }
            }
        }

        private static void DefinePools(RequestBuilder rb) {
            if(!TheRealTransitionRando.Settings.Enabled)
                return;
            ItemGroupBuilder trtrGroup = null;
            string label = RBConsts.SplitGroupPrefix + "RealTransition";
            foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups()) {
                if(igb.label == label) {
                    trtrGroup = igb;
                    break;
                }
            }
            trtrGroup ??= rb.MainItemStage.AddItemGroup(label);
            rb.OnGetGroupFor.Subscribe(0.01f, ResolveTrtrGroup);
            bool ResolveTrtrGroup(RequestBuilder rb, string name, RequestBuilder.ElementType type, out GroupBuilder gb) {
                gb = default;
                return false;
            }
        }

        private static bool StartStuff(Random rng, GenerationSettings gs, SettingsPM pm, out rStartDef def) {
            if(!TheRealTransitionRando.Settings.Enabled) {
                def = null;
                return false;
            }
            BuiltinRequests.SelectStart(rng, gs, pm, out rStartDef def2);
            def = def2 with { Transition = "Transition-" + def2.Transition };
            return true;
        }

        private static void RemoveTransitions(RequestBuilder rb) {
            FieldInfo _transitionsField = typeof(Data).GetField("_transitions", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, TransitionDef> _transitions = _transitionsField.GetValue(null) as Dictionary<string, TransitionDef>;

            if(TheRealTransitionRando.Settings.Enabled) {
                _transitions.Clear();
            }
            else if(_transitions.Count == 0) {
                Dictionary<string, TransitionDef> restored = JsonUtil.Deserialize<Dictionary<string, TransitionDef>>("RandomizerMod.Resources.Data.transitions.json");
                _transitions.Clear();
                foreach(KeyValuePair<string, TransitionDef> kvp in restored) {
                    _transitions[kvp.Key] = kvp.Value;
                }
            }
        }

        private static void CheckCompatibilities(RequestBuilder _) {
            CompatChecks.Run();
        }
    }
}
