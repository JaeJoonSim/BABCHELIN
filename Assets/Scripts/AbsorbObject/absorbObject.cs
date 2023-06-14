using UnityEngine;
using UnityEngine.InputSystem.XR;

public class absorbObject : MonoBehaviour
{
    [Header("사이즈")]
    [SerializeField]
    private absorb.objectSize size;

    private int addBullet;

    public AnimationCurve accelerationCurve;

    [Header("흡수시간")]
    [SerializeField]
    private float absorbTime;
    [SerializeField]
    private float curAbsorbTime;

    [Header("범위 밖 유지시간")]
    [SerializeField]
    private float absorbKeepTime = 1f;

    public bool inAbsorbArea;
    public bool isAbsorb;

    [Header("흔들림 변수")]
    public float maxSpeed = 20f;     // 최대 속도

    private Quaternion initialRotation;  // 초기 회전값
    private float startTime;

    private GameObject showObj;


    void Start()
    {
        switch (size)
        {
            case absorb.objectSize.small:
                absorbTime = absorb.Instance.absorbTimeSmall;
                addBullet = absorb.Instance.addBulletSmall;
                showObj = Instantiate(absorb.Instance.showAbsorbSmall, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
                break;
            case absorb.objectSize.medium:
                absorbTime = absorb.Instance.absorbTimeMedium;
                addBullet = absorb.Instance.addBulletMedium;
                showObj = Instantiate(absorb.Instance.showAbsorbMedium, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
                break;
            case absorb.objectSize.large:
                absorbTime = absorb.Instance.absorbTimeLarge;
                addBullet = absorb.Instance.addBulletLarge;
                showObj = Instantiate(absorb.Instance.showAbsorbLarge, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
                break;
            default:
                break;
        }

        absorbKeepTime = absorb.Instance.absorbKeepTime;

        inAbsorbArea = false;
        isAbsorb = false;

        initialRotation = transform.rotation;
        curAbsorbTime = absorbTime;


    }

    private void Update()
    {
        switch (size)
        {
            case absorb.objectSize.small:
                absorbTime = absorb.Instance.absorbTimeSmall;
                addBullet = absorb.Instance.addBulletSmall;
                break;
            case absorb.objectSize.medium:
                absorbTime = absorb.Instance.absorbTimeMedium;
                addBullet = absorb.Instance.addBulletMedium;
                break;
            case absorb.objectSize.large:
                absorbTime = absorb.Instance.absorbTimeLarge;
                addBullet = absorb.Instance.addBulletLarge;
                break;
            default:
                break;
        }
        absorbTime = absorbTime - (absorbTime / 100) * absorb.Instance.Player.GetComponent<PlayerController>().TotalStatus.absorbSpd.value;
        if (Vector3.Distance(absorb.Instance.Player.position, transform.position) < absorb.Instance.Player.GetComponent<PlayerController>().TotalStatus.absorbRange.value)
            showObj.SetActive(true);
        else
            showObj.SetActive(false);
        if (!isAbsorb)
        {
            if (inAbsorbArea)
            {
                curAbsorbTime -= Time.deltaTime;

                if (curAbsorbTime <= 0)
                {
                    isAbsorb = true;
                }

                absorbKeepTime = absorb.Instance.absorbKeepTime;

                shake();
            }
            else
            {
                if (curAbsorbTime != absorbTime && 0 <= curAbsorbTime)
                {
                    absorbKeepTime -= Time.deltaTime;
                    if (absorbKeepTime <= 0)
                        curAbsorbTime = absorbTime;
                    shake();
                }
                else
                {
                    startTime = Time.time;
                    transform.rotation = initialRotation;
                }
            }
        }
        else
        {
            BackGroundSouund.Instance.PlaySound("objectAbsorb");
            Instantiate(absorb.Instance.BulletEssence, transform.position, Quaternion.identity).GetComponent<BulletEssence>().setAddValue(addBullet);
            Destroy(gameObject);
        }
    }

    void shake()
    {

        float t = (curAbsorbTime) / absorbTime;
        float acceleration = accelerationCurve.Evaluate(t);
        float currentSpeed = Mathf.Lerp(maxSpeed, 1, acceleration);

        float yRotation = Mathf.Sin(acceleration * maxSpeed) * 20f;
        Debug.Log(yRotation);
        transform.rotation = initialRotation * Quaternion.Euler(0, yRotation, 0);
        //transform.localRotation = Quaternion.Euler(0f, currentAngle, -90f);

        
      
    }


}
