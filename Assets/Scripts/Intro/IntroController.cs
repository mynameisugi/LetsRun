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

            Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            horseObject.transform.position = randomPosition;

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            horseObject.transform.rotation = randomRotation;    
        }

        // ��ֹ� ����

        
        // ���ϵ� ����
    }

    private void Update()
    {
      
    }
}
