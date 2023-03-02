using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<GameObject> carPrefabs;
    public Vector2 carCooldownMinMax;
    public float chanceOfGhostDriver = 0.05f;
    public Transform carspawnPointRightLane;
    public Transform carSpawnPointLeftLane;
    public Transform ghostdrivingRightLane;
    public Transform ghostdrivingLeftLane;

    public List<GameObject> jaywalkerPrefabs;
    public Vector2 jaywalkCooldownMinMax;
    public List<Transform> jaywalkSpawnpoints;

    float timeUntilCarSpawn = 0f;
    float timeUntilJaywalkerSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ResetGhostDrivingCooldown();
        ResetJaywalkingCooldown();
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilCarSpawn -= Time.deltaTime;
        timeUntilJaywalkerSpawn -= Time.deltaTime;
        if(timeUntilCarSpawn <= 0)
        {
            SpawnCar();
            ResetGhostDrivingCooldown();
        }
        if(timeUntilJaywalkerSpawn <= 0)
        {
            SpawnJaywalker();
            ResetJaywalkingCooldown();
        }
    }

    void ResetGhostDrivingCooldown()
    {
        timeUntilCarSpawn = Random.Range(carCooldownMinMax.x, carCooldownMinMax.y);
    }

    void ResetJaywalkingCooldown()
    {
        timeUntilJaywalkerSpawn = Random.Range(jaywalkCooldownMinMax.x, jaywalkCooldownMinMax.y);
    }

    void SpawnJaywalker()
    {
        GameObject jaywalkPrefab = jaywalkerPrefabs[Random.Range(0, jaywalkerPrefabs.Count)];
        Transform jaywalkSpawnpoint = jaywalkSpawnpoints[Random.Range(0, jaywalkSpawnpoints.Count)];

        GameObject jaywalker = Instantiate(jaywalkPrefab, jaywalkSpawnpoint.position, Quaternion.identity);
        jaywalker.transform.forward = jaywalkSpawnpoint.forward;
    }

    void SpawnCar()
    {
        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        Vector3 carPos;
        Vector3 carForward;
        List<Transform> spawnPoints = new List<Transform>();
        if(Random.Range(0, 1f) < chanceOfGhostDriver)
        {
            spawnPoints.Add(ghostdrivingLeftLane);
            spawnPoints.Add(ghostdrivingRightLane);
            Debug.Log("Spawned ghost driver");
        }
        else
        {
            spawnPoints.Add(carSpawnPointLeftLane);
            spawnPoints.Add(carspawnPointRightLane);
        }
        if(Random.Range(0, 1f) < 0.5f)
        {
            carPos = spawnPoints[0].position;
            carForward = spawnPoints[0].forward;
        }
        else
        {
            carPos = spawnPoints[1].position;
            carForward = spawnPoints[1].forward;
        }
        GameObject car = Instantiate(carPrefab, carPos, Quaternion.identity);
        car.transform.forward = carForward;
    }
}