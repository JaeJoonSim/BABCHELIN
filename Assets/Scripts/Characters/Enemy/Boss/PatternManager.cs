using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PatternManager : MonoBehaviour
{
    [SerializeField] private BossPattern currentPattern;
    [SerializeField] private List<BossPattern> patternQueue;
    [SerializeField] private int maxQueueSize = 3;
    public List<BossPattern> basicPatterns { get; set; }

    public BossPattern CurrentPattern
    {
        get { return currentPattern; }
        set { currentPattern = value; }
    }

    public void EnqueuePattern(BossPattern pattern)
    {
        if (patternQueue.Count < maxQueueSize || pattern.type == BossPattern.PatternType.Gimmick)
        {
            patternQueue.Add(pattern);
        }
    }

    public BossPattern DequeuePattern()
    {
        if (patternQueue.Count > 0)
        {
            BossPattern dequeuedPattern = patternQueue[0];
            patternQueue.RemoveAt(0);
            return dequeuedPattern;
        }
        else
        {
            // 큐가 비어 있을 때 BasicPatterns에서 랜덤한 기본 패턴을 반환합니다.
            int randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
            return basicPatterns[randomIndex];
        }
    }


    public void ClearPatterns(BossPattern.PatternType patternType)
    {
        patternQueue.RemoveAll(pattern => pattern.type == patternType);
    }

    private IEnumerator ExecutePattern(float duration, Action onPatternCompleted)
    {
        yield return new WaitForSeconds(duration);
        onPatternCompleted?.Invoke();
        DequeuePattern();
    }
}