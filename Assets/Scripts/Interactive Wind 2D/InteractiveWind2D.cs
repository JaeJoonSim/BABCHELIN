using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveWind2D : MonoBehaviour
{
    [Tooltip("얼마나 많은 물리적 상호작용이 스프라이트를 휘게 하는지.")]
    public float rotationFactor = 1.5f;
    [Tooltip("물리적 상호작용이 적용되는 힘")]
    public float bendInSpeed = 8f;
    [Tooltip("물리적 상호작용이 해제되는 힘")]
    public float bendOutSpeed = 8f;

    [Tooltip("비활성화된 경우 이동중에만 구부러짐")]
    public bool stayBent = true;
    [Tooltip("상호작용 최소 속도")]
    public float minBendSpeed = 1f;

    [Tooltip("비활성화 상태에서는 기본 재질을 교체")]
    public bool hyperPerformanceMode = false;
    [Tooltip("시작 시 Z 위치에 약간의 오프셋을 추가합니다.렌더 순서의 임의 재지정을 방지합니다.")]
    public bool randomOffsetZ = true;
    [Tooltip("비활성 상태에서 전환할 재료를 정의합니다.")]
    public bool customMaterial = false;
    [Tooltip("기본 스프라이트 재료에 사용되는 셰이더입니다.")]
    public string inactiveShader = "Sprites/Default";
    [Tooltip("비활성 상태일 때 사용되는 재료입니다.")]
    public Material inactiveMaterial;

    Material mat;
    HashSet<Collider> collidersInside;
    BoxCollider boxCollider;

    float currentBending;
    float currentRotationDirection;
    bool isActive;
    bool newDirection;

    float lastPosition;
    float lastBend;
    float currentBendTarget;
    bool bentInLastFrame;

    SpriteRenderer sr;
    static Material defaultMaterial;

    int rotationId;

    void Start()
    {
        collidersInside = new HashSet<Collider>();
        boxCollider = GetComponent<BoxCollider>();
        sr = GetComponent<SpriteRenderer>();
        mat = sr.material;

        if (defaultMaterial == null)
        {
            if (customMaterial)
            {
                defaultMaterial = inactiveMaterial;
            }
            else
            {
                defaultMaterial = new Material(Shader.Find(inactiveShader));
            }
        }

        if (hyperPerformanceMode)
        {
            sr.material = defaultMaterial;

            if (randomOffsetZ)
            {
                Vector3 position = transform.position;
                position.z += Random.value * 0.1f;
                transform.position = position;
            }
        }

        rotationId = Shader.PropertyToID("_WindRotation");
    }

    void FixedUpdate()
    {
        if (isActive == false) return;

        Vector2 localPosition = new Vector2(0, -1000000);

        foreach (Collider collider in collidersInside)
        {
            if (collider != null)
            {
                if (localPosition.y < -99999)
                {
                    localPosition = collider.bounds.center - transform.position;
                }
                else
                {
                    if (!newDirection)
                    {
                        Vector2 newLocalPosition = (collider.bounds.center - transform.position);
                        if ((currentRotationDirection < 0 && newLocalPosition.x > localPosition.x) || (currentRotationDirection > 0 && newLocalPosition.x < localPosition.x))
                        {
                            localPosition = newLocalPosition;
                        }
                    }
                    else
                    {
                        localPosition = ((Vector2)(collider.bounds.center - transform.position) + localPosition) * 0.5f; //Position Deviation (multiple colliders)
                    }
                }
            }
        }

        if (localPosition.y > -99999)
        {
            if (newDirection)
            {
                if (localPosition.x < 0)
                {
                    currentRotationDirection = -1;
                }
                else
                {
                    currentRotationDirection = 1;
                }

                newDirection = false;
            }

            float targetBending = 0;
            if (currentRotationDirection < 0)
            {
                targetBending = Mathf.Clamp01((localPosition.x + boxCollider.size.x * 0.5f) / boxCollider.size.x);
            }
            else
            {
                targetBending = Mathf.Clamp01((boxCollider.size.x * 0.5f - localPosition.x) / boxCollider.size.x);
            }

            if (stayBent)
            {
                currentBendTarget = targetBending;
            }
            else
            {
                bool moved = Mathf.Abs(lastPosition - localPosition.x) > Time.fixedDeltaTime * minBendSpeed;

                if (moved && lastBend != 0 && currentRotationDirection > 0 == (localPosition.x - lastPosition) > 0)
                {
                    moved = false;
                }

                if (moved || bentInLastFrame)
                {
                    currentBendTarget = targetBending;
                    lastBend = targetBending;
                    bentInLastFrame = true;

                    if (!moved)
                    {
                        bentInLastFrame = false;
                    }
                }
                else
                {
                    currentBendTarget = Mathf.Lerp(currentBendTarget, 0, Time.fixedDeltaTime * bendInSpeed);

                    if (Mathf.Abs(currentBending) < 0.01f)
                    {
                        newDirection = true;
                    }
                }
                lastPosition = localPosition.x;
            }

            currentBending += (currentBendTarget * currentRotationDirection - currentBending) * Mathf.Min(bendInSpeed * Time.fixedDeltaTime, 1);
            UpdateShader();
        }
        else
        {
            currentBending -= currentBending * Mathf.Min(bendOutSpeed * Time.fixedDeltaTime, 1);
            UpdateShader();

            if (Mathf.Abs(currentBending) < 0.005f)
            {
                isActive = false;
                lastBend = 0;

                if (hyperPerformanceMode)
                {
                    sr.material = defaultMaterial;
                }
            }
        }
    }

    public void UpdateShader()
    {
        mat.SetFloat(rotationId, -1f * currentBending * rotationFactor);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collidersInside.Count == 0 || Mathf.Abs(currentBending) < 0.2f)
        {
            newDirection = true;
        }

        collidersInside.Add(collision);

        if (hyperPerformanceMode && isActive == false)
        {
            sr.material = mat;
        }
        isActive = true;
    }
    void OnTriggerExit(Collider collision)
    {
        if (collidersInside.Contains(collision))
        {
            collidersInside.Remove(collision);
        }
    }

    public static void DefaultCollider(BoxCollider2D box)
    {
        box.isTrigger = true;
        box.size = new Vector2(2, 1);
        box.offset = new Vector2(0, -0.5f);
    }
}