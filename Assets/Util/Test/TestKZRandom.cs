using UnityEngine;
using System.Collections;

public class TestKZRandom : MonoBehaviour {
    public void Start() {
        int[] data=new int[] {1, 2, 3};
        Debug.Log(KZUtil.GetString(data));
        KZUtil.Shuffle(data);
        Debug.Log(KZUtil.GetString(data));
    }
}
