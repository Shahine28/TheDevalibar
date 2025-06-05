// OutlineShader.shader
Shader "Custom/NightSoulOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,1,1) // 描边颜色
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.01 // 描边厚度
        _NoiseTex ("Noise Texture", 2D) = "white" {} // 噪声纹理，用于能量扰动
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 1.0 // 噪声动画速度
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.5 // 噪声强度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" } // 确保在模型之后渲染
        LOD 100

        // Pass 1: 描边通道
        Pass
        {
            Cull Front // 剔除正面，只渲染背面，避免Z-fighting
            ZWrite Off // 不写入深度，确保描边在模型后面
            Blend SrcAlpha OneMinusSrcAlpha // 启用透明混合

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // 添加UV，用于噪声纹理
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; // 传递UV到片元着色器
                float3 worldNormal : TEXCOORD1; // 世界空间法线
                float3 worldPos : TEXCOORD2; // 世界空间位置
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
                sampler2D _NoiseTex;
                float4 _NoiseTex_ST;
                float _NoiseSpeed;
                float _NoiseIntensity;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                // 膨胀顶点，创建描边效果
                float3 expandedPos = v.vertex.xyz + v.normal * _OutlineThickness;
                o.vertex = TransformObjectToHClip(expandedPos);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex); // 转换UV
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz); // 原始世界位置
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // 能量扰动效果
                float time = _Time.y * _NoiseSpeed;
                float2 noiseUV = i.uv + float2(time, time); // 简单的时间偏移动画
                float noise = tex2D(_NoiseTex, noiseUV).r; // 获取噪声值
                
                // 根据噪声强度调整描边颜色
                float alpha = _OutlineColor.a;
                float noiseFactor = saturate(noise * _NoiseIntensity); // 噪声因子
                
                // 模拟能量扰动，使描边颜色在某些区域变暗或透明
                // 这里可以根据需求调整扰动逻辑，例如：
                // 1. 降低描边透明度：alpha *= (1.0 - noiseFactor);
                // 2. 混合两种颜色：float4 finalColor = lerp(_OutlineColor, _DisturbanceColor, noiseFactor);
                // 3. 简单地将噪声应用于alpha：alpha = _OutlineColor.a * (1.0 - noiseFactor);
                
                // 为了演示，我们简单地让噪声影响描边的可见性
                // 噪声值越高，描边越透明，模拟扰动消失的效果
                alpha = _OutlineColor.a * (1.0 - noiseFactor); 

                return float4(_OutlineColor.rgb, alpha);
            }
            ENDHLSL
        }

        // Pass 2: 渲染原始模型
        // 这个Pass通常不是描边着色器的一部分，而是模型自身材质的渲染。
        // 但为了完整性，如果你的模型没有自己的材质，可以在这里简单渲染。
        // 实际使用时，模型会使用一个标准的URP Lit/Unlit材质。
        // Pass
        // {
        //     Cull Back // 剔除背面
        //     ZWrite On // 写入深度
        //     HLSLPROGRAM
        //     #pragma vertex vert_model
        //     #pragma fragment frag_model
        //
        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //
        //     struct appdata_model
        //     {
        //         float4 vertex : POSITION;
        //         float3 normal : NORMAL;
        //         float2 uv : TEXCOORD0;
        //     };
        //
        //     struct v2f_model
        //     {
        //         float4 vertex : SV_POSITION;
        //         float2 uv : TEXCOORD0;
        //     };
        //
        //     v2f_model vert_model (appdata_model v)
        //     {
        //         v2f_model o;
        //         o.vertex = TransformObjectToHClip(v.vertex.xyz);
        //         o.uv = v.uv;
        //         return o;
        //     }
        //
        //     float4 frag_model (v2f_model i) : SV_Target
        //     {
        //         return float4(1, 0, 0, 1); // 简单红色，实际应使用模型纹理和光照
        //     }
        //     ENDHLSL
        // }
    }
}