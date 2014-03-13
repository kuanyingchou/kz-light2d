using UnityEngine;
using System.Collections;

public class TestKZUtil : MonoBehaviour {
    void Start () {
        KZUtil.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
    }
}
