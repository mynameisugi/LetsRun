using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 다음 경기를 시행하는 매니저
/// </summary>
public class RaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject racePrefab;

    [SerializeField]
    private Race.RaceInfo[] infos;

    public enum RaceType
    {
        Easy = 0, // 500m
        Normal = 1, // 1000m
        Hard = 2 // 1500m
    }

    private void Start()
    {
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 60, () => { // 1분 전에 다음 경기 준비
            var raceObj = Instantiate(racePrefab, transform);
            CurrentRace = raceObj.GetComponent<Race>();
            CurrentRace.info = infos[(int)NextRace];

            NextRace = (RaceType)Random.Range(0, 3); // 그 다음 경기 랜덤 선택
        });
    }

    /// <summary>
    /// 현재 경기
    /// </summary>
    public Race CurrentRace { get; private set; } = null;

    /// <summary>
    /// 다음 경기 종류
    /// </summary>
    public RaceType NextRace { get; private set; } = RaceType.Easy;

    /// <summary>
    /// 다음 경기 종류를 수정
    /// </summary>
    public void SetNextRace(RaceType type) => NextRace = type;

}