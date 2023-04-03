// Made with Amplify Shader Editor v1.9.0.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Interactive Wind 2D/Wind Uber Lit"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_MaskMap("Mask Map", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 3)) = 1
		[Toggle(_ENABLEWIND_ON)] _EnableWind("Enable Wind", Float) = 1
		_WindRotation("Wind: Rotation", Float) = 0
		_WindMaxRotation("Wind: Max Rotation", Float) = 2
		_WindRotationWindFactor("Wind: Rotation Wind Factor", Float) = 1
		_WindSquishFactor("Wind: Squish Factor", Float) = 0.3
		_WindSquishWindFactor("Wind: Squish Wind Factor", Range( 0 , 1)) = 0
		[Toggle(_WINDLOCALWIND_ON)] _WindLocalWind("Wind: Local Wind", Float) = 0
		_WindNoiseScale("Wind: Noise Scale", Float) = 0.1
		_WindMinIntensity("Wind: Min Intensity", Float) = -0.5
		_WindMaxIntensity("Wind: Max Intensity", Float) = 0.5
		[Toggle(_WINDHIGHQUALITYNOISE_ON)] _WindHighQualityNoise("Wind: High Quality Noise", Float) = 0
		[Toggle(_WINDISPARALLAX_ON)] _WindIsParallax("Wind: Is Parallax", Float) = 0
		_WindXPosition("Wind: X Position", Float) = 0
		_WindFlip("Wind: Flip", Float) = 0
		[Toggle(_ENABLEUVDISTORT_ON)] _EnableUVDistort("Enable UV Distort", Float) = 1
		[KeywordEnum(UV,World)] _ShaderSpace("Shader Space", Float) = 0
		_UVDistortFade("UV Distort: Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_UVDistortShaderMask("UV Distort: Shader Mask", 2D) = "white" {}
		_UVDistortFrom("UV Distort: From", Vector) = (-0.02,-0.02,0,0)
		_UVDistortTo("UV Distort: To", Vector) = (0.02,0.02,0,0)
		_UVDistortSpeed("UV Distort: Speed", Vector) = (2,2,0,0)
		_UVDistortNoiseScale("UV Distort: Noise Scale", Vector) = (0.1,0.1,0,0)
		_UVDistortNoiseTexture("UV Distort: Noise Texture", 2D) = "white" {}
		[Toggle(_ENABLEUVSCALE_ON)] _EnableUVScale("Enable UV Scale", Float) = 0
		_UVScaleScale("UV Scale: Scale", Vector) = (1,1,0,0)
		_UVScalePivot("UV Scale: Pivot", Vector) = (0.5,0.5,0,0)
		[Toggle(_ENABLEBRIGHTNESS_ON)] _EnableBrightness("Enable Brightness", Float) = 0
		_Brightness("Brightness", Float) = 1
		[Toggle(_ENABLECONTRAST_ON)] _EnableContrast("Enable Contrast", Float) = 0
		_Contrast("Contrast", Float) = 1
		[Toggle(_ENABLESATURATION_ON)] _EnableSaturation("Enable Saturation", Float) = 0
		_Saturation("Saturation", Float) = 1
		[Toggle(_ENABLEHUE_ON)] _EnableHue("Enable Hue", Float) = 0
		[ASEEnd]_Hue("Hue", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Lit" "ShaderGraphShader"="true" "PreviewType"="Plane" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		ENDHLSL

		
		Pass
		{
			Name "Sprite Lit"
			Tags { "LightMode"="Universal2D" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 120106


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3

			#define _SURFACE_TYPE_TRANSPARENT 1

			#define SHADERPASS SHADERPASS_SPRITELIT
			#define SHADERPASS_SPRITELIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
			
			#if USE_SHAPE_LIGHT_TYPE_0
			SHAPE_LIGHT(0)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_1
			SHAPE_LIGHT(1)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_2
			SHAPE_LIGHT(2)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_3
			SHAPE_LIGHT(3)
			#endif

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _ENABLEHUE_ON
			#pragma shader_feature_local _ENABLESATURATION_ON
			#pragma shader_feature_local _ENABLECONTRAST_ON
			#pragma shader_feature_local _ENABLEBRIGHTNESS_ON
			#pragma shader_feature_local _ENABLEUVSCALE_ON
			#pragma shader_feature_local _ENABLEUVDISTORT_ON
			#pragma shader_feature_local _ENABLEWIND_ON
			#pragma shader_feature_local _WINDLOCALWIND_ON
			#pragma shader_feature_local _WINDHIGHQUALITYNOISE_ON
			#pragma shader_feature_local _WINDISPARALLAX_ON
			#pragma shader_feature_local _SHADERSPACE_UV _SHADERSPACE_WORLD


			sampler2D _MainTex;
			float WindMinIntensity;
			float WindMaxIntensity;
			float WindNoiseScale;
			float WindTime;
			sampler2D _UVDistortNoiseTexture;
			sampler2D _UVDistortShaderMask;
			sampler2D _MaskMap;
			sampler2D _NormalMap;
			CBUFFER_START( UnityPerMaterial )
			float4 _UVDistortShaderMask_ST;
			float4 _MainTex_TexelSize;
			float2 _UVDistortTo;
			float2 _UVScaleScale;
			float2 _UVScalePivot;
			float2 _UVDistortNoiseScale;
			float2 _UVDistortSpeed;
			float2 _UVDistortFrom;
			float _Saturation;
			float _Contrast;
			float _Brightness;
			float _UVDistortFade;
			float _WindRotationWindFactor;
			float _WindSquishWindFactor;
			float _WindSquishFactor;
			float _WindFlip;
			float _WindMaxRotation;
			float _WindRotation;
			float _WindNoiseScale;
			float _WindXPosition;
			float _WindMaxIntensity;
			float _WindMinIntensity;
			float _Hue;
			float _NormalIntensity;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				float3 positionWS : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
				float _EnableAlphaTexture;
			#endif

			float FastNoise101_g1( float x )
			{
				float i = floor(x);
				float f = frac(x);
				float s = sign(frac(x/2.0)-0.5);
				    
				float k = 0.5+0.5*sin(i);
				return s*f*(f-1.0)*((16.0*k-4.0)*f*(f-1.0)-1.0);
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_color = v.color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;
				o.screenPosition = vertexInput.positionNDC;
				o.positionWS = vertexInput.positionWS;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				float3 positionWS = IN.positionWS.xyz;

				#ifdef _WINDLOCALWIND_ON
				float staticSwitch117_g1 = _WindMinIntensity;
				#else
				float staticSwitch117_g1 = WindMinIntensity;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch118_g1 = _WindMaxIntensity;
				#else
				float staticSwitch118_g1 = WindMaxIntensity;
				#endif
				float4 transform62_g1 = mul(GetWorldToObjectMatrix(),float4( 0,0,0,1 ));
				#ifdef _WINDISPARALLAX_ON
				float staticSwitch111_g1 = _WindXPosition;
				#else
				float staticSwitch111_g1 = transform62_g1.x;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch113_g1 = _WindNoiseScale;
				#else
				float staticSwitch113_g1 = WindNoiseScale;
				#endif
				float temp_output_50_0_g1 = ( ( staticSwitch111_g1 * staticSwitch113_g1 ) + WindTime );
				float x101_g1 = temp_output_50_0_g1;
				float localFastNoise101_g1 = FastNoise101_g1( x101_g1 );
				float2 temp_cast_0 = (temp_output_50_0_g1).xx;
				float simplePerlin2D121_g1 = snoise( temp_cast_0*0.5 );
				simplePerlin2D121_g1 = simplePerlin2D121_g1*0.5 + 0.5;
				#ifdef _WINDHIGHQUALITYNOISE_ON
				float staticSwitch123_g1 = simplePerlin2D121_g1;
				#else
				float staticSwitch123_g1 = ( localFastNoise101_g1 + 0.5 );
				#endif
				float lerpResult86_g1 = lerp( staticSwitch117_g1 , staticSwitch118_g1 , staticSwitch123_g1);
				float clampResult29_g1 = clamp( ( ( _WindRotationWindFactor * lerpResult86_g1 ) + _WindRotation ) , -_WindMaxRotation , _WindMaxRotation );
				float2 temp_output_1_0_g1 = IN.texCoord0.xy;
				float temp_output_39_0_g1 = ( temp_output_1_0_g1.y + _WindFlip );
				float3 appendResult43_g1 = (float3(0.5 , -_WindFlip , 0.0));
				float2 appendResult27_g1 = (float2(0.0 , ( _WindSquishFactor * min( ( ( _WindSquishWindFactor * abs( lerpResult86_g1 ) ) + abs( _WindRotation ) ) , _WindMaxRotation ) * temp_output_39_0_g1 )));
				float3 rotatedValue19_g1 = RotateAroundAxis( appendResult43_g1, float3( ( appendResult27_g1 + temp_output_1_0_g1 ) ,  0.0 ), float3( 0,0,1 ), ( clampResult29_g1 * temp_output_39_0_g1 ) );
				#ifdef _ENABLEWIND_ON
				float2 staticSwitch95 = (rotatedValue19_g1).xy;
				#else
				float2 staticSwitch95 = IN.texCoord0.xy;
				#endif
				float2 texCoord2_g304 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				#if defined(_SHADERSPACE_UV)
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#elif defined(_SHADERSPACE_WORLD)
				float2 staticSwitch1_g304 = (positionWS).xy;
				#else
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#endif
				float2 lerpResult21_g305 = lerp( _UVDistortFrom , _UVDistortTo , tex2D( _UVDistortNoiseTexture, ( ( staticSwitch1_g304 + ( _UVDistortSpeed * _TimeParameters.x ) ) * _UVDistortNoiseScale ) ).r);
				float2 appendResult2_g307 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 uv_UVDistortShaderMask = IN.texCoord0.xy * _UVDistortShaderMask_ST.xy + _UVDistortShaderMask_ST.zw;
				float4 tex2DNode3_g308 = tex2D( _UVDistortShaderMask, uv_UVDistortShaderMask );
				#ifdef _ENABLEUVDISTORT_ON
				float2 staticSwitch97 = ( staticSwitch95 + ( lerpResult21_g305 * ( 100.0 / appendResult2_g307 ) * ( _UVDistortFade * ( tex2DNode3_g308.r * tex2DNode3_g308.a ) ) ) );
				#else
				float2 staticSwitch97 = staticSwitch95;
				#endif
				#ifdef _ENABLEUVSCALE_ON
				float2 staticSwitch96 = ( ( ( staticSwitch97 - _UVScalePivot ) / _UVScaleScale ) + _UVScalePivot );
				#else
				float2 staticSwitch96 = staticSwitch97;
				#endif
				float4 tex2DNode17 = tex2D( _MainTex, staticSwitch96 );
				float4 temp_output_2_0_g310 = tex2DNode17;
				float4 appendResult6_g310 = (float4(( (temp_output_2_0_g310).rgb * _Brightness ) , temp_output_2_0_g310.a));
				#ifdef _ENABLEBRIGHTNESS_ON
				float4 staticSwitch101 = appendResult6_g310;
				#else
				float4 staticSwitch101 = tex2DNode17;
				#endif
				float4 temp_output_1_0_g311 = staticSwitch101;
				float3 saferPower5_g311 = abs( (temp_output_1_0_g311).rgb );
				float3 temp_cast_3 = (_Contrast).xxx;
				float4 appendResult4_g311 = (float4(pow( saferPower5_g311 , temp_cast_3 ) , temp_output_1_0_g311.a));
				#ifdef _ENABLECONTRAST_ON
				float4 staticSwitch103 = appendResult4_g311;
				#else
				float4 staticSwitch103 = staticSwitch101;
				#endif
				float4 temp_output_1_0_g312 = staticSwitch103;
				float4 break2_g313 = temp_output_1_0_g312;
				float3 temp_cast_6 = (( ( break2_g313.x + break2_g313.y + break2_g313.z ) / 3.0 )).xxx;
				float3 lerpResult5_g312 = lerp( temp_cast_6 , (temp_output_1_0_g312).rgb , _Saturation);
				float4 appendResult8_g312 = (float4(lerpResult5_g312 , temp_output_1_0_g312.a));
				#ifdef _ENABLESATURATION_ON
				float4 staticSwitch107 = appendResult8_g312;
				#else
				float4 staticSwitch107 = staticSwitch103;
				#endif
				float4 temp_output_2_0_g314 = staticSwitch107;
				float3 hsvTorgb1_g314 = RGBToHSV( temp_output_2_0_g314.rgb );
				float3 hsvTorgb3_g314 = HSVToRGB( float3(( hsvTorgb1_g314.x + _Hue ),hsvTorgb1_g314.y,hsvTorgb1_g314.z) );
				float4 appendResult8_g314 = (float4(hsvTorgb3_g314 , temp_output_2_0_g314.a));
				#ifdef _ENABLEHUE_ON
				float4 staticSwitch108 = appendResult8_g314;
				#else
				float4 staticSwitch108 = staticSwitch107;
				#endif
				
				float2 temp_output_8_0_g316 = staticSwitch96;
				
				float3 unpack14_g316 = UnpackNormalScale( tex2D( _NormalMap, temp_output_8_0_g316 ), _NormalIntensity );
				unpack14_g316.z = lerp( 1, unpack14_g316.z, saturate(_NormalIntensity) );
				
				float4 Color = ( staticSwitch108 * IN.ase_color );
				float4 Mask = tex2D( _MaskMap, temp_output_8_0_g316 );
				float3 Normal = unpack14_g316;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, IN.texCoord0.xy);
					Color.a = lerp ( Color.a, alpha.r, _EnableAlphaTexture);
				#endif
				
				Color *= IN.color;
			
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, Mask, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.texCoord0.xy, half2(IN.screenPosition.xy / IN.screenPosition.w), inputData);
				SETUP_DEBUG_DATA_2D(inputData, positionWS);
				return CombinedShapeLightShared(surfaceData, inputData);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Sprite Normal"
			Tags { "LightMode"="NormalsRendering" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 120106


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITENORMAL

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _ENABLEHUE_ON
			#pragma shader_feature_local _ENABLESATURATION_ON
			#pragma shader_feature_local _ENABLECONTRAST_ON
			#pragma shader_feature_local _ENABLEBRIGHTNESS_ON
			#pragma shader_feature_local _ENABLEUVSCALE_ON
			#pragma shader_feature_local _ENABLEUVDISTORT_ON
			#pragma shader_feature_local _ENABLEWIND_ON
			#pragma shader_feature_local _WINDLOCALWIND_ON
			#pragma shader_feature_local _WINDHIGHQUALITYNOISE_ON
			#pragma shader_feature_local _WINDISPARALLAX_ON
			#pragma shader_feature_local _SHADERSPACE_UV _SHADERSPACE_WORLD


			sampler2D _MainTex;
			float WindMinIntensity;
			float WindMaxIntensity;
			float WindNoiseScale;
			float WindTime;
			sampler2D _UVDistortNoiseTexture;
			sampler2D _UVDistortShaderMask;
			sampler2D _NormalMap;
			CBUFFER_START( UnityPerMaterial )
			float4 _UVDistortShaderMask_ST;
			float4 _MainTex_TexelSize;
			float2 _UVDistortTo;
			float2 _UVScaleScale;
			float2 _UVScalePivot;
			float2 _UVDistortNoiseScale;
			float2 _UVDistortSpeed;
			float2 _UVDistortFrom;
			float _Saturation;
			float _Contrast;
			float _Brightness;
			float _UVDistortFade;
			float _WindRotationWindFactor;
			float _WindSquishWindFactor;
			float _WindSquishFactor;
			float _WindFlip;
			float _WindMaxRotation;
			float _WindRotation;
			float _WindNoiseScale;
			float _WindXPosition;
			float _WindMaxIntensity;
			float _WindMinIntensity;
			float _Hue;
			float _NormalIntensity;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 tangentWS : TEXCOORD3;
				float3 bitangentWS : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float FastNoise101_g1( float x )
			{
				float i = floor(x);
				float f = frac(x);
				float s = sign(frac(x/2.0)-0.5);
				    
				float k = 0.5+0.5*sin(i);
				return s*f*(f-1.0)*((16.0*k-4.0)*f*(f-1.0)-1.0);
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord5.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				float3 normalWS = TransformObjectToWorldNormal( v.normal );
				o.normalWS = -GetViewForwardDir();
				float4 tangentWS = float4( TransformObjectToWorldDir( v.tangent.xyz ), v.tangent.w );
				o.tangentWS = normalize( tangentWS );
				half crossSign = (tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
				o.bitangentWS = crossSign * cross( normalWS, tangentWS.xyz ) * tangentWS.w;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#ifdef _WINDLOCALWIND_ON
				float staticSwitch117_g1 = _WindMinIntensity;
				#else
				float staticSwitch117_g1 = WindMinIntensity;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch118_g1 = _WindMaxIntensity;
				#else
				float staticSwitch118_g1 = WindMaxIntensity;
				#endif
				float4 transform62_g1 = mul(GetWorldToObjectMatrix(),float4( 0,0,0,1 ));
				#ifdef _WINDISPARALLAX_ON
				float staticSwitch111_g1 = _WindXPosition;
				#else
				float staticSwitch111_g1 = transform62_g1.x;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch113_g1 = _WindNoiseScale;
				#else
				float staticSwitch113_g1 = WindNoiseScale;
				#endif
				float temp_output_50_0_g1 = ( ( staticSwitch111_g1 * staticSwitch113_g1 ) + WindTime );
				float x101_g1 = temp_output_50_0_g1;
				float localFastNoise101_g1 = FastNoise101_g1( x101_g1 );
				float2 temp_cast_0 = (temp_output_50_0_g1).xx;
				float simplePerlin2D121_g1 = snoise( temp_cast_0*0.5 );
				simplePerlin2D121_g1 = simplePerlin2D121_g1*0.5 + 0.5;
				#ifdef _WINDHIGHQUALITYNOISE_ON
				float staticSwitch123_g1 = simplePerlin2D121_g1;
				#else
				float staticSwitch123_g1 = ( localFastNoise101_g1 + 0.5 );
				#endif
				float lerpResult86_g1 = lerp( staticSwitch117_g1 , staticSwitch118_g1 , staticSwitch123_g1);
				float clampResult29_g1 = clamp( ( ( _WindRotationWindFactor * lerpResult86_g1 ) + _WindRotation ) , -_WindMaxRotation , _WindMaxRotation );
				float2 temp_output_1_0_g1 = IN.texCoord0.xy;
				float temp_output_39_0_g1 = ( temp_output_1_0_g1.y + _WindFlip );
				float3 appendResult43_g1 = (float3(0.5 , -_WindFlip , 0.0));
				float2 appendResult27_g1 = (float2(0.0 , ( _WindSquishFactor * min( ( ( _WindSquishWindFactor * abs( lerpResult86_g1 ) ) + abs( _WindRotation ) ) , _WindMaxRotation ) * temp_output_39_0_g1 )));
				float3 rotatedValue19_g1 = RotateAroundAxis( appendResult43_g1, float3( ( appendResult27_g1 + temp_output_1_0_g1 ) ,  0.0 ), float3( 0,0,1 ), ( clampResult29_g1 * temp_output_39_0_g1 ) );
				#ifdef _ENABLEWIND_ON
				float2 staticSwitch95 = (rotatedValue19_g1).xy;
				#else
				float2 staticSwitch95 = IN.texCoord0.xy;
				#endif
				float2 texCoord2_g304 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float3 ase_worldPos = IN.ase_texcoord5.xyz;
				#if defined(_SHADERSPACE_UV)
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#elif defined(_SHADERSPACE_WORLD)
				float2 staticSwitch1_g304 = (ase_worldPos).xy;
				#else
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#endif
				float2 lerpResult21_g305 = lerp( _UVDistortFrom , _UVDistortTo , tex2D( _UVDistortNoiseTexture, ( ( staticSwitch1_g304 + ( _UVDistortSpeed * _TimeParameters.x ) ) * _UVDistortNoiseScale ) ).r);
				float2 appendResult2_g307 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 uv_UVDistortShaderMask = IN.texCoord0.xy * _UVDistortShaderMask_ST.xy + _UVDistortShaderMask_ST.zw;
				float4 tex2DNode3_g308 = tex2D( _UVDistortShaderMask, uv_UVDistortShaderMask );
				#ifdef _ENABLEUVDISTORT_ON
				float2 staticSwitch97 = ( staticSwitch95 + ( lerpResult21_g305 * ( 100.0 / appendResult2_g307 ) * ( _UVDistortFade * ( tex2DNode3_g308.r * tex2DNode3_g308.a ) ) ) );
				#else
				float2 staticSwitch97 = staticSwitch95;
				#endif
				#ifdef _ENABLEUVSCALE_ON
				float2 staticSwitch96 = ( ( ( staticSwitch97 - _UVScalePivot ) / _UVScaleScale ) + _UVScalePivot );
				#else
				float2 staticSwitch96 = staticSwitch97;
				#endif
				float4 tex2DNode17 = tex2D( _MainTex, staticSwitch96 );
				float4 temp_output_2_0_g310 = tex2DNode17;
				float4 appendResult6_g310 = (float4(( (temp_output_2_0_g310).rgb * _Brightness ) , temp_output_2_0_g310.a));
				#ifdef _ENABLEBRIGHTNESS_ON
				float4 staticSwitch101 = appendResult6_g310;
				#else
				float4 staticSwitch101 = tex2DNode17;
				#endif
				float4 temp_output_1_0_g311 = staticSwitch101;
				float3 saferPower5_g311 = abs( (temp_output_1_0_g311).rgb );
				float3 temp_cast_3 = (_Contrast).xxx;
				float4 appendResult4_g311 = (float4(pow( saferPower5_g311 , temp_cast_3 ) , temp_output_1_0_g311.a));
				#ifdef _ENABLECONTRAST_ON
				float4 staticSwitch103 = appendResult4_g311;
				#else
				float4 staticSwitch103 = staticSwitch101;
				#endif
				float4 temp_output_1_0_g312 = staticSwitch103;
				float4 break2_g313 = temp_output_1_0_g312;
				float3 temp_cast_6 = (( ( break2_g313.x + break2_g313.y + break2_g313.z ) / 3.0 )).xxx;
				float3 lerpResult5_g312 = lerp( temp_cast_6 , (temp_output_1_0_g312).rgb , _Saturation);
				float4 appendResult8_g312 = (float4(lerpResult5_g312 , temp_output_1_0_g312.a));
				#ifdef _ENABLESATURATION_ON
				float4 staticSwitch107 = appendResult8_g312;
				#else
				float4 staticSwitch107 = staticSwitch103;
				#endif
				float4 temp_output_2_0_g314 = staticSwitch107;
				float3 hsvTorgb1_g314 = RGBToHSV( temp_output_2_0_g314.rgb );
				float3 hsvTorgb3_g314 = HSVToRGB( float3(( hsvTorgb1_g314.x + _Hue ),hsvTorgb1_g314.y,hsvTorgb1_g314.z) );
				float4 appendResult8_g314 = (float4(hsvTorgb3_g314 , temp_output_2_0_g314.a));
				#ifdef _ENABLEHUE_ON
				float4 staticSwitch108 = appendResult8_g314;
				#else
				float4 staticSwitch108 = staticSwitch107;
				#endif
				
				float2 temp_output_8_0_g316 = staticSwitch96;
				float3 unpack14_g316 = UnpackNormalScale( tex2D( _NormalMap, temp_output_8_0_g316 ), _NormalIntensity );
				unpack14_g316.z = lerp( 1, unpack14_g316.z, saturate(_NormalIntensity) );
				
				float4 Color = ( staticSwitch108 * IN.color );
				float3 Normal = unpack14_g316;
				
				Color *= IN.color;

				return NormalsRenderingShared( Color, Normal, IN.tangentWS.xyz, IN.bitangentWS, IN.normalWS);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Sprite Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 120106


			#pragma vertex vert
			#pragma fragment frag


			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _ENABLEHUE_ON
			#pragma shader_feature_local _ENABLESATURATION_ON
			#pragma shader_feature_local _ENABLECONTRAST_ON
			#pragma shader_feature_local _ENABLEBRIGHTNESS_ON
			#pragma shader_feature_local _ENABLEUVSCALE_ON
			#pragma shader_feature_local _ENABLEUVDISTORT_ON
			#pragma shader_feature_local _ENABLEWIND_ON
			#pragma shader_feature_local _WINDLOCALWIND_ON
			#pragma shader_feature_local _WINDHIGHQUALITYNOISE_ON
			#pragma shader_feature_local _WINDISPARALLAX_ON
			#pragma shader_feature_local _SHADERSPACE_UV _SHADERSPACE_WORLD


			sampler2D _MainTex;
			float WindMinIntensity;
			float WindMaxIntensity;
			float WindNoiseScale;
			float WindTime;
			sampler2D _UVDistortNoiseTexture;
			sampler2D _UVDistortShaderMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _UVDistortShaderMask_ST;
			float4 _MainTex_TexelSize;
			float2 _UVDistortTo;
			float2 _UVScaleScale;
			float2 _UVScalePivot;
			float2 _UVDistortNoiseScale;
			float2 _UVDistortSpeed;
			float2 _UVDistortFrom;
			float _Saturation;
			float _Contrast;
			float _Brightness;
			float _UVDistortFade;
			float _WindRotationWindFactor;
			float _WindSquishWindFactor;
			float _WindSquishFactor;
			float _WindFlip;
			float _WindMaxRotation;
			float _WindRotation;
			float _WindNoiseScale;
			float _WindXPosition;
			float _WindMaxIntensity;
			float _WindMinIntensity;
			float _Hue;
			float _NormalIntensity;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float FastNoise101_g1( float x )
			{
				float i = floor(x);
				float f = frac(x);
				float s = sign(frac(x/2.0)-0.5);
				    
				float k = 0.5+0.5*sin(i);
				return s*f*(f-1.0)*((16.0*k-4.0)*f*(f-1.0)-1.0);
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 positionWS = IN.positionWS.xyz;

				#ifdef _WINDLOCALWIND_ON
				float staticSwitch117_g1 = _WindMinIntensity;
				#else
				float staticSwitch117_g1 = WindMinIntensity;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch118_g1 = _WindMaxIntensity;
				#else
				float staticSwitch118_g1 = WindMaxIntensity;
				#endif
				float4 transform62_g1 = mul(GetWorldToObjectMatrix(),float4( 0,0,0,1 ));
				#ifdef _WINDISPARALLAX_ON
				float staticSwitch111_g1 = _WindXPosition;
				#else
				float staticSwitch111_g1 = transform62_g1.x;
				#endif
				#ifdef _WINDLOCALWIND_ON
				float staticSwitch113_g1 = _WindNoiseScale;
				#else
				float staticSwitch113_g1 = WindNoiseScale;
				#endif
				float temp_output_50_0_g1 = ( ( staticSwitch111_g1 * staticSwitch113_g1 ) + WindTime );
				float x101_g1 = temp_output_50_0_g1;
				float localFastNoise101_g1 = FastNoise101_g1( x101_g1 );
				float2 temp_cast_0 = (temp_output_50_0_g1).xx;
				float simplePerlin2D121_g1 = snoise( temp_cast_0*0.5 );
				simplePerlin2D121_g1 = simplePerlin2D121_g1*0.5 + 0.5;
				#ifdef _WINDHIGHQUALITYNOISE_ON
				float staticSwitch123_g1 = simplePerlin2D121_g1;
				#else
				float staticSwitch123_g1 = ( localFastNoise101_g1 + 0.5 );
				#endif
				float lerpResult86_g1 = lerp( staticSwitch117_g1 , staticSwitch118_g1 , staticSwitch123_g1);
				float clampResult29_g1 = clamp( ( ( _WindRotationWindFactor * lerpResult86_g1 ) + _WindRotation ) , -_WindMaxRotation , _WindMaxRotation );
				float2 temp_output_1_0_g1 = IN.texCoord0.xy;
				float temp_output_39_0_g1 = ( temp_output_1_0_g1.y + _WindFlip );
				float3 appendResult43_g1 = (float3(0.5 , -_WindFlip , 0.0));
				float2 appendResult27_g1 = (float2(0.0 , ( _WindSquishFactor * min( ( ( _WindSquishWindFactor * abs( lerpResult86_g1 ) ) + abs( _WindRotation ) ) , _WindMaxRotation ) * temp_output_39_0_g1 )));
				float3 rotatedValue19_g1 = RotateAroundAxis( appendResult43_g1, float3( ( appendResult27_g1 + temp_output_1_0_g1 ) ,  0.0 ), float3( 0,0,1 ), ( clampResult29_g1 * temp_output_39_0_g1 ) );
				#ifdef _ENABLEWIND_ON
				float2 staticSwitch95 = (rotatedValue19_g1).xy;
				#else
				float2 staticSwitch95 = IN.texCoord0.xy;
				#endif
				float2 texCoord2_g304 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				#if defined(_SHADERSPACE_UV)
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#elif defined(_SHADERSPACE_WORLD)
				float2 staticSwitch1_g304 = (positionWS).xy;
				#else
				float2 staticSwitch1_g304 = ( texCoord2_g304 / ( 100.0 * (_MainTex_TexelSize).xy ) );
				#endif
				float2 lerpResult21_g305 = lerp( _UVDistortFrom , _UVDistortTo , tex2D( _UVDistortNoiseTexture, ( ( staticSwitch1_g304 + ( _UVDistortSpeed * _TimeParameters.x ) ) * _UVDistortNoiseScale ) ).r);
				float2 appendResult2_g307 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 uv_UVDistortShaderMask = IN.texCoord0.xy * _UVDistortShaderMask_ST.xy + _UVDistortShaderMask_ST.zw;
				float4 tex2DNode3_g308 = tex2D( _UVDistortShaderMask, uv_UVDistortShaderMask );
				#ifdef _ENABLEUVDISTORT_ON
				float2 staticSwitch97 = ( staticSwitch95 + ( lerpResult21_g305 * ( 100.0 / appendResult2_g307 ) * ( _UVDistortFade * ( tex2DNode3_g308.r * tex2DNode3_g308.a ) ) ) );
				#else
				float2 staticSwitch97 = staticSwitch95;
				#endif
				#ifdef _ENABLEUVSCALE_ON
				float2 staticSwitch96 = ( ( ( staticSwitch97 - _UVScalePivot ) / _UVScaleScale ) + _UVScalePivot );
				#else
				float2 staticSwitch96 = staticSwitch97;
				#endif
				float4 tex2DNode17 = tex2D( _MainTex, staticSwitch96 );
				float4 temp_output_2_0_g310 = tex2DNode17;
				float4 appendResult6_g310 = (float4(( (temp_output_2_0_g310).rgb * _Brightness ) , temp_output_2_0_g310.a));
				#ifdef _ENABLEBRIGHTNESS_ON
				float4 staticSwitch101 = appendResult6_g310;
				#else
				float4 staticSwitch101 = tex2DNode17;
				#endif
				float4 temp_output_1_0_g311 = staticSwitch101;
				float3 saferPower5_g311 = abs( (temp_output_1_0_g311).rgb );
				float3 temp_cast_3 = (_Contrast).xxx;
				float4 appendResult4_g311 = (float4(pow( saferPower5_g311 , temp_cast_3 ) , temp_output_1_0_g311.a));
				#ifdef _ENABLECONTRAST_ON
				float4 staticSwitch103 = appendResult4_g311;
				#else
				float4 staticSwitch103 = staticSwitch101;
				#endif
				float4 temp_output_1_0_g312 = staticSwitch103;
				float4 break2_g313 = temp_output_1_0_g312;
				float3 temp_cast_6 = (( ( break2_g313.x + break2_g313.y + break2_g313.z ) / 3.0 )).xxx;
				float3 lerpResult5_g312 = lerp( temp_cast_6 , (temp_output_1_0_g312).rgb , _Saturation);
				float4 appendResult8_g312 = (float4(lerpResult5_g312 , temp_output_1_0_g312.a));
				#ifdef _ENABLESATURATION_ON
				float4 staticSwitch107 = appendResult8_g312;
				#else
				float4 staticSwitch107 = staticSwitch103;
				#endif
				float4 temp_output_2_0_g314 = staticSwitch107;
				float3 hsvTorgb1_g314 = RGBToHSV( temp_output_2_0_g314.rgb );
				float3 hsvTorgb3_g314 = HSVToRGB( float3(( hsvTorgb1_g314.x + _Hue ),hsvTorgb1_g314.y,hsvTorgb1_g314.z) );
				float4 appendResult8_g314 = (float4(hsvTorgb3_g314 , temp_output_2_0_g314.a));
				#ifdef _ENABLEHUE_ON
				float4 staticSwitch108 = appendResult8_g314;
				#else
				float4 staticSwitch108 = staticSwitch107;
				#endif
				
				float4 Color = ( staticSwitch108 * IN.color );

				#if defined(DEBUG_DISPLAY)
					SurfaceData2D surfaceData;
					InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
					InputData2D inputData;
					InitializeInputData(positionWS.xy, half2(IN.texCoord0.xy), inputData);
					half4 debugColor = 0;

					SETUP_DEBUG_DATA_2D(inputData, positionWS);

					if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
					{
						return debugColor;
					}
				#endif

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }
        
            Cull Off
        
            HLSLPROGRAM
        
            #define ASE_SRP_VERSION 120106

        
            #pragma target 2.0
			#pragma exclude_renderers d3d11_9x 
			#pragma vertex vert
			#pragma fragment frag
        
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENESELECTIONPASS 1
        
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
			

			CBUFFER_START( UnityPerMaterial )
			float4 _UVDistortShaderMask_ST;
			float4 _MainTex_TexelSize;
			float2 _UVDistortTo;
			float2 _UVScaleScale;
			float2 _UVScalePivot;
			float2 _UVDistortNoiseScale;
			float2 _UVDistortSpeed;
			float2 _UVDistortFrom;
			float _Saturation;
			float _Contrast;
			float _Brightness;
			float _UVDistortFade;
			float _WindRotationWindFactor;
			float _WindSquishWindFactor;
			float _WindSquishFactor;
			float _WindFlip;
			float _WindMaxRotation;
			float _WindRotation;
			float _WindNoiseScale;
			float _WindXPosition;
			float _WindMaxIntensity;
			float _WindMinIntensity;
			float _Hue;
			float _NormalIntensity;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
        
            
            int _ObjectId;
            int _PassValue;
            
			
			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.clipPos = TransformWorldToHClip(positionWS);
		
				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				
				float4 Color = float4( 1, 1, 1, 1 );

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }
        
            Cull Back
        
            HLSLPROGRAM
        
            #define ASE_SRP_VERSION 120106

        
            #pragma target 2.0
			#pragma exclude_renderers d3d11_9x 
			#pragma vertex vert
			#pragma fragment frag
        
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1
        
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        	

			CBUFFER_START( UnityPerMaterial )
			float4 _UVDistortShaderMask_ST;
			float4 _MainTex_TexelSize;
			float2 _UVDistortTo;
			float2 _UVScaleScale;
			float2 _UVScalePivot;
			float2 _UVDistortNoiseScale;
			float2 _UVDistortSpeed;
			float2 _UVDistortFrom;
			float _Saturation;
			float _Contrast;
			float _Brightness;
			float _UVDistortFade;
			float _WindRotationWindFactor;
			float _WindSquishWindFactor;
			float _WindSquishFactor;
			float _WindFlip;
			float _WindMaxRotation;
			float _WindRotation;
			float _WindNoiseScale;
			float _WindXPosition;
			float _WindMaxIntensity;
			float _WindMinIntensity;
			float _Hue;
			float _NormalIntensity;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
        
            float4 _SelectionID;
        
			
			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.clipPos = TransformWorldToHClip(positionWS);
		
				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				
				float4 Color = float4( 1, 1, 1, 1 );
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "InteractiveWind2D.InteractiveWindShaderGUI"
	Fallback Off
	
}
/*ASEBEGIN
Version=19002
7;315;1715;911;-1708.491;426.1664;1;True;False
Node;AmplifyShaderEditor.TexCoordVertexDataNode;19;-3109.2,-25.78311;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-845.595,-161.8598;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;93;-2871.628,78.63365;Inherit;False;_Wind;6;;1;ec83a27ec2e35b8448baba8208c7d3fd;0;1;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;116;-3085.925,-417.5252;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;112;-2787.651,-190.6187;Inherit;False;ShaderSpace;21;;304;be729ef05db9c224caec82a3516038dc;0;1;3;SAMPLER2D;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;95;-2646.999,-32.56262;Inherit;False;Property;_EnableWind;Enable Wind;5;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;111;-2318.146,-183.8598;Inherit;False;_UVDistort;23;;305;d6b8c102b9317a0418c08eb00598bec7;0;5;27;FLOAT;0;False;28;FLOAT2;0,0;False;1;FLOAT2;0,0;False;26;SAMPLER2D;;False;3;SAMPLER2D;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;97;-1939.292,-14.77417;Inherit;False;Property;_EnableUVDistort;Enable UV Distort;20;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;96;-1270.48,-46.32493;Inherit;False;Property;_EnableUVScale;Enable UV Scale;31;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;104;238.9317,-27.199;Inherit;False;_Contrast;39;;311;4615bb5f6468d1e4b879b863721c1960;0;1;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;94;-1569.877,68.20679;Inherit;False;_UVScale;32;;309;9e0460161ab290d45839710d925e18c3;0;1;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;101;-93.03845,-148.7292;Inherit;False;Property;_EnableBrightness;Enable Brightness;35;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;103;530.9072,-146.7212;Inherit;False;Property;_EnableContrast;Enable Contrast;38;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;106;832.351,9.519784;Inherit;False;_Saturation;42;;312;d00cab46f082e684d9395d44e5a9f33a;0;1;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;107;1123.077,-79.17775;Inherit;False;Property;_EnableSaturation;Enable Saturation;41;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;105;1535.63,41.54014;Inherit;False;_Hue;45;;314;0bae267a40388e6498a395bd27428bc6;0;1;2;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;108;1899.791,-104.8561;Inherit;False;Property;_EnableHue;Enable Hue;44;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;15;2212.265,-93.82904;Inherit;False;TintVertex;-1;;315;b0b94dd27c0f3da49a89feecae766dcc;0;1;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;117;2259.168,-219.8434;Inherit;False;LitHandler;1;;316;851662d67a92ce04d84817ff63c501f2;0;1;8;FLOAT2;0,0;False;2;COLOR;0;FLOAT3;5
Node;AmplifyShaderEditor.FunctionNode;102;-389.3303,-39.81989;Inherit;False;_Brightness;36;;310;168f0b77e62f7fb42a256db752700c2b;0;1;2;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;113;2657.595,-91.1935;Float;False;True;-1;2;InteractiveWind2D.InteractiveWindShaderGUI;0;14;Interactive Wind 2D/Wind Uber Lit;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Lit;0;0;Sprite Lit;6;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;6;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;PreviewType=Plane;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;5;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;118;2657.595,-61.1935;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;SceneSelectionPass;0;3;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;114;2572.595,-82.1935;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Normal;0;1;Sprite Normal;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=NormalsRendering;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;115;2572.595,-82.1935;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;Sprite Forward;0;2;Sprite Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;119;2657.595,-61.1935;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;199187dac283dbe4a8cb1ea611d70c58;True;ScenePickingPass;0;4;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;17;0;116;0
WireConnection;17;1;96;0
WireConnection;93;1;19;0
WireConnection;112;3;116;0
WireConnection;95;1;19;0
WireConnection;95;0;93;0
WireConnection;111;28;112;0
WireConnection;111;1;95;0
WireConnection;111;3;116;0
WireConnection;97;1;95;0
WireConnection;97;0;111;0
WireConnection;96;1;97;0
WireConnection;96;0;94;0
WireConnection;104;1;101;0
WireConnection;94;1;97;0
WireConnection;101;1;17;0
WireConnection;101;0;102;0
WireConnection;103;1;101;0
WireConnection;103;0;104;0
WireConnection;106;1;103;0
WireConnection;107;1;103;0
WireConnection;107;0;106;0
WireConnection;105;2;107;0
WireConnection;108;1;107;0
WireConnection;108;0;105;0
WireConnection;15;1;108;0
WireConnection;117;8;96;0
WireConnection;102;2;17;0
WireConnection;113;1;15;0
WireConnection;113;2;117;0
WireConnection;113;3;117;5
ASEEND*/
//CHKSM=0D5FFF36694E04D6DBD32F53042ACCFA69D8CC3E