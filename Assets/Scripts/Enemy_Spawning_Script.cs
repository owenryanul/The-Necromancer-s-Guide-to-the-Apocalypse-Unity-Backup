using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public class Enemy_Spawning_Script : MonoBehaviour
{
    [Header("SpawnRates")]
    private float currentSpawnCooldown;

    /*[Header("Enemy Prefabs")]
    public GameObject EnemyType1;*/

    private bool hordeIsSpawning;
    private Horde currentHorde;
    private int currentWaveCount;
    private Wave currentWave;

    [Header("Hordes")]
    [SerializeField]
    public List<Horde> hordes;

    // Start is called before the first frame update
    void Start()
    {
        currentSpawnCooldown = 0;
        currentWaveCount = 0;
        hordeIsSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hordeIsSpawning)
        {
            updateWaves();

            currentSpawnCooldown -= Time.deltaTime;
            if (currentSpawnCooldown <= 0)
            {
                spawnEnemyFromPool();
                currentWave.numberOfEnemiesInWave--;
                Debug.Log("Wave " + (currentWaveCount + 1) + " Number of spawns remaining: " + currentWave.numberOfEnemiesInWave);
                currentSpawnCooldown = currentWave.spawnCooldown;
            }
        }
    }

    //Advances the current horde to the next wave if that wave is completed(has spawned the set number of enemies)
    private void updateWaves()
    {
        if(currentWave.numberOfEnemiesInWave < 1)
        {
            Debug.Log("Wave " + currentWaveCount + 1 + " Complete.");
            currentWaveCount++;
            //if the previous wave was the last. Stop spawning enemies
            if(currentWaveCount >= currentHorde.waves.Count)
            {
                //End of horde
                hordeIsSpawning = false;
                Debug.Log("Horde: " + currentHorde.name + " Finished.");
            }
            else
            {
                //Advance to the next wave, rolling any remaining pools from the new wave if set to.
                Wave temp = currentWave;
                currentWave = currentHorde.waves[currentWaveCount];

                //if last wave was set to roll remainder into next wave. Add any pools from that wave, that still have enemies, to the new current wave.
                if (temp.rollRemainderIntoNextWave)
                {
                    foreach (EnemyPool apool in temp.enemyPools)
                    {
                        if (apool.quantity > 0)
                        {
                            currentWave.enemyPools.Add(apool);
                        }
                    }
                }
            }
        }
    }

    //Determine which enemy from the pools of enemies in the current wave should be spawned. Then spawn it.
    private void spawnEnemyFromPool()
    {
        int poolSize = 0;
        foreach(EnemyPool apool in currentWave.enemyPools)
        {
            poolSize += apool.quantity;
        }

        int i = Random.Range(1, poolSize);
        Debug.Log("Spawning Enemy from pool with original i = " + i);

        foreach(EnemyPool apool in currentWave.enemyPools)
        {
            //if i falls inside this pool, spawn enemy from that pool, reduce size of pool and break loop.
            if(i <= apool.quantity)
            {
                Debug.Log("Spawning Enemy from pool with i = " + i + " where pool quanity = " + apool.quantity);
                spawnEnemy(apool.enemyPrefab, apool.spawnOnLeftSide);
                apool.quantity--;
                
                break;
            }
            else //if i falls outside this pool, subtract the size of this pool from i. then move onto next pool
            {
                Debug.Log("I exceeds pool quanity of: " + apool.quantity + ", new i = " + apool.quantity);
                i -= apool.quantity;
            }
        }
    }

    //Spawn a new enemy at a randomly determined grid endspace on the far right of the field. (Or far left if spawnOnLeft == true)
    private void spawnEnemy(GameObject enemy, bool spawnOnLeft = false)
    {
        GameObject spawnPoint;
        if (!spawnOnLeft)
        {
            spawnPoint = Space_Script.findGridEndSpace(4, Random.Range(0, 5));
        }
        else
        {
            spawnPoint = Space_Script.findGridEndSpace(-1, Random.Range(0, 5));
        }
        GameObject anEnemy = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        anEnemy.GetComponent<Enemy_AI_script>().nextSpace = spawnPoint;
        Debug.Log("Enemy Spawned: " + enemy.name);
    }

    //Set the current horde to a horde with the matching name. Then starts the horde. Displays a warning in console if a matching horde wasn't found.
    public void setHorde(string hordeName)
    {
        bool hordeFound = false;
        foreach(Horde ahorde in hordes)
        {
            if(string.Equals(ahorde.name, hordeName))
            {
                hordeFound = true;
                currentHorde = new Horde(ahorde); //pass ahorde by values
                currentWaveCount = 0;
                currentWave = currentHorde.waves[0]; //set current wave to the first wave in the horde
                hordeIsSpawning = true; //start the horde spawning
                Debug.Log("Starting horde " + hordeName + ".");
                break;
            }
        }

        if(!hordeFound)
        {
            Debug.LogWarning("Horde with name " + hordeName + ", not found");
        }
    }

    [System.Serializable]
    public class Horde
    {
        public string name;
        public List<Wave> waves;
        public List<string> tags;

        public Horde(Horde hordeValues)
        {
            this.name = hordeValues.name;
            this.waves = new List<Wave>();
            foreach(Wave aWaveValues in hordeValues.waves)
            {
                this.waves.Add(new Wave(aWaveValues));
            }
            this.tags = hordeValues.tags;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public float spawnCooldown;
        public int numberOfEnemiesInWave;
        public bool rollRemainderIntoNextWave;
        public List<EnemyPool> enemyPools;

        public Wave(Wave waveValues)
        {
            this.spawnCooldown = waveValues.spawnCooldown;
            this.numberOfEnemiesInWave = waveValues.numberOfEnemiesInWave;
            this.rollRemainderIntoNextWave = waveValues.rollRemainderIntoNextWave;
            this.enemyPools = new List<EnemyPool>();
            foreach(EnemyPool aPoolValues in waveValues.enemyPools)
            {
                this.enemyPools.Add(new EnemyPool(aPoolValues));
            }
        }
    }

    [System.Serializable]
    public class EnemyPool
    {
        public int quantity;
        public GameObject enemyPrefab;
        public bool spawnOnLeftSide;

        public EnemyPool(EnemyPool poolValues)
        {
            this.quantity = poolValues.quantity;
            this.enemyPrefab = poolValues.enemyPrefab;
            this.spawnOnLeftSide = poolValues.spawnOnLeftSide;
        }
    }
}
