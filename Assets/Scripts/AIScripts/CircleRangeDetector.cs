using UnityEngine;

/// <summary>
/// 圆形范围检测器
/// </summary>
public class CircleRangeDetector : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("检测半径")]
    public float radius = 5f;
    [Tooltip("要检测的目标层级")]
    public LayerMask targetLayer;

    [Header("调试设置")]
    [Tooltip("是否显示检测范围")]
    public bool drawGizmos = true;
    [Tooltip("调试显示颜色")]
    public Color gizmoColor = Color.green;

    /// <summary>
    /// 检测目标是否在圆形范围内
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    /// <returns>是否在范围内</returns>
    public bool IsTargetInRange(Vector3 targetPosition)
    {
        // 计算与目标的距离
        float distance = Vector3.Distance(transform.position, targetPosition);
        // 判断距离是否小于等于半径
        return distance <= radius;
    }

    /// <summary>
    /// 获取圆形范围内的所有碰撞体
    /// </summary>
    /// <returns>范围内的碰撞体数组</returns>
    public Collider[] GetTargetsInRange()
    {
        // 使用Physics.OverlapSphere检测球形范围内的碰撞体
        return Physics.OverlapSphere(transform.position, radius, targetLayer);
    }

    /// <summary>
    /// 获取圆形范围内特定类型的对象
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件数组</returns>
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

        // 裁剪数组以移除空元素
        System.Array.Resize(ref components, count);
        return components;
    }

    /// <summary>
    /// 场景调试绘制
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}