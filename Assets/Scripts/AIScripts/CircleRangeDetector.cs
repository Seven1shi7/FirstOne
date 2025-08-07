using UnityEngine;

/// <summary>
/// Բ�η�Χ�����
/// </summary>
public class CircleRangeDetector : MonoBehaviour
{
    [Header("�������")]
    [Tooltip("���뾶")]
    public float radius = 5f;
    [Tooltip("Ҫ����Ŀ��㼶")]
    public LayerMask targetLayer;

    [Header("��������")]
    [Tooltip("�Ƿ���ʾ��ⷶΧ")]
    public bool drawGizmos = true;
    [Tooltip("������ʾ��ɫ")]
    public Color gizmoColor = Color.green;

    /// <summary>
    /// ���Ŀ���Ƿ���Բ�η�Χ��
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ��</param>
    /// <returns>�Ƿ��ڷ�Χ��</returns>
    public bool IsTargetInRange(Vector3 targetPosition)
    {
        // ������Ŀ��ľ���
        float distance = Vector3.Distance(transform.position, targetPosition);
        // �жϾ����Ƿ�С�ڵ��ڰ뾶
        return distance <= radius;
    }

    /// <summary>
    /// ��ȡԲ�η�Χ�ڵ�������ײ��
    /// </summary>
    /// <returns>��Χ�ڵ���ײ������</returns>
    public Collider[] GetTargetsInRange()
    {
        // ʹ��Physics.OverlapSphere������η�Χ�ڵ���ײ��
        return Physics.OverlapSphere(transform.position, radius, targetLayer);
    }

    /// <summary>
    /// ��ȡԲ�η�Χ���ض����͵Ķ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>�������</returns>
    public T[] GetTargetsInRange<T>() where T : Component
    {
        Collider[] colliders = GetTargetsInRange();
        T[] components = new T[colliders.Length];
        int count = 0;

        foreach (Collider col in colliders)
        {
            T component = col.GetComponent<T>();
            if (component != null)
            {
                components[count++] = component;
            }
        }

        // �ü��������Ƴ���Ԫ��
        System.Array.Resize(ref components, count);
        return components;
    }

    /// <summary>
    /// �������Ի���
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}