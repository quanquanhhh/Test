Shader "Custom/TextShineDiagonal"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _ShineWidth ("Shine Width", Range(0.02, 0.5)) = 0.15
        _ShineIntensity ("Shine Intensity", Range(0, 3)) = 1.5
        _ShineSpeed ("Shine Speed", Range(0, 10)) = 2.0
        _ShineAngle ("Shine Angle", Range(-1, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            fixed4 _ShineColor;
            float _ShineWidth;
            float _ShineIntensity;
            float _ShineSpeed;
            float _ShineAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;

                // 扫光中心，从左到右
                float center = frac(_Time.y * _ShineSpeed);

                // ✅ 斜向投影（关键）
                float proj = i.uv.x + i.uv.y * _ShineAngle;

                float dist = abs(proj - center);

                float shine = saturate(1.0 - dist / _ShineWidth);
                shine = pow(shine, 3.0); // 中央更亮

                fixed4 shineCol = _ShineColor * shine * _ShineIntensity;

                fixed4 col = baseCol + shineCol * baseCol.a;
                col.a = baseCol.a;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
