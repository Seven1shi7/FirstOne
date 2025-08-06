using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JIneng : MonoBehaviour
{
    private void Start()
    {
        
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
}
