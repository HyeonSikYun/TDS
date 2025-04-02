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
        StartCoroutine(SpawnZombiesWithInterval());
    }

    IEnumerator SpawnZombiesWithInterval()
    {
        List<int> spawnIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnIndexes.Add(i);
        }

        for (int i = 0; i < spawnIndexes.Count; i++)
        {
            int temp = spawnIndexes[i];
            int randomIndex = Random.Range(i, spawnIndexes.Count);
            spawnIndexes[i] = spawnIndexes[randomIndex];
            spawnIndexes[randomIndex] = temp;
        }

        while (currentZombieCount < maxZombieCount)
        {
            for (int i = 0; i < spawnIndexes.Count; i++)
            {
                int index = spawnIndexes[i];  

                if (index < zombiePrefabs.Length)  
                {
                    GameObject zombiePrefab = zombiePrefabs[index];  
                    Transform spawnPoint = spawnPoints[index];  

                    GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

                    Zombie zombieScript = zombie.GetComponent<Zombie>();

                    if (zombieScript != null)
                    {
                        zombieScript.truck = Truck;
                    }

                    currentZombieCount++;

                    if (currentZombieCount >= maxZombieCount)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}
