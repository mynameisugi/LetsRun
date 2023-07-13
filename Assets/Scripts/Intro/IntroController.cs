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

    private void Start()
    {
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
            horse.stats.skin = i;
        }

        // ��ֹ� ����

        
        // ���ϵ� ����
    }

    private void Update()
    {
      
    }
}
