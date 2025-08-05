using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanDetection : MonoBehaviour
{
    public float viewRadius = 5f;//视野距离
    public int viewAngleStep = 20;//射线密度
    [Range(0, 360)]
    public float viewAngle = 270f;//视野角度
    public Move move;
    void Start()
    {
        Move movee = GetComponent<Move>();

        movee.accack1.interactable = false;
    }

    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        // 计算最左侧方向的向量
        Vector3 forward_left = Quaternion.Euler(0, -(viewAngle / 2f), 0) * transform.forward * viewRadius;

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 v = Quaternion.Euler(0, (viewAngle / viewAngleStep) * i, 0) * forward_left;// 根据当前角度计算方向向量
            Vector3 pos = transform.position + v;// 计算射线终点

            // 在Scene中绘制线条(仅方便观察，Game视图中不可见)
            Debug.DrawLine(transform.position, pos, Color.red);

            // 射线检测
            Ray ray = new Ray(transform.position, v);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, viewRadius))
            {
                // 如果射线击中碰撞体且物体标签为"Enemy"
                if (hitInfo.collider.tag == "Enemy")
                {
                    // 输出提示信息
                    Debug.Log("视野内有敌人");

                   //move.accack1.interactable = true; // 启用攻击按钮

                }
            }
        }
    }
}