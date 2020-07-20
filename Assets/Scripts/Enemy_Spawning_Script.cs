using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Enemy_Spawning_Script : MonoBehaviour
{
    public static Enemy_Spawning_Script instance;

    [Header("SpawnRates")]
    private float currentSpawnCooldown;

    [Header("Button")]
    public GameObject startHordeButtonPrefab;
    public GameObject finishBattleButtonPrefab;

    private bool hordeIsSpawning;
    private bool hordeIsReadyToSpawn;
    private bool hordeHasSpawnedAllEnemies;
    private Horde currentHorde;
    private int currentWaveCount;
    private Wave currentWave;

    [Header("Hordes")]
    [SerializeField]
    public List<Horde> hordes;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            currentSpawnCooldown = 0;
            currentWaveCount = 0;
            hordeIsReadyToSpawn = false;
            hordeIsSpawning = false;
            hordeHasSpawnedAllEnemies = false;
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.LogWarning("Saving Hordes to json files.");
            foreach (Horde aHorde in hordes)
            {
                //Debug.Log("Json of Horde = " + currentHorde.toJson());
                aHorde.saveAsJson();
                //Debug.Log("Horde from Json = " + Horde.fromJson(currentHorde.toJson()).waves[0].enemyPools[1].enemyPrefab.name);
            }
        }

        if(Input.GetKeyDown(KeyCode.F10))
        {
            string json = Horde.loadJsonFromFile("Test Horde 1");
            if (json != null)
            {
                Horde horde = Horde.fromJson(json);
                Debug.Log("2nd enemy type from first wave of Horde loaded = " + horde.waves[0].enemyPools[1].enemyPrefab.name);
            }
        }


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
        //if currently in battle scene and horde is loaded and start battle button does not exisit, create one.
        else if (SceneManager.GetActiveScene().name == "Battle Test Rework Scene" 
                && hordeIsReadyToSpawn 
                && GameObject.FindGameObjectWithTag("Start Horde Button") == null) //TODO: Move this logic to a OnLoadScene method
        {
            Debug.LogWarning("Building Start horde button");
            GameObject button = Instantiate(startHordeButtonPrefab, GameObject.Find("UI Canvas").transform);
            button.GetComponent<Button>().onClick.AddListener(() => startHorde());
        }
        //if currently in battle scene and horde has spawned all enemies and there are no enemies left and the finish battle button does not exist, create one.
        else if (SceneManager.GetActiveScene().name == "Battle Test Rework Scene" 
                && hordeHasSpawnedAllEnemies 
                && GameObject.FindGameObjectWithTag("Finish Battle Button") == null
                && GameObject.FindGameObjectsWithTag("Enemy").Length < 1) 
        {
            Debug.LogWarning("Building Finish Battle button");
            GameObject button = Instantiate(finishBattleButtonPrefab, GameObject.Find("UI Canvas").transform);
            button.GetComponent<Button>().onClick.AddListener(() => returnToMap());
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
                hordeIsReadyToSpawn = false;
                hordeHasSpawnedAllEnemies = true;
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
    public static void setHorde(string hordeName)
    {
        bool hordeFound = false;
        foreach(Horde ahorde in instance.hordes)
        {
            if(string.Equals(ahorde.name, hordeName))
            {
                hordeFound = true;
                instance.currentHorde = new Horde(ahorde); //pass ahorde by values
                instance.currentWaveCount = 0;
                instance.currentWave = instance.currentHorde.waves[0]; //set current wave to the first wave in the horde
                instance.hordeIsReadyToSpawn = true; //start the horde spawning
                Debug.Log("Loaded horde " + hordeName + ".");
                break;
            }
        }

        if(!hordeFound)
        {
            Debug.LogWarning("Horde with name " + hordeName + ", not found");
        }
    }

    public static Horde findHorde(string inName)
    {
        foreach(Horde aHorde in instance.hordes)
        {
            if(aHorde.name == inName)
            {
                return aHorde;
            }
        }

        return null;
    }

    public static void startHorde()
    {
        instance.hordeIsSpawning = true;
        Destroy(GameObject.FindGameObjectWithTag("Start Horde Button"));
    }

    public static void returnToMap()
    {
        SceneManager.LoadSceneAsync("Map Scene", LoadSceneMode.Single);
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

        public Horde(HordeJson jHordeValues)
        {
            this.name = jHordeValues.name;
            this.waves = new List<Wave>();
            foreach (WaveJson aWaveValues in jHordeValues.waves)
            {
                this.waves.Add(new Wave(aWaveValues));
            }
            this.tags = jHordeValues.tags;
        }

        public int getTotalSize()
        {
            int i = 0;
            foreach(Wave aWave in this.waves)
            {
                i += aWave.numberOfEnemiesInWave;
            }
            return i;
        }

        public string toJson()
        {
            string raw = JsonUtility.ToJson(new HordeJson(this), true);
            return raw;
        }

        public static Horde fromJson(string jsonIn)
        {
            string j = jsonIn;
            HordeJson jHorde = JsonUtility.FromJson<HordeJson>(j);
            return new Horde(jHorde);           
        }

        public void saveAsJson()
        {
            string json = this.toJson();

            string path = Application.persistentDataPath + "/databases/hordes/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += this.name.Replace(" ", "_") + ".json"; //Replaces any spaces in the horde name with _, to make it compatiable with file storage
            if (!File.Exists(path))
            {
                //File.Create(path);
                Debug.Log("Opening writer to path: " + path);
                StreamWriter writer = File.AppendText(path);
                writer.Write(json);
                writer.Close();
            }
            else
            {
                Debug.Log("Opening writer to path: " + path);
                StreamWriter writer = new StreamWriter(path, false);
                writer.Write(json);
                writer.Close();
            }
        }

        public static string loadJsonFromFile(string fileNameWithoutExtension)
        {
            string path = Application.persistentDataPath + "/databases/hordes/";
            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Error: Directory: " + path + " not found.");
                return null;
            }

            path += fileNameWithoutExtension.Replace(" ", "_") + ".json"; //Replace any spaces in the passed filename with _, as the save system converts all spaces  in the filename to _ when saving the file.

            if (!File.Exists(path))
            {
                Debug.LogWarning("Error: File: " + path + " not found.");
                return null;
            }

            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            Debug.Log("Json retrieved from file: " + json);
            reader.Close();

            return json;
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

        public Wave(WaveJson jWaveValues)
        {
            this.spawnCooldown = jWaveValues.spawnCooldown;
            this.numberOfEnemiesInWave = jWaveValues.numberOfEnemiesInWave;
            this.rollRemainderIntoNextWave = jWaveValues.rollRemainderIntoNextWave;
            this.enemyPools = new List<EnemyPool>();
            foreach (EnemyPoolJson aPoolValues in jWaveValues.enemyPools)
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

        public EnemyPool(EnemyPoolJson jPoolValues)
        {
            this.quantity = jPoolValues.quantity;
            this.enemyPrefab = Resources.Load<GameObject>(jPoolValues.enemyPrefab);
            this.spawnOnLeftSide = jPoolValues.spawnOnLeftSide;
        }
    }

    //JSON-versions of the above 3 classes. Used for storing a horde as Json.
    //KEY DIFFERENCES: EnemyPoolJson.prefab stores the prefab as a string. No other differences, everything else is a straight copy from the non-JSON objects.
    //Used instead of JsonUtility.toJson for Hordes because prefabs are jsonified as instance ids by default. Using these we can store prefabs by their
    //names/paths for easy of human-readablity
    [System.Serializable]
    public class HordeJson
    {
        public string name;
        public List<WaveJson> waves;
        public List<string> tags;

        //Create a HordeJson from a Horde object
        public HordeJson(Horde hordeToJson)
        {
            this.name = hordeToJson.name;
            this.tags = hordeToJson.tags;
            this.waves = new List<WaveJson>();
            foreach(Wave aWaveToJson in hordeToJson.waves)
            {
                this.waves.Add(new WaveJson(aWaveToJson));
            }
        }

    }

    [System.Serializable]
    public class WaveJson
    {
        public float spawnCooldown;
        public int numberOfEnemiesInWave;
        public bool rollRemainderIntoNextWave;
        public List<EnemyPoolJson> enemyPools;

        //Create a WaveJson from a Wave object
        public WaveJson(Wave waveToJson)
        {
            this.spawnCooldown = waveToJson.spawnCooldown;
            this.numberOfEnemiesInWave = waveToJson.numberOfEnemiesInWave;
            this.rollRemainderIntoNextWave = waveToJson.rollRemainderIntoNextWave;
            this.enemyPools = new List<EnemyPoolJson>();
            foreach(EnemyPool poolToJson in waveToJson.enemyPools)
            {
                this.enemyPools.Add(new EnemyPoolJson(poolToJson));
            }
        }
    }

    [System.Serializable]
    public class EnemyPoolJson
    {
        public int quantity;
        public string enemyPrefab; //Json version of enemyPrefab stores name/path to prefab rather than instanceID
        public bool spawnOnLeftSide;

        public EnemyPoolJson(EnemyPool poolToJson)
        {
            this.quantity = poolToJson.quantity;
            this.spawnOnLeftSide = poolToJson.spawnOnLeftSide;
            this.enemyPrefab = "Prefabs/Enemies/" + poolToJson.enemyPrefab.name;
        }
    }
}
