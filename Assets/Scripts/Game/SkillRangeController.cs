using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能范围控制器 - 提供UI控制功能
/// </summary>
public class SkillRangeController : MonoBehaviour
{
    [Header("UI控制")]
    public Button toggleButton; // 切换显示按钮
    public Text statusText; // 状态显示文本
    public KeyCode toggleKey = KeyCode.V; // 切换显示的按键
    
    [Header("技能范围设置")]
    public Move playerMove; // 玩家移动组件
    public SkillRangeVisualizer visualizer; // 技能范围可视化器
    
    private bool isRangeVisible = false;
    
    void Start()
    {
        // 自动查找组件
        if (playerMove == null)
            playerMove = FindObjectOfType<Move>();
            
        if (visualizer == null && playerMove != null)
            visualizer = playerMove.GetComponent<SkillRangeVisualizer>();
        
        // 设置按钮事件
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSkillRange);
        }
        
        // 更新UI状态
        UpdateUI();
    }
    
    void Update()
    {
        // 按键切换显示
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleSkillRange();
        }
    }
    
    /// <summary>
    /// 切换技能范围显示
    /// </summary>
    public void ToggleSkillRange()
    {
        if (visualizer == null) return;
        
        isRangeVisible = !isRangeVisible;
        visualizer.ToggleDisplay();
        
        UpdateUI();
    }
    
    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        if (statusText != null)
        {
            statusText.text = isRangeVisible ? "技能范围: 显示" : "技能范围: 隐藏";
        }
        
        if (toggleButton != null)
        {
            Text buttonText = toggleButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isRangeVisible ? "隐藏范围" : "显示范围";
            }
        }
    }
    
    /// <summary>
    /// 显示技能范围
    /// </summary>
    public void ShowSkillRange()
    {
        if (visualizer == null) return;
        
        isRangeVisible = true;
        visualizer.ShowSkillRanges();
        UpdateUI();
    }
    
    /// <summary>
    /// 隐藏技能范围
    /// </summary>
    public void HideSkillRange()
    {
        if (visualizer == null) return;
        
        isRangeVisible = false;
        // 这里需要添加隐藏方法到SkillRangeVisualizer
        UpdateUI();
    }
    
    /// <summary>
    /// 设置技能参数
    /// </summary>
    public void SetSkillParameters(float radius, float width, float length, float angle)
    {
        if (playerMove == null) return;
        
        playerMove.skillRadius = radius;
        playerMove.skillWidth = width;
        playerMove.skillLength = length;
        playerMove.skillAngle = angle;
    }
} 