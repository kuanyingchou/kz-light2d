﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KZLight : MonoBehaviour {
    public bool debug = true;
    public bool dynamicUpdate = false;

    [Range(-180, 180)] 
    public float direction = 0; 
    [Range(0, 180)] 
    public float angleOfView = 60;
    public Color color = new Color(255, 255, 255, 32);
    private Color oldColor;
    [Range(1, 1000)]
    public float range = 10; 
    public int numberOfRays = 500;
    public Material lightMaterial;

    public GameObject[] targets; 

    [Range(0, 1)]
    public float alpha = .5f;

    //[Range(0.5f, 1.5f)]
    //public float scale = 1.01f;

    [Range(1, 50)]
    public int numberOfDuplicates = 1;
    private int oldNumberOfDuplicates;

    [Range(0, 5)]
    public float duplicateDiff = .5f;
    public float duplicateZDiff = .1f;

    //[ private 
    private static int TEXTURE_SIZE = 32;
    private static float TWO_PI = Mathf.PI * 2;
    private static string DEFAULT_SHADER = 
            //"Unlit/Transparent";
            //"Custom/TransparentSingleColorShader";
            //"Particles/Additive";
            "Somian/Unlit/Transparent";
    private static float TEXTURE_SCALE = .98f;
    private Mesh[] mesh;
    private GameObject[] light;
    private List<RaycastHit> hits = new List<RaycastHit>();
    private MeshStrategy meshStrategy = new SeparateStrategy();
    //] 

    public void Start() {
        if(lightMaterial == null) {
            lightMaterial = CreateMaterial();
        }
        UpdateProperties();
        //Debug.Log(Mathf.PI);
        //if(debug) UnitTest();
    }
    public void LateUpdate() {
        if(dynamicUpdate) UpdateProperties();
        for(int i=0; i<numberOfDuplicates; i++) {
            Vector3 pos = light[i].transform.position;
            List<RaycastHit> hits = Shed(pos, direction, angleOfView);
            UpdateLightMesh(mesh[i], pos, hits);
        }
    }

    //[ private

    private Material CreateMaterial() {
        Material material = new Material(Shader.Find(DEFAULT_SHADER));
        material.mainTexture = CreateTexture();
        return material;
    }

    private Texture2D CreateTexture() {
        Texture2D texture = new Texture2D(
                TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false);
        for(int x=0; x<texture.width; x++) {
            for(int y=0; y<texture.height; y++) {
                float a = alpha - ((float)y/TEXTURE_SIZE * alpha);
                texture.SetPixel(x, y, new Color(color.r, color.g, color.b, a));
                //texture.SetPixel(x, y, Color.green);
            }
        }
        texture.Apply();
        return texture;
    }

    private void UpdateProperties() {
        if(oldNumberOfDuplicates != numberOfDuplicates) {
            Initialize();
        }
        /*
        if(oldColor != color) {
            //lightMaterial.color = color;
            lightMaterial = CreateMaterial();
            oldColor = color;
        }
        */
        SetLightPositions();
    }

    private void Initialize() {
        if(light != null) {
            for(int i=0; i<light.Length; i++) {
                GameObject.DestroyImmediate(light[i]);
            }
        }
        light = new GameObject[numberOfDuplicates];
        mesh = new Mesh[numberOfDuplicates];
        for(int i=0; i<numberOfDuplicates; i++) {
            light[i] = new GameObject();
            light[i].name = "Light-"+i;
            light[i].transform.parent = transform;

            MeshRenderer renderer = light[i].AddComponent<MeshRenderer>();
            renderer.material = lightMaterial;

            MeshFilter filter = light[i].AddComponent<MeshFilter>();
            mesh[i] = filter.mesh;
            mesh[i].MarkDynamic();
        }
        oldNumberOfDuplicates = numberOfDuplicates;
    }
    
    private void SetLightPositions() {
        if(numberOfDuplicates == 1) {
            light[0].transform.localPosition = Vector3.zero;
        } else {
            PlaceLightsInCircle();
        } 
    }

    private void PlaceLightsInCircle() {
        float angle = 0;
        for(int i=0; i<numberOfDuplicates; i++) {
            Vector3 diff = new Vector3(
                    Mathf.Cos(angle), 
                    Mathf.Sin(angle), 
                    transform.position.z + duplicateZDiff * i) * 
                    duplicateDiff;
            light[i].transform.localPosition = diff;
            angle += TWO_PI / numberOfDuplicates;
        }
    }

    private List<RaycastHit> Shed(
            Vector3 lightSource, float angleDeg, float viewDeg) {
        hits.Clear();

        RaycastHit hit;

        float angleRad = angleDeg * Mathf.Deg2Rad;
        float viewRad = viewDeg * Mathf.Deg2Rad;
        float start = angleRad - viewRad * .5f;
        float end = angleRad + viewRad * .5f;

        float angle = start;
        for(int i=0; i<numberOfRays; i++) {
            Vector3 d= new Vector3(
                    Mathf.Cos(angle), 
                    Mathf.Sin(angle), 
                    0);
            if(Physics.Raycast(lightSource, d, out hit, range)) {
                hits.Add(hit);
            } else {
                RaycastHit h = new RaycastHit();
                h.point = lightSource + d*range;
                h.distance = range;
                hits.Add(h);
            }
            angle += viewRad / numberOfRays;
        }

        //hits = SimplifyHits(hits); 

        if(debug) DrawHits(lightSource, hits);

        hits.Reverse(); //TODO: remove this

        return hits;
    }

    private void DrawHits(Vector3 lightSource, List<RaycastHit> hits) {
        for(int i=0; i<hits.Count; i++) {
            Debug.DrawRay(lightSource, hits[i].point - lightSource, Color.green);
        }
    }

    //>>> didn't see much improvement, just moved burden from gpu to cpu
    private List<RaycastHit> SimplifyHits(List<RaycastHit> hits) {
        List<RaycastHit> reducedHits = new List<RaycastHit>();
        if(hits.Count > 2) {
            reducedHits.Add(hits[0]);
            reducedHits.Add(hits[1]);
            Vector3 last = hits[1].point - hits[0].point;
            for(int i=2; i<hits.Count; i++) {
                Vector3 diff = hits[i].point - hits[i-1].point;
                if(Similar(Vector3.Angle(diff, last), 0, 0.001f)) {
                    reducedHits.RemoveAt(reducedHits.Count - 1);
                }
                reducedHits.Add(hits[i]);
                last = diff;
            }
        }
        return reducedHits;
    }

    private void UpdateLightMesh(
            Mesh mesh, Vector3 pos, List<RaycastHit> hits) {
//Debug.DrawRay(Camera.main.transform.position, lightSource - Camera.main.transform.position, Color.red);
        if(hits.Count <= 0) {
            Debug.Log("hits nothing!");
            return;
        }

        
        if(debug) {
            DrawLightPolygon(hits);
        }

        mesh.Clear();
        Vector3[] vertices = meshStrategy.CreateVertices(hits, pos);
        mesh.vertices = vertices;
        mesh.triangles = meshStrategy.CreateTriangles(vertices);
        mesh.normals = meshStrategy.CreateNormals(vertices);
        mesh.uv = meshStrategy.CreateUV(vertices, hits, range);

        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private void DrawLightPolygon(List<RaycastHit> hits) {
        Vector3 from = hits[0].point;
        for(int i=1; i<hits.Count; i++) {
            Vector3 to = hits[i].point;
            //Debug.Log(x);
            Debug.DrawLine(from, to, Color.red);
            from = to;
        }
        //if(hits.Count > 1) {
        //    Debug.DrawLine(hits[hits.Count -1], hits[0], Color.red);
        //}
    }

    interface MeshStrategy {
        Vector3[] CreateVertices(List<RaycastHit> hits, Vector3 pos);
        int[] CreateTriangles(Vector3[] vertices); 
        Vector3[] CreateNormals(Vector3[] vertices);
        Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range);
    }
    class SeparateStrategy : MeshStrategy {
        public Vector3[] CreateVertices(List<RaycastHit> hits, Vector3 pos) {
            int numTriangles = hits.Count - 1;
            Vector3[] vertices = new Vector3[numTriangles * 3];
            int p = 0;
            int index = 0;
            for(int i=0; i<numTriangles; i++) {
                vertices[index++] = Vector3.zero;
                vertices[index++] = hits[p++].point - pos;
                vertices[index++] = hits[p].point - pos;
            }
            return vertices;
        }
        public int[] CreateTriangles(Vector3[] vertices) {
            int[] triangles = new int[vertices.Length];
            for(int i=0; i<triangles.Length; i++) {
                triangles[i] = i;
            }
            return triangles;
        }
        public Vector3[] CreateNormals(Vector3[] vertices) {
            Vector3[] normals = new Vector3[vertices.Length];
            for(int i=0; i<normals.Length; i++) {
                normals[i] = -Vector3.forward;
            }
            return normals;
        }
        public Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range) {
            Vector2[] uvs = new Vector2[vertices.Length];
            float x = 1;
            int index = 0;
            int hitIndex = 1;
            float span = uvs.Length / 3; 
            float y = hits[0].distance * TEXTURE_SCALE / range;
            while(index < uvs.Length) {
                uvs[index++] = new Vector2(x, 0);
                uvs[index++] = new Vector2(x, y);
                x -= 1f / span;
                y = hits[hitIndex++].distance * TEXTURE_SCALE / range;
                //if(x < 0) Debug.Log("!!! x = "+x);
                uvs[index++] = new Vector2(x, y);
            }
            return uvs;
        }
    }

    class CombineStrategy : MeshStrategy {
        public Vector3[] CreateVertices(List<RaycastHit> hits, Vector3 pos) {
            Vector3[] vertices = new Vector3[hits.Count + 1];
            vertices[0] = Vector3.zero;
            for(int i=1; i<vertices.Length; i++) {
                vertices[i] = hits[i-1].point - pos;
            }
            return vertices;
        }
        public int[] CreateTriangles(Vector3[] vertices) {
            int numTriangles = vertices.Length - 2;
            int index = 0;
            int p = 1, q = 2;
            int[] triangles = new int[numTriangles * 3];
            for(int i=0; i<numTriangles; i++) {
                triangles[index++] = 0;
                triangles[index++] = p++;
                triangles[index++] = q++;
                //if(q >= vertices.Length) q = 1;
            }
            return triangles;
        }

        public Vector3[] CreateNormals(Vector3[] vertices) {
            Vector3[] normals = new Vector3[vertices.Length];
            for(int i=0; i<normals.Length; i++) {
                normals[i] = -Vector3.forward;
            }
            return normals;
        }

        public Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range) {
            Vector2[] uv = new Vector2[vertices.Length];
            uv[0] = Vector2.zero;
            for(int i=1; i<uv.Length; i++) {
                uv[i] = new Vector2(
                        hits[i-1].distance / range, 0);
            }
            return uv;
        }
    }



    //[ utilities
    private float GetAngle(Vector3 dir) {
        return Mathf.Atan2(dir.y, dir.x);
    }

    private bool Similar(float a, float b, float err) {
        return Mathf.Abs(a - b) < err;
    }
