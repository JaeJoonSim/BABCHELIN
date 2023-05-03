using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Foliage
{
    #region Enums
    public enum Foliage2D_Pattern
    {
        Random,
        Consecutive
    }

    public enum Foliage2D_OverlappingType
    {
        Fixed,
        Random
    }

    public enum Foliage2D_PathType
    {
        Linear,
        Smooth
    }
    #endregion

    public class Foliage2D_Path : MonoBehaviour
    {
        #region Private fields
        private Foliage2D foliage2D;
        private Vector2 posOffset;
        private Vector2 lineNormal;
        private Vector2 pointOnTheLine;
        private Vector2 line;
        private float foliagePrefabWidth;
        private float distanceFromStart = 0;
        private float prevDistanceFromStart = 0;
        private float previousWidth = 0;
        private float lineLength;
        private float angleInRad;
        private float angleInDeg;
        private int foliageCount;
        private int objectIndex;
        private int prevIndex;
        private int maxCount;
        private int prefabIndex = -1;
        private bool updatedPrefabIndex = false;
        #endregion

        #region Public fields
        public Foliage2D_Pattern foliagePattern = Foliage2D_Pattern.Random;
        public Foliage2D_OverlappingType foliageOverlapType = Foliage2D_OverlappingType.Fixed;
        public Foliage2D_PathType foliagePathType = Foliage2D_PathType.Linear;
        public List<Vector2> handlesPosition = new List<Vector2>();
        public List<Vector2> handleControlsPos = new List<Vector2>();
        public List<GameObject> foliageOnPath = new List<GameObject>();
        public List<GameObject> foliagePrefabs = new List<GameObject>();
        public List<float> nodeTension = new List<float>();
        public List<float> nodeBias = new List<float>();
        public float overlappingFactor = 0.4f;
        public float minOverlappingFactor = 0f;
        public float maxOverlappingFactor = 0.8f;
        public float bias = 0f;
        public float tension = 0f;
        public float firstObjectOffset = 0f;
        public float lastObjectOffset = 0f;
        public float biasScale = 4f;
        public float tensionScale = 4f;
        public float zOffset = 0.01f;
        public bool uniformValues = true;
        public int foliagePrefabListSize = 1;
        #endregion

        #region Class methods
        public void RecreateFoliage()
        {
            foliageCount = foliageOnPath.Count;
            distanceFromStart = 0;
            prevDistanceFromStart = 0;
            maxCount = 0;
            float ZAxisOffset = zOffset;

            List<GameObject> foliageP = new List<GameObject>();
            int len = foliagePrefabs.Count;

            foliageP.Clear();
            for (int k = 0; k < len; k++)
            {
                if (foliagePrefabs[k] != null)
                    foliageP.Add(foliagePrefabs[k]);
            }
            maxCount = foliageP.Count;

            if (maxCount == 0)
                return;
            objectIndex = 1;

            for (int i = 0; i < handlesPosition.Count - 1; i++)
            {
                line = handlesPosition[i + 1] - handlesPosition[i];
                angleInRad = Mathf.Atan2(line.y, line.x);
                angleInDeg = angleInRad * Mathf.Rad2Deg;
                lineLength = line.magnitude;
                line.Normalize();
                lineNormal = new Vector2(-line.y, line.x);
                distanceFromStart = 0;
                prevDistanceFromStart = 0;

                while (true)
                {
                    updatedPrefabIndex = false;

                    if (foliageCount < objectIndex)
                    {
                        if (foliagePattern == Foliage2D_Pattern.Random)
                        {
                            prefabIndex = Random.Range(0, maxCount);
                        }
                        else
                        {
                            prefabIndex++;
                            if (prefabIndex >= maxCount || prefabIndex < 0)
                                prefabIndex = 0;
                        }

                        updatedPrefabIndex = true;
                        foliage2D = foliageP[prefabIndex].GetComponent<Foliage2D>();
                        DistanceFromStart();
                    }
                    else
                    {
                        foliage2D = foliageOnPath[objectIndex - 1].GetComponent<Foliage2D>();
                        DistanceFromStart();
                    }

                    if (distanceFromStart > lineLength)
                    {
                        foliage2D = foliageOnPath[prevIndex].GetComponent<Foliage2D>();
                        distanceFromStart = lineLength - foliage2D.width / 2f + lastObjectOffset;

                        if (foliagePathType == Foliage2D_PathType.Smooth)
                            SmoothPath(i);
                        else
                            pointOnTheLine = Vector2.Lerp(handlesPosition[i], handlesPosition[i + 1], distanceFromStart / lineLength);

                        posOffset = lineNormal * (foliage2D.height / 2f);
                        foliageOnPath[prevIndex].transform.position = transform.TransformPoint(new Vector3(pointOnTheLine.x + posOffset.x, pointOnTheLine.y + posOffset.y, ZAxisOffset));

                        if (updatedPrefabIndex && foliagePattern == Foliage2D_Pattern.Consecutive)
                            RestorePrefabIndex();

                        break;
                    }

                    if (foliagePathType == Foliage2D_PathType.Smooth)
                        SmoothPath(i);
                    else
                        pointOnTheLine = Vector2.Lerp(handlesPosition[i], handlesPosition[i + 1], distanceFromStart / lineLength);

                    if (foliageCount < objectIndex)
                    {
                        ZAxisOffset *= -1;
                        Vector3 pos = pointOnTheLine + posOffset;
                        pos.z = zOffset;
                        GameObject obj = Instantiate(foliageP[prefabIndex], transform.TransformPoint(pos), Quaternion.Euler(new Vector3(0, 0, angleInDeg))) as GameObject;
                        obj.transform.parent = transform;
                        foliageOnPath.Add(obj);
                        prevIndex = foliageOnPath.Count - 1;
                        foliageCount = foliageOnPath.Count;
                        Foliage2D objFoliage = obj.GetComponent<Foliage2D>();
                        objFoliage.RebuildMesh();
                        objectIndex++;
                    }
                    else
                    {
                        ZAxisOffset *= -1;
                        prevIndex = objectIndex - 1;
                        foliageOnPath[objectIndex - 1].transform.position = transform.TransformPoint(new Vector3(pointOnTheLine.x + posOffset.x, pointOnTheLine.y + posOffset.y, ZAxisOffset));
                        foliageOnPath[objectIndex - 1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleInDeg));
                        objectIndex++;
                    }
                }
            }

            if (foliageCount + 1 > objectIndex)
            {
                int lenD = foliageCount + 1 - objectIndex;

                for (int i = 0; i < lenD; i++)
                {
                    int last = foliageOnPath.Count - 1;
                    DestroyImmediate(foliageOnPath[last]);
                    foliageOnPath.RemoveAt(last);

                    if (foliagePattern == Foliage2D_Pattern.Consecutive)
                        RestorePrefabIndex();
                }
            }
        }

        private void RestorePrefabIndex()
        {
            prefabIndex--;
            if (prefabIndex == -1)
                prefabIndex = maxCount - 1;
        }

        private void DistanceFromStart()
        {
            if (objectIndex == 1 || distanceFromStart == 0)
            {
                distanceFromStart = foliage2D.width / 2f + firstObjectOffset;

                if (distanceFromStart == 0)
                    distanceFromStart = 0.0001f;

                prevDistanceFromStart = distanceFromStart;
                previousWidth = foliage2D.width;
                posOffset = lineNormal * (foliage2D.height / 2f);
            }
            else
            {
                float currentOverlappingFactor;

                if (foliageOverlapType == Foliage2D_OverlappingType.Fixed)
                    currentOverlappingFactor = overlappingFactor;
                else
                    currentOverlappingFactor = Random.Range(minOverlappingFactor, maxOverlappingFactor);
                
                if (currentOverlappingFactor <= 0.5f)
                    distanceFromStart = prevDistanceFromStart + (previousWidth / 2f) + ((foliage2D.width / 2f) - (foliage2D.width * currentOverlappingFactor));
                else
                    distanceFromStart = prevDistanceFromStart + (previousWidth / 2f) - ((foliage2D.width * currentOverlappingFactor) - (foliage2D.width / 2f));

                prevDistanceFromStart = distanceFromStart;
                previousWidth = foliage2D.width;
                posOffset = lineNormal * (foliage2D.height / 2f);
            }
        }

        private void SmoothPath(int index)
        {
            Vector2 startPoint = Vector2.zero;
            Vector2 endPoint = Vector2.zero;
            Vector2 tanget;

            if (index == 0)
                startPoint = new Vector2(handlesPosition[index].x - 1 * line.x, handlesPosition[index].y - 1 * line.y);
            else
                startPoint = handlesPosition[index - 1];

            if (index == handlesPosition.Count - 2)
                endPoint = new Vector2(handlesPosition[index + 1].x - 1 * line.x, handlesPosition[index + 1].y - 1 * line.y);
            else
                endPoint = handlesPosition[index + 2];
            
            pointOnTheLine = new Vector2(
                HermiteInterpolation(startPoint.x, handlesPosition[index].x, handlesPosition[index + 1].x, endPoint.x, index),
                HermiteInterpolation(startPoint.y, handlesPosition[index].y, handlesPosition[index + 1].y, endPoint.y, index));

            tanget = GetTangent(index, startPoint, endPoint);
            angleInRad = Mathf.Atan2(tanget.y, tanget.x);
            angleInDeg = angleInRad * Mathf.Rad2Deg;
        }

        private float HermiteInterpolation(float p1, float p2, float p3, float p4, int index)
        {
            float m0, m1, mu, mu2, mu3;
            float a0, a1, a2, a3;
            float currentTension;
            float currentBias;

            mu = distanceFromStart / lineLength;

            if (uniformValues)
            {
                currentTension = tension;
                currentBias = bias;
            }
            else
            {
                currentTension = Mathf.Lerp(nodeTension[index], nodeTension[index + 1], mu);
                currentBias = Mathf.Lerp(nodeBias[index], nodeBias[index + 1], mu);
            }

            mu2 = mu * mu;
            mu3 = mu2 * mu;
            m0 = (p2 - p1) * (1 + currentBias) * (1 - currentTension) / 2 + (p3 - p2) * (1 - currentBias) * (1 - currentTension) / 2;
            m1 = (p3 - p2) * (1 + currentBias) * (1 - currentTension) / 2 + (p4 - p3) * (1 - currentBias) * (1 - currentTension) / 2;
            a0 = 2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + mu;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;

            return (a0 * p2 + a1 * m0 + a2 * m1 + a3 * p3);
        }

        private Vector2 GetTangent(int index, Vector2 startPoint, Vector2 endPoint)
        {
            float mu = distanceFromStart / lineLength;
            return new Vector2(
                HermiteSlope(startPoint.x, handlesPosition[index].x, handlesPosition[index + 1].x, endPoint.x, mu, index),
                HermiteSlope(startPoint.y, handlesPosition[index].y, handlesPosition[index + 1].y, endPoint.y, mu, index));
        }

        private float HermiteSlope(float p1, float p2, float p3, float p4, float mu, int index)
        {
            float m0, m1, mu2;
            float a0, a1, a2, a3;
            float currentTension;
            float currentBias;

            if (uniformValues)
            {
                currentTension = tension;
                currentBias = bias;
            }
            else
            {
                currentTension = Mathf.Lerp(nodeTension[index], nodeTension[index + 1], mu);
                currentBias = Mathf.Lerp(nodeBias[index], nodeBias[index + 1], mu);
            }

            mu2 = mu * mu;
            m0 = ((1 - currentTension) * (currentBias + 1) * (p2 - p1) + (1 - currentBias) * (1 - currentTension) * (p3 - p2)) / 2;
            m1 = ((1 - currentTension) * (currentBias + 1) * (p3 - p2) + (1 - currentBias) * (1 - currentTension) * (p4 - p3)) / 2;
            a0 = (3 * mu2 - 4 * mu + 1);
            a1 = (3 * mu2 - 2 * mu);
            a2 = p2 * (6 * mu2 - 6 * mu);
            a3 = p3 * (6 * mu - 6 * mu2);

            return (a0 * m0 + a1 * m1 + a2 + a3);
        }

        public void ReCenterPivotPoint()
        {
            Vector2 center = Vector2.zero;

            for (int i = 0; i < handlesPosition.Count; i++)
            {
                center += handlesPosition[i];
            }

            center = center / handlesPosition.Count + new Vector2(transform.position.x, transform.position.y);
            Vector2 offset = center - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

            for (int i = 0; i < handlesPosition.Count; i++)
            {
                handlesPosition[i] -= offset;
            }

            gameObject.transform.position = new Vector3(center.x, center.y, gameObject.transform.position.z);
            RecreateFoliage();
        }

        public void ClearList()
        {
            int len = foliageOnPath.Count - 1;

            for (int i = len; i >= 0; i--)
            {
                DestroyImmediate(foliageOnPath[i]);
                foliageOnPath.RemoveAt(i);
            }
            prefabIndex = -1;
            foliageOnPath.Clear();
            RecreateFoliage();
        }

        public void AddPathPoint(Vector2 hP)
        {
            handlesPosition.Add(hP);
            handleControlsPos.Add(new Vector2(hP.x + 2f, hP.y));
            nodeTension.Add(0f);
            nodeBias.Add(0f);
        }
        #endregion
    }
}
