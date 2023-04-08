// Made with Amplify Shader Editor v1.9.1.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpellCards/Grid"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,0)
		_LineWidth("Line Width", Range( 0 , 1)) = 0.13
		_GridSize("Grid Size", Vector) = (11,6,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _Tint;
			uniform float _LineWidth;
			uniform float2 _GridSize;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 texCoord11 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.5).xx;
				float2 temp_cast_1 = (0.5).xx;
				float2 break25 = ( ( abs( ( frac( ( ( texCoord11 * _GridSize ) - temp_cast_0 ) ) - temp_cast_1 ) ) * (_GridSize).yx ) * 0.25 );
				float blendOpSrc28 = break25.x;
				float blendOpDest28 = break25.y;
				float temp_output_3_0_g1 = ( _LineWidth - ( saturate( min( blendOpSrc28 , blendOpDest28 ) )) );
				float4 appendResult5 = (float4((_Tint).rgb , ( _Tint.a * saturate( ( temp_output_3_0_g1 / fwidth( temp_output_3_0_g1 ) ) ) )));
				
				
				finalColor = appendResult5;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19103
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-1096.638,-581.248;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;16;-632.7996,-581.5;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-449.7576,-581.5225;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;20;-642.6093,-261.2539;Inherit;False;FLOAT2;1;0;2;3;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1357.49,-580.0238;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1646.622,-578.7132;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;15;-914.7996,-580.5;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-790.7996,-580.5;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;25;81.50794,-578.2478;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;43;651.002,-580.73;Inherit;True;Step Antialiasing;-1;;1;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;625.3203,-762.1341;Inherit;False;Property;_Tint;Tint;0;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0.2;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;41;851.17,-761.3171;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1233.609,-756.972;Float;False;True;-1;2;ASEMaterialInspector;100;5;SpellCards/Grid;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.BlendOpsNode;28;231.508,-583.248;Inherit;True;Darken;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;5;1089.577,-756.4122;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-71.31612,-578.9285;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-243.3162,-488.9285;Inherit;False;Constant;_Width;Width;3;0;Create;True;0;0;0;False;0;False;0.25;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;903.182,-641.743;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1328.638,-472.248;Inherit;False;Constant;_GridCenterOffset;GridCenterOffset;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1023.8,-473.5;Inherit;False;Constant;_OffsetIndividualUVs;OffsetIndividualUVs;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;13;-1562.491,-260.4177;Inherit;False;Property;_GridSize;Grid Size;2;0;Create;True;0;0;0;False;0;False;11,6;11,6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;38;488.8834,-499.6283;Inherit;False;Property;_LineWidth;Line Width;1;0;Create;True;0;0;0;False;0;False;0.13;0.16;0;1;0;1;FLOAT;0
WireConnection;39;0;14;0
WireConnection;39;1;40;0
WireConnection;16;0;17;0
WireConnection;19;0;16;0
WireConnection;19;1;20;0
WireConnection;20;0;13;0
WireConnection;14;0;11;0
WireConnection;14;1;13;0
WireConnection;15;0;39;0
WireConnection;17;0;15;0
WireConnection;17;1;18;0
WireConnection;25;0;46;0
WireConnection;43;1;28;0
WireConnection;43;2;38;0
WireConnection;41;0;8;0
WireConnection;1;0;5;0
WireConnection;28;0;25;0
WireConnection;28;1;25;1
WireConnection;5;0;41;0
WireConnection;5;3;42;0
WireConnection;46;0;19;0
WireConnection;46;1;47;0
WireConnection;42;0;8;4
WireConnection;42;1;43;0
ASEEND*/
//CHKSM=F472419E7B87A7871E9C9BE0E59582613C59D627