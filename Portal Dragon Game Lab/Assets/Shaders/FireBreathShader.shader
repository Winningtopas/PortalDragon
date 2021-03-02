Shader "MyShaders/Fire Breath"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_AnimationSpeed("Animation Speed", Range(0, 3)) = 0
		_OffsetSize("Offset Size", Range(0, 10)) = 0
		_Emission("Emission", float) = 0
		[HDR] _EmissionColor("Color", Color) = (0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            float4 _MainTex_ST;
			float4 _TintColor;
			float _AnimationSpeed;
			float _OffsetSize;

            v2f vert (appdata v)
            {
                v2f o;

				v.vertex.x += sin(_Time.y * _AnimationSpeed + v.vertex.y * _OffsetSize);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 color = tex2D(_MainTex, i.uv) * _TintColor;

				// combine all masked textures as output
				return color;
            }
            ENDCG
        }
    }
}
