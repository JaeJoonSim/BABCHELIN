using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewGimmickPattern", menuName = "Patterns/GimmickPattern")]
[System.Serializable]
public class GimmickScriptableObject : PatternScriptableObject
{
    public int triggerHealthLine;
    public event Action OnHealthThresholdReached;

    public void CheckHealthThreshold()
    {
        OnHealthThresholdReached?.Invoke();
    }

    public override void ExecutePattern(Skunk skunk)
    {
        Debug.Log("Gimmick : " + patternName);
    }
}
