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

    public event Action<PatternScriptableObject> OnPatternChange;

    Skunk skunk;

    private void Start()
    {
        skunk = health.gameObject.GetComponent<Skunk>();
    }

    private void Update()
    {
        if (CurrentPattern == null && !skunk.destructionStun && !skunk.isPatternPause)
        {
            DequeuePattern(health);
        }
        else if (CurrentPattern != null && !skunk.destructionStun && !skunk.isPatternPause)
        {
            remainingPatternDuration -= Time.deltaTime;

            if (gimmickPatterns.Count > 0)
            {
                GimmickScriptableObject selectedGimmick = gimmickPatterns[0];
                if (health.multipleHealthLine <= selectedGimmick.triggerHealthLine)
                {
                    patternList.Insert(0, selectedGimmick);
                    usedGimmickPatterns.Add(selectedGimmick);
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
                usedGimmickPatterns.Add(usedGimmick);
            }

            CurrentPattern = patternList[0];
            patternList.RemoveAt(0);
            remainingPatternDuration = CurrentPattern.duration;

            Skunk skunk = FindObjectOfType<Skunk>();
            skunk.state.CURRENT_STATE = CurrentPattern.patternState;
            skunk.wasFarting = false;
            CurrentPattern.onPatternStart?.Invoke();
            OnPatternChange?.Invoke(CurrentPattern);
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

                if (randomIndex < basicPatterns.Count)
                {
                    BasicPatternScriptableObject selectedPattern = basicPatterns[randomIndex];
                    if (patternList.Count == 0 || !patternList[patternList.Count - 1].Equals(selectedPattern))
                    {
                        patternList.Add(selectedPattern);
                    }
                }
                else
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
                        if (patternList.Count == 0 || !patternList[patternList.Count - 1].Equals(selectedGimmick))
                        {
                            patternList.Insert(0, selectedGimmick);
                            Debug.Log("기믹패턴예약");
                            gimmickPatterns.Remove(selectedGimmick);
                        }
                    }
                }
            }
            else if (basicPatterns.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
                BasicPatternScriptableObject selectedPattern = basicPatterns[randomIndex];
                if (patternList.Count == 0 || !patternList[patternList.Count - 1].Equals(selectedPattern))
                {
                    patternList.Add(selectedPattern);
                }
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
                    if (patternList.Count == 0 || !patternList[patternList.Count - 1].Equals(selectedGimmick))
                    {
                        patternList.Insert(0, selectedGimmick);
                        Debug.Log("기믹패턴예약");
                        gimmickPatterns.Remove(selectedGimmick);
                    }
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
            BasicPatternScriptableObject selectedPattern;
            do
            {
                int randomIndex = UnityEngine.Random.Range(0, basicPatterns.Count);
                selectedPattern = basicPatterns[randomIndex];
            }
            while (patternList.Count > 0 && patternList[patternList.Count - 1].Equals(selectedPattern));

            patternList.Add(selectedPattern);
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

        while (patternList.Count < 3)
        {
            ReserveNewPattern(health);
        }

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

        if (!skunk.destructionStun && !skunk.isPatternPause)
            DequeuePattern(health);
    }

}
