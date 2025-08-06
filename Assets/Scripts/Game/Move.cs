using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public Yaogan yaogan;

    JIneng jIneng;
    private bool isdown = false;
    public Animation animation;
    public Button accack1;
    public Button accack2;
    public Button accack3;
    public Button accack4;
    
    // 技能判定参数
    public float skillRadius = 5f; // 技能半径
    public float skillWidth = 3f;  // 矩形技能宽度
    public float skillLength = 8f; // 矩形技能长度
    public float skillAngle = 60f; // 扇形技能角度
    
    // Start is called before the first frame update
    void Start()
    {
        yaogan.move = move;
        animation = GetComponent<Animation>();
        accack1.onClick.AddListener(() =>
        {

            animation.Play("attack");
            
        });

        accack2.onClick.AddListener(() =>
        {
            animation.Play("attack2");
        });

        accack3.onClick.AddListener(() =>
        {
            animation.Play("attack3");
        });

        accack4.onClick.AddListener(() =>
        {
            animation.Play("skill");
            // 使用技能判定
            UseSkill();
        });

        

    }

    /// <summary>
    /// 使用技能并检测目标
    /// </summary>
    private void UseSkill()
    {
        // 获取所有敌人（假设敌人有Enemy标签）
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            
            // 圆形技能判定
            if (IsInCircle(transform.position, enemyPosition, skillRadius))
            {
                Debug.Log($"敌人 {enemy.name} 在圆形技能范围内");
                // 对敌人造成伤害
                // DealDamage(enemy);
            }
            
            // 矩形技能判定
            if (IsInRectangle(transform.position, transform.forward, enemyPosition, skillWidth, skillLength))
            {
                Debug.Log($"敌人 {enemy.name} 在矩形技能范围内");
                // 对敌人造成伤害
                // DealDamage(enemy);
            }
            
            // 扇形技能判定
            if (IsInSector(transform.position, transform.forward, enemyPosition, skillRadius, skillAngle))
            {
                Debug.Log($"敌人 {enemy.name} 在扇形技能范围内");
                // 对敌人造成伤害
                // DealDamage(enemy);
            }
        }
    }

    /// <summary>
    /// 判断目标点是否在以skillOrigin为圆心，半径为radius的圆形区域内
    /// </summary>
    /// <param name="skillOrigin">技能释放点</param>
    /// <param name="target">目标点</param>
    /// <param name="radius">圆形半径</param>
    /// <returns>是否在圆形区域内</returns>
    public bool IsInCircle(Vector3 skillOrigin, Vector3 target, float radius)
    {
        // 计算目标点与技能释放点的距离
        return Vector3.Distance(skillOrigin, target) <= radius;
    }

    /// <summary>
    /// 判断目标点是否在以skillOrigin为中心，skillForward为朝向的矩形区域内
    /// </summary>
    /// <param name="skillOrigin">技能释放点</param>
    /// <param name="skillForward">技能朝向</param>
    /// <param name="target">目标点</param>
    /// <param name="width">矩形宽度</param>
    /// <param name="length">矩形长度</param>
    /// <returns>是否在矩形区域内</returns>
    public bool IsInRectangle(Vector3 skillOrigin, Vector3 skillForward, Vector3 target, float width, float length)
    {
        // 计算目标点相对于技能释放点的向量
        Vector3 dir = target - skillOrigin;
        // 计算目标点在技能朝向上的投影距离
        float forwardDist = Vector3.Dot(dir, skillForward.normalized);
        // 计算目标点在技能右侧方向上的投影距离
        float rightDist = Vector3.Dot(dir, Vector3.Cross(Vector3.up, skillForward).normalized);
        // 判断是否在矩形范围内
        return forwardDist >= 0 && forwardDist <= length && Mathf.Abs(rightDist) <= width / 2;
    }

    /// <summary>
    /// 判断目标点是否在以skillOrigin为圆心，skillForward为朝向，半径为radius，夹角为angle的扇形区域内
    /// </summary>
    /// <param name="skillOrigin">技能释放点</param>
    /// <param name="skillForward">技能朝向</param>
    /// <param name="target">目标点</param>
    /// <param name="radius">扇形半径</param>
    /// <param name="angle">扇形夹角（度）</param>
    /// <returns>是否在扇形区域内</returns>
    public bool IsInSector(Vector3 skillOrigin, Vector3 skillForward, Vector3 target, float radius, float angle)
    {
        // 计算目标点相对于技能释放点的向量
        Vector3 dir = target - skillOrigin;
        // 判断距离是否在半径范围内
        if (dir.magnitude > radius) return false;
        // 计算目标点与技能朝向的夹角
        float angleToTarget = Vector3.Angle(skillForward, dir);
        // 判断夹角是否在扇形范围内
        return angleToTarget <= angle / 2;
    }

    private void move(Vector2 vector)
    {
        Quaternion to = Quaternion.LookRotation(new Vector3(vector.x, 0, vector.y));
        transform.rotation = to;
    }

    // Update is called once per frame
    void Update()
    {

        if (yaogan.ismove)
        {
            // isdown = true;
            transform.position += transform.forward * 5 * Time.deltaTime;
            animation.Stop("free");
            animation.Play("walk");
        }
        else
        {
            animation.Stop("walk");
            animation.Blend("idle");
        }

    }


    public void LateUpdate()
    {


    }

}
