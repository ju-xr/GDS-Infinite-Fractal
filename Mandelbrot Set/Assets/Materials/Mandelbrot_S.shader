//Ref:https://www.youtube.com/watch?v=zmWkhlocBRY
Shader "Ju/Mandelbrot_S"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // 0,0 is the center of the area for rendering
        // 4,4 the size of the area
        _Area( "Area", vector) = (0,0,4,4)
            _Angle("Angle", range(-3.1415, 3.1415)) = 0
            _MaxIter ("MaxIter", range(4, 5000)) = 255
            _Color("Colour", range(0,1)) = 0.5
            _Repeat("Repeat", float) = 1
            _Speed("Speed", float) = 1
            _Symmetry("Symmetry", range(0,1)) = 1

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            float4 _Area;
            float _Angle, _MaxIter, _Color, _Repeat, _Speed, _Symmetry;


            //Rotation - pivot - point around where to rotate
            float2 rot(float2 p, float2 pivot, float a) {
                float s = sin(a);
                float c = cos(a);

                p -= pivot;
                p = float2(p.x * c - p.y * s, p.x * s + p.y * c);

                //shift back to the original pivot
                p += pivot;

                return p;
            }

            //fractal algorithm
            fixed4 frag(v2f i) : SV_Target
            {


                //vaviable c: start position initialize to UV coordinate
                float2 uv = i.uv - .5;
                uv = abs(uv);
                uv = rot(uv, 0, .25 * 3.1415);
                uv = abs(uv);

                uv = lerp(i.uv - .5, uv, _Symmetry);

                float2 c = _Area.xy + uv*_Area.zw;
                c = rot(c, _Area.xy, _Angle);

                float r = 20; //escape radius
                float r2 = r * r;

                //vaviable z: keep tack of where pixel is jumping across screen
                float2 z, zPrevious;

                //iteration loop
                float iter;
                for (iter = 0; iter < _MaxIter; iter++) 
                {
                    zPrevious = rot(z,0,_Time.y);
                    //equation
                    z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + c;
                    if (dot(z,zPrevious) > r2) break;
                }
                if (iter > _MaxIter) return 0;
                
                float dist = length(z); //distance from origin
                float fracIter = (dist - r) / (r2 - r);//linear interpolation
                fracIter = log2(log(dist) / log(r));// double exponential interpolation

                //iter -= fracIter;

                float m = sqrt(iter / _MaxIter);

                //color
                float4 col = sin(float4(0.3, 0.45, 0.65, 1) * m * 20) * 0.5 + 0.5; //Procedual colors
                col = tex2D(_MainTex, float2(m * _Repeat + _Time.y * _Speed, _Color));


                float angle = atan2(z.x, z.y); // -pi and pi
                //if(i.uv.x >.5) //split screen with black mask
                col *= smoothstep(3, 0, fracIter);

                col *= 1+ sin(angle * 2 + _Time.y*4)* .2; // shadow + moving with speed
                return col;
            }
            ENDCG
        }
    }
}
