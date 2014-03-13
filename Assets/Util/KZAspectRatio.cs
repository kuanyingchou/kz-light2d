using UnityEngine;
using System.Collections;

//2013.12.12  ken  initial version

public class KZAspectRatio : MonoBehaviour {
    public float targetAspectRatio = 1280f / 800; //e.g. 1.6
    public bool liveUpdate = false;
    private Color backdropColor = Color.black; 
    private string backdropName = "_backdrop";
    //] there would be only one backdrop in a scene
    private Rect original;

    //Since Awake() still gets called when the script is disabled, 
    //we use Start() here
    public void Start() {
        original = camera.rect;
        AddBackdrop();
        Adjust();
    }
    public void Update() {
        if(liveUpdate) Adjust();
    }
    public void Adjust() {
        //camera.aspect = targetAspectRatio; //will distort the view

        float s = GetScreenAspectRatio();

        //[ >>> don't know how I ended up with this, but it works!
        if(targetAspectRatio > s) {
            SetCameraRect(
                original.x,
                original.y * (s / targetAspectRatio) + 
                        ((targetAspectRatio - s) / 
                        targetAspectRatio * .5f),
                original.width,
                original.height * (s / targetAspectRatio)
            );
        } else if(targetAspectRatio < s) {
            SetCameraRect(
                original.x * (targetAspectRatio / s) + 
                        (s - targetAspectRatio) / 
                        s * .5f,
                original.y,
                original.width * (targetAspectRatio / s),
                original.height
            );
        } else {
            SetCameraRect(original);
        }
    }

    private void AddBackdrop() {
        if(GameObject.Find(backdropName) != null) return;
        GameObject o = new GameObject();
        o.name = backdropName;

        Camera c = o.AddComponent<Camera>();
        c.depth = -2; //>>> how about conflicts?
        c.cullingMask = 0; //I'm blind
        c.backgroundColor = backdropColor;
        c.orthographic = true; //maybe orthographic is cheaper
    }

    private float GetScreenAspectRatio() {
        return (float)Screen.width / Screen.height; //e.g. 1.7
    }

    private void SetCameraRect(float x, float y, float w, float h) {
        camera.rect = new Rect(x, y, w, h);
    }
    private void SetCameraRect(Rect r) {
        camera.rect = r;
    }
}
