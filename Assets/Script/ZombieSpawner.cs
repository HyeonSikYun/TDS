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
        // ó���� �ڷ�ƾ�� �����Ͽ� ������� ���������� ����
        StartCoroutine(SpawnZombiesWithInterval());
    }

    // �ڷ�ƾ�� ����Ͽ� 1�� �������� ���� ����
    IEnumerator SpawnZombiesWithInterval()
    {
        // ���� ��ġ�� �ε����� �����ϰ� ����
        List<int> spawnIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnIndexes.Add(i);
        }

        // �ε����� �������� ����
        for (int i = 0; i < spawnIndexes.Count; i++)
        {
            int temp = spawnIndexes[i];
            int randomIndex = Random.Range(i, spawnIndexes.Count);
            spawnIndexes[i] = spawnIndexes[randomIndex];
            spawnIndexes[randomIndex] = temp;
        }

        // �ִ� ���� ���� �� ������ ����
        while (currentZombieCount < maxZombieCount)
        {
            // ���� ������� ���� ���� (1�� ��������)
            for (int i = 0; i < spawnIndexes.Count; i++)
            {
                int index = spawnIndexes[i];  // �������� ���� �ε���

                // �� ���� �����ǿ� �´� ���� ������ ����
                if (index < zombiePrefabs.Length)  // zombiePrefabs �迭�� ũ�⸦ �Ѿ�� �ʵ��� Ȯ��
                {
                    GameObject zombiePrefab = zombiePrefabs[index];  // �ε����� ����Ͽ� Ư�� ���� �������� ����
                    Transform spawnPoint = spawnPoints[index];  // �� ���� ��ġ�� �´� ���� ����Ʈ ����

                    // ���� �ش� ��ġ�� ����
                    GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

                    // Zombie ��ũ��Ʈ ��������
                    Zombie zombieScript = zombie.GetComponent<Zombie>();

                    if (zombieScript != null)
                    {
                        // Ʈ���� ��ġ�� ���� ��ũ��Ʈ�� ����
                        zombieScript.truck = Truck;
                    }
                    else
                    {
                        Debug.LogError("���� �����տ� Zombie ��ũ��Ʈ�� �����ϴ�!");
                    }

                    // ���� �� ����
                    currentZombieCount++;

                    // �ִ� ���� ���� �����ϸ� ���� ����
                    if (currentZombieCount >= maxZombieCount)
                    {
                        break;
                    }

                    // 1�� �������� ���� ����
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}
