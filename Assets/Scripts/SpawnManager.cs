using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static SpawnManager instance;
    [SerializeField] GameObject[] Enemies;
    [SerializeField] GameObject player;
    [SerializeField] Eliot.Environment.WayPointsGroup waypoint;
    [SerializeField] GameObject[] Bosses;
    [SerializeField] GameObject[] spawnpionts;
    int index = 0;
    int wave=1;
    public int EnemiesAlive = 0;
    float spwantime=61;
    float spwanedattime = 0;

    public static SpawnManager Instance
    {
        get
        {
            if (SpawnManager.instance == null)
            {
                SpawnManager.instance = FindObjectOfType<SpawnManager>();
            }
            return SpawnManager.instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        wave = 1;
        index = 0;
        EnemiesAlive = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemiesAlive < 5)
        {
            if (wave == 0 || EnemiesAlive == 0)
            {
                spwanedattime = Time.time + spwantime;
                Enemies[0].GetComponent<Enemy_Combat>().Player = player;
                Enemies[0].GetComponent<Eliot.AgentComponents.EliotAgent>().WayPoints = waypoint;
                Instantiate(Enemies[0], spawnpionts[0].transform);
                EnemiesAlive++;
                Enemies[0].GetComponent<Enemy_Combat>().Player = player;
                Enemies[0].GetComponent<Eliot.AgentComponents.EliotAgent>().WayPoints = waypoint;
                Instantiate(Enemies[0], spawnpionts[1].transform);
                EnemiesAlive++;
                wave = 1;

            }
            else if (spwanedattime < Time.time)
            {
                spwanedattime = Time.time + spwantime;
                int randenememny = Random.Range(0, 2);
                int randenspwanpoint = Random.Range(0, 8);
                if (randenememny != 1)
                {
                    Enemies[randenememny].GetComponent<Enemy_Combat>().Player = player;
                }
                Enemies[randenememny].GetComponent<Eliot.AgentComponents.EliotAgent>().WayPoints = waypoint;
                Instantiate(Enemies[randenememny], spawnpionts[randenspwanpoint].transform);

            }
        }
    }
}
