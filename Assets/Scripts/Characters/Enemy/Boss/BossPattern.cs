using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewBossPattern", menuName = "BossPattern")]
public class BossPattern : ScriptableObject
{
    public enum PatternType
    {
        Basic,
        Gimmick
    }

    public string patternName;
    public PatternType type;
    public float duration;
    public Action onPatternStart;
    public Action onPatternEnd;
}