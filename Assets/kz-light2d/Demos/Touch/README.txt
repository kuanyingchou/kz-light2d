TODO:
* button group? What if sb accidentally presses next and previous buttons at the same time?
  >>> multi tourh can be disabled by settting max concurrent touch to 1
* drag on screen, press on object  
* use mouse and touch simutaneously

BUG: 
* mouse id > finger id mapping

HISTORY:
2013.3.26  ken  move to git.
2013.3.21  ken  Add OnTouchClicked() to APTouch.
2013.3.19  ken  Remove APTouchable, APTouchObserver, APTouchAdapter and use SendMessage() instead. 
                With the experience from the Family project, I feel taht the class Touchable is not necessary. 
                It adds one more procedure to clients, since one needs to remember to add both a Collider and a 
                Touchable to a touchable object. Further more, a touchable object usually has only 
                one TouchAdapter attached. It is not necessary to have a list of TouchObservers for every touchable objects.
                Thus, I want to remove TouchAdapter and Touchable and follow the Unity style. Any GameObjects that 
                need to do something while being touched only have to write OnTouchXXX() methods.
                
2013.3.18  ken  use FixedUpdate() as entry point. More tests are needed.
2013.3.14  ken  fix double-hit problem in multi-camera settings(clicking pause will also click objects behind it). 
                Now a scene can have only one APTouchManager, and it can be attached to any GameObject. Since 
                it takes care of all the cameras in the scene, the GameObject it is attached to need not have a 
                camera attached. The priority of objects is 1) camera's depth, 2) GUILayer, and 3) the distance between
                a hit point to a camera.
2013.3.13  ken  remove layer mask parameter of the Raycast call since it caused a ray to trigger more than one hits
                if objets overlap with each other. Be aware that the objects must overlap physically in scene 
                in order to resolve the double hit problem.
2013.3.13  ken  add layer mask parameter to the Raycast call. Now different touch managers can co-exist
                in a single scene, each attached to a different camera. A touch manager uses the culling 
                mask of the attached camera as it's layer mask, thus will only be aware of the objects 
                seen in the camera. 
2013.3.12  ken  1) extract APInput from APTouchManager
                2) rename APTouchStrategy to APTouchDispatcher
2013.3.8   ken  mark APTouchAdapter.Start() as virtual for overridden
2013.3.6   ken  change menu item route from APTools/Touch to APTouch.
2013.3.5   ken  add "Create Touch Adapter" menu item.
2013.3.4   ken  1. add "Add Touch Manager", "Mark as Touchable" menu items
                2. fix touch manager, all touch events are dispatched 
                   to the first clicked object. implement multi-touch on a 
				   single object using original implementation
                3. fix dragger performance bug
2013.2.27  ken  1. add APMultiTouchable, APMultiTouchAdapter, implementation pended.
                2. ignore extra touches on a single object.
