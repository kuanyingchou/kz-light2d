using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KZTexLight : KZLight {
    public bool enableShadow = true;
    public int iteration = 0;
    public float overflow= 0.05f;
    public float shadowBrightness = 1;
    public bool enableSoftEdges = true;
    public int edgeCutout = 1; //for blurry edges

    public override void LateUpdate() {
        base.LateUpdate(); //: may run unnecessary code
        lightMaterial.mainTexture = CreateTexture();
    }
    public override Vector3[] CreateVertices(
            List<RaycastHit> hits, Vector3 pos, 
            float direction, float angleOfView, 
            float range) {

        float angleRad = direction * Mathf.Deg2Rad;
        float viewRad = angleOfView * Mathf.Deg2Rad;
        float start = angleRad - viewRad * .5f;
        float end = angleRad + viewRad * .5f;

        int destIndex= 0;
        List<Vector3> dests = new List<Vector3>();

        float angle = end;
        for(int i=0; i<numberOfRays; i++) {
            Vector3 d= new Vector3(
                    Mathf.Cos(angle), 
                    Mathf.Sin(angle), 
                    0);
            dests.Add(d * range);
            angle -= viewRad / (numberOfRays-1);
        }

        int numTriangles = dests.Count - 1;
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

    public override KZTexture Filter(KZTexture texture) {
        texture = base.Filter(texture);
        if(enableShadow) {
            ApplyShadow(texture, hits, radius, overflow, shadowBrightness);
        }
        if(enableSoftEdges) {
            ApplySoftEdges(texture, edgeCutout);
        }
        for(int i=0; i<iteration; i++) {
            texture = KZTexture.BoxBlur(texture);
        }
        return texture;
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
                Color original = texture.GetPixel(x, i);
                //Color shadowColor = KZTexture.GetColor(original, 0);
                Color shadowColor = 
                        //Color.black
                        KZTexture.GetColor(
                            original, 
                            original.a * (i/(texture.height-1f) * brightness)
                        );
                texture.SetPixel(x, i, shadowColor);
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

}
