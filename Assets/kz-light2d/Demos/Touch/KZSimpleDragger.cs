using UnityEngine;
using System.Collections;

public class KZSimpleDragger : MonoBehaviour {
    
    private int toucher=0;
    
    public void OnTouchBegan(KZTouchEvent e) {
        toucher++;
    }
    
    public void OnTouchMoved(KZTouchEvent e) {
        if(toucher>0) {
            Drag(e.touch, e.camera);
        }
    }
    
    public void TouchEnded(KZTouchEvent e) {
        toucher--;
    }
    
    private void Drag(KZTouch t, Camera cam) {
        Vector3 current = ScreenToWorld(t.position, cam);
        transform.position = 
            new Vector3(current.x, current.y, transform.position.z);
    }
    
    private Vector3 ScreenToWorld(Vector2 pos, Camera cam) {
        return cam.ScreenToWorldPoint(
            new Vector3(pos.x, pos.y, cam.nearClipPlane));
    }
    
}
