using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat_Tracking_Script : MonoBehaviour
{
    public int battlesWonStat;
    public int encountersClearedStat;
    public static Stat_Tracking_Script instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int getBattlesWonStat()
    {
        return instance.battlesWonStat;
    }

    public static void addBattlesWonStat()
    {
        instance.battlesWonStat++;
    }

    public static void setBattlesWonStat(int inBattlesWon)
    {
        instance.battlesWonStat = inBattlesWon;
    }

    public static int getEncountersClearedStat()
    {
        return instance.encountersClearedStat;
    }

    public static void addEncountersClearedStat()
    {
        instance.encountersClearedStat++;
    }

    public static void setEncountersClearedStat(int inEncountersCleared)
    {
        instance.encountersClearedStat = inEncountersCleared;
    }
}
