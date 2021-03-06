
Shader "Obi/Fluid/OpaqueFluid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Smoothness ("Smoothness", Range (0, 1)) = 0.75
	}

	SubShader
	{
		Pass
		{

			Name "OpaqueFluid"         
			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "ObiFluids.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityImageBasedLighting.cginc"

			struct vin
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : POSITION;
			};

			v2f vert (vin v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;

				return o;
			}
	
			float _Smoothness;

			UNITY_DECLARE_SHADOWMAP(_MyShadowMap);

			fout frag (v2f i)
			{
				fout fo;
				fo.color = fixed4(0,0,0,1);

				float3 eyePos,eyeNormal, worldPos, worldNormal, worldView;
				SetupEyeSpaceFragment(i.uv,eyePos,eyeNormal);
				GetWorldSpaceFragment(eyePos,eyeNormal,worldPos,worldNormal,worldView);

				// directional light shadow (cascades)
				float4 viewZ = -eyePos.z;
				float4 zNear = float4( viewZ >= _LightSplitsNear );
				float4 zFar = float4( viewZ < _LightSplitsFar );
				float4 weights = zNear * zFar;

				// shadowing:
				float4 wPos = float4(worldPos,1);
				float3 shadowCoord0 = mul( unity_WorldToShadow[0], wPos).xyz;
				float3 shadowCoord1 = mul( unity_WorldToShadow[1], wPos).xyz;
				float3 shadowCoord2 = mul( unity_WorldToShadow[2], wPos).xyz;
				float3 shadowCoord3 = mul( unity_WorldToShadow[3], wPos).xyz;
				float4 shadowCoord = float4(shadowCoord0 * weights[0] + shadowCoord1 * weights[1] + shadowCoord2 * weights[2] + shadowCoord3 * weights[3],1);
				float atten = UNITY_SAMPLE_SHADOW(_MyShadowMap, shadowCoord);

				// gi specular:
				Unity_GlossyEnvironmentData g;
				g.roughness	= 1-_Smoothness;
				g.reflUVW = reflect(-worldView,worldNormal);
				float3 giSpec = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, g);

				// lighting vectors:
				float3 lightDirWorld = normalize(_WorldSpaceLightPos0.xyz);
				half3 h = normalize( lightDirWorld + worldView );
				float nh = BlinnTerm ( worldNormal, h );
				float nl = DotClamped( worldNormal, lightDirWorld);	
				float nv = DotClamped( worldNormal, worldView);
				
				// energy-conserving microfacet specular lightning:
				half V = SmithBeckmannVisibilityTerm (nl, nv, 1-_Smoothness);
				half D = NDFBlinnPhongNormalizedTerm(nh,RoughnessToSpecPower(1-_Smoothness));
    			float spec = (V * D) * (UNITY_PI/4);
				if (IsGammaSpace())
					spec = sqrt(max(1e-4h, spec));
				spec = max(0, spec * nl);

				float fresnel = FresnelTerm(0.3,nv);

				// foam:
				fixed4 foam = tex2D(_Foam,i.uv);

				// final color:
				fo.color.rgb = tex2D(_Thickness,i.uv).rgb * (UNITY_LIGHTMODEL_AMBIENT + nl * _LightColor0 * atten) + 
								(spec * atten + giSpec) * fresnel + foam;

				OutputFragmentDepth(eyePos,fo);

				return fo;
			}
			ENDCG
		}		

	}
}
