using UnityEngine;

public class absorbObject : MonoBehaviour
{
    [Header("������")]
    [SerializeField]
    private absorb.objectSize size;

    private int addBullet;

    [Header("����ð�")]
    [SerializeField]
    private float absorbTime;
    [SerializeField]
    private float curAbsorbTime;

    [Header("���� �� �����ð�")]
    [SerializeField]
    private float absorbKeepTime = 1f;

    public bool inAbsorbArea;
    public bool isAbsorb;

    [Header("��鸲 ����")]
    public float speed = 1f;         // ��鸮�� �ӵ�
    public float maxSpeed = 20f;     // �ִ� �ӵ�
    public float acceleration = 5f;  // ���ӵ�

    private Quaternion initialRotation;  // �ʱ� ȸ����
    private float startTime;
    private float currentSpeed;


    void Start()
    {
        switch (size)
        {
            case absorb.objectSize.small:
                absorbTime = absorb.Instance.absorbTimeSmall;
                addBullet = absorb.Instance.addBulletSmall;
                Instantiate(absorb.Instance.showAbsorbSmall, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
                break;
            case absorb.objectSize.medium:
                absorbTime = absorb.Instance.absorbTimeMedium;
                addBullet = absorb.Instance.addBulletMedium;
                Instantiate(absorb.Instance.showAbsorbMedium, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
                break;
            case absorb.objectSize.large:
                absorbTime = absorb.Instance.absorbTimeLarge;
                addBullet = absorb.Instance.addBulletLarge;
                Instantiate(absorb.Instance.showAbsorbLarge, transform.position + new Vector3(0, 1, -2), Quaternion.identity, transform);
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
                    currentSpeed = 0;
                    transform.rotation = initialRotation;
                }
            }
        }
        else
        {
            Instantiate(absorb.Instance.BulletEssence, transform.position, Quaternion.identity).GetComponent<BulletEssence>().setAddValue(addBullet);
            Destroy(gameObject);
        }
    }

    void shake()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);
        float yRotation = Mathf.Sin((Time.time - startTime) * currentSpeed * speed) * currentSpeed;
        transform.rotation = initialRotation * Quaternion.Euler(0, yRotation, 0);
    }


}
