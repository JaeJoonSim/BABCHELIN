using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : BaseMonoBehaviour
{
    public PatternScriptableObject CurrentPattern;
    public List<PatternScriptableObject> patternList;

    public List<BasicPatternScriptableObject> basicPatterns;
    public List<GimmickScriptableObject> gimmickPatterns;
    public List<GimmickScriptableObject> usedGimmickPatterns;

    [SerializeField] private float remainingPatternDuration;
    [SerializeField] private Health health;

    private void Update()
    {
        if (CurrentPattern == null)
        {
            DequeuePattern(health);
        }
        else
        {
            remainingPatternDuration -= Time.deltaTime;

            // Add this block
            if (gimmickPatterns.Count > 0)
            {
                GimmickScriptableObject selectedGimmick = gimmickPatterns[0];
                if (health.multipleHealthLine <= selectedGimmick.triggerHealthLine)
                {
                    patternList.Insert(0, selectedGimmick);
                    usedGimmickPatterns.Add(selectedGimmick);  // Add the used gimmick to the usedGimmickPatterns list
                    gimmickPatterns.RemoveAt(0);
                }
            }

            if (remainingPatternDuration <= 0)
            {
                CurrentPattern.onPatternEnd?.Invoke();
                DequeuePattern(health);
            }
        }
    }

    public void DequeuePattern(Health health)
    {
        if (patternList.Count > 0)
        {
            if (CurrentPattern is GimmickScriptableObject usedGimmick)
            {
                usedGimmickPatterns.Add(usedGimmick); // Add the used gimmick to the usedGimmickPatterns list
            }

            CurrentPattern = patternList[0];
            patternList.RemoveAt(0);
            remainingPatternDuration = CurrentPattern.duration;

            Skunk skunk = FindObjectOfType<Skunk>();
            skunk.state.CURRENT_STATE = CurrentPattern.patternState;

            CurrentPattern.onPatternStart?.Invoke();
        }

        if (patternList.Count < 3)
        {
            ReserveNewPattern(health);
        }
    }

    private void ReserveNewPattern(Health health)
    {
        int randomIndex;

        while (patternList.Count < 3)
        {
            if (basicPatterns.Count > 0 && gimmickPatterns.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count + gimmickPatterns.Count);

                if (randomIndex < basicPatterns.Count) // Basic Pattern
                {
                    patternList.Add(basicPatterns[randomIndex]);
                }
                else // Gimmick Pattern
                {
                    List<GimmickScriptableObject> validGimmicks = new List<GimmickScriptableObject>();

                    foreach (GimmickScriptableObject gimmick in gimmickPatterns)
                    {
                        if (health.multipleHealthLine <= gimmick.triggerHealthLine)
                        {
                            validGimmicks.Add(gimmick);
                        }
                    }

                    if (validGimmicks.Count > 0)
                    {
                        randomIndex = UnityEngine.Random.Range(0, validGimmicks.Count);
                        GimmickScriptableObject selectedGimmick = validGimmicks[randomIndex];
                        patternList.Insert(0, selectedGimmick);
                        Debug.Log("기믹패턴예약");
                        gimmickPatterns.Remove(selectedGimmick);
                    }
                }
            }
            else if (basicPatterns.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
                patternList.Add(basicPatterns[randomIndex]);
            }
            else if (gimmickPatterns.Count > 0)
            {
                List<GimmickScriptableObject> validGimmicks = new List<GimmickScriptableObject>();

                foreach (GimmickScriptableObject gimmick in gimmickPatterns)
                {
                    if (health.multipleHealthLine <= gimmick.triggerHealthLine)
                    {
                        validGimmicks.Add(gimmick);
                    }
                }

                if (validGimmicks.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, validGimmicks.Count);
                    GimmickScriptableObject selectedGimmick = validGimmicks[randomIndex];
                    patternList.Insert(0, selectedGimmick);
                    Debug.Log("기믹패턴예약");
                    gimmickPatterns.Remove(selectedGimmick);
                }
            }
        }
    }

    public void EnqueuePattern(BasicPatternScriptableObject pattern)
    {
        if (patternList.Count >= 3)
        {
            return;
        }

        patternList.Add(pattern);
    }

    public void ClearPatterns()
    {
        patternList.Clear();
    }

    private void RegisterBasicPatterns()
    {
        int maxReservations = 3;

        for (int i = 0; i < maxReservations; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
            patternList.Add(basicPatterns[randomIndex]);
        }
    }

    private void RegisterGimmickPatterns()
    {
        int maxReservations = 3;

        for (int i = 0; i < maxReservations; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, gimmickPatterns.Count);
            var randomGimmickPattern = gimmickPatterns[randomIndex];

            if (health.multipleHealthLine <= randomGimmickPattern.triggerHealthLine)
            {
                randomGimmickPattern.OnHealthThresholdReached += () =>
                {
                    ClearPatterns();
                    patternList.Add(randomGimmickPattern);
                    Debug.Log("기믹패턴 예약");
                };
            }
        }
    }

    public void Initialize()
    {
        patternList.Clear();
        RegisterBasicPatterns();

        // Add this block
        while (patternList.Count < 3)
        {
            ReserveNewPattern(health);
        }

        // Make sure no gimmick pattern is reserved at game start
        int i = 0;
        while (i < patternList.Count)
        {
            if (patternList[i] is GimmickScriptableObject gimmick &&
                health.multipleHealthLine > gimmick.triggerHealthLine)
            {
                patternList.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        DequeuePattern(health);
    }

}
