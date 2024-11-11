Shader "Custom/BiomesShader"
{
    Properties
    {
        [Header(Textures)]
        _BiomeTexArray ("Biome Texture Array", 2DArray) = "white" {} 
        _GridTex ("Grid Texture", 2D) = "white" {}
        
        [Header(Colors)]
        _DesertColor ("Desert Color", Color) = (1.0, 0.84, 0.4, 1.0)
        _ForestColor ("Forest Color", Color) = (0.13, 0.55, 0.13, 1.0)
        _GrasslandColor ("Grassland Color", Color) = (0.5, 0.8, 0.3, 1.0)
        _JungleColor ("Jungle Color", Color) = (0.0, 0.4, 0.0, 1.0)
        _MountainColor ("Mountain Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _SavannaColor ("Savanna Color", Color) = (0.96, 0.64, 0.38, 1.0)
        _TundraColor ("Tundra Color", Color) = (0.8, 0.9, 0.95, 1.0)
        _SwampColor ("Swamp Color", Color) = (0.4, 0.3, 0.2, 1.0)
        _WaterColor ("Water Color", Color) = (0.0, 0.5, 1.0, 1.0)

        [Header(Parameters)]
        _TextureScale ("Texture Scale", Float) = 1.0
        _BlendDistance ("Blend Distance", Range(0.001, 1)) = 0.01
        _ColorTolerance ("Color Tolerance", Range(0.001, 1)) = 0.01
        _NoiseScale ("Noise Scale", Float) = 10.0
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        _BiomeBlendSharpness ("Biome Blend Sharpness", Range(1, 20)) = 10
        
        [Header(Grid Settings)]
        [Toggle(GRID_ON)] _GridEnabled ("Grid Enabled", Float) = 0
        _GridBlend ("Grid Blend Strength", Range(0.0, 1.0)) = 0.5
        _HexScale ("Hex Scale", Vector) = (1, 1, 0, 0)
        
        [Header(Debug)]
        [Toggle] _DebugMode ("Debug Mode", Float) = 0
        [KeywordEnum(None, Desert, Forest, Grassland, Jungle, Mountain, Savanna, Tundra, Swamp, Water)] 
        _DebugBiome ("Debug Biome", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _DEBUGBIOME_NONE _DEBUGBIOME_DESERT _DEBUGBIOME_FOREST _DEBUGBIOME_GRASSLAND _DEBUGBIOME_JUNGLE _DEBUGBIOME_MOUNTAIN _DEBUGBIOME_SAVANNA _DEBUGBIOME_TUNDRA _DEBUGBIOME_SWAMP _DEBUGBIOME_WATER
            #pragma target 3.5
            #pragma multi_compile _ GRID_ON

            UNITY_DECLARE_TEX2DARRAY(_BiomeTexArray);

            #include "UnityCG.cginc"
            #include "HexCellData.cginc"
            
            sampler2D _DesertTex, _ForestTex, _GrasslandTex, _JungleTex;
            sampler2D _MountainTex, _SavannaTex, _TundraTex, _SwampTex, _WaterTex;
            sampler2D _GridTex;
            float4 _DesertColor, _ForestColor, _GrasslandColor, _JungleColor;
            float4 _MountainColor, _SavannaColor, _TundraColor, _SwampColor, _WaterColor;
            float _TextureScale, _BlendDistance, _ColorTolerance, _BiomeBlendSharpness;
            float _NoiseScale, _NoiseStrength;
            float _GridBlend;
            float2 _HexScale;
            float _DebugMode;
            float _GlobalGridEnabled;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Input {
                float4 color : COLOR;
                float3 worldPos;
                float3 terrain;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD1;
                float3 terrain : TEXCOORD2;
                float3 visibility : TEXCOORD3;
            };

            struct BiomeInfo {
                float weight;
                float4 texColor;
            };

            float rand(float2 coord) {
                return frac(sin(dot(coord.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 coord) {
                float2 i = floor(coord);
                float2 f = frac(coord);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(rand(i), rand(i + float2(1.0, 0.0)), u.x),
                           lerp(rand(i + float2(0.0, 1.0)), rand(i + float2(1.0, 1.0)), u.x),
                           u.y);
            }

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.texcoord * float2(14.0 / _HexScale.x, 12.12436 / _HexScale.y);
                o.color = v.color;

                float4 cell0 = GetCellData(v, 0);
                float4 cell1 = GetCellData(v, 1);
                float4 cell2 = GetCellData(v, 2);

                o.terrain.x = cell0.w;
                o.terrain.y = cell1.w;
                o.terrain.z = cell2.w;

                o.visibility.x = cell0.x;
                o.visibility.y = cell1.x;
                o.visibility.z = cell2.x;
                o.visibility = lerp(0.25, 1, o.visibility);

                return o;
            }

            BiomeInfo GetBiomeInfo(float3 targetColor, float4 biomeColor, int biomeIndex,
                float2 uv, int debugBiomeIndex, int currentBiomeIndex, float3 visibility)
            {
                BiomeInfo info;
                float colorDist = length(targetColor - biomeColor.rgb);
                float weight = 1.0 - saturate(colorDist / _ColorTolerance);
                weight = pow(weight, _BiomeBlendSharpness);
                weight *= visibility.x;
                
                if (_DebugMode > 0 && debugBiomeIndex == currentBiomeIndex) {
                    info.weight = weight;
                    info.texColor = float4(weight.xxx, 1);
                } else {
                    info.weight = weight;
                    info.texColor = UNITY_SAMPLE_TEX2DARRAY(_BiomeTexArray, float3(uv, biomeIndex));
                }
                
                return info;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float whiteThreshold = 0.9;
                
                float2 noiseUV = i.worldPos.xz * _NoiseScale;
                float noiseValue = noise(noiseUV);
                float2 offsetUV = i.uv + noiseValue * _NoiseStrength;
                
                int debugBiomeIndex = 0;
                #ifdef _DEBUGBIOME_DESERT
                debugBiomeIndex = 1;
                #elif _DEBUGBIOME_FOREST
                debugBiomeIndex = 2;
                #elif _DEBUGBIOME_GRASSLAND
                debugBiomeIndex = 3;
                #elif _DEBUGBIOME_JUNGLE
                debugBiomeIndex = 4;
                #elif _DEBUGBIOME_MOUNTAIN
                debugBiomeIndex = 5;
                #elif _DEBUGBIOME_SAVANNA
                debugBiomeIndex = 6;
                #elif _DEBUGBIOME_TUNDRA
                debugBiomeIndex = 7;
                #elif _DEBUGBIOME_SWAMP
                debugBiomeIndex = 8;
                #elif _DEBUGBIOME_WATER
                debugBiomeIndex = 9;
                #endif
                
                BiomeInfo biomes[8];
                biomes[0] = GetBiomeInfo(i.color.rgb, _DesertColor, 0, offsetUV, debugBiomeIndex, 1, i.visibility);
                biomes[1] = GetBiomeInfo(i.color.rgb, _ForestColor, 1, offsetUV, debugBiomeIndex, 2, i.visibility);
                biomes[2] = GetBiomeInfo(i.color.rgb, _GrasslandColor, 2, offsetUV, debugBiomeIndex, 3, i.visibility);
                biomes[3] = GetBiomeInfo(i.color.rgb, _JungleColor, 3, offsetUV, debugBiomeIndex, 4, i.visibility);
                biomes[4] = GetBiomeInfo(i.color.rgb, _MountainColor, 4, offsetUV, debugBiomeIndex, 5, i.visibility);
                biomes[5] = GetBiomeInfo(i.color.rgb, _SavannaColor, 5, offsetUV, debugBiomeIndex, 6, i.visibility);
                biomes[6] = GetBiomeInfo(i.color.rgb, _TundraColor, 6, offsetUV, debugBiomeIndex, 7, i.visibility);
                biomes[7] = GetBiomeInfo(i.color.rgb, _SwampColor, 7, offsetUV, debugBiomeIndex, 8, i.visibility);

                float totalWeight = 0.0;
                fixed4 finalTex = fixed4(0, 0, 0, 1);

                for (int idx = 0; idx < 8; idx++)
                {
                    totalWeight += biomes[idx].weight;
                    finalTex += biomes[idx].texColor * biomes[idx].weight;
                }

                finalTex /= totalWeight;
                
                float2 gridUV = i.worldPos.xz;
                
                float temp = gridUV.x;
                gridUV.x = gridUV.y;
                gridUV.y = temp;
                
                gridUV.y *= -1;
                
                gridUV.x *= 1 / (4 * 6.06217783);
                gridUV.y *= 1 / (2 * 10.5);
                
                fixed4 gridTexColor = tex2D(_GridTex, gridUV);

                const float whiteFactor = smoothstep(whiteThreshold, 1.0, dot(gridTexColor.rgb, float3(1.0, 1.0, 1.0)));
                gridTexColor.a = lerp(gridTexColor.a, 0.0, whiteFactor);

                const float luminance = dot(finalTex.rgb, float3(0.299, 0.587, 0.114));

                float gridStrength = _GridBlend * gridTexColor.a * i.visibility.x;
                gridStrength *= lerp(1.0, 0.5, luminance);

                gridStrength *= _GlobalGridEnabled;

                const float gridContrast = lerp(0.1, 1.7, luminance);
                gridTexColor.rgb = pow(gridTexColor.rgb, gridContrast);
                
                finalTex = lerp(finalTex, lerp(finalTex, gridTexColor, gridStrength), step(0.05, gridTexColor.a));

                return finalTex;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