/*
    private bool Similar(Vector3 a, Vector3 b, Vector3 lightSource, float err) {
        return Vector3.Angle(a - lightSource, b - lightSource) < err;
    }

    private Mesh GetMesh(GameObject o) {
        return o.GetComponent<MeshFilter>().mesh;
    }

    private int CompareAngle(float a, float b) {
        return (a - b)>0?1:-1;
    }

    private bool IsVisible(Vector3 p, Vector3 dir, float range) {
        float a = Vector3.Angle(p, dir);
        //if(a < 0) Debug.LogError(a);
        return a < range * .5f + 0.001f;
    }


    private bool IsWithinAngles(float a, float start, float range, float err) {
        a = GetValidRad(a);
        start = GetValidRad(start);
        if(a > start - err && a < start + range + err) return true;
        else return false;
    }

    // return a in -Mathf.PI to Mathf.PI
    private float GetValidRad(float a) {
        float res;
        if(a > Mathf.PI) {
            res = a % TWO_PI - TWO_PI;
        } else if(a < -Mathf.PI){
            res = a % TWO_PI + TWO_PI;
        } else {
            res = a;
        }
        if(res < -Mathf.PI || res > Mathf.PI) Debug.LogError(res + " !!!");
        return res;
    }

    // return a in 0 to 2 * Mathf.PI
    private float GetRadIn2PI(float a) {
        float res;
        if(a > TWO_PI) {
            res = a % TWO_PI;
        } else if(a < 0){
            res = a % TWO_PI + TWO_PI;
        } else {
            res = a;
        }
        if(res < 0 || res > TWO_PI) Debug.LogError(res + " !!!");
        return res;
    }
    //[ tests
    private void TestIsWithinAngles() {
        for(float start = -Mathf.PI; start < Mathf.PI; start += Mathf.PI / 180) {
            float end = start + Mathf.PI / 2;
            for(float angle = start; angle < end; angle+=(Mathf.PI / 180)) {
                bool isWithin = IsWithinAngles(angle, start, end, 0.01f);
                if(!isWithin) Debug.LogError(string.Format(
                        "angle = {0}, start={1}, end={2} => {3}",
                        angle * Mathf.Rad2Deg,
                        start * Mathf.Rad2Deg,
                        end * Mathf.Rad2Deg,
                        isWithin));
            }
        }

    }
    private void UnitTest() {
        TestIsWithinAngles();
    }
*/
}
