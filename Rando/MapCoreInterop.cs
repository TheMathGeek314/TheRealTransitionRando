using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using ItemChanger.Internal;
using RandoMapCore.Pathfinder;
using RandoMapCore.Pathfinder.Actions;
using RandomizerCore.Logic;

namespace TheRealTransitionRando {
    internal static class MapCoreInterop {
        public static void Hook() {
            RmcPathfinder.OnGetConnectionProvidedActions += getWaypointDefs;
        }

        private static IEnumerable<WaypointActionDef> getWaypointDefs() {
            string prefix = "Transition-";
            IEnumerable<KeyValuePair<string, AbstractPlacement>> locations = Ref.Settings.Placements.Where(placement => placement.Value.Items.Any(item => item.name.StartsWith(prefix)));
            IEnumerable<(string, string)> itemTuples = locations.SelectMany(placement => placement.Value.Items.Where(item => item.name.StartsWith(prefix)), (placement, item) => (placement.Key, item.name));

            return itemTuples.Select(((string location, string item) t) => new WaypointActionDef() {
                Start = "Trtr_Waypoint-" + t.location,
                Destination = t.item.Split('-')[1],
                Text = t.location,
                Logic = new RawLogicDef(t.location, $"{RandomizerMod.RandomizerMod.RS.TrackerData.lm.LogicLookup[t.location].ToInfix()} + {t.item}")
                //CompassObjects = ???
            });
        }
    }
}
