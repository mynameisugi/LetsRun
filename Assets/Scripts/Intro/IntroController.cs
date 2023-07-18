using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private GameObject horsePrefab;

    [SerializeField]
    private Transform horseSpawn;

    private enum Status
    {
        /// <summary>
        /// 말 선택 이전
        /// </summary>
        HorseSelect,
        /// <summary>
        /// 말 탑승 직후, 가속 방법 설명
        /// </summary>
        HorseJustRode,
        /// <summary>
        /// 말 가속해봄. 말 감속/회전 설명
        /// </summary>
        HorseRan,
        /// <summary>
        /// 말 선택 확정
        /// </summary>
        Obstacletutorial
    }

    private Status curStatus = Status.HorseSelect;

    private HorseController[] horses;

    private void Start()
    {
        Debug.Log("Start Intro!");
        GameManager.Instance().Save.SaveToPrefs(0);

        horses = new HorseController[10];
        // 말 생성
        for (int i = 0; i < 10; i++)
        {
            var horseObject = Instantiate(horsePrefab);

            float spawnSize = horseSpawn.localScale.x * 0.5f;
            Vector3 randomPosition = horseSpawn.position + new Vector3(Random.Range(-spawnSize, spawnSize), 0f, Random.Range(-spawnSize, spawnSize));
            horseObject.transform.position = randomPosition;

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            horseObject.transform.rotation = randomRotation;

            // 스킨 설정
            var horse = horseObject.GetComponent<HorseController>();
            horse.playerRidable = true;
            horse.stats.skin = i;

            horses[i] = horse;
        }
    }

    private PlayerManager player = null;

    private void Update()
    {
        if (!player)
        {
            player = PlayerManager.Instance();
            if (!player) return;
        }
        switch (curStatus)
        {
            case Status.HorseSelect:
                if (player.IsRiding)
                {
                    curStatus = Status.HorseJustRode;
                    player.GUI.SetMessageBoxText("가속하려면 양손을 뒤로 당겼다 빠르게 앞으로 내리쳐 채찍질합니다.");
                }
                break;

            case Status.HorseJustRode:
                foreach (var horse in horses)
                {
                    if (horse.isPlayerRiding)
                    {
                        if (horse.CurMode > 2f)
                        {
                            curStatus = Status.HorseRan;
                            player.GUI.SetMessageBoxText("감속하려면 양손을 쥐고 뒤로 당깁니다.\n옆으로 틀려면 한 손만 뒤로 당깁니다.");
                        }
                        break;
                    }
                }
                break;

            case Status.HorseRan:
                player.GUI.SetMessageBoxText("오른손 잡기 트리거를 누르고\n왼손의 손목시계를 조작해 메뉴를 사용할 수 있습니다.");
                break;
        }
    }

    public void OnSpawnExited()
    {
        foreach (var horse in horses)
        {
            if (!horse.isPlayerRiding) Destroy(horse.gameObject);
            else PlayerManager.Instance().horse = horse;
        }

        curStatus = Status.Obstacletutorial;
        player.GUI.SetMessageBoxText(string.Empty);
    }

    public void EndTutorial()
    {
        player.GUI.SetMessageBoxText(string.Empty);
        if (GameSettings.Values.doAutoSave) GameManager.Instance().Save.SaveToPrefs(0);
        Destroy(gameObject);
    }
}
