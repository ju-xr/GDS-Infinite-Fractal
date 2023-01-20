
Shader "Unlit/Fra3D"
{/*
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST .001
#define TAU 6.283185
#define PI 3.141592
#define S smoothstep
#define T iTime

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));//follow object
                o.hitPos = v.vertex;// object space
                return o;
            }

            float4 Rot(float a) {
                float s = sin(a), c = cos(a);
                return float4(c, -s, s, c);
            }

            float sdBox(float3 p, float3 s) {
                p = abs(p) - s;
                return length(max(p, 0.)) + min(max(p.x, max(p.y, p.z)), 0.);
            }

            float2 N(float angle) {
                // angle to vector
                return float2(sin(angle), cos(angle));
            }

            float2 Koch(float2 uv) {
                uv.x = abs(uv.x);

                float3 col = float2(0);
                float d;

                float angle = 0.;
                float2 n = N((5. / 6.) * 3.1415);

                uv.y += tan((5. / 6.) * 3.1415) * .5;
                d = dot(uv - float2(.5, 0), n);
                uv -= max(0., d) * n * 2.;

                float scale = 1.;

                n = N((2. / 3.) * 3.1415);
                uv.x += .5;
                for (int i = 0; i < 4; i++) {
                    uv *= 3.;
                    scale *= 3.;
                    uv.x -= 1.5;

                    uv.x = abs(uv.x);
                    uv.x -= .5;
                    d = dot(uv, n);
                    uv -= min(0., d) * n * 2.;
                }
                uv /= scale;
                return uv;
            }


            float GetDist(float3 p) {

                p.xz *= Rot(T * .2);

                
                // straight intersection
                //vec2 xy = Koch(p.xy);
                //vec2 yz = Koch(p.yz);
                //vec2 xz = Koch(p.xz);
                //float d = max(xy.y, max(yz.y, xz.y));
                


                float2 xz = Koch(float2(length(p.xz), p.y));
                float2 yz = Koch(float2(length(p.yz), p.x));
                float2 xy = Koch(float2(length(p.xy), p.z));
                float d = max(xy.x, max(yz.x, xz.x));

                d = mix(d, length(p) - .5, .5);
                return d;
            }

            float RayMarch(float3 ro, float3 rd) {
                float dO = 0.;

                for (int i = 0; i < MAX_STEPS; i++) {
                    float3 p = ro + rd * dO;
                    float dS = GetDist(p);
                    dO += dS;
                    if (dO > MAX_DIST || abs(dS) < SURF_DIST) break;
                }

                return dO;
            }

            float3 GetNormal(float3 p) {
                float d = GetDist(p);
                float2 e = float2(.001, 0);

                float3 n = d - float3(
                    GetDist(p - e.xyy),
                    GetDist(p - e.yxy),
                    GetDist(p - e.yyx));

                return normalize(n);
            }

            float3 GetRayDir(float2 uv, float2 p, float3 l, float z) {
                float3 f = normalize(l - p),
                    r = normalize(cross(vec3(0, 1, 0), f)),
                    u = cross(f, r),
                    c = f * z,
                    i = c + uv.x * r + uv.y * u,
                    d = normalize(i);
                return d;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - .5;
    vec2 m = iMouse.xy / iResolution.xy;
    float3 ro = i.ro;//float3(0,0,-3); //origin
    float3 rd = normalize(i.hitPos - ro);
    float d = RayMarch(ro, rd);


                // sample the texture
                fixed4 col = 0;

                if (d >= MAX_DIST)
                    discard;
                else {
                    float3 p = ro + rd * d;
                    float3 n = GetNormal(p);
                    float3 r = reflect(rd, n);

                    float dif = dot(n, normalize(float3(1, 2, 3))) * .5 + .5;
                    col = float3(dif);

                    col = n * .5 + .5;
                    col *= texture(iChannel0, r).rgb;
                }
                //col *= 0.;
                //vec2 st = Koch(uv)*4.;
                //col = vec3(st.y);
                col = pow(col, float3(.4545));	// gamma correction

                fragColor = vec4(col, 1.0);

                return col;
            }
            ENDCG
        }
    }*/
}
