using UnityEngine;
using System.Collections;

public class TestUV : MonoBehaviour {
    public void PrintUV() {
        Vector2[] uv = GetComponent<MeshFilter>().mesh.uv;
        for(int i=0; i<uv.Length; i++) {
            Debug.Log(uv[i]);
        }
    }
    public void PrintNormals() {
        Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
        for(int i=0; i<normals.Length; i++) {
            Debug.Log(normals[i]);
        }
    }
    public void Start() {
        PrintUV();
        PrintNormals();
    }
}
