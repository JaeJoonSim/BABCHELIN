using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PatternManager : MonoBehaviour
{
    [SerializeField] private Queue<BossPattern> patternQueue = new Queue<BossPattern>();
    [SerializeField] private BossPattern currentPattern = null;

    public BossPattern CurrentPattern
    {
        get { return currentPattern; }
        set { currentPattern = value; }
    }

    public void EnqueuePattern(BossPattern pattern)
    {
        if (patternQueue.Count < 3)
        {
            patternQueue.Enqueue(pattern);
        }
    }

    public void DequeuePattern()
    {
        if (patternQueue.Count > 0)
        {
            currentPattern = patternQueue.Dequeue();
            currentPattern.onPatternStart?.Invoke();

            StartCoroutine(ExecutePattern(currentPattern.duration, () =>
            {
                currentPattern.onPatternEnd?.Invoke();
                currentPattern = null;
            }));
        }
    }

    private IEnumerator ExecutePattern(float duration, Action onPatternCompleted)
    {
        yield return new WaitForSeconds(duration);
        onPatternCompleted?.Invoke();
        DequeuePattern();
    }

    public void ClearPatterns(BossPattern.PatternType patternType)
    {
        patternQueue = new Queue<BossPattern>(patternQueue.Where(p => p.type != patternType));
    }
}