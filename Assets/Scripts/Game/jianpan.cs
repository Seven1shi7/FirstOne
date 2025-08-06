using UnityEngine;

public class TriggerFollower : MonoBehaviour
{
    // Ҫ�����Ŀ������
    public Transform target;
    // �����ƫ�����������Ŀ�������λ�ã�
    public Vector3 offset = Vector3.zero;
    // �������תƫ����
    public Vector3 rotationOffset = Vector3.zero;

    private Collider triggerCollider;

    void Start()
    {
        // ��ȡ��ǰ�����ϵĴ��������
        triggerCollider = GetComponent<Collider>();

        // ȷ����ǰ�������ײ���ѹ�ѡIs Trigger
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning("�빴ѡ��ײ����Is Triggerѡ��");
            triggerCollider.isTrigger = true;
        }

        // ���Ŀ���Ƿ�ֵ
        if (target == null)
        {
            Debug.LogError("��ָ��Ҫ�����Ŀ�����壡");
        }
    }

    void Update()
    {
        // ���Ŀ����ڣ������λ�ú���ת
        if (target.tag=="Player")
        {
            // ����λ�ã�Ŀ��λ�� + ƫ����
            transform.position = target.position + offset;

            // ������ת��Ŀ����ת + ��תƫ��������ѡ��
            transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
        }
    }

    // ���������ʾ��
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�������Ӵ���: " + other.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        // �����Ӵ�ʱ���߼�
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("�������뿪: " + other.gameObject.name);
    }
}
