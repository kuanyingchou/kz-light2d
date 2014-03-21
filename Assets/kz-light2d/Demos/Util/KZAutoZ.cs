using UnityEngine;
using System.Collections;

public class KZAutoZ {
    public static float maxZ = 100;
    public static float minZ = 0;
    public static float yRatio;
    public static float yRatioXMaxZ;
    
    public void Init() {
        yRatio = 1.0f / Screen.height;
        yRatioXMaxZ = yRatio * maxZ;    
    }

    public void Adjust(Transform transform) {
        //Debug.Log (yRatio);
        transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                maxZ + transform.position.y * yRatioXMaxZ); 
    }
}
