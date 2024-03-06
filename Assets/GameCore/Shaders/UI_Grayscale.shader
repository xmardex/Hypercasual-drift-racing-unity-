Shader "Custom/Grayscale" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            float gray = dot(c.rgb, float3(0.3, 0.59, 0.11));
            o.Albedo = gray;
        }
        ENDCG
    }
    FallBack "Diffuse"
}