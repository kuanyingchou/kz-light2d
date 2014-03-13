using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KZLight : MonoBehaviour {
    public float distance = 10;
    public GameObject[] targets;
    public Vector3 lightPosition;
    public float angle = 0, view = 60;
    public bool debug = true;
    private Mesh mesh;
    private GameObject light;
    private List<Vector3> hits = new List<Vector3>();
    private Vector3 lastHit;

    public void Start() {
        light = GameObject.CreatePrimitive(PrimitiveType.Quad);
        light.name = "Light";
        Material lightMaterial = 
                Resources.Load("Light", typeof(Material)) as Material;
        light.renderer.material = lightMaterial;
        mesh = light.GetComponent<MeshFilter>().mesh;
        mesh.MarkDynamic();
    }
    public void Update() {
        light.transform.position = lightPosition;
        Vector3 source = light.transform.position;
        List<Vector3> hits = Shoot(source, angle, view);
        UpdateLightMesh(source, hits, mesh);
        //FindShadow();
    }
    public void FixedUpdate() {
    }

    public void UpdateLightMesh(Vector3 source, List<Vector3> hits, Mesh mesh) {
//Debug.DrawRay(Camera.main.transform.position, source - Camera.main.transform.position, Color.red);
        if(hits.Count <= 0) {
            Debug.Log("what?");
            return;
        }
        if(debug) {
            Vector3 from = hits[0];
            for(int i=1; i<hits.Count; i++) {            
                Vector3 to = hits[i];
                //Debug.Log(x);
                Debug.DrawLine(from, to, Color.red);
                from = to;
            }
            if(hits.Count > 1) {
                Debug.DrawLine(hits[hits.Count -1], hits[0], Color.red);
            }
        }

        //[ vertices
        Vector3[] vertices = new Vector3[hits.Count + 1];
        vertices[0] = Vector3.zero;
        for(int i=1; i<vertices.Length; i++) {
            vertices[i] = hits[i-1] - source;
        }
        mesh.Clear();
        //Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        //[ triangles
        int numTriangles = hits.Count;
        int index = 0;
        int p = 1, q = 2;
        int[] triangles = new int[hits.Count * 3];
        for(int i=0; i<numTriangles; i++) {
            triangles[index++] = 0;
            triangles[index++] = p++;
            triangles[index++] = q++;
            if(q >= vertices.Length) q = 1;
        }
        mesh.triangles = triangles;

/*
for(int i=0; i<triangles.Length; i++) {
    Debug.Log(triangles[i]);
}
*/

        Vector3[] normals = new Vector3[vertices.Length];
        for(int i=0; i<normals.Length; i++) {
            normals[i] = -Vector3.forward;
        }
        mesh.normals = normals;

        Vector2[] uvs = new Vector2[vertices.Length];
        Bounds bounds = mesh.bounds;
        for(int i=0; i<uvs.Length; i++) {
            uvs[i] = new Vector2(
                    (bounds.size.x - vertices[i].x) / bounds.size.x,
                    (bounds.size.y - vertices[i].y) / bounds.size.y);
            //uvs[i] = Vector2.zero;
        }
        mesh.uv = uvs;
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    public List<Vector3> Shoot(Vector3 source, float angleDeg, float viewDeg) {
    /*
        float angle = angleDeg * Mathf.Deg2Rad;
        float view = viewDeg * Mathf.Deg2Rad;
        float start = angle - view * .5f;
        float end = angle + view * .5f;
    */
    
        ClearHits();
        RaycastHit hit;

        for(int i = 0; i<targets.Length; i++) {
            GameObject o = targets[i];
            Transform t = o.transform;
            Collider c = o.collider;
            Mesh mesh = GetMesh(o);
            Vector3[] vertices = mesh.vertices;
            for(int j=0; j<vertices.Length; j++) {
                Vector3 v = t.TransformPoint(vertices[j] * 1.001f);
                Vector3 v2 = new Vector3(v.x, v.y, source.z);
                if(Physics.Raycast(source, v2 - source, out hit, distance)) {
if(debug) Debug.DrawRay(source, hit.point - source, Color.green);
                    AddHit(hit.point);
                }
                v = o.transform.TransformPoint(vertices[j] * 0.999f);
                v2 = new Vector3(v.x, v.y, source.z);
                if(Physics.Raycast(source, v2 - source, out hit, distance)) {
if(debug) Debug.DrawRay(source, hit.point - source, Color.green);
                    AddHit(hit.point);
                }
            }
        }

/*
        Vector3 startDir = new Vector3(
                Mathf.Cos(start), Mathf.Sin(start), 0).normalized;
        if(Physics.Raycast(source, startDir, out hit, distance)) {
            hits.Add(hit.point - source);
        } else {
            hits.Add(source + startDir * distance);
        }

        Vector3 endDir = new Vector3(
                Mathf.Cos(end), Mathf.Sin(end), 0).normalized;
        if(Physics.Raycast(source, endDir, out hit, distance)) {
            hits.Add(hit.point - source);
        } else {
            hits.Add(source + endDir * distance);
        }

        //hits.RemoveAll(x => !IsVisible(x - source, start, end));
*/
        //hits = hits.Distinct().ToList();
        //hits.Sort(
        //        (a, b) => 
        //        (GetAngle(a - source) - GetAngle(b - source)) >= 0? 1: -1) ;
        hits = hits.OrderBy(x => -GetAngle(x - source)).ToList();

        return hits;
    }

    private void ClearHits() {
        lastHit = Vector3.zero;
        hits.Clear();
    }
    private void AddHit(Vector3 h) {
        if(! Similar(h, lastHit, lightPosition, 0.001f)) {
            hits.Add(h);
            lastHit = h;
        } else {
            //Debug.Log("omit "+h);
        }
    }
    private bool Similar(Vector3 a, Vector3 b, Vector3 source, float err) {
        return Vector3.Angle(a - source, b - source) < err;
    }

    private Mesh GetMesh(GameObject o) {
        return o.GetComponent<MeshFilter>().mesh;
    }


    public void FindShadow() {
        Vector3 source = transform.position;
        for(int b = 0; b<targets.Length; b++) {
            GameObject o = targets[b];
            Mesh mesh = o.GetComponent<MeshCollider>().sharedMesh;
            Vector3[] vertices = mesh.vertices;
            for(int i=0; i<vertices.Length; i++) {
                Vector3 v = o.transform.TransformPoint(vertices[i] * 1.0001f);
                Vector3 v2 = new Vector3(v.x, v.y, source.z);
                RaycastHit hit;
                if(Physics.Raycast(v2, v2 - source, out hit, distance)) {
                    Debug.DrawRay(v2, hit.point - v2, Color.green);
                }
                //Vector3 cp = Camera.main.transform.position;
                //Debug.DrawRay(cp, hit.point - cp, Color.green);
            }
        }
    }
    public float GetAngle(Vector3 dir) {
        float angle = Mathf.Atan2(dir.y, dir.x);
        //Debug.Log(angle);
        return angle;
    }
    public bool IsVisible(Vector3 p, float startAngle, float endAngle) {
        float angle = Mathf.Atan2(p.y, p.x);
        if(angle >= startAngle && angle < endAngle) return true;
        else return false;
    }


}
