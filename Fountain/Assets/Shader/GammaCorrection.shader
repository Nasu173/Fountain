Shader "Hidden/GammaCorrection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gamma ("Gamma Value", Range(0.2, 3.0)) = 1.0
    }
    SubShader
    {
        // 不写入深度缓冲区，不改变渲染状态
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Gamma;  // 声明从属性传递进来的变量

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                
                // 应用伽马校正：输出 = 输入 ^ (1/伽马值)
                // 注意：为了避免除零错误，确保_Gamma不为0
                float invGamma = 1.0 / max(_Gamma, 0.001);
                color.rgb = pow(color.rgb, invGamma);
                
                return color;
            }
            ENDCG
        }
    }
    
    // 回退方案 - 如果没有效果，至少显示原始画面
    Fallback Off
}
