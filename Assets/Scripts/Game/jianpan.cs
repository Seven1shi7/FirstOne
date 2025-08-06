using UnityEngine;

public class TriggerFollower : MonoBehaviour
{
    // 要跟随的目标物体
    public Transform target;
    // 跟随的偏移量（相对于目标物体的位置）
    public Vector3 offset = Vector3.zero;
    // 跟随的旋转偏移量
    public Vector3 rotationOffset = Vector3.zero;

    private Collider triggerCollider;

    void Start()
    {
        // 获取当前物体上的触发器组件
        triggerCollider = GetComponent<Collider>();

        // 确保当前物体的碰撞器已勾选Is Trigger
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning("请勾选碰撞器的Is Trigger选项");
            triggerCollider.isTrigger = true;
        }

        // 检查目标是否赋值
        if (target == null)
        {
            Debug.LogError("请指定要跟随的目标物体！");
        }
    }

    void Update()
    {
        // 如果目标存在，则更新位置和旋转
        if (target.tag=="Player")
        {
            // 设置位置：目标位置 + 偏移量
            transform.position = target.position + offset;

            // 设置旋转：目标旋转 + 旋转偏移量（可选）
            transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
        }
    }

    // 触发器检测示例
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发器接触到: " + other.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        // 持续接触时的逻辑
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("触发器离开: " + other.gameObject.name);
    }
}
