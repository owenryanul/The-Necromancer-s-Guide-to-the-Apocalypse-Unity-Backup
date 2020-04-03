using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawning_Script : MonoBehaviour
{
    [Header("SpawnRates")]
    public float spawnCooldown;
    private float currentSpawnCooldown;

    [Header("Enemy Prefabs")]
    public GameObject EnemyType1;

    // Start is called before the first frame update
    void Start()
    {
        currentSpawnCooldown = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpawnCooldown -= Time.deltaTime;
        if(currentSpawnCooldown <= 0)
        {
            spawnEnemy();
            currentSpawnCooldown = spawnCooldown;
        }
    }

    private void spawnEnemy()
    {
        GameObject spawnPoint =  Space_Script.findGridEndSpace(3, Random.Range(0, 3));
        GameObject anEnemy = Instantiate(EnemyType1, spawnPoint.transform.position, spawnPoint.transform.rotation);
        anEnemy.GetComponent<Enemy_AI_script>().nextSpace = spawnPoint;
    }
}
