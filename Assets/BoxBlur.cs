using UnityEngine;
using System.Collections;

public class BoxBlur {
    //private static float intensity = 0.111f;
    //private static float[] matrix = {
    //    0.0625f, 0.0625f, 0.0625f, 
    //    0.0625f, 0.5f, 0.0625f, 
    //    0.0625f, 0.0625f, 0.0625f, 
    //};
    private static float[] matrix = {
        0.111f, 0.111f, 0.111f, 
        0.111f, 0.111f, 0.111f, 
        0.111f, 0.111f, 0.111f, 
    };
    private static Color transparent = new Color(1, 1, 1, 0);

    public static Texture2D Blur(Texture2D texture) {
        Texture2D buffer = new Texture2D(
                texture.width, texture.height, texture.format, false);
        for(int x=0; x<texture.width; x++) {
            for(int y=0; y<texture.height; y++) {
                BlurPixel(texture, buffer, x, y);
            }
        }
        buffer.Apply();
        return buffer;
    }

    private static void BlurPixel(
            Texture2D texture, Texture2D buffer, int x, int y) {

        Color res = new Color(0, 0, 0, 0);
        int index = 0;
        for(int r = -1; r <= 1; r++) {
            for(int s = -1; s <= 1; s++) {
                Color c = BoxBlur.Mul(
                        BoxBlur.GetPixel(texture, x+s, y+r),
                        matrix[index++]);
                res.r += c.r; 
                res.g += c.g; 
                res.b += c.b; 
                res.a += c.a; 
            }
        }
        //Debug.Log(res);
        buffer.SetPixel(x, y, res);
    }

    private static Color Add(params Color[] colors) {
        float r = 0, g = 0, b = 0, a = 0;
        for(int i=0; i<colors.Length; i++) {
            r += colors[i].r;
            g += colors[i].g;
            b += colors[i].b;
            a += colors[i].a;
        }
        return new Color(r, g, b, a);
    }
    private static Color Add(Color lhs, Color rhs) {
        return new Color(
                Mathf.Min(1, lhs.r + rhs.r), 
                Mathf.Min(1, lhs.g + rhs.g), 
                Mathf.Min(1, lhs.b + rhs.b), 
                Mathf.Min(1, lhs.a + rhs.a));
    }

    private static Color Mul(Color input, float f) {
        return new Color(
                input.r * f, input.g * f, input.b * f, input.a * f);
    }

    private static Color GetPixel(Texture2D texture, int x, int y) {
        if(x < 0 || x >= texture.width ||
           y < 0 || y >= texture.height) return transparent;
        return texture.GetPixel(x, y);
    }
}
