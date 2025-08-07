#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    void OnSceneGUI()
    {
        SkillData skill = (SkillData)target;
        Handles.color = skill.previewColor;
        Vector3 pos = Vector3.zero;
        switch (skill.shape)
        {
            case SkillShape.Circle:
                Handles.DrawWireDisc(pos, Vector3.up, skill.range);
                break;
            case SkillShape.Rectangle:
                Vector3 size = new Vector3(skill.width, 0, skill.range);
                Handles.DrawWireCube(pos + Vector3.forward * skill.range / 2, size);
                break;
            case SkillShape.Sector:
                Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0, -skill.angle/2, 0) * Vector3.forward, skill.angle, skill.range);
                break;
            case SkillShape.Projectile:
                Handles.DrawLine(pos, pos + Vector3.forward * skill.range);
                break;
        }
    }
}
#endif
