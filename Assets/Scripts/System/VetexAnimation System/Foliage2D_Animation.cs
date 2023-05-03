using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Foliage
{
    #region Enums
    public enum Foliage2D_MeshBending
    {
        Simple,
        Smart
    }
    #endregion

    public class Foliage2D_Animation : MonoBehaviour
    {
        #region Private fields
        private List<Collider2D> collider2DObject;
        private List<Collider> collider3DObject;
        private List<float> enterOffset;
        private Vector3[] initialVertexPos;
        private Vector3[] finalVertexPos;
        private Vector3[] posOffset;
        private Vector2[] centerLinePoints;
        private Foliage2D foliage2D;
        private Animator animator;
        private Mesh meshFilter;
        private bool isBending = false;
        private int horizontalVerts;
        private float[] anglesInDeg;
        #endregion

        #region Public fields
        public Foliage2D_MeshBending foliageBending = Foliage2D_MeshBending.Simple;

        public List<float> offsetFactor = new List<float>();

        public Vector3 offset;

        public bool changeAnimationSpeed = false;

        public float minSpeed = 0.5f;

        public float maxSpeed = 2.0f;

        public float minSeconds = 0;

        public float maxSeconds = 1.5f;
        #endregion

        #region Class methods
        void Start()
        {
            animator = GetComponent<Animator>();
            foliage2D = GetComponent<Foliage2D>();
            meshFilter = GetComponent<MeshFilter>().sharedMesh;
            initialVertexPos = meshFilter.vertices;
            horizontalVerts = foliage2D.widthSegments + 1;
            finalVertexPos = meshFilter.vertices;
            collider2DObject = new List<Collider2D>();
            collider3DObject = new List<Collider>();
            enterOffset = new List<float>();
            anglesInDeg = new float[foliage2D.heightSegments + 1];
            posOffset = new Vector3[foliage2D.heightSegments + 1];
            centerLinePoints = new Vector2[foliage2D.heightSegments + 1];

            if (animator != null && changeAnimationSpeed)
            {
                StartCoroutine(SetAnimationSpeed());
            }

            if (offsetFactor.Count != foliage2D.heightSegments + 1)
            {
                offsetFactor.Clear();
                int len = foliage2D.heightSegments + 1;
                float offset = 1f / foliage2D.heightSegments;

                for (int i = 0; i < len; i++)
                {
                    offsetFactor.Add(offset * i);
                }
            }
        }

        IEnumerator SetAnimationSpeed()
        {
            animator.speed = Random.Range(minSpeed, maxSpeed);

            yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));

            animator.speed = 1f;
        }

        void Update()
        {
            UpdateVertsPos();

            int len = collider2DObject.Count;

            for (int i = 0; i < len; i++)
            {
                float offset = collider2DObject[i].transform.position.x - transform.position.x;

                if ((Mathf.Sign(enterOffset[i]) != Mathf.Sign(offset)) && !isBending && animator != null)
                {
                    if (collider2DObject[i].transform.position.x > transform.position.x)
                    {
                        Debug.Log("Right");
                        animator.SetTrigger("tRight");
                    }
                    else
                    {
                        Debug.Log("Left");
                        animator.SetTrigger("tLeft");
                    }

                    isBending = true;
                }
            }

            meshFilter.vertices = finalVertexPos;
        }

        private void UpdateVertsPos()
        {
            if (foliageBending == Foliage2D_MeshBending.Simple)   
                SimpleMeshBending();  
            else
                SmartMeshBending();  
        }

        private void SimpleMeshBending()
        {
            int hLen = foliage2D.heightSegments + 1;
            int wLen = foliage2D.widthSegments + 1;

            for (int i = 0; i < hLen; i++)
            {
                posOffset[i] = new Vector3(offset.x * offsetFactor[i], offset.y * offsetFactor[i], offset.z * offsetFactor[i]);

                for (int j = 0; j < wLen; j++)
                {
                    int vertIndex = horizontalVerts * i + j;

                    finalVertexPos[vertIndex] = new Vector3(
                        initialVertexPos[vertIndex].x + posOffset[i].x,
                        initialVertexPos[vertIndex].y + posOffset[i].y,
                        initialVertexPos[vertIndex].z + posOffset[i].z);
                }
            }
        }

        private void SmartMeshBending()
        { 
            int dir = offset.x < 0 ? -1 : 1;
            float heightOfSegment = foliage2D.height / foliage2D.heightSegments;
            centerLinePoints[0] = new Vector2(offset.x * offsetFactor[0], initialVertexPos[0].y);

            int hLen = foliage2D.heightSegments + 1;
            int wLen = foliage2D.widthSegments + 1;
            for (int i = 1; i < hLen; i++)
            {
                float xOffset = offset.x * offsetFactor[i];

                if (Mathf.Abs(xOffset) < heightOfSegment)
                    centerLinePoints[i] = new Vector2(centerLinePoints[i - 1].x + xOffset, centerLinePoints[i - 1].y + Mathf.Sqrt((heightOfSegment * heightOfSegment) - (xOffset * xOffset)));
                else
                {
                    float y = Mathf.Abs(xOffset) - heightOfSegment;
                    centerLinePoints[i] = new Vector2(centerLinePoints[i - 1].x + dir * Mathf.Sqrt((heightOfSegment * heightOfSegment) - (y * y)), centerLinePoints[i - 1].y - y);
                }
            }

            for (int i = 1; i < hLen; i++)
            {
                Vector2 line = centerLinePoints[i] - centerLinePoints[i - 1];
                anglesInDeg[i] = Mathf.Atan2(line.y, line.x) * Mathf.Rad2Deg - 90f;

                float angleInRadians = anglesInDeg[i] * Mathf.Deg2Rad;

                for (int j = 0; j < wLen; j++)
                {
                    int vertIndex = horizontalVerts * i + j;

                    finalVertexPos[vertIndex] = new Vector3(
                        initialVertexPos[vertIndex].x + centerLinePoints[i].x,
                        centerLinePoints[i].y,
                        initialVertexPos[vertIndex].z);

                    Vector3 vertCurPos = finalVertexPos[vertIndex];

                    finalVertexPos[vertIndex] = new Vector3(
                        (vertCurPos.x - centerLinePoints[i].x) * Mathf.Cos(angleInRadians) - (vertCurPos.y - centerLinePoints[i].y) * Mathf.Sin(angleInRadians) + centerLinePoints[i].x,
                        (vertCurPos.x - centerLinePoints[i].x) * Mathf.Sin(angleInRadians) + (vertCurPos.y - centerLinePoints[i].y) * Mathf.Cos(angleInRadians) + centerLinePoints[i].y,
                        0.0f);     
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!collider3DObject.Contains(other))
            {
                collider3DObject.Add(other);

                enterOffset.Add(other.transform.position.x - transform.position.x);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (collider3DObject.Contains(other))
            {
                int index = collider3DObject.IndexOf(other);
                collider3DObject.Remove(other);
                enterOffset.RemoveAt(index);
            }

            isBending = false;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!collider2DObject.Contains(other))
            {
                Debug.Log(other.name);
                collider2DObject.Add(other);

                enterOffset.Add(other.transform.position.x - transform.position.x);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (collider2DObject.Contains(other))
            {
                int index = collider2DObject.IndexOf(other);
                collider2DObject.Remove(other);
                enterOffset.RemoveAt(index);
            }

            isBending = false;
        }
        #endregion
    }
}
