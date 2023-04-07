using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    private static Matrix4x4 matrix = Matrix4x4.identity;
    public static bool gizmos = true;

    private static void SetColor(Color color)
    {
        if (gizmos && Gizmos.color != color)
        {
            Gizmos.color = color;
        }
    }

    public static void DrawLine(Vector3 a, Vector3 b, Color color)
    {
        SetColor(color);
        if(gizmos)
        {
            Gizmos.DrawLine(matrix.MultiplyPoint3x4(a), matrix.MultiplyPoint3x4(b));
        }
        else
        {
            Debug.DrawLine(matrix.MultiplyPoint3x4(a), matrix.MultiplyPoint3x4(b), color);
        }
    }

}
