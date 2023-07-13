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
        Horseselect,
        Obstacletutorial,
        Endtutorial
    }

    private Status curStatus = Status.Horseselect;

    private HorseController[] horses; 

    private void Start()
    {
        horses = new HorseController[10];
        // 말 생성
        for(int i = 0; i< 10; i++) 
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

        // 장애물 생성

        
        // 지하도 막기
    }

    private void Update()
    {
      
    }

    public void OnSpawnExited()
    {
        foreach (var horse in horses)
        {
            if (!horse.isPlayerRiding) Destroy(horse.gameObject);
            else PlayerManager.Instance().horse = horse;
        }
        
        curStatus = Status.Obstacletutorial;

        //if (GameSettings.Values.doAutoSave) GameManager.Instance().Save.SaveToPrefs(0);
    }
}
