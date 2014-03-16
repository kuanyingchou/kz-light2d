//from http://wiki.unity3d.com/index.php/Transparent_Single_Color_Shader

Shader "Custom/TransparentSingleColorShader" {
  Properties {
    _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
  }
  SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off
    LOD 200
    //[ added by ken, 2014.3.15
    ZWrite Off // don't write to depth buffer 
    Blend SrcAlpha One // additive blending
    //]

    CGPROGRAM
    #pragma surface surf Lambert

    fixed4 _Color;

    // Note: pointless texture coordinate. I couldn't get Unity (or Cg)
    //       to accept an empty Input structure or omit the inputs.
    struct Input {
      float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o) {
      o.Albedo = _Color.rgb;
      o.Emission = _Color.rgb; // * _Color.a;
      o.Alpha = _Color.a;
    }
    ENDCG
  }
  FallBack "Diffuse"
}
