using UnityEngine;

public class absorbObject : MonoBehaviour
{
    [Header("사이즈")]
    [SerializeField]
    private absorb.objectSize size;

    private int addBullet;

    [Header("흡수시간")]
    [SerializeField]
    private float absorbTime;
    private float curAbsorbTime;

    [Header("범위 밖 유지시간")]
    [SerializeField]
    private float absorbKeepTime = 1f;

    public bool inAbsorbArea;
    public bool isAbsorb;

    [Header("흔들림 변수")]
    public float speed = 1f;         // 흔들리는 속도
    public float maxSpeed = 10f;     // 최대 속도
    public float acceleration = 1f;  // 가속도

    private Quaternion initialRotation;  // 초기 회전값
    private float startTime;
    private float currentSpeed;

    void Start()
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
            if (inAbsorbArea && transform.position.z >= 0f)
            {
                curAbsorbTime -= Time.deltaTime;

                if (curAbsorbTime <= 0)
                {
                    isAbsorb = true;
                    transform.localScale = transform.localScale / 2;
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
            Absorb();
    }
    void Absorb()
    {
        transform.position = transform.position + (absorb.Instance.Player.position - transform.position).normalized * absorb.Instance.speed * Time.deltaTime;
    }
    void shake()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);
        float yRotation = Mathf.Sin((Time.time - startTime) * currentSpeed * speed) * currentSpeed;
        transform.rotation = initialRotation * Quaternion.Euler(0, yRotation, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAbsorb && collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().addBullet(addBullet);
            Destroy(gameObject);

            Cream parent = gameObject.GetComponentInParent<Cream>();
            if (parent != null)
                Destroy(parent);
        }

    }

}
