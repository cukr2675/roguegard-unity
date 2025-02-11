Shader "Ochalike Sprites/UI/Custom Shift"
{
    Properties
    {
        _MinHueThreshold("MinHueThreshold", Float) = 0.13888  // 50. / 360.
        _MaxHueThreshold("MaxHueThreshold", Float) = 0.22222  // 80. / 360.
        _SaturationThreshold("SaturationThreshold", Float) = .3
        _ValueThreshold("ValueThreshold", Float) = .6
        [Header(OnLowSaturation)]
        _ShiftS0 ("ShiftS0", Float) = -.25
        _ShiftV0 ("ShiftV0", Float) = 0
        [Header(OnHighSaturation)]
        _ShiftS1 ("ShiftS1", Float) = -.25
        _ShiftV1 ("ShiftV1", Float) = .25
        [Space]
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnitySprites.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            half3 rgb2hsv(fixed3 c)
            {
                half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
                half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

                half d = q.x - min(q.w, q.y);
                half e = 1.0e-10;
                return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed3 hsv2rgb(half3 c)
            {
                half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }
            
            half _MinHueThreshold;
            half _MaxHueThreshold;
            half _SaturationThreshold;
            half _ValueThreshold;
            half _ShiftS0, _ShiftV0;
            half _ShiftS1, _ShiftV1;

            // テクスチャの RGB で Renderer.color の HSV をシフトする
            fixed3 custom_shift(half3 hsv, fixed4 tex)
            {
                // Renderer.color の色相または彩度を比較して、シフトパターンを判定する。
                // MinHueThreshold <= 色相 < MaxHueThreshold または SaturationThreshold <= 彩度 または 明度 < ValueThreshold なら S1, V1 、それ以外は S0, V0 。
                // 白肌と褐色肌で影色を変えるために明度は比較しない。
                int hueRatio = step(_MinHueThreshold, hsv.x) * step(hsv.x, _MaxHueThreshold);  // MinHueThreshold <= 色相 < MaxHueThreshold
                int satRatio = step(1., hsv.y / _SaturationThreshold);                         // SaturationThreshold <= 彩度
                int valRatio = step(hsv.z, _ValueThreshold);                                   // 明度 < ValueThreshold
                half3 shift = (tex.xyz - 128. / 255.) * 2.;  // 0 ～ 255 を -1 ～ +1 に変換する
                shift.yz += lerp(half2(_ShiftS0, _ShiftV0), half2(_ShiftS1, _ShiftV1), max(max(hueRatio, satRatio), valRatio)) * shift.x;  // shift.x (影色シフト) に応じて彩度シフトと明度シフトを加算

                // Renderer.color の彩度がゼロのとき、彩度シフトを無効化して代わりに明度をシフトする。
                // 白の彩度を上げると赤くなってしまうため必要。
                int monochromeRatio = step(hsv.y, 0.);
                //shift.z = lerp(shift.z, -shift.y, monochromeRatio * .5);  // 遷移（明度のみシフトしていると彩度に潰される）
                shift.z -= shift.y * monochromeRatio;                       // 加算（白飛び・黒つぶれが発生する）
                shift.y *= (1 - monochromeRatio);  // 彩度シフトをゼロにする

                // Renderer.color にシフトを適用して最終的なシフト色を取得
                hsv.yz *= saturate(1. + shift.yz);                                // シフトの -1 ～  0 に応じて Renderer.color に 0 ～ 1 を乗算
                hsv.yz = 1. - saturate((1. - hsv.yz) * saturate(1. - shift.yz));  // シフトの  0 ～ +1 に応じて Renderer.color を反転して乗算
                
                // テクスチャのアルファ値で、テクスチャ色とシフト色を判定する。
                int shiftRatio = step(1., tex.a / (128. / 255.));
                return lerp(tex.rgb, hsv2rgb(hsv), shiftRatio);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);

                half3 hsv = rgb2hsv(IN.color.xyz);
                //fixed3 hsv = IN.color.xyz; // Renderer.color に HSV を入れる場合

                c.rgb = custom_shift(hsv, c);
                
                // テクスチャのアルファ値が 40% ~ 50%, 90% ~ 100% のとき不透明。 Renderer.color.a はそのまま適用
                c.a = min(fmod(c.a, 128. / 255.) / (.8 * 128. / 256.), 1.) * IN.color.a;

                // RGB それぞれ 248 を最大にする。
                c.rgb *= 248. / 255.;

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
