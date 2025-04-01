using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefabs;  // 좀비 프리팹 배열
    public Transform[] spawnPoints;     // 스폰 위치 배열
    public float spawnInterval = 5f;    // 좀비 스폰 간격
    public Transform Truck;             // 트럭의 Transform을 참조

    public int maxZombieCount = 30;     // 최대 좀비 수
    private int currentZombieCount = 0; // 현재 좀비 수

    void Start()
    {
        // 처음에 코루틴을 시작하여 좀비들을 순차적으로 생성
        StartCoroutine(SpawnZombiesWithInterval());
    }

    // 코루틴을 사용하여 1초 간격으로 좀비 생성
    IEnumerator SpawnZombiesWithInterval()
    {
        // 스폰 위치의 인덱스를 랜덤하게 섞기
        List<int> spawnIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnIndexes.Add(i);
        }

        // 인덱스를 랜덤으로 섞기
        for (int i = 0; i < spawnIndexes.Count; i++)
        {
            int temp = spawnIndexes[i];
            int randomIndex = Random.Range(i, spawnIndexes.Count);
            spawnIndexes[i] = spawnIndexes[randomIndex];
            spawnIndexes[randomIndex] = temp;
        }

        // 최대 좀비 수가 될 때까지 생성
        while (currentZombieCount < maxZombieCount)
        {
            // 랜덤 순서대로 좀비 스폰 (1초 간격으로)
            for (int i = 0; i < spawnIndexes.Count; i++)
            {
                int index = spawnIndexes[i];  // 랜덤으로 섞인 인덱스

                // 각 스폰 포지션에 맞는 좀비 프리팹 선택
                if (index < zombiePrefabs.Length)  // zombiePrefabs 배열의 크기를 넘어가지 않도록 확인
                {
                    GameObject zombiePrefab = zombiePrefabs[index];  // 인덱스를 사용하여 특정 좀비 프리팹을 선택
                    Transform spawnPoint = spawnPoints[index];  // 각 스폰 위치에 맞는 스폰 포인트 선택

                    // 좀비를 해당 위치에 생성
                    GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

                    // Zombie 스크립트 가져오기
                    Zombie zombieScript = zombie.GetComponent<Zombie>();

                    if (zombieScript != null)
                    {
                        // 트럭의 위치를 좀비 스크립트에 설정
                        zombieScript.truck = Truck;
                    }
                    else
                    {
                        Debug.LogError("좀비 프리팹에 Zombie 스크립트가 없습니다!");
                    }

                    // 좀비 수 증가
                    currentZombieCount++;

                    // 최대 좀비 수에 도달하면 루프 종료
                    if (currentZombieCount >= maxZombieCount)
                    {
                        break;
                    }

                    // 1초 간격으로 좀비 생성
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}
