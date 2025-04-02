using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefabs;  // ���� ������ �迭
    public Transform[] spawnPoints;     // ���� ��ġ �迭
    public float spawnInterval = 5f;    // ���� ���� ����
    public Transform Truck;             // Ʈ���� Transform�� ����

    public int maxZombieCount = 30;     // �ִ� ���� ��
    private int currentZombieCount = 0; // ���� ���� ��

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
