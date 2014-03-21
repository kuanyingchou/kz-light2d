using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// an object pool for caching, created for the traffic project

public class KZPool<T> where T: Component {
    private Queue<T> resources;
    
    public KZPool(IList<GameObject> prefabs, int capacity) {
        GenerateGameObjects(prefabs, capacity);
    }
    public T Pop() {
        if(resources.Count == 0) {
            return null;
        } else {
            T p = resources.Dequeue();
            SetEnabled(p, true);
            return p;
        }
    }
    public void Push(T p) {
        SetEnabled(p, false);
        resources.Enqueue(p);
    }
    
    private void GenerateGameObjects(IList<GameObject> prefabs, int capacity) {
        resources = new Queue<T>();
        for(int i=0; i<capacity; i++) {
            T p = New(prefabs[Random.Range(0, prefabs.Count)]);
            SetEnabled(p, false);
            resources.Enqueue(p);
        }
    }
    private T New(GameObject prefab) {
        return (GameObject.Instantiate(prefab) as GameObject).GetComponent<T>();
    }
    private void SetEnabled(Component target, bool enabled) {
        #if UNITY_4
        target.gameObject.SetActive(enabled);
        #else
        target.gameObject.SetActiveRecursively(enabled);
        #endif
    }

    public static void Test() {
        //KZPool pool = new KZPool<Transform>(prefabs, 24);
        //pool.Pop();
        //pool.Push(p);
    }
    
}
