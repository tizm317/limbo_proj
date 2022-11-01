using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_", menuName = "Skill System/Skill Data", order = 1)]
public class SkillData : ScriptableObject
{
    public string Name { get { return _name; } set { _name = value; } }
    public Sprite IconSprite { get { return _iconSprite; } set { _iconSprite = value; } }
    public string Tooltip { get { return _tooltip; } set { _tooltip = value; } }

    [Tooltip("스킬 이름")]
    [SerializeField] private string _name;
    [Tooltip("스킬 아이콘")]
    [SerializeField] private Sprite _iconSprite;

    [Tooltip("스킬 설명")]
    [TextArea] // 여러줄 + 자동 줄바꿈 (vs Multline(자동X))
    [SerializeField] private string _tooltip;
}
