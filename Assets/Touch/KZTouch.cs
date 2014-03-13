using UnityEngine;
using System.Collections;


//An immutable wrapper around Unity's Touch struct. Unlike Touch, 
//this class can also be constructed manually.

//2013.2.26  ken  initial version
public class KZTouch {
    
    public readonly int fingerId=0;
    public readonly Vector2 position=Vector2.zero;
    public readonly Vector2 deltaPosition=Vector2.zero;
    public readonly float deltaTime=0;
    public readonly int tapCount=0;
    public readonly TouchPhase phase=TouchPhase.Ended; //?
    public bool isProcessed=false;
    //>>> enum KZTouchPhase {Hovered, Dragged} ?

    public KZTouch(Touch t) {
        fingerId=t.fingerId;//fid; //t.fingerId; 
        //[ since fingerId didn't start with
        //  0 after resume
        position=t.position;
        deltaPosition=t.deltaPosition;
        deltaTime=t.deltaTime;
        tapCount=t.tapCount;
        phase=t.phase;
    }
    public KZTouch(int id, Vector2 pos, Vector2 dpos, 
                   float dtime, int tap, TouchPhase p) {
        fingerId=id;
        position=pos;
        deltaPosition=dpos;
        deltaTime=dtime;
        tapCount=tap;
        phase=p;
    }
    public override string ToString() {
        return "Touch " + fingerId + " - "+phase + " "+position;
    }
}
