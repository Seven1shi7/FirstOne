using UnityEngine;

/// <summary>
/// 伤害测试脚本 - 用于快速测试伤害系统
/// </summary>
public class DamageTest : MonoBehaviour
{
    [Header("测试设置")]
    public KeyCode testDamageKey = KeyCode.T; // 测试伤害按键
    public float testDamage = 10f; // 测试伤害值
    public string testDamageType = "测试"; // 测试伤害类型
    
    [Header("自动测试")]
    public bool autoTest = false; // 自动测试
    public float testInterval = 2f; // 测试间隔
    
    private float testTimer = 0f;
    
    void Update()
    {
        // 手动测试
        if (Input.GetKeyDown(testDamageKey))
        {
            TestDamage();
        }
        
        // 自动测试
        if (autoTest)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testInterval)
            {
                TestDamage();
                testTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// 测试伤害
    /// </summary>
    public void TestDamage()
    {
        Debug.Log("=== 开始伤害测试 ===");
        
        // 查找所有敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemies.Length == 0)
        {
            Debug.LogWarning("没有找到任何敌人！请确保场景中有带'Enemy'标签的敌人");
            return;
        }
        
        Debug.Log($"找到 {enemies.Length} 个敌人，开始测试伤害");
        
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Debug.Log($"对敌人 {enemy.name} 造成 {testDamage} 点 {testDamageType} 伤害");
                enemyComponent.TakeDamage(testDamage, testDamageType);
            }
            else
            {
                Debug.LogError($"敌人 {enemy.name} 没有Enemy组件！");
            }
        }
        
        Debug.Log("=== 伤害测试完成 ===");
    }
    
    /// <summary>
    /// 检查敌人状态
    /// </summary>
    public void CheckEnemyStatus()
    {
        Debug.Log("=== 检查敌人状态 ===");
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Debug.Log($"敌人 {enemy.name}: 生命值 {enemyComponent.currentHealth}/{enemyComponent.maxHealth}, 存活: {enemyComponent.IsAlive()}");
            }
        }
        
        Debug.Log("=== 状态检查完成 ===");
    }
    
    /// <summary>
    /// 重置所有敌人生命值
    /// </summary>
    public void ResetAllEnemies()
    {
        Debug.Log("=== 重置所有敌人生命值 ===");
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.currentHealth = enemyComponent.maxHealth;
                enemyComponent.UpdateHealthUI();
                Debug.Log($"重置敌人 {enemy.name} 的生命值");
            }
        }
        
        Debug.Log("=== 重置完成 ===");
    }
    
    void OnGUI()
    {
        // 显示测试信息
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("伤害测试工具");
        GUILayout.Label($"按 {testDamageKey} 测试伤害");
        GUILayout.Label($"测试伤害: {testDamage}");
        GUILayout.Label($"自动测试: {(autoTest ? "开启" : "关闭")}");
        
        if (GUILayout.Button("检查敌人状态"))
        {
            CheckEnemyStatus();
        }
        
        if (GUILayout.Button("重置所有敌人"))
        {
            ResetAllEnemies();
        }
        
        GUILayout.EndArea();
    }
} 