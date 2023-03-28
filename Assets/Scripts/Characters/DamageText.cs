using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Tooltip("�ؽ�Ʈ �̵� �ӵ�")]
    [SerializeField] private float moveSpeed;
    [Tooltip("���� ��ȯ �ӵ�")]
    [SerializeField] private float alphaSpeed;
    [Tooltip("������Ʈ ���� �ð�")]
    [SerializeField] private float destroyTime;

    private TextMeshProUGUI damageText;
    private Color alpha;
    private Camera mainCam;
    private Enemy enemy;

    private void Start()
    {
        mainCam = Camera.main;

        ShowDamageText();
    }

    private void ShowDamageText()
    {
        enemy = GetComponentInParent<Enemy>();
        damageText = GetComponent<TextMeshProUGUI>();

        if (enemy != null)
            damageText.text = enemy.hitDamage.ToString();

        alpha = damageText.color;
        Invoke("DestroyObject", destroyTime);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime);
        damageText.color = alpha;
    }

    private void LateUpdate()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
