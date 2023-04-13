using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<GameObject> carPrefabs;
    public Vector2 carCooldownMinMax;
    public float chanceOfGhostDriver = 0.05f;
    public float carPositionRandomness = 1.5f;
    public float carRotationRandomness = 2f;
    public Transform carspawnPointRightLane;
    public Transform carSpawnPointLeftLane;
    public Transform ghostdrivingRightLane;
    public Transform ghostdrivingLeftLane;

    public List<GameObject> pedestrianPrefabs;
    public Vector2 pedestrianCooldownMinMax;
    public List<Transform> pedestrianSpawnpointsLeftsidewalk;
    public List<Transform> pedestrianSpawnpointsRightsidewalk;
    public float pedestrianLeftsidewalkOffset = 0f;
    public float pedestrianRightsidewalkOffset = 0f;
    public float pedestrianPositionRandomness = 0.15f;
    public float pedestrianRotationRandomness = 2f;

    public List<GameObject> jaywalkerPrefabs;
    public Vector2 jaywalkCooldownMinMax;
    public List<Transform> jaywalkSpawnpoints;
    public float jaywalkerRotationRandomness = 5f;

    public GameObject baseModel;
    public GameObject shadowReceiver;
    public bool transparent = false;


    float timeUntilCarSpawn = 0f;
    float timeUntilJaywalkerSpawn = 0f;
    float timeUntilPedestrianSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //ResetGhostDrivingCooldown();
        //ResetJaywalkingCooldown();
        //ResetPedestrianCooldown();
    }

    // Update is called once per frame
    void Update()
    {
        if (transparent)
        {
            shadowReceiver.SetActive(true);
            baseModel.SetActive(false);
        }
        else
        {
            shadowReceiver.SetActive(false);
            baseModel.SetActive(true);
        }

        timeUntilCarSpawn -= Time.deltaTime;
        timeUntilJaywalkerSpawn -= Time.deltaTime;
        timeUntilPedestrianSpawn -= Time.deltaTime;
        if (timeUntilCarSpawn <= 0)
        {
            SpawnCar();
            ResetGhostDrivingCooldown();
        }
        if (timeUntilJaywalkerSpawn <= 0)
        {
            SpawnJaywalker();
            ResetJaywalkingCooldown();
        }
        if (timeUntilPedestrianSpawn <= 0)
        {
            SpawnPedestrian();
            ResetPedestrianCooldown();
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

    void ResetPedestrianCooldown()
    {
        timeUntilPedestrianSpawn = Random.Range(pedestrianCooldownMinMax.x, pedestrianCooldownMinMax.y);
    }

    void SpawnJaywalker()
    {
        GameObject jaywalkPrefab = jaywalkerPrefabs[Random.Range(0, jaywalkerPrefabs.Count)];
        Transform jaywalkSpawnpoint = jaywalkSpawnpoints[Random.Range(0, jaywalkSpawnpoints.Count)];

        GameObject jaywalker = Instantiate(jaywalkPrefab, jaywalkSpawnpoint.position, Quaternion.identity);
        jaywalker.transform.forward = jaywalkSpawnpoint.forward;
        var rotation = jaywalker.transform.localEulerAngles;
        rotation.y += Random.Range(-jaywalkerRotationRandomness, jaywalkerRotationRandomness);
        jaywalker.transform.localEulerAngles = rotation;
    }

    void SpawnPedestrian()
    {
        GameObject pedestrianPrefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Count)];
        Transform pedestrianSpawnpoint;
        Vector3 pedestrianPos;

        bool rightSidewalk = false;
        if (Random.Range(0, 1f) > 0.5f) { rightSidewalk = true; }
        if (rightSidewalk)
        {
            pedestrianSpawnpoint = pedestrianSpawnpointsRightsidewalk[Random.Range(0, pedestrianSpawnpointsRightsidewalk.Count)];
            pedestrianPos = pedestrianSpawnpoint.position;
            pedestrianPos.x += pedestrianRightsidewalkOffset;
        }
        else
        {
            pedestrianSpawnpoint = pedestrianSpawnpointsLeftsidewalk[Random.Range(0, pedestrianSpawnpointsLeftsidewalk.Count)];
            pedestrianPos = pedestrianSpawnpoint.position;
            pedestrianPos.x += pedestrianLeftsidewalkOffset;
        }

        pedestrianPos.x += Random.Range(-pedestrianPositionRandomness, pedestrianPositionRandomness); // Add a bit of randomness to get better data.

        GameObject pedestrian = Instantiate(pedestrianPrefab, pedestrianPos, Quaternion.identity);
        pedestrian.transform.forward = pedestrianSpawnpoint.forward;
        var rotation = pedestrian.transform.localEulerAngles;
        rotation.y += Random.Range(-pedestrianRotationRandomness, pedestrianRotationRandomness);
        pedestrian.transform.localEulerAngles = rotation;
    }

    void SpawnCar()
    {
        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        Vector3 carPos;
        Vector3 carForward;
        List<Transform> spawnPoints = new List<Transform>();
        if (Random.Range(0, 1f) < chanceOfGhostDriver)
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
        if (Random.Range(0, 1f) < 0.5f)
        {
            carPos = spawnPoints[0].position;
            carForward = spawnPoints[0].forward;
        }
        else
        {
            carPos = spawnPoints[1].position;
            carForward = spawnPoints[1].forward;
        }

        carPos.x += Random.Range(-carPositionRandomness, carPositionRandomness);
        GameObject car = Instantiate(carPrefab, carPos, Quaternion.identity);
        car.transform.forward = carForward;
        var rotation = car.transform.localEulerAngles;
        rotation.y += Random.Range(-carRotationRandomness, carRotationRandomness);
        car.transform.localEulerAngles = rotation;
    }
}
