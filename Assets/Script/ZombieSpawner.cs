using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform spawnPoint;
    public Transform towerTarget; 
    public float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnZombieRoutine());
    }

    IEnumerator SpawnZombieRoutine()
    {
        while (true)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnZombie()
    {
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
        Zombie moveScript = zombie.AddComponent<Zombie>();
        moveScript.target = towerTarget;
    }
}
