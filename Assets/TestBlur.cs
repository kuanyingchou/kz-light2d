using UnityEngine;
using System.Collections;

public class TestBlur : MonoBehaviour {
    private static string DEFAULT_SHADER = 
            "Unlit/Transparent";
    public void Start() {
        KZTexture texture = new KZTexture(32, 32);
        int edge = 4;            
        Color transparent = new Color(0, 0, 0, 0);
        for(int y=0; y<texture.height; y++) {
            for(int x=0; x<texture.width; x++) {
                texture.SetPixel(x, y, transparent);
            }
        }
        for(int y=edge; y<texture.height-edge; y++) {
            for(int x=edge; x<texture.width-edge; x++) {
                texture.SetPixel(x, y, Color.white);
            }
        }
        //for(int y=0; y<texture.height / 2; y++) {
        //    for(int x=0; x<texture.width / 2; x++) {
        //        texture.SetPixel(x, y, Color.black);
        //    }
        //}

        for(int i=0;i<1;i++) {
            texture = KZTexture.BoxBlur(texture);
        }

        Material material = new Material(Shader.Find(DEFAULT_SHADER));
        material.mainTexture = texture.ToTexture2D();
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        obj.GetComponent<MeshRenderer>().material = material;
    }
}
