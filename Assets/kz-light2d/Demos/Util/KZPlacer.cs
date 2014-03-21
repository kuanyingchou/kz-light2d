using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KZPlacer : MonoBehaviour {

    public bool liveUpdate = true;
    public bool rename = true;
    public GameObject prefab;
    public Dimension[] dimensions;
    public Vector3 origin = Vector3.zero;

    private List<GameObject> elements = new List<GameObject>(); 
    
    public GameObject[] userElements = null; 
    //] "prefab" would be ignored if this field is defined

    public void Start() {
        //Init(); 
        //] since Init() would reset user's settings, we should 
        //  not call it at Start()
        Debug.Log(dimensions.Length);
        Place();
    }
    public void Update() {
        if(liveUpdate) {
            Place();
        }
    }

    private void Init() {
        dimensions = new Dimension[1];
        dimensions[0] = new Dimension(1, Vector3.zero);
    }

    public void Place() {
        if(userElements != null && userElements.Length > 0) {
            if(CalculateSize() != userElements.Length) {
                Debug.LogWarning("size mismatch!");
            }
            Place(userElements, dimensions, origin, rename);
        } else {
            AdjustSize(CalculateSize());
            GameObject[] targets = elements.ToArray();
            Place(targets, dimensions, origin, rename);
        }
    }

    private int CalculateSize() {
        int size = 1;
        for(int i=0; i<dimensions.Length; i++) {
            size *= dimensions[i].length;
        }
        return size;
    }

    private void AdjustSize(int size) {
        if(size < 0) return;
        if(elements.Count == size) return;
        while(size > elements.Count) {
            elements.Add(KZUtil.Instantiate(prefab));
        }
        while(size < elements.Count) {
            int last = elements.Count - 1;
            GameObject o = elements[last];
            elements.RemoveAt(last);
            GameObject.Destroy(o);
        }
    }

    public static void Place(
            GameObject[] targets, 
            Dimension[] dim, 
            Vector3 origin, 
            bool rename) {
        if(targets.Length <= 0) return; 

        int k = 0;
        GameObject first = targets[k];
        first.transform.position = origin;
        if(rename) first.name = k.ToString();
        if(targets.Length == 1) return;

        k++;
        List<GameObject> group = new List<GameObject>();
        group.Add(first);
        for(int d=0; d<dim.Length; d++) { 
            int s = dim[d].length;
            //Debug.Log(d+"D : "+s);
            List<GameObject> newGroup = new List<GameObject>();

            for(int i=1; i<s; i++) {
                for(int j=0; j<group.Count; j++) {
                    GameObject o = targets[k];
                    if(rename) o.name = k.ToString();
                    o.transform.position = 
                            group[j].transform.position + dim[d].step * i;
                    //Debug.Log("placing element "+k+" at "+o.transform.position);
                    k++;
                    if(k == targets.Length) return; 
                    //] we have run out of elements

                    newGroup.Add(o);
                }
            }
            group.AddRange(newGroup);
        }
    }

    //[ mark as serializable so that the type can be shown in inspector
    [System.Serializable] 
    public class Dimension {
        public int length;
        public Vector3 step;
        public Dimension(int len, Vector3 s) {
            length = len;
            step = s;
        }
    }

}
