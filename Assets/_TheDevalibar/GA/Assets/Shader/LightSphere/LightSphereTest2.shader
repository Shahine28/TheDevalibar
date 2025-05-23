Shader "LightSphereTest2" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _StencilLayer("Stencil layer", int) = 1
        _StencilLayerPlusOne("Stencil Layer + 1", int ) = 2
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}

         ZWrite off
       
        CGINCLUDE
        #include "UnityCG.cginc"
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex);
                return o;
            }
        ENDCG



        Pass {

            Stencil {
                   // writes back faces to a stencil buffer
                Ref [_StencilLayer]
                Comp always
                Pass replace
            }
            colormask 0
            Cull Front
            ZTest greater
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(v2f i) : SV_Target {
                return half4(0.2,0.2,0,1);
            }
            ENDCG
        }
        Pass {
            Stencil {
                // where front faces match back faces incriment the stencil buffer
                Ref [_StencilLayer]
                Comp Equal
                Pass IncrSat
            }
            colormask 0
            Cull Back
            ZTest less
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

               half4 frag(v2f i) : SV_Target {
                return half4(0,0.2,0.2,1);
            }
            ENDCG
        }

           Pass
        {
        ColorMask RGB
        Cull Front
        ZTest Always
        Stencil {
            // only renders pixels that made it past the second phase
            Ref [_StencilLayerPlusOne]
            // should really work out how to do ^ programatically
            // can't seem to do any math in the ref line
            Comp equal 
        }

        // max blending
        // personally I don't want doubling up of differnt light bands
        BlendOP Max
        Blend One One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            float4 _Color;
            
            fixed4 frag (v2f i) : SV_Target
            {

                return _Color;
            }
            ENDCG
        }
    } 
} 