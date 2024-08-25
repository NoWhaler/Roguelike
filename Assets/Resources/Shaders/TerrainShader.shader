Shader "Custom/SimpleTerrainShader" {
    Properties {
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _RockTex ("Rock Texture", 2D) = "white" {}
        _SnowTex ("Snow Texture", 2D) = "white" {}
        _DetailTex ("Detail Texture", 2D) = "gray" {}
        _SnowHeight ("Snow Height", Float) = 0.8
        _RockHeight ("Rock Height", Float) = 0.3
        _RockSteepness ("Rock Steepness", Float) = 0.7
    }
    SubShader {
        Tags {"RenderType"="Opaque"}
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _GrassTex;
        sampler2D _RockTex;
        sampler2D _SnowTex;
        sampler2D _DetailTex;
        float _SnowHeight;
        float _RockHeight;
        float _RockSteepness;

        struct Input {
            float2 uv_GrassTex;
            float2 uv_DetailTex;
            float3 worldPos;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float height = IN.worldPos.y;
            float steepness = 1 - IN.worldNormal.y;

            fixed4 grass = tex2D(_GrassTex, IN.uv_GrassTex);
            fixed4 rock = tex2D(_RockTex, IN.uv_GrassTex);
            fixed4 snow = tex2D(_SnowTex, IN.uv_GrassTex);
            fixed4 detail = tex2D(_DetailTex, IN.uv_DetailTex);

            fixed4 c = lerp(grass, rock, saturate(steepness - _RockSteepness));
            c = lerp(c, snow, saturate((height - _SnowHeight) * 5));
            c = lerp(c, rock, saturate((steepness - _RockSteepness) * 2));
            c = lerp(c, grass, saturate((_RockHeight - height) * 5));

            c *= detail * 2;

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
