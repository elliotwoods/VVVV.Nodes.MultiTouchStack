# Introduction

The MultiTouch Stack is an engine for VVVV which manages a set of slides that the user can manipulate with multitouch gestures, e.g. drag/pinch/spin.

Functionality is split between nodes to help the developer design their own usage. The `World` node keeps track of all the slides in the stack (we often interchangeably use the terms 'stack' and 'world').

The `AddSlide` node is used to add new entries.

Meanwhile the `GetSlides` and `Slide` nodes can be used to get information about slides currently in the `World`.

![World](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/World.PNG)

The SetTransform node can be used to immediately update the position, rotation and scale of slides within the stack.

![SetTransform](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/SetTransform.PNG)

# Filtering

The `Filter` nodes can be used to pick specific slides from the world based on their index, or by string tags which were added at the time of creation of the slide.

![Filter](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/Filter.PNG)

# Hit Test

Custom hit test regions can be applied to the slides using the `HitTest` nodes. The `HitTest (Transform)` allows for standard hit regions to be applied with a transform (e.g. circle, quad).

![HitTest-Transform](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/HitTest-Transform.PNG)

Whilst the `HitTest (Image)` node allows an image file to be used as the hit test region.

![HitTest-Image](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/HitTest-Image.PNG)

# Hit Callback

To create more complex behaviours, we can use the `HitEvent` to define custom hit regions on the slide with their own hit test regions.

We then can respond to actions which happen on these regions using the `HitCallback` node. We can select which events we want to listen to, e.g. `Down`, `ReleaseInside`, `ReleaseOutside`.

![HitTest-HitCallback](https://raw.githubusercontent.com/elliotwoods/VVVV.Nodes.MultiTouchStack/master/readme/HitCallback.PNG)

# Debugging

For debugging:

* `Preview (Debug DX11)` node can be used to draw a spread of slides and associated data (e.g. index, tags, attached cursor count)
* `PreviewCursor (Debug DX11)` can be used to draw the cursors in the world
* `TouchSimulator (Debug)` can be used to emulate multiple touches using a mouse. Use the space key to hold down a touch at the current mouse position, then use the left mouse button elsewhere to produce a second touch.
