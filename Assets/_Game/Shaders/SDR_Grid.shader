// Made with Amplify Shader Editor v1.9.1.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpellCards/Grid"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,1)
		_EnemyZoneColor("Enemy Zone Color", Color) = (1,0,0,0)
		_LineWidth("Line Width", Range( 0 , 1)) = 0.13
		_GridSize("Grid Size", Vector) = (12,7,0,0)
		_EnemyZoneCoverage("Enemy Zone Coverage", Range( 1 , 7)) = 1
		_EnemyZoneLineOpacity("Enemy Zone Line Opacity", Float) = 1
		_EnemyZoneCellOpacity("Enemy Zone Cell Opacity", Float) = 0

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
			uniform float4 _EnemyZoneColor;
			uniform float _EnemyZoneCoverage;
			uniform float2 _GridSize;
			uniform float _LineWidth;
			uniform float _EnemyZoneCellOpacity;
			uniform float _EnemyZoneLineOpacity;

			
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
				float GridHeight66 = _GridSize.y;
				float2 texCoord68 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float HeightMask74 = step( (1.0 + (floor( _EnemyZoneCoverage ) - 0.0) * (0.0 - 1.0) / (GridHeight66 - 0.0)) , saturate( ( floor( ( texCoord68.y * GridHeight66 ) ) / GridHeight66 ) ) );
				float4 lerpResult82 = lerp( _Tint , _EnemyZoneColor , HeightMask74);
				float2 texCoord11 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.5).xx;
				float2 temp_cast_1 = (0.5).xx;
				float2 break25 = ( ( abs( ( frac( ( ( texCoord11 * _GridSize ) - temp_cast_0 ) ) - temp_cast_1 ) ) * (_GridSize).yx ) * 0.25 );
				float blendOpSrc28 = break25.x;
				float blendOpDest28 = break25.y;
				float temp_output_3_0_g1 = ( _LineWidth - ( saturate( min( blendOpSrc28 , blendOpDest28 ) )) );
				float GridLines73 = saturate( ( temp_output_3_0_g1 / fwidth( temp_output_3_0_g1 ) ) );
				float lerpResult86 = lerp( ( _Tint.a * GridLines73 ) , ( _EnemyZoneCellOpacity + ( _EnemyZoneLineOpacity * GridLines73 ) ) , HeightMask74);
				float4 appendResult5 = (float4((lerpResult82).rgb , saturate( lerpResult86 )));
				
				
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
Node;AmplifyShaderEditor.CommentaryNode;72;-2913.45,-707.8255;Inherit;False;2730.683;551.6721;;19;73;38;43;66;11;39;13;18;40;47;46;28;25;17;15;14;20;19;16;Grid Layout;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;71;-2913.839,-1420.555;Inherit;False;1594.956;540.667;;13;74;70;69;67;68;51;65;59;52;56;50;60;57;Grid Height Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;-2236.669,-1146.093;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;60;-1998.669,-1147.093;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;50;-1753.221,-1357.755;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;56;-2431.669,-1146.093;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2610.521,-1146.656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;59;-2056.668,-1366.092;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;65;-2209.839,-1363.713;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;68;-2860.839,-1184.713;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;67;-2863.839,-1028.714;Inherit;False;66;GridHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-2288.839,-1281.713;Inherit;False;66;GridHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;70;-2352.839,-1009.714;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;16;-1849.626,-656.0775;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1666.584,-656.1;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;20;-1859.436,-335.8317;Inherit;False;FLOAT2;1;0;2;3;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-2574.316,-654.6013;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;15;-2131.626,-655.0775;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-2007.626,-655.0775;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;25;-1135.319,-652.8253;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BlendOpsNode;28;-985.3187,-657.8255;Inherit;True;Darken;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1288.143,-653.506;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1460.143,-563.5061;Inherit;False;Constant;_Width;Width;3;0;Create;True;0;0;0;False;0;False;0.25;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2545.464,-546.8256;Inherit;False;Constant;_GridCenterOffset;GridCenterOffset;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2240.626,-548.0776;Inherit;False;Constant;_OffsetIndividualUVs;OffsetIndividualUVs;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;13;-2779.318,-334.9955;Inherit;False;Property;_GridSize;Grid Size;3;0;Create;True;0;0;0;False;0;False;12,7;11,6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-2329.464,-654.8255;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2863.45,-653.2907;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-2607.81,-270.1536;Inherit;False;GridHeight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;43;-681.766,-655.3075;Inherit;True;Step Antialiasing;-1;;1;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-457.0027,-658.4922;Inherit;True;GridLines;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-996.8846,-395.2061;Inherit;False;Property;_LineWidth;Line Width;2;0;Create;True;0;0;0;False;0;False;0.13;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1541.756,-1364.79;Inherit;False;HeightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;1016.182,-620.743;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;1017.08,-774.338;Inherit;False;74;HeightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;2050.609,-824.972;Float;False;True;-1;2;ASEMaterialInspector;100;5;SpellCards/Grid;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.DynamicAppendNode;5;1906.577,-824.4122;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;83;1211.08,-922.338;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;84;627.0801,-1085.338;Inherit;False;Property;_EnemyZoneColor;Enemy Zone Color;1;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;632.3203,-900.1341;Inherit;False;Property;_Tint;Tint;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,0.2;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;41;1635.17,-1070.317;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;82;1340.08,-1066.338;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;690.2708,-603.4336;Inherit;False;73;GridLines;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2546.321,-1370.555;Inherit;False;Property;_EnemyZoneCoverage;Enemy Zone Coverage;4;0;Create;True;0;0;0;False;0;False;1;0;1;7;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;81;1714.08,-757.338;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;1246.71,-567.3668;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;1499.71,-521.3668;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;1013.08,-698.338;Inherit;False;Property;_EnemyZoneLineOpacity;Enemy Zone Line Opacity;5;0;Create;True;0;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;1258.71,-287.3668;Inherit;False;Property;_EnemyZoneCellOpacity;Enemy Zone Cell Opacity;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;86;1494.71,-706.3668;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
WireConnection;57;0;56;0
WireConnection;57;1;70;0
WireConnection;60;0;57;0
WireConnection;50;0;59;0
WireConnection;50;1;60;0
WireConnection;56;0;52;0
WireConnection;52;0;68;2
WireConnection;52;1;67;0
WireConnection;59;0;65;0
WireConnection;59;2;69;0
WireConnection;65;0;51;0
WireConnection;70;0;67;0
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
WireConnection;28;0;25;0
WireConnection;28;1;25;1
WireConnection;46;0;19;0
WireConnection;46;1;47;0
WireConnection;39;0;14;0
WireConnection;39;1;40;0
WireConnection;66;0;13;2
WireConnection;43;1;28;0
WireConnection;43;2;38;0
WireConnection;73;0;43;0
WireConnection;74;0;50;0
WireConnection;42;0;8;4
WireConnection;42;1;75;0
WireConnection;1;0;5;0
WireConnection;5;0;41;0
WireConnection;5;3;81;0
WireConnection;83;0;76;0
WireConnection;41;0;82;0
WireConnection;82;0;8;0
WireConnection;82;1;84;0
WireConnection;82;2;83;0
WireConnection;81;0;86;0
WireConnection;87;0;79;0
WireConnection;87;1;75;0
WireConnection;88;0;89;0
WireConnection;88;1;87;0
WireConnection;86;0;42;0
WireConnection;86;1;88;0
WireConnection;86;2;76;0
ASEEND*/
//CHKSM=F862374EEEFDB60C30317CFB0AD57A64B6960BF4