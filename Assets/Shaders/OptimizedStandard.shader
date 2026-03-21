Shader "Mobile/OptimizedStandard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        [NoScaleOffset] _EmissionMap ("Emission", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        // Mobile LOD - Only 1 pass, no complex computations
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
        
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _EmissionMap;
        
        fixed4 _Color;
        fixed _Glossiness;
        fixed _Metallic;
        fixed3 _EmissionColor;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
            // Normal mapping (optional for mobile, reduces performance cost)
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            fixed3 emission = tex2D(_EmissionMap, IN.uv_MainTex) * _EmissionColor;
            o.Emission = emission * 0.5; // Reduced emission intensity
        }
        ENDCG
    }
    
    FallBack "Mobile/Diffuse"
}
