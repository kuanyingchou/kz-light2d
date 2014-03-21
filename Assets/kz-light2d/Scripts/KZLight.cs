using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KZLight : MonoBehaviour {
    public bool debug = true;

    //[ basic properties
    /*[Range(-180, 180)]*/ public float direction = 0; 
    /*[Range(0, 720)]*/ public float angleOfView = 90;
    /*[Range(1, 20)]*/ public float radius = 10; 
    
    public Material lightMaterial;
    public Color color = new Color(1, 1, 1, 1); 
    private Color oldColor; //used for live update
    public Color tint = new Color(1, .94f, .59f, 1);
    /*[Range(0, 1)]*/ public float alpha = .5f;
    public float shadowBrightness = 1;

    public bool enableTint = false;
    public bool enableFallOff = true;
    public bool enableSoftEdges = true;
    public bool enableShadowTexture = true;

    public bool enablePerlin = false;
    public float perlinScale = 5;
    public float perlinStart = 5;

    //[ advanced properties
    public int textureWidth = 128;
    public int textureHeight = 128;
    public int numberOfRays = 128;
    public int iteration = 0;
    public int edgeCutout = 2; //for blurry edges
    public float overflow= 0.05f;

    //public int eventThreshold = 5; //: TODO

    /*[Range(1, 10)]*/ public int numberOfDuplicates = 1;
    private int oldNumberOfDuplicates;

    /*[Range(0, 5)]*/ public float duplicateDiff = .5f;
    public float duplicateZDiff = .1f;

    //[ private 
    private bool dynamicUpdate = true;
    private static float TWO_PI = Mathf.PI * 2;
    private static string DEFAULT_SHADER = 
            //"Diffuse";
            //"Unlit/Transparent";
            //"Custom/TransparentSingleColorShader";
            //"Particles/Additive";
            "Mobile/Particles/Additive";
            //"Somian/Unlit/Transparent";
    private Mesh[] meshes;
    private GameObject[] lights;
    private List<RaycastHit> hits = new List<RaycastHit>();
    private MeshStrategy meshStrategy = 
            //new SeparateStrategy();
            new SeparateTextureStrategy();
            //new CombineStrategy();
    private KZTexture texture;
    private Texture2D texture2d;
    private Dictionary<GameObject, int> seenObjects= 
            new Dictionary<GameObject, int>();
    private Dictionary<GameObject, int> lastSeenObjects= 
            new Dictionary<GameObject, int>();

    public void Start() {
        if(lightMaterial == null) {
            //lightMaterial = CreateMaterial();
            Debug.LogError("Please assign a material!");
            return;
        }
        lightMaterial = CreateMaterial(lightMaterial);
        UpdateProperties();
        
        //if(debug) UnitTest();
    }

    public void LateUpdate() {
        if(lightMaterial == null) return;

        if(dynamicUpdate) UpdateProperties();

        SetLightPositions();

        for(int i=0; i<numberOfDuplicates; i++) {
            Vector3 pos = lights[i].transform.position;
            List<RaycastHit> hits = 
                    Scan(pos, direction, angleOfView, radius);
            UpdateLightMesh(meshes[i], pos, hits);
            lightMaterial.mainTexture = CreateTexture(hits);
        }
    }

    //[ private

    private static Material CreateMaterial(Material m) {
        //return m;
        //Material material = (Material)GameObject.Instantiate(m);
        Material material = new Material(m);
        return material;
    }

    private Texture2D CreateTexture(List<RaycastHit> hits) {
        //[ use a smaller width here to create a blurry effect 
        if(enableTint) ApplyColorWithTint(texture, color, tint, alpha);
        else ApplyColor(texture, color, alpha);
        if(enableSoftEdges) ApplySoftEdges(texture, edgeCutout);
        if(enableFallOff) ApplyGradient(texture, alpha);
        if(enablePerlin) ApplyPerlin(texture, perlinStart, perlinScale);
        if(enableShadowTexture) {
            ApplyShadow(texture, hits, radius, overflow, shadowBrightness);
        }

        for(int i=0; i<iteration; i++) {
            texture = KZTexture.BoxBlur(texture);
        }

        texture2d = texture.ToTexture2D(texture2d);
        return texture2d;
    }

    private static void ApplyColor(
            KZTexture texture, Color c, float alphaScale) {
        texture.Clear(new Color(c.r, c.g, c.b, c.a * alphaScale));
    }

    private static void ApplyColorWithTint(
            KZTexture texture, Color c, Color tint, float alphaScale) {
        for(int y=0; y<texture.height; y++) {
            Color t = KZTexture.GetTint(
                    c, tint, (float)y / (texture.height-1));
            for(int x=0; x<texture.width; x++) {
                texture.SetPixel(x, y, 
                        new Color(t.r, t.g, t.b, t.a * alphaScale));
            }
        }
                
    }

    private static void ApplySoftEdges(
            KZTexture texture, int numOfPixels) {
        for(int y=0; y<texture.height; y++) {
            Color color = KZTexture.GetColor(texture.GetPixel(0, y), 0);
            for(int i=0; i<numOfPixels; i++) {
                texture.SetPixel(i, y, color);
                texture.SetPixel(texture.width - 1 - i, y, color);
            }
        }
    }

    private static void ApplyGradient(KZTexture texture, float maxAlpha) {
        for(int y=0; y<texture.height; y++) {
            float a = maxAlpha - ((float)y/(texture.height-1) * maxAlpha);
            for(int x=0; x<texture.width; x++) {
                Color c = texture.GetPixel(x, y);
                texture.SetPixel(x, y, new Color(c.r, c.g, c.b, c.a * a));
            }
        }
    }
    private static void ApplyPerlin(
            KZTexture texture, float perlinStart, float perlinScale) {
        for(int x=0; x<texture.width; x++) {
            float perlin = Mathf.PerlinNoise(
                        perlinStart +
                        (float)x / texture.width * perlinScale, 0);
            for(int y=0; y<texture.height; y++) {
                //Debug.Log(perlin);
                Color c = texture.GetPixel(x, y);
                texture.SetPixel(x, y, new Color(
                        c.r, c.g, c.b, Mathf.Min(1, perlin * c.a)));
            }
        }
    }

    private void UpdateProperties() {
        if(IsDirty()) {
            Initialize();
        }
    }

    private bool IsDirty() {
        return oldNumberOfDuplicates != numberOfDuplicates;
    }

    private void Initialize() {
        if(lights != null) {
            for(int i=0; i<lights.Length; i++) {
                GameObject.DestroyImmediate(lights[i]);
            }
        }
        lights = new GameObject[numberOfDuplicates];
        meshes = new Mesh[numberOfDuplicates];
        for(int i=0; i<numberOfDuplicates; i++) {
            lights[i] = new GameObject();
            lights[i].name = "Light-"+i;
            lights[i].transform.parent = transform;

            MeshRenderer renderer = lights[i].AddComponent<MeshRenderer>();
            renderer.material = lightMaterial;

            MeshFilter filter = lights[i].AddComponent<MeshFilter>();
            meshes[i] = filter.mesh;
            //meshes[i].MarkDynamic();
        }
        oldNumberOfDuplicates = numberOfDuplicates;

        texture = new KZTexture(textureWidth, textureHeight);
        texture2d = new Texture2D(textureWidth, textureHeight, 
                TextureFormat.ARGB32, false);
                //TextureFormat.RGB24, false);
        texture2d.wrapMode = TextureWrapMode.Clamp;
    }
    
    private void SetLightPositions() {
        if(numberOfDuplicates == 1) {
            lights[0].transform.localPosition = Vector3.zero;
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
                    0) * 
                    duplicateDiff;
            diff += new Vector3(0, 0, 
                    transform.position.z + duplicateZDiff * i);
            lights[i].transform.localPosition = diff;
            angle += TWO_PI / numberOfDuplicates;
        }
    }

    private List<RaycastHit> Scan(
            Vector3 lightSource, 
            float angleDeg, float viewDeg, float range) {

        hits.Clear();
        RaycastHit hit;

        float angleRad = angleDeg * Mathf.Deg2Rad;
        float viewRad = viewDeg * Mathf.Deg2Rad;
        float start = angleRad - viewRad * .5f;
        //float end = angleRad + viewRad * .5f;

        float angle = start;
        for(int i=0; i<numberOfRays; i++) {
            Vector3 d= ToVector3(angle); 
            if(Physics.Raycast(lightSource, d, out hit, 
                    Mathf.Abs(range))) {
                hits.Add(hit);
                Track(hit.transform.gameObject);
            } else {
                RaycastHit h = new RaycastHit();
                h.point = lightSource + d*range;
                h.distance = range;
                hits.Add(h);
            }
            angle += viewRad / numberOfRays;
        }

        //TODO: block check
        //hits = SimplifyHits(hits); 
        HandleEvents();
        if(debug) DrawHits(lightSource, hits);
        //hits.Reverse(); //TODO: remove this
        return hits;
    }

    private static Vector3 ToVector3(float angle) {
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    }

    private void Track(GameObject obj) {
        if(seenObjects.ContainsKey(obj)) {
            int count = seenObjects[obj];
            seenObjects[obj] = count + 1;
        } else {
            seenObjects.Add(obj, 1);
        }
    }

    private void SendLeave(GameObject obj) {
        if(obj) obj.SendMessage("LeaveLight", 
                SendMessageOptions.DontRequireReceiver);
    }
    private void SendEnter(GameObject obj) {
        if(obj) obj.SendMessage("EnterLight", 
                SendMessageOptions.DontRequireReceiver);
    }
    private void HandleEvents() {
        foreach(var obj in seenObjects.Keys) {
            if(!lastSeenObjects.ContainsKey(obj)) {
                SendEnter(obj);
            }
        }
        foreach(var obj in lastSeenObjects.Keys) {
            if(!seenObjects.ContainsKey(obj)) {
                SendLeave(obj);
            }
        }
        Dictionary<GameObject, int> temp = lastSeenObjects;
        lastSeenObjects = seenObjects;
        seenObjects = temp;
        seenObjects.Clear();
    }

    private void DrawHits(Vector3 lightSource, List<RaycastHit> hits) {
        for(int i=0; i<hits.Count; i++) {
            Debug.DrawRay(lightSource, 
                    hits[i].point - lightSource, Color.green);
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
            mesh.Clear();
            return;
        }

        
        if(debug) {
            DrawLightPolygon(hits);
        }

        mesh.Clear();
        Vector3[] vertices = meshStrategy.CreateVertices(
                hits, pos, direction, angleOfView, radius);
        mesh.vertices = vertices;
        mesh.triangles = meshStrategy.CreateTriangles(vertices);
        mesh.normals = meshStrategy.CreateNormals(vertices);
        mesh.uv = meshStrategy.CreateUV(vertices, hits, radius);

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
    private static void ApplyShadow(
            KZTexture texture, List<RaycastHit> hits, 
            float range, float overflow, float brightness) {
        if(hits.Count == 0) return;        
        for(int x=0; x<texture.width; x++) {
            int hitIndex = 
                    Mathf.RoundToInt(
                        Mathf.Min(
                            1,
                            (float)x / (texture.width-1) 
                        ) * (hits.Count - 1)
                    );
            int hy = 
                    Mathf.RoundToInt(
                        Mathf.Min(
                            1,
                            (hits[hitIndex].distance + overflow) / range
                        ) * (texture.height - 1)
                    );
            for(int i=hy; i<texture.height; i++) {
                int xIndex = texture.width - 1 - x;
                Color original = texture.GetPixel(xIndex, i);
                //Color shadowColor = KZTexture.GetColor(original, 0);
                Color shadowColor = 
                        //Color.black
                        KZTexture.GetColor(
                            original, 
                            original.a * (i/(texture.height-1f) * brightness)
                        );
                texture.SetPixel(xIndex, i, shadowColor);
            }
        }
    }

    interface MeshStrategy {
        Vector3[] CreateVertices(List<RaycastHit> hits, Vector3 pos,
                float direction, float angleOfView, float range);
        int[] CreateTriangles(Vector3[] vertices); 
        Vector3[] CreateNormals(Vector3[] vertices);
        Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range);
    }
    class SeparateStrategy : MeshStrategy {
        public virtual Vector3[] CreateVertices(
                List<RaycastHit> hits, Vector3 pos, 
                float direction, float angleOfView, float range) {
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
        public virtual Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range) {
            Vector2[] uvs = new Vector2[vertices.Length];
            float x = 1;
            int index = 0;
            int hitIndex = 1;
            //float span = uvs.Length / 3; 
            float y = hits[0].distance / range;
            while(index < uvs.Length) {
                uvs[index++] = new Vector2(x, 0);
                uvs[index++] = new Vector2(x, y);
                x -= 1f / uvs.Length;
                y = hits[hitIndex++].distance / range;
                //if(x < 0) Debug.Log("!!! x = "+x);
                uvs[index++] = new Vector2(x, y);
            }
            return uvs;
        }
    }

    class SeparateTextureStrategy : SeparateStrategy {

        public override Vector3[] CreateVertices(
                List<RaycastHit> hits, Vector3 pos, 
                float direction, float angleOfView, float range) {

            float angleRad = direction * Mathf.Deg2Rad;
            float viewRad = angleOfView * Mathf.Deg2Rad;
            float start = angleRad - viewRad * .5f;
            //float end = angleRad + viewRad * .5f;

            Vector3[] dests = new Vector3[hits.Count];
            float angle = start;
            for(int i=0; i<dests.Length; i++) {
                Vector3 d= new Vector3(
                        Mathf.Cos(angle), 
                        Mathf.Sin(angle), 
                        0);
                dests[dests.Length - 1 - i] = d * range;
                angle += viewRad / dests.Length;
            }

            int numTriangles = dests.Length - 1;
            Vector3[] vertices = new Vector3[numTriangles * 3];
            int p = 0;
            int index = 0;
            for(int i=0; i<numTriangles; i++) {
                vertices[index++] = Vector3.zero;
                vertices[index++] = dests[p++];
                vertices[index++] = dests[p];
            }
            return vertices;
        }
        public override Vector2[] CreateUV(
                Vector3[] vertices, List<RaycastHit> hits, float range) {
            Vector2[] uvs = new Vector2[vertices.Length];
            int index = 0;
            float span = uvs.Length / 3; 
            float x = 0;
            while(index < uvs.Length) {
                //uvs[index++] = Vector2.zero;
                uvs[index++] = new Vector2(x, 0);
                uvs[index++] = new Vector2(x, 1);
                x += 1f / span;
                //if(x < 0) Debug.Log("!!! x = "+x);
                uvs[index++] = new Vector2(x, 1);
            }
            return uvs;
        }
    }

    class CombineStrategy : MeshStrategy {
        public Vector3[] CreateVertices(List<RaycastHit> hits, Vector3 pos,
                float direction, float angleOfView, float range) {
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

    //[ tests
    private bool IsWithinAngles(
            float a, float start, float range, float err) {
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
    private void TestPressure() {
        for(int i=0; i< 100; i++) {
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.transform.position = new Vector3(i, 0, 0);
        }
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
        TestPressure();
    }

/*
    private bool Similar(Vector3 a, Vector3 b, Vector3 lightSource, float err) {
        return Vector3.Angle(a - lightSource, b - lightSource) < err;
    }

    private Mesh GetMesh(GameObject o) {
        return o.GetComponent<MeshFilter>().meshes;
    }

    private int CompareAngle(float a, float b) {
        return (a - b)>0?1:-1;
    }

    private bool IsVisible(Vector3 p, Vector3 dir, float range) {
        float a = Vector3.Angle(p, dir);
        //if(a < 0) Debug.LogError(a);
        return a < range * .5f + 0.001f;
    }

*/
}
