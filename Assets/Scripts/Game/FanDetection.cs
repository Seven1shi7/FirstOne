using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ȷ������UI�����ռ�

public class FanDetection : MonoBehaviour
{
    public float viewRadius = 5f;
    public int viewAngleStep = 20;
    [Range(0, 360)]
    public float viewAngle = 270f;
    public Move move; // ��Ҫ��Inspector�и�ֵ��ͨ�������ȡ

    void Start()
    {
        // �Զ���ȡ��ǰ�����ϵ�Move����������ͬһ�������ϣ�
        if (move == null)
            move = GetComponent<Move>();

        // ˫���пգ�����Start�оͱ���
        if (move != null && move.accack1 != null)
        {
            move.accack1.interactable = false;
        }
        else
        {
            Debug.LogError("Move�����accack1��ťδ��ȷ��ֵ��");
        }
    }

    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        // ���ж�move�Ƿ�Ϊ�գ������������
        if (move == null)
            return;

        Vector3 forward_left = Quaternion.Euler(0, -(viewAngle / 2f), 0) * transform.forward * viewRadius;
        bool enemyInSight = false; // ����һ����־λ�ж��Ƿ��е�������Ұ��

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 v = Quaternion.Euler(0, (viewAngle / viewAngleStep) * i, 0) * forward_left;
            Vector3 pos = transform.position + v;
            Debug.DrawLine(transform.position, pos, Color.red);

            Ray ray = new Ray(transform.position, v);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, viewRadius))
            {
                if (hitInfo.collider.tag == "Enemy")
                {
                    Debug.Log("��Ұ���е���");
                    enemyInSight = true;
                    break; // �ҵ�һ�����˾Ϳ����˳�ѭ��
                }
            }
        }

        // ͳһ���ð�ť״̬������������
        //if (move.accack1 != null)
        //{
        //    move.accack1.interactable = enemyInSight;
        //}
        //else
        //{
        //    Debug.LogError("accack1��ťδ��ֵ������Move�ű��е�����");
        //}
    }
}