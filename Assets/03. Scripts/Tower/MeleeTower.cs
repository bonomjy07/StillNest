using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    bool attackCooltime = false; // true �� ��Ÿ����

    private List<GameObject> monstersInRange = new List<GameObject>();

    private Renderer rend;
    private Color originColor;
    private float flashDuration = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        originColor = rend.material.color;

        StartCoroutine(MeleeAttack());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator MeleeAttack()
    {
        while (true)
        {
            //if (!attackCooltime)
            //{
            //attackCooltime = true;
            foreach (GameObject mob in monstersInRange)
            {
                Debug.Log("Attack point 2");
                if (mob == null)
                    continue;

                MonsterController mc = mob.GetComponent<MonsterController>();
                if (mc != null)
                {
                    mc.TakeDamage(attackDamage);
                    Debug.Log("Attack point 1");
                }
            }

            StartCoroutine(AttackEffect());
            //}
            yield return new WaitForSeconds(attackSpeed);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("�� ������ �߰�1");
            if (!monstersInRange.Contains(other.gameObject)) // �ߺ��˻�
            {
                Debug.Log("�� ������ �߰�2");
                monstersInRange.Add(other.gameObject);
            }
            // ���� ���̸� �־��ְ�
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // ���� �ȿ��� ���Ͱ� �״� ���� Destroy�ż� null�� ��
            // �װ��� ó���������
            for (int i = monstersInRange.Count - 1; i >= 0; i--)
            {
                if (monstersInRange[i] == null)
                {
                    monstersInRange.RemoveAt(i);
                }
            }

            if(other.gameObject != null && monstersInRange.Contains(other.gameObject))
            {
                Debug.Log("�� ������ ����");
                monstersInRange.Remove(other.gameObject);
                // ���� ���̸� ���ְ�
            }
        }
    }

    IEnumerator AttackEffect()
    {
        rend.material.color = Color.blue;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originColor;
    }
}
