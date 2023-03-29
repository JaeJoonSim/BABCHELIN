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
    private Enemy enemy;

    private void Start()
    {
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

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
