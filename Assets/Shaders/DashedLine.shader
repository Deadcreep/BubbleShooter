Shader "Custom/DashedLine"{
    Properties{
        _Rep("Repeat Count", float) = 5
        _Alpha("Alpha", Range(0, 1))=0.5
    }
    SubShader{
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Rep;
            float _Alpha;            

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct v2f{
                float2 uv : TEXCOORD0;              
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert (appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv.x = o.uv.x * _Rep * 0.5;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target{
                i.uv.x = fmod(i.uv.x, 0.5);
                float s = i.uv.x - 0.25;
                fixed4 color = i.color;                
                color.a = step(0.0, s) * _Alpha;
                return color;
            }
            ENDCG
        }
    }
}