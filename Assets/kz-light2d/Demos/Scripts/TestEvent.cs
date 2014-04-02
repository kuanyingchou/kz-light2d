using UnityEngine;
using System.Collections;

public class TestEvent : MonoBehaviour {
    public void EnterLight(GameObject o) {
        Debug.Log("enter light: "+o.name);
    }
    public void LeaveLight(GameObject o) {
        Debug.Log("leave light "+o.name);
    }
}
