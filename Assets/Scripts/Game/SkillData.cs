using UnityEngine;

public enum SkillShape { Circle, Rectangle, Sector, Projectile }

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public SkillShape shape;
    public float range = 5f;
    public float width = 3f;
    public float angle = 60f;
    public float damage = 30f;
    public float cooldown = 2f;
    public GameObject effectPrefab;
    public Color previewColor = Color.red;
    // 可扩展更多参数
}
