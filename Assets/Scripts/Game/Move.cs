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
    
    // 伤害设置
    public float circleDamage = 30f; // 圆形技能伤害
    public float rectangleDamage = 40f; // 矩形技能伤害
    public float sectorDamage = 50f; // 扇形技能伤害
    public float attackDamage = 20f; // 普通攻击伤害
    
    // 可视化设置
    public bool showSkillRange = true; // 是否显示技能范围
    public Color circleColor = Color.red; // 圆形范围颜色
    public Color rectangleColor = Color.blue; // 矩形范围颜色
    public Color sectorColor = Color.green; // 扇形范围颜色
    public float lineWidth = 2f; // 线条宽度
    
    private SkillRangeVisualizer visualizer; // 技能范围可视化器
    
    // Start is called before the first frame update
    void Start()
    {
        yaogan.move = move;
        animation = GetComponent<Animation>();
        
        // 获取或添加技能范围可视化器
        visualizer = GetComponent<SkillRangeVisualizer>();
        if (visualizer == null)
        {
            visualizer = gameObject.AddComponent<SkillRangeVisualizer>();
        }
        
        accack1.onClick.AddListener(() =>
        {
            animation.Play("attack");
            UseAttackSkill(attackDamage, "普通攻击");
            // 显示技能范围
            if (visualizer != null) visualizer.ShowSkillRanges();
        });

        accack2.onClick.AddListener(() =>
        {
            animation.Play("attack2");
            UseAttackSkill(attackDamage * 1.5f, "重击");
            // 显示技能范围
            if (visualizer != null) visualizer.ShowSkillRanges();
        });

        accack3.onClick.AddListener(() =>
        {
            animation.Play("attack3");
            UseAttackSkill(attackDamage * 2f, "连击");
            // 显示技能范围
            if (visualizer != null) visualizer.ShowSkillRanges();
        });

        accack4.onClick.AddListener(() =>
        {
            animation.Play("skill");
            // 使用技能判定
            UseSkill();
            // 显示技能范围
            if (visualizer != null) visualizer.ShowSkillRanges();
        });
    }

    /// <summary>
    /// 使用技能并检测目标
    /// </summary>
    private void UseSkill()
    {
        // 获取所有敌人（假设敌人有Enemy标签）
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        Debug.Log($"找到 {enemies.Length} 个敌人");
        
        if (enemies.Length == 0)
        {
            Debug.LogWarning("没有找到任何敌人！请确保敌人有'Enemy'标签");
            return;
        }
        
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            float distance = Vector3.Distance(transform.position, enemyPosition);
            
            Debug.Log($"检查敌人 {enemy.name}，距离: {distance:F2}，技能半径: {skillRadius}");
            
            // 圆形技能判定
            if (IsInCircle(transform.position, enemyPosition, skillRadius))
            {
                Debug.Log($"敌人 {enemy.name} 在圆形技能范围内");
                DealDamage(enemy, circleDamage, "圆形");
            }
            
            // 矩形技能判定
            if (IsInRectangle(transform.position, transform.forward, enemyPosition, skillWidth, skillLength))
            {
                Debug.Log($"敌人 {enemy.name} 在矩形技能范围内");
                DealDamage(enemy, rectangleDamage, "矩形");
            }
            
            // 扇形技能判定
            if (IsInSector(transform.position, transform.forward, enemyPosition, skillRadius, skillAngle))
            {
                Debug.Log($"敌人 {enemy.name} 在扇形技能范围内");
                DealDamage(enemy, sectorDamage, "扇形");
            }
        }
    }
    
    /// <summary>
    /// 使用攻击技能（近战攻击）
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="attackType">攻击类型</param>
    private void UseAttackSkill(float damage, string attackType)
    {
        // 获取所有敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        Debug.Log($"使用{attackType}，找到 {enemies.Length} 个敌人");
        
        if (enemies.Length == 0)
        {
            Debug.LogWarning("没有找到任何敌人！请确保敌人有'Enemy'标签");
            return;
        }
        
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            float distance = Vector3.Distance(transform.position, enemyPosition);
            
            // 近战攻击范围（较小的圆形范围）
            float attackRange = 2f;
            
            Debug.Log($"检查敌人 {enemy.name}，距离: {distance:F2}，攻击范围: {attackRange}");
            
            if (IsInCircle(transform.position, enemyPosition, attackRange))
            {
                Debug.Log($"敌人 {enemy.name} 在{attackType}范围内");
                DealDamage(enemy, damage, attackType);
            }
        }
    }
    
    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="enemy">敌人对象</param>
    /// <param name="damage">伤害值</param>
    /// <param name="damageType">伤害类型</param>
    private void DealDamage(GameObject enemy, float damage, string damageType)
    {
        Debug.Log($"尝试对敌人 {enemy.name} 造成 {damage} 点 {damageType} 伤害");
        
        // 获取敌人的Enemy组件
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError($"敌人 {enemy.name} 没有Enemy组件！");
            return;
        }
        
        if (!enemyComponent.IsAlive())
        {
            Debug.LogWarning($"敌人 {enemy.name} 已经死亡");
            return;
        }
        
        // 对敌人造成伤害
        enemyComponent.TakeDamage(damage, damageType);
        Debug.Log($"成功对敌人 {enemy.name} 造成 {damage} 点 {damageType} 伤害");
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

    /// <summary>
    /// 在Scene视图中绘制技能范围
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showSkillRange) return;

        // 绘制圆形范围
        DrawCircleRange();
        
        // 绘制矩形范围
        DrawRectangleRange();
        
        // 绘制扇形范围
        DrawSectorRange();
    }

    /// <summary>
    /// 绘制圆形技能范围
    /// </summary>
    private void DrawCircleRange()
    {
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(transform.position, skillRadius);
        
        // 绘制圆形填充（半透明）
        Gizmos.color = new Color(circleColor.r, circleColor.g, circleColor.b, 0.2f);
        Gizmos.DrawSphere(transform.position, skillRadius);
    }

    /// <summary>
    /// 绘制矩形技能范围
    /// </summary>
    private void DrawRectangleRange()
    {
        Vector3 center = transform.position + transform.forward * (skillLength / 2);
        Vector3 right = Vector3.Cross(Vector3.up, transform.forward).normalized;
        
        // 计算矩形的四个顶点
        Vector3 topLeft = center + right * (skillWidth / 2) + transform.forward * (skillLength / 2);
        Vector3 topRight = center - right * (skillWidth / 2) + transform.forward * (skillLength / 2);
        Vector3 bottomLeft = center + right * (skillWidth / 2) - transform.forward * (skillLength / 2);
        Vector3 bottomRight = center - right * (skillWidth / 2) - transform.forward * (skillLength / 2);
        
        // 绘制矩形边框
        Gizmos.color = rectangleColor;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
        
        // 绘制矩形填充（半透明）
        Gizmos.color = new Color(rectangleColor.r, rectangleColor.g, rectangleColor.b, 0.2f);
        Gizmos.DrawMesh(CreateRectangleMesh(), center, transform.rotation);
    }

    /// <summary>
    /// 绘制扇形技能范围
    /// </summary>
    private void DrawSectorRange()
    {
        Gizmos.color = sectorColor;
        
        // 绘制扇形边界
        int segments = 20;
        float angleStep = skillAngle / segments;
        Vector3 startPoint = transform.position;
        
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -skillAngle / 2 + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 endPoint = transform.position + direction * skillRadius;
            
            if (i > 0)
            {
                Gizmos.DrawLine(startPoint, endPoint);
            }
            startPoint = endPoint;
        }
        
        // 绘制扇形填充（半透明）
        Gizmos.color = new Color(sectorColor.r, sectorColor.g, sectorColor.b, 0.2f);
        Gizmos.DrawMesh(CreateSectorMesh(), transform.position, transform.rotation);
    }

    /// <summary>
    /// 创建矩形网格用于填充
    /// </summary>
    private Mesh CreateRectangleMesh()
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-skillWidth / 2, 0, -skillLength / 2);
        vertices[1] = new Vector3(skillWidth / 2, 0, -skillLength / 2);
        vertices[2] = new Vector3(skillWidth / 2, 0, skillLength / 2);
        vertices[3] = new Vector3(-skillWidth / 2, 0, skillLength / 2);
        
        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    /// <summary>
    /// 创建扇形网格用于填充
    /// </summary>
    private Mesh CreateSectorMesh()
    {
        Mesh mesh = new Mesh();
        
        int segments = 20;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        
        // 中心点
        vertices[0] = Vector3.zero;
        
        // 扇形边界点
        for (int i = 0; i <= segments; i++)
        {
            float angle = -skillAngle / 2 + (skillAngle / segments) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            vertices[i + 1] = direction * skillRadius;
        }
        
        // 创建三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    public void LateUpdate()
    {


    }

}
