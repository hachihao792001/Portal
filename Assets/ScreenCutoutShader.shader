Shader "Unlit/ScreenCutoutShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            vertexOutput vert(vertexInput v)
            {
                vertexOutput o;

                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.position);
                o.uv = v.uv;

                return o;
            }

            half4 frag(vertexOutput i) : SV_Target
            {
                i.screenPos /= i.screenPos.w;
				fixed4 col = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));
				
				return col;
            }
            ENDCG
        }
    }
}