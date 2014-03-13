using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//A helper class for getting real or virtual touches 
//2013.3.12  ken  initial version
public class KZInput {
    public static bool enableTouchSimulationWithMouse=true;
    public static bool enabled = true;
    private static TouchSimulator sim=new TouchSimulator();
    private static KZTouch[] touches=new KZTouch[0];

    //should be called prior to GetTouches()
    public static void UpdateTouches() {
        if(!enabled) return;
        if(Input.touchCount>0) {
            //touches.Length means number of fingers on the screen
            touches=new KZTouch[Input.touchCount];
            for(int i=0; i<touches.Length; i++) {
                touches[i]=new KZTouch(Input.GetTouch(i));
            }
        } else {
            if(enableTouchSimulationWithMouse){
                touches = sim.GetSimulatedTouches();
            } else {
                touches = new KZTouch[0];
            }
        }
    }
    
    //Use this method instead of Input.touches for touch simulation
    //may be used for several times by different cameras in a frame
    public static KZTouch[] GetTouches() { 
        if(enabled) return touches;
        else return new KZTouch[0];
    }
    
    //touch simulation with a mouse
    private class TouchSimulator {
        private Vector2 lastMousePosition=Vector2.zero;
        private float lastTime=0;
        private const int BUTTON_COUNT=3;
        private bool[] wasMouseButtonDown=new bool[BUTTON_COUNT]; //0:left, 1:right, 2:middle
        //private int[] fingerId=new int[BUTTON_COUNT]; //index is for button id
            
        public TouchSimulator() {}
        
        public KZTouch[] GetSimulatedTouches() {
            Vector2 currentMousePosition=Input.mousePosition;
            Vector2 deltaPosition=currentMousePosition-lastMousePosition;
            float currentTime=Time.realtimeSinceStartup; //?
            float deltaTime=currentTime-lastTime;
            bool mouseMoved=(deltaPosition!=Vector2.zero)?true:false;
            List<KZTouch> touches=new List<KZTouch>();
            
            for(int i=0; i<wasMouseButtonDown.Length; i++) {
                KZTouch touch=null;
                if(Input.GetMouseButton(i)) { 
                    if( ! wasMouseButtonDown[i]) {
                        //mouse button was up but is down -> button pressed
                        touch=new KZTouch(
                            i, currentMousePosition, deltaPosition, 
                            deltaTime, 0, TouchPhase.Began);
                        wasMouseButtonDown[i]=true;
                    } else {    
                        //mouse button was down and is down -> button still pressed
                        if(mouseMoved) {
                            touch=new KZTouch(
                                i, currentMousePosition, deltaPosition, 
                                deltaTime, 0, TouchPhase.Moved);
                        } else {
                            touch=new KZTouch(
                                i, currentMousePosition, deltaPosition, 
                                deltaTime, 0, TouchPhase.Stationary);
                        }
                    }
                    touches.Add (touch);
                } else {                            
                    if(wasMouseButtonDown[i]) { 
                        //mouse button was down but is up -> button released
                        touch=new KZTouch(
                            i, currentMousePosition, deltaPosition,
                            deltaTime, 0, TouchPhase.Ended);
                        touches.Add (touch);
                        wasMouseButtonDown[i]=false;
                    } else {
                        //mouse button was up and is still up -> nothing interesting here
                    }
                }
            }
            
            lastMousePosition=currentMousePosition;
            lastTime=currentTime;
            return touches.ToArray();
        }
        
    }
}
