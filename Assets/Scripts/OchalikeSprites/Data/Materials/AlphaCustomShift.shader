// Renderer.color のアルファ値で淡色と濃色を手動で切り替えるための CustomShift

Shader "Ochalike Sprites/Sprites/Alpha Custom Shift"
{
    Properties
    {
        _SaturationThreshold("SaturationThreshold", Float) = .3
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnitySprites.cginc"

            half4 rgb2hsv(fixed4 c)
            {
                half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
                half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

                half d = q.x - min(q.w, q.y);
                half e = 1.0e-10;
                return half4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, c.a);
            }

            fixed3 hsv2rgb(half3 c)
            {
                half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            half _SaturationThreshold;
            half _ShiftS0, _ShiftV0;
            half _ShiftS1, _ShiftV1;

            // テクスチャの RGB で Renderer.color の HSV をシフトする
            fixed3 custom_shift(half4 hsv, fixed4 tex)
            {
                // Renderer.color.a を比較して、シフトパターンを判定する。
                int satRatio = step(1., hsv.w / (128. / 255.));
                half3 shift = (tex.xyz - 128. / 255.) * 2.;  // 0 ～ 255 を -1 ～ +1 に変換する
                shift.yz += lerp(half2(_ShiftS0, _ShiftV0), half2(_ShiftS1, _ShiftV1), satRatio) * shift.x;  // shift.x (影色シフト) に応じて彩度シフトと明度シフトを加算
                
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
                
                half4 hsv = rgb2hsv(IN.color);
                //fixed4 hsv = IN.color; // Renderer.color に HSV を入れる場合

                c.rgb = custom_shift(hsv, c);

                // テクスチャのアルファ値と Renderer.color.a がそれぞれ 40% ~ 50%, 90% ~ 100% のとき不透明。
                c.a = min(fmod(c.a, 128. / 255.) / (.8 * 128. / 256.), 1.) * min(fmod(hsv.a, 128. / 255.) / (.8 * 128. / 256.), 1.);

                // RGB それぞれ 248 を最大にする。
                c.rgb *= 248. / 255.;

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
