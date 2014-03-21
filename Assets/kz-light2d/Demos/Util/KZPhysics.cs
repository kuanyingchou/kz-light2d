using UnityEngine;
using System.Collections;

public class KZPhysics : MonoBehaviour {
    public static void Kick(GameObject target, Vector3 to, 
            float mass, float duration){
	float g = - Physics.gravity.y;
        Vector3 from = target.transform.position;
	
	float vy = (to.y - from.y + (0.5f * g * duration * duration)) / 
                duration;
	float vx = (to.x - from.x) / duration;
        float vz = (to.z - from.z) / duration;
	
	Vector3 force = new Vector3(vx, vy, vz);
        target.rigidbody.AddForce(force * mass, ForceMode.Impulse);
    }

}
