using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : BaseMonoBehaviour
{
    public PatternScriptableObject CurrentPattern;
    [SerializeField] private Queue<PatternScriptableObject> patternQueue = new Queue<PatternScriptableObject>();

    public List<BasicPatternScriptableObject> basicPatterns;
    public List<GimmickScriptableObject> gimmickPatterns;

    private void Update()
    {
        if (CurrentPattern == null)
        {
            DequeuePattern();
        }
        else
        {
            CurrentPattern.duration -= Time.deltaTime;
            if (CurrentPattern.duration <= 0)
            {
                CurrentPattern.onPatternEnd?.Invoke();
                DequeuePattern();
            }
        }
    }

    public void DequeuePattern()
    {
        if (patternQueue.Count > 0)
        {
            CurrentPattern = patternQueue.Dequeue();
            CurrentPattern.onPatternStart?.Invoke();
        }
    }

    public void EnqueuePattern(BasicPatternScriptableObject pattern)
    {
        patternQueue.Enqueue(pattern);
    }

    public void ClearPatterns()
    {
        patternQueue.Clear();
    }

    private void RegisterBasicPatterns()
    {
        int maxReservations = 3;

        for (int i = 0; i < maxReservations; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
            patternQueue.Enqueue(basicPatterns[randomIndex]);
        }
    }

    private void RegisterGimmickPatterns()
    {
        int maxReservations = 3;

        for (int i = 0; i < maxReservations; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, gimmickPatterns.Count);
            var randomGimmickPattern = gimmickPatterns[randomIndex];

            randomGimmickPattern.OnHealthThresholdReached += () =>
            {
                ClearPatterns();
                patternQueue.Enqueue(randomGimmickPattern);
            };
        }
    }

    public void Initialize()
    {
        patternQueue.Clear();
        RegisterBasicPatterns();
        RegisterGimmickPatterns();
        DequeuePattern();
    }
}
