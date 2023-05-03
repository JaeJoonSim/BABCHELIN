using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Foliage
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Foliage2D : MonoBehaviour
    {
        #region Private fields

        private Foliage2D_Mesh dynamicMesh;
        private Vector2 unitsPerUV = Vector2.one;
        private Mesh mesh;
        private float scaleX = 1f;
        private float scaleY = 1f;
        private int hVerts = 1;
        private int vVerts = 1;
        private Foliage2D_Mesh DynamicMesh
        {
            get
            {
                if (dynamicMesh == null)
                    dynamicMesh = new Foliage2D_Mesh();
                return dynamicMesh;
            }
        }
        #endregion

        #region Public fields
        public bool prefabInstanceIsCreated = false;

        public float pixelsPerUnit = 100f;

        public float width = 1.0f;

        public float height = 1.0f;

        public int widthSegments = 3;

        public int heightSegments = 3;
        #endregion

        #region Class methods
        /// <summary>
        /// 버텍스 위치 및 UV 설정
        /// </summary>
        public void RebuildMesh()
        {
            DynamicMesh.Clear();

            unitsPerUV.x = GetComponent<Renderer>().sharedMaterial.mainTexture.width / pixelsPerUnit;
            unitsPerUV.y = GetComponent<Renderer>().sharedMaterial.mainTexture.height / pixelsPerUnit;

            width = unitsPerUV.x;
            height = unitsPerUV.y;

            hVerts = widthSegments + 1;
            vVerts = heightSegments + 1;

            scaleX = width / widthSegments;
            scaleY = height / heightSegments;

            float startX = -unitsPerUV.x / 2f;
            float startY = -unitsPerUV.y / 2f;

            for (int y = 0; y < vVerts; y++)
            {
                for (int x = 0; x < hVerts; x++)
                {
                    Vector3 vertPos = new Vector3(x * scaleX + startX, y * scaleY + startY, 0.0f);
                    float U = (vertPos.x / width) + 0.5f;
                    float V = y == 0 ? 0 : (scaleY * y) / height;
                    DynamicMesh.AddVertex(vertPos, 0.0f, new Vector2(U, V));
                }
            }

            DynamicMesh.GenerateTriangles(widthSegments, heightSegments, hVerts);

            mesh = GetComponent<MeshFilter>().sharedMesh = GetMesh();
            string name = GetMeshName();
            if (mesh == null || mesh.name != name)
            {
                mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();
                mesh.name = name;
            }

            DynamicMesh.Build(ref mesh);

            if (prefabInstanceIsCreated)
                prefabInstanceIsCreated = false;
        }

        /// <summary>
        /// 개체에 메쉬가 없는 경우 메쉬를 변환하거나 메쉬를 추가
        /// </summary>
        Mesh GetMesh()
        {
            MeshFilter filter;

            filter = GetComponent<MeshFilter>();

            string newName = GetMeshName();
            Mesh result = filter.sharedMesh;

            if (prefabInstanceIsCreated)
            {
#if UNITY_EDITOR
                if (filter.sharedMesh == null || filter.sharedMesh.name != newName)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(this) + "/FoliageMeshes/" + newName + ".asset";
                    Mesh assetMesh = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
                    if (assetMesh != null)
                    {
                        result = assetMesh;
                    }
                    else
                    {
                        path = Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this)) + "/FoliageMeshes";
                        string assetName = "/" + newName + ".asset";
                        result = new Mesh();
                        result.name = newName;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        try
                        {
                            UnityEditor.AssetDatabase.CreateAsset(result, path + assetName);
                            UnityEditor.AssetDatabase.Refresh();
                        }
                        catch
                        {
                            Debug.LogError("프리팹을 저장할 수 없습니다. 메시 파일을 삭제 했지만 프리팹에서 여전히 참조하고 있을 수 있습니다. Unity를 다시 시작하면 문제가 해결됩니다.");
                        }
                    }
                }
#endif
            }
            else
            {
                if (filter.sharedMesh == null || filter.sharedMesh.name != newName)
                    result = new Mesh();

            }
            result.name = newName;
            return result;
        }

        public string GetMeshName()
        {
            if (prefabInstanceIsCreated)
            {
                string name;
                Transform curr;

                name = gameObject.name;
                curr = gameObject.transform.parent;

                while (curr != null) { name = curr.name + "." + name; curr = curr.transform.parent; }
                name += "-Mesh";

                return name;
            }
            else
            {
                return string.Format("{0}{1}-Mesh", gameObject.name, gameObject.GetInstanceID());
            }
        }

        /// <summary>
        /// 초기 마테리얼 설정
        /// </summary>
        public void SetDefaultMaterial()
        {
            Renderer rend = GetComponent<Renderer>();
            Material m = Resources.Load("Default_FoliageMaterial", typeof(Material)) as Material;
            if (m != null)
            {
                rend.material = m;

                unitsPerUV.x = GetComponent<Renderer>().sharedMaterial.mainTexture.width / pixelsPerUnit;
                unitsPerUV.y = GetComponent<Renderer>().sharedMaterial.mainTexture.height / pixelsPerUnit;
            }
            else
            {
                Debug.LogWarning("마테리얼을 찾을 수 없습니다. 이 문제가 발생하는 가장 큰 이유는 하위 폴더를 이동하거나 리소스 폴더에서 Default_FoliageMaterial 이 삭제되거나 이름을 변경했기 때문입니다. 마테리얼의 이름을 변경한 경우 새 이름을 설정하려면 이 메시지를 클릭해주세요.");
            }
        }
        #endregion
    }
}
