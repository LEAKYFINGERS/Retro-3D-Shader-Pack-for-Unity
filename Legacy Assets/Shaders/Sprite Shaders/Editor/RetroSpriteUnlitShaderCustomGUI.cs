﻿//////////////////////////////////////////////////
// Author:				LEAKYFINGERS
// Date created:		24.10.19
// Date last edited:	03.11.19
// References:          https://docs.unity3d.com/Manual/SL-CustomShaderGUI.html
//                      https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
//////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEditor;

// The class which defines a custom GUI for materials using the Retro 3D Unlit shaders.
public class RetroSpriteUnlitShaderCustomGUI : ShaderGUI
{
    public enum VertexJitterSpace
    {
        World,
        Screen
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        this.materialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        ShaderPropertiesGUI(material);
    }


    // The static class containing the strings used for the GUI labels and tooltips.
    private static class Styles
    {
        public static string ColorTintText = "Color Tint"; 

        public static string RetroText = "Retro Properties";      
        public static string VertexJitterIntensityText = "Vertex Jitter Intensity";
        public static GUIContent VertexJitterSpaceText = new GUIContent("Vertex Jitter Space");
        public static GUIContent AffineMapText = new GUIContent("Affine Texture Mapping", "Disable for default perspective-correct texture mapping");        
        public static string DrawDistanceText = "Vertex Draw Distance";
    }
        
    private MaterialEditor materialEditor;
    private MaterialProperty colorTint = null;
    private MaterialProperty vertexJitter = null;
    private MaterialProperty drawDistance = null;

    // Gets the properties used by the shader and uses them to update the corresponding member variables.
    private void FindProperties(MaterialProperty[] properties)
    {
        colorTint = FindProperty("_ColorTint", properties);
        vertexJitter = FindProperty("_VertJitter", properties);
        drawDistance = FindProperty("_DrawDist", properties);
    }

    // Displays the shader properties on the material GUI.
    private void ShaderPropertiesGUI(Material material)
    {       
        EditorGUIUtility.labelWidth = 0.0f; // Use default label width.
                        
        DoColorTintArea(material);

        EditorGUILayout.Space();

        // The 'Retro Properties' section:
        GUILayout.Label(Styles.RetroText, EditorStyles.boldLabel);
        DoVertexJitterArea(material);
        DoAffineMappingArea(material);
        DoDrawDistanceArea(material);
    }

    private void DoColorTintArea(Material material)
    {
        materialEditor.ColorProperty(colorTint, Styles.ColorTintText);
    }

    private void DoVertexJitterArea(Material material)
    {
        materialEditor.RangeProperty(vertexJitter, Styles.VertexJitterIntensityText);

        VertexJitterSpace vertexJitterSpace = Array.IndexOf(material.shaderKeywords, "ENABLE_SCREENSPACE_JITTER") != -1 ? VertexJitterSpace.Screen : VertexJitterSpace.World;
        EditorGUI.BeginChangeCheck();
        vertexJitterSpace = (VertexJitterSpace)EditorGUILayout.EnumPopup(Styles.VertexJitterSpaceText, vertexJitterSpace);
        if (EditorGUI.EndChangeCheck())
        {
            if (vertexJitterSpace == VertexJitterSpace.Screen)
                material.EnableKeyword("ENABLE_SCREENSPACE_JITTER");
            else if (vertexJitterSpace == VertexJitterSpace.World)
                material.DisableKeyword("ENABLE_SCREENSPACE_JITTER");
        }
    }

    private void DoAffineMappingArea(Material material)
    {
        bool affineMappingEnabled = Array.IndexOf(material.shaderKeywords, "ENABLE_AFFINE_TEXTURE_MAPPING") != -1;

        EditorGUI.BeginChangeCheck();

        affineMappingEnabled = EditorGUILayout.Toggle(Styles.AffineMapText, affineMappingEnabled);

        if (EditorGUI.EndChangeCheck())
        {
            if (affineMappingEnabled)
                material.EnableKeyword("ENABLE_AFFINE_TEXTURE_MAPPING");
            else
                material.DisableKeyword("ENABLE_AFFINE_TEXTURE_MAPPING");
        }
    }

    private void DoDrawDistanceArea(Material material)
    {
        materialEditor.FloatProperty(drawDistance, Styles.DrawDistanceText);
    }
}