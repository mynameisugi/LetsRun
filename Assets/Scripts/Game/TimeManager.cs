using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    public const string SAVEKEY = "Time";

    /// <summary>
    /// 경기 시작 주기. (600초: 10분)
    /// </summary>
    public const int LOOP = 600;

    public TimeManager(float time)
    {
        // 저장된 시간 불러오기
        //Now = time;
        Now = 0f;

        // 저장할 때 현재 시간 저장
        GameManager.Instance().Save.OnSaveToPref += (save) =>
        { save.SaveValue(SAVEKEY, Mathf.FloorToInt(Now)); };

        // 이벤트를 저장할 리스트 초기화
        events.Clear();
    }

    /// <summary>
    /// 경기 시작으로부터 현재 시간. [0, <see cref="LOOP"/>)
    /// </summary>
    public float Now { get; private set; } = 0f;

    public void Update()
    {
        if (events.Count > 0 && Now < nextEvent && Now + Time.deltaTime >= nextEvent)
        {
            InvokeEvents();
        }
        // 현재 시간 갱신
        Now = (Now + Time.deltaTime) % LOOP;
    }

    private void InvokeEvents()
    {
        // 다음 이벤트를 시행할 시간이 되면 등록된 이벤트 시행
        foreach (var a in events[nextEventIdx].actions.ToArray()) a?.Invoke();
        // 다음 이벤트 시간을 마크
        nextEventIdx = (nextEventIdx + 1) % events.Count;
        nextEvent = events[nextEventIdx].time;
    }

    public void RequestSkipForward(int skip)
    {
        // TODO: IEnumerator로 만들고, 페이드 아웃 / 인 효과 넣기
        int now = Mathf.FloorToInt(Now);
        for (int i = 0; i < skip; ++i)
        {
            ++now;
            if (nextEvent == now) InvokeEvents();
        }
        Now = now;
        if (GameSettings.Values.doAutoSave) // 자동 저장
            GameManager.Instance().Save.SaveToPrefs(0);
    }

    private readonly List<Event> events = new();
    private int nextEvent = 0;
    private int nextEventIdx = 0;

    private struct Event
    {
        public Event(int time, Action action)
        {
            this.time = time;
            actions = new List<Action> { action };
        }

        public int time;
        public List<Action> actions;

        public static int Comparer(Event A, Event B) => A.time.CompareTo(B.time);
    }

    /// <summary>
    /// 새로운 이벤트를 등록
    /// </summary>
    /// <param name="time">원하는 시간(초). <see cref="LOOP"/> = 경기 시작</param>
    /// <param name="action">실행하고자하는 함수</param>
    public void RegisterEvent(int time, Action action)
    {
        for (int i = 0; i < events.Count; ++i)
        {
            if (events[i].time == time) // 이미 존재하는 시간의 이벤트면
            {
                events[i].actions.Add(action); // 액션만 추가하고 종료
                return;
            }
        }
        events.Add(new Event(time, action));
        events.Sort(Event.Comparer);
        if (events.Count == 1 || // 첫 이벤트이거나
            (Now < time && (nextEvent < Now || time < nextEvent))) // 새 이벤트가 다음 이벤트보다 먼저면
            nextEvent = time; // 다음 이벤트를 새 이벤트로 재지정
        nextEventIdx = events.FindIndex(x => x.time == nextEvent); // 다음 이벤트 인덱스 다시 찾기
    }

    /// <summary>
    /// 등록한 이벤트를 제거
    /// </summary>
    /// <param name="time">제거하려는 함수가 등록됐던 시간</param>
    /// <param name="action">제거하려는 함수</param>
    /// <returns>제거 성공 여부</returns>
    public bool UnregisterEvent(int time, Action action)
    {
        for (int i = 0; i < events.Count; ++i)
        {
            if (events[i].time == time)
            {
                return events[i].actions.Remove(action);
            }
        }
        return false;
    }

}