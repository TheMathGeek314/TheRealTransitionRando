# TheRealTransitionRando

This Randomizer connection adds transitions into the same pool as the rest of the randomized items.

This is effectively an uncoupled room rando. Touching a transition or interacting with a door will grant an item, and picking up a shiny or otherwise obtaining a check could send you to a new scene. 
Walking into any transition is now a location, and coming out of any transition is now the effect of an item. Since there is no logic for entering a room from an arbitrary check location, the room rando must necessarily be uncoupled.

Unreliable or unavoidable locations (such as Seer, Grubfather, journal entries, etc.) are not permitted to hold transition items, but if I've missed any, please let me know.

## Interop and other mods
- This should work normally with **RandoSettingsManager**.
- **RandoMapCore** was updated to allow for pathfinder support (thank you Phenomenal).
- This connection does not interact with base Rando's transition randomizer settings, so rando's settings will not make a difference.
  - **TrandoPlus** settings will likely also not matter, but some configurations may throw errors during generation.
- **BugPrince**, **Scattered and Lost**, and various Godhome connections are recognized when they add new rooms or transitions.
- **MilliGolf** messes with transition objects and is not considered compatible with TheRealTransitionRando
- **ItemSync** will mark locations as checked but will not apply transitions items to remote clients

## Known issues
- When purchasing a transition from a shop, you may lose some UI elements temporarily. Pausing and unpausing should bring them back.