using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 다음 경기를 시행하는 매니저
/// </summary>
public class RaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject racePrefab;

    public enum RaceType
    {
        Easy, // 500m
        Normal, // 1000m
        Hard // 1500m
    }

    private void Start()
    {
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 60, () => { // 1분 전에 다음 경기 준비
            var raceObj = Instantiate(racePrefab, transform);
            CurrentRace = raceObj.GetComponent<Race>();
            CurrentRace.type = NextType;

            NextType = (RaceType)Random.Range(0, 3); // 그 다음 경기 랜덤 선택
        });
    }

    /// <summary>
    /// 현재 경기
    /// </summary>
    public Race CurrentRace { get; private set; } = null;

    /// <summary>
    /// 다음 경기 종류
    /// </summary>
    public RaceType NextType { get; private set; } = RaceType.Easy;

    /// <summary>
    /// 다음 경기 종류를 수정
    /// </summary>
    public void SetNextRace(RaceType type) => NextType = type;

}