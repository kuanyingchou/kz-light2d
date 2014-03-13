using UnityEngine;

public class KZTouchEvent {
    public readonly KZTouch touch;
    public readonly GameObject first;
    public readonly GameObject current;
    public readonly GameObject last;
    public readonly Camera camera;
    public readonly RaycastHit hit;
    
    public KZTouchEvent(KZTouch t, GameObject f, GameObject l, GameObject c, Camera cam, RaycastHit h) {
        touch=t;
        first=f;
        current=c;
        last=l;
        camera=cam;
        hit=h;
    }
    public override string ToString() {
        return "touch: "+ touch.ToString()
               +" first: "+first
               +" last: "+last
               +" current: "+current
               +" camra: "+camera
               +" hit: "+hit;
    }
}
