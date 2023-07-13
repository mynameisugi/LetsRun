using System.Collections;
using System.Collections.Generic;
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
        /// �� ���� ����
        /// </summary>
        HorseSelect,
        /// <summary>
        /// �� ž�� ����, ���� ��� ����
        /// </summary>
        HorseJustRode,
        /// <summary>
        /// �� �����غ�. �� ����/ȸ�� ����
        /// </summary>
        HorseRan,
        /// <summary>
        /// �� ���� Ȯ��
        /// </summary>
        Obstacletutorial
    }

    private Status curStatus = Status.HorseSelect;

    private HorseController[] horses; 

    private void Start()
    {
        horses = new HorseController[10];
        // �� ����
        for(int i = 0; i< 10; i++) 
        {
            var horseObject = Instantiate(horsePrefab);

            float spawnSize = horseSpawn.localScale.x * 0.5f;
            Vector3 randomPosition = horseSpawn.position + new Vector3(Random.Range(-spawnSize, spawnSize), 0f, Random.Range(-spawnSize, spawnSize));
            horseObject.transform.position = randomPosition;

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            horseObject.transform.rotation = randomRotation;

            // ��Ų ����
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
                    player.GUI.SetMessageBoxText("�����Ϸ��� ����� �ڷ� ���� ������ ������ ������ ä�����Ѵ�.");
                }
                break;

            case Status.HorseJustRode:
                foreach (var horse in horses)
                {
                    if (horse.isPlayerRiding) {
                        if(horse.CurMode > 2f)
                        {
                            curStatus = Status.HorseRan;
                            player.GUI.SetMessageBoxText("�����Ϸ��� ����� ��� �ڷ� ����.\n������ Ʋ���� �� �ո� �ڷ� ����.");
                        }
                        break;
                    }
                }
                break;

            case Status.HorseRan:
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

        //if (GameSettings.Values.doAutoSave) GameManager.Instance().Save.SaveToPrefs(0);
    }
}
