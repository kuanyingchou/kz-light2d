﻿using UnityEngine;
using System.Collections;

public class KZTexture {
    public static Color transparent = new Color(1, 1, 1, 0);
    private Color[] pixels; 

    public int width {
        get { return _width; }
    }    
    public int height {
        get { return _height; }
    }
    private int _width;
    private int _height;

    public KZTexture(int w, int h) {
        pixels = new Color[w * h];
        _width = w; _height = h;
    }

    public void Clear() {
        Clear(transparent);
    }
    public void Clear(Color c) {
        for(int i=0; i<pixels.Length; i++) {
            pixels[i] = c;
        }
    }

    public void SetPixel(int x, int y, Color c) {
        pixels[y * width + x] = c;
    }
    public Color GetPixel(int x, int y) {
        return pixels[y * width + x];
    }
    public Color GetPixel(int x, int y, Color defaultColor) {
        if(x < 0 || x >= width ||
           y < 0 || y >= height) return defaultColor;
        return pixels[y * width + x];
    }

    public Texture2D ToTexture2D() {
        Texture2D t2d = new Texture2D(
                width,
                height, 
                TextureFormat.ARGB32, 
                false);
        t2d.SetPixels(pixels);
        t2d.Apply();
        return t2d;
    }
    public Texture2D ToTexture2D(Texture2D t2d) {
        if(t2d.width != width || t2d.height != height) {
            Debug.Log("size mismatch!");
        }
        t2d.SetPixels(pixels);
        t2d.Apply();
        return t2d;
    }

    //private static float intensity = 0.111f;
    //private static float[] matrix = {
    //    0.0625f, 0.0625f, 0.0625f, 
    //    0.0625f, 0.5f, 0.0625f, 
    //    0.0625f, 0.0625f, 0.0625f, 
    //};

    private static float[,] box = {
        { 0.111f, 0.111f, 0.111f}, 
        { 0.111f, 0.111f, 0.111f}, 
        { 0.111f, 0.111f, 0.111f}, 
    };

    private static float[,] linear3 = {
        {1/3f, 1/3f, 1/3f}
    };

    private static float[,] linear5 = {
        {1/5f, 1/5f, 1/5f, 1/5f, 1/5f}
    };

    private static float[,] linear7 = {
        {1/7f, 1/7f, 1/7f, 1/7f, 1/7f, 1/7f, 1/7f}
    };

    public static KZTexture BoxBlur(KZTexture texture) {
        return BoxBlur(texture, linear3);
    }
    public static KZTexture BoxBlur(KZTexture texture, float[,] kernel) {
        KZTexture buffer = new KZTexture(
                texture.width, texture.height);
        for(int x=0; x<texture.width; x++) {
            for(int y=0; y<texture.height; y++) {
                BlurPixel(texture, buffer, x, y, kernel);
            }
        }
        return buffer;
    }

    private static void BlurPixel(
            KZTexture src, KZTexture dest, 
            int x, int y, float[,] kernel) {

        Color color = new Color(0, 0, 0, 0);
        Color defaultColor = GetColor(src.GetPixel(x, y), 0);
        int index = 0;
        int row = kernel.GetLength(0);
        int col = kernel.GetLength(1);
        int halfRow = row / 2;
        int halfCol = col / 2;

        for(int i=0; i<row; i++) {
            for(int j=0; j<col; j++) {
                Color c = Mul(
                        src.GetPixel(x - halfCol + j, 
                                     y - halfRow + i, defaultColor),
                        kernel[i, j]);
                color.r += c.r; 
                color.g += c.g; 
                color.b += c.b; 
                color.a += c.a; 
            }
        }
        //Debug.Log(color);
        dest.SetPixel(x, y, color);
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

/*
    private static Color GetPixel(KZTexture texture, int x, int y) {
        if(x < 0 || x >= texture.width ||
           y < 0 || y >= texture.height) return transparent;
        return texture.GetPixel(x, y);
    }
*/
    public static Color GetTint(Color a, Color b, float tintFactor) {
        return new Color(a.r + (b.r - a.r) * tintFactor,
                         a.g + (b.g - a.g) * tintFactor,
                         a.b + (b.b - a.b) * tintFactor,
                         a.a + (b.a - a.a) * tintFactor);

    }
    public static Color GetColor(Color c, float alphaOverride) {
        return new Color(c.r, c.g, c.b, alphaOverride);
    }
}
