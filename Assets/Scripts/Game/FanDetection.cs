using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 确保引入UI命名空间

public class FanDetection : MonoBehaviour
{
    public float viewRadius = 5f;
    public int viewAngleStep = 20;
    [Range(0, 360)]
    public float viewAngle = 270f;
    public Move move; // 需要在Inspector中赋值或通过代码获取

    void Start()
    {
        // 自动获取当前物体上的Move组件（如果在同一个物体上）
        if (move == null)
            move = GetComponent<Move>();

        // 双重判空，避免Start中就报错
        if (move != null && move.accack1 != null)
        {
            move.accack1.interactable = false;
        }
        else
        {
            Debug.LogError("Move组件或accack1按钮未正确赋值！");
        }
    }

    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        // 先判断move是否为空，避免后续错误
        if (move == null)
            return;

        Vector3 forward_left = Quaternion.Euler(0, -(viewAngle / 2f), 0) * transform.forward * viewRadius;
        bool enemyInSight = false; // 增加一个标志位判断是否有敌人在视野内

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
                    Debug.Log("视野内有敌人");
                    enemyInSight = true;
                    break; // 找到一个敌人就可以退出循环
                }
            }
        }

        // 统一设置按钮状态，避免多次设置
        //if (move.accack1 != null)
        //{
        //    move.accack1.interactable = enemyInSight;
        //}
        //else
        //{
        //    Debug.LogError("accack1按钮未赋值！请检查Move脚本中的引用");
        //}
    }
}