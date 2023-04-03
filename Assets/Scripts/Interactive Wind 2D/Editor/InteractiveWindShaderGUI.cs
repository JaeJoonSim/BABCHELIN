using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InteractiveWindShaderGUI : ShaderGUI
{
    static bool textureSetup;
    static bool windSetup;
    static bool interactionSetup;
    static bool otherInfo;

    static bool showMainTexture;
    static bool showBonus;
    static bool showLit;

    bool isOpen;

    static Shader currentDefaultShader;
    static float[] defaultFloats;
    static Vector4[] defaultVectors;
    static Color[] defaultColors;

    float lastParallax;
    float lastLocalWind;

    public static bool showUpgradeInfo;


}