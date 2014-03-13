using UnityEngine;
using System.Collections;

//A touch observer used for dragging a gameobject on an orthographic camera.
//Works with normal game objects, not gui elements.

//2013.3.19  ken  remove momentum, use new sendmessage methods
//2013.3.4   ken  add momentum, not finished yet.
//2013.2.26  ken  initial version
public class KZDragger : MonoBehaviour {
    
    private int toucher=0;
    private Vector3 last=Vector3.zero;
    private float distance = 0;
    
    public void OnTouchBegan(KZTouchEvent e) {
        toucher++;

        //[ >>> the object will be locked in a plan 
        //      parallel to camera's near clip plane, with
        //      a distance of "distance".
        distance = (e.current.transform.position - 
                    e.camera.transform.position).magnitude; 
        last=ScreenToWorld(e.touch.position, e.camera);

    }
    
    public void OnTouchMoved(KZTouchEvent e) {
        if(toucher>0) {
            Drag(e.touch, e.camera);
        }
    }
    
    public void TouchEnded(KZTouchEvent e) {
        toucher--;
        last=Vector3.zero;
        distance = 0;
    }
    
    private void Drag(KZTouch t, Camera cam) {
        Vector3 current = ScreenToWorld(t.position, cam);
        Vector3 mov = GetMovement(current);
        //Debug.Log("move:" + mov +", current: "+current+", last: "+last);
        transform.position += mov;
        last=current;
    }
    
    private Vector3 GetMovement(Vector3 pos) {
        if(last == Vector3.zero) return last;
        else return new Vector3(pos.x-last.x, pos.y-last.y, pos.z-last.z);
    }
    
    private Vector3 ScreenToWorld(Vector2 pos, Camera cam) {
        return cam.ScreenToWorldPoint(
            new Vector3(pos.x, pos.y, distance));
    }
}


