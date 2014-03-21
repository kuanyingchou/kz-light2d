using UnityEngine;
using System.Collections;

public class KZRotator : MonoBehaviour {
    
    private Vector3 lastMousePosition = Vector3.zero;
    private bool mousePressed;
    private Vector3 rotation = Vector3.zero;

    public float speed = 1.0f;
    
    public void Start() {
    }
    
    public void Update () {
        if(Input.GetMouseButtonDown(0)) {
            mousePressed = true;
        } else if(Input.GetMouseButtonUp(0)) {
            mousePressed = false;
            lastMousePosition = Vector3.zero;
        }
        if(mousePressed) {
            Vector3 newPos = Input.mousePosition;
            if(lastMousePosition != Vector3.zero) {
                Rotate(newPos - lastMousePosition);
            }
            lastMousePosition = newPos;
        }
    }
    
    private void Rotate(Vector3 diff) {
        rotation = diff * speed;
        //transform.rotation = Quaternion.identity;
        transform.Rotate(rotation.y, -rotation.x, 0, Space.World);
    }

}
