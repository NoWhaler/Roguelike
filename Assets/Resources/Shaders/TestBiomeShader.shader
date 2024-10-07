Shader "Custom/BiomesShader"
{
    Properties
    {
        [Header(Textures)]
        _DesertTex ("Desert Texture", 2D) = "white" {}
        _ForestTex ("Forest Texture", 2D) = "white" {}
        _GrasslandTex ("Grassland Texture", 2D) = "white" {}
        _JungleTex ("Jungle Texture", 2D) = "white" {}
        _MountainTex ("Mountain Texture", 2D) = "white" {}
        _SavannaTex ("Savanna Texture", 2D) = "white" {}
        _TundraTex ("Tundra Texture", 2D) = "white" {}
        _SwampTex ("Swamp Texture", 2D) = "white" {}
        _WaterTex ("Water Texture", 2D) = "white" {}
        
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
        
        [Header(Debug)]
        [Toggle] _DebugMode ("Debug Mode", Float) = 0
        [KeywordEnum(None, Desert, Forest, Grassland, Jungle, Mountain, Savanna, Tundra, Swamp, Water)] 
        _DebugBiome ("Debug Biome", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _DEBUGBIOME_NONE _DEBUGBIOME_DESERT _DEBUGBIOME_FOREST _DEBUGBIOME_GRASSLAND _DEBUGBIOME_JUNGLE _DEBUGBIOME_MOUNTAIN _DEBUGBIOME_SAVANNA _DEBUGBIOME_TUNDRA _DEBUGBIOME_SWAMP _DEBUGBIOME_WATER
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _DesertTex, _ForestTex, _GrasslandTex, _JungleTex;
            sampler2D _MountainTex, _SavannaTex, _TundraTex, _SwampTex, _WaterTex;
            float4 _DesertColor, _ForestColor, _GrasslandColor, _JungleColor;
            float4 _MountainColor, _SavannaColor, _TundraColor, _SwampColor, _WaterColor;
            float _TextureScale, _BlendDistance, _ColorTolerance, _BiomeBlendSharpness;
            float _NoiseScale, _NoiseStrength;
            float _DebugMode;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv * _TextureScale;
                o.color = v.color;
                return o;
            }

            BiomeInfo GetBiomeInfo(float3 targetColor, float4 biomeColor, sampler2D biomeTex, float2 uv, int debugBiomeIndex, int currentBiomeIndex)
            {
                BiomeInfo info;
                float colorDist = length(targetColor - biomeColor.rgb);
                float weight = 1.0 - saturate(colorDist / _ColorTolerance);
                weight = pow(weight, _BiomeBlendSharpness);
                
                if (_DebugMode > 0 && debugBiomeIndex == currentBiomeIndex) {
                    info.weight = weight;
                    info.texColor = float4(weight.xxx, 1);
                } else {
                    info.weight = weight;
                    info.texColor = tex2D(biomeTex, uv);
                }
                
                return info;
            }

            fixed4 frag (v2f i) : SV_Target
            {
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
                
                BiomeInfo biomes[9];
                biomes[0] = GetBiomeInfo(i.color.rgb, _DesertColor, _DesertTex, offsetUV, debugBiomeIndex, 1);
                biomes[1] = GetBiomeInfo(i.color.rgb, _ForestColor, _ForestTex, offsetUV, debugBiomeIndex, 2);
                biomes[2] = GetBiomeInfo(i.color.rgb, _GrasslandColor, _GrasslandTex, offsetUV, debugBiomeIndex, 3);
                biomes[3] = GetBiomeInfo(i.color.rgb, _JungleColor, _JungleTex, offsetUV, debugBiomeIndex, 4);
                biomes[4] = GetBiomeInfo(i.color.rgb, _MountainColor, _MountainTex, offsetUV, debugBiomeIndex, 5);
                biomes[5] = GetBiomeInfo(i.color.rgb, _SavannaColor, _SavannaTex, offsetUV, debugBiomeIndex, 6);
                biomes[6] = GetBiomeInfo(i.color.rgb, _TundraColor, _TundraTex, offsetUV, debugBiomeIndex, 7);
                biomes[7] = GetBiomeInfo(i.color.rgb, _SwampColor, _SwampTex, offsetUV, debugBiomeIndex, 8);
                biomes[8] = GetBiomeInfo(i.color.rgb, _WaterColor, _WaterTex, offsetUV, debugBiomeIndex, 9);
                
                float4 finalColor = float4(0,0,0,0);
                float totalWeight = 0;
                
                for(int j = 0; j < 9; j++) {
                    finalColor += biomes[j].texColor * biomes[j].weight;
                    totalWeight += biomes[j].weight;
                }
                
                if(totalWeight > 0) {
                    finalColor /= totalWeight;
                }
                
                return _DebugMode > 0 ? finalColor : finalColor * fixed4(i.color.rgb, 1);
            }
            ENDCG
        }
    }
}
