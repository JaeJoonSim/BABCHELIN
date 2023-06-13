#if UNITY_EDITOR
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeMaterial : Editor
{
    [MenuItem("Material/ChangeMaterial")]
    public static void ChangeMat()
    {
        GameObject[] sprites = Selection.gameObjects;
        foreach (GameObject sprite in sprites)
        {
            var s = sprite.GetComponent<SpriteRenderer>();
            if (s != null)
            {
                var path = AssetDatabase.GUIDToAssetPath("72372d8c6b7d87247a37325d4825ce7a");
                s.material = AssetDatabase.LoadAssetAtPath<Material>(path);
                s.sortingOrder = 0;
            }

            var m = sprite.GetComponent<MeshRenderer>();
            if(m != null && sprite.GetComponent<SkeletonAnimation>())
            {
                m.material.renderQueue = 3002;
            }

        }
    }
}
#endif