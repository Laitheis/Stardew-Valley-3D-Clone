using UnityEngine;
using System;
using Zenject;

public class GameTimeManager : MonoBehaviour
{
    public event Action OnMinutePassed;
    public event Action OnHourPassed;
    public event Action OnDayPassed;

    [Header("Config")]
    public float realSecondsPerGameMinute = 1f; // сколько секунд IRL занимает минута в игре
    public int dayStartHour = 0;                // начало дня
    public int dayEndHour = 24;                 // конец дня

    [Header("Current Time (debuggable in inspector)")]
    public int currentDay = 1;
    public int currentYear = 1;
    public Season currentSeason = Season.Spring;
    [Range(0, 23)] public int currentHour = 0;
    [Range(0, 59)] public int currentMinute = 0;

    [Header("Debug Controls")]
    public bool pauseTime = false;
    public bool skipMinute = false;
    public bool skipHour = false;
    public bool skipDay = false;

    [Inject] private CropManager _cropManager;

    private float timeAccumulator = 0f;

    public static GameTimeManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (pauseTime) return;

        timeAccumulator += Time.deltaTime;

        if (timeAccumulator >= realSecondsPerGameMinute)
        {
            timeAccumulator -= realSecondsPerGameMinute;
            AdvanceMinute();
        }

        if (skipMinute) { skipMinute = false; AdvanceMinute(); }
        if (skipHour) { skipHour = false; AdvanceHour(); }
        if (skipDay) { skipDay = false; AdvanceDay(); }
    }

    private void AdvanceMinute()
    {
        currentMinute++;
        if (currentMinute >= 60)
        {
            currentMinute = 0;
            AdvanceHour();
        }
        OnMinutePassed?.Invoke();
        //Debug.Log($"[GameTime] Minute advanced: {currentHour:D2}:{currentMinute:D2}");
    }

    private void AdvanceHour()
    {
        currentHour++;
        if (currentHour >= dayEndHour)
        {
            currentHour = dayStartHour;
            AdvanceDay();
        }
        OnHourPassed?.Invoke();
        Debug.Log($"[GameTime] Hour advanced: {currentHour}:00");
    }

    private void AdvanceDay()
    {
        currentDay++;
        if (currentDay > 28)
        {
            currentDay = 1;
            AdvanceSeason();
        }
        OnDayPassed?.Invoke();
        Debug.Log($"[GameTime] Day advanced: Day {currentDay}, Season {currentSeason}, Year {currentYear}");

        _cropManager.OnDayEnd(currentSeason);
    }

    private void AdvanceSeason()
    {
        currentSeason++;
        if ((int)currentSeason > 3)
        {
            currentSeason = Season.Spring;
            currentYear++;
        }
        Debug.Log($"[GameTime] Season advanced: {currentSeason}, Year {currentYear}");
    }
}
