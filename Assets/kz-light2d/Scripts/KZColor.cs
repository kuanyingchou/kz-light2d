using UnityEngine;
using System.Collections;

// a helper for UnityEngine.Color
public class KZColor {
    public static Color Add(params Color[] colors) {
        float r = 0, g = 0, b = 0, a = 0;
        for(int i=0; i<colors.Length; i++) {
            r += colors[i].r;
            g += colors[i].g;
            b += colors[i].b;
            a += colors[i].a;
        }
        return new Color(r, g, b, a);
    }
    public static Color Add(Color lhs, Color rhs) {
        return new Color(
                Mathf.Min(1, lhs.r + rhs.r), 
                Mathf.Min(1, lhs.g + rhs.g), 
                Mathf.Min(1, lhs.b + rhs.b), 
                Mathf.Min(1, lhs.a + rhs.a));
    }

    public static Color Mul(Color input, float f) {
        return new Color(
                input.r * f, input.g * f, input.b * f, input.a * f);
    }

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
