using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JIneng : MonoBehaviour
{
    private void Start()
    {
        
    }



    /// <summary>
    /// �ж�Ŀ����Ƿ�����skillOriginΪ���ģ�skillForwardΪ����ľ���������
    /// </summary>
    /// <param name="skillOrigin">�����ͷŵ�</param>
    /// <param name="skillForward">���ܳ���</param>
    /// <param name="target">Ŀ���</param>
    /// <param name="width">���ο��</param>
    /// <param name="length">���γ���</param>
    /// <returns>�Ƿ��ھ���������</returns>
    public bool IsInRectangle(Vector3 skillOrigin, Vector3 skillForward, Vector3 target, float width, float length)
    {
        // ����Ŀ�������ڼ����ͷŵ������
        Vector3 dir = target - skillOrigin;
        // ����Ŀ����ڼ��ܳ����ϵ�ͶӰ����
        float forwardDist = Vector3.Dot(dir, skillForward.normalized);
        // ����Ŀ����ڼ����Ҳ෽���ϵ�ͶӰ����
        float rightDist = Vector3.Dot(dir, Vector3.Cross(Vector3.up, skillForward).normalized);
        // �ж��Ƿ��ھ��η�Χ��
        return forwardDist >= 0 && forwardDist <= length && Mathf.Abs(rightDist) <= width / 2;
    }


    /// <summary>
    /// �ж�Ŀ����Ƿ�����skillOriginΪԲ�ģ��뾶Ϊradius��Բ��������
    /// </summary>
    /// <param name="skillOrigin">�����ͷŵ�</param>
    /// <param name="target">Ŀ���</param>
    /// <param name="radius">Բ�ΰ뾶</param>
    /// <returns>�Ƿ���Բ��������</returns>
    public bool IsInCircle(Vector3 skillOrigin, Vector3 target, float radius)
    {
        // ����Ŀ����뼼���ͷŵ�ľ���
        return Vector3.Distance(skillOrigin, target) <= radius;
    }



    /// <summary>
    /// �ж�Ŀ����Ƿ�����skillOriginΪԲ�ģ�skillForwardΪ���򣬰뾶Ϊradius���н�Ϊangle������������
    /// </summary>
    /// <param name="skillOrigin">�����ͷŵ�</param>
    /// <param name="skillForward">���ܳ���</param>
    /// <param name="target">Ŀ���</param>
    /// <param name="radius">���ΰ뾶</param>
    /// <param name="angle">���μнǣ��ȣ�</param>
    /// <returns>�Ƿ�������������</returns>
    public bool IsInSector(Vector3 skillOrigin, Vector3 skillForward, Vector3 target, float radius, float angle)
    {
        // ����Ŀ�������ڼ����ͷŵ������
        Vector3 dir = target - skillOrigin;
        // �жϾ����Ƿ��ڰ뾶��Χ��
        if (dir.magnitude > radius) return false;
        // ����Ŀ����뼼�ܳ���ļн�
        float angleToTarget = Vector3.Angle(skillForward, dir);
        // �жϼн��Ƿ������η�Χ��
        return angleToTarget <= angle / 2;
    }
}
