using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public bool simulationActive = false;

    public List<GameObject> carPrefabs;
    public Vector2 carCooldownMinMax;
    public float carPositionRandomness = 1.5f;
    public float carRotationRandomness = 2f;
    public float carLeftOffset = 0f;
    public float carRightOffset = 0f;
    public Transform carspawnPointRightLane;
    public Transform carSpawnPointLeftLane;

    public List<GameObject> pedestrianPrefabs;
    public Vector2 pedestrianCooldownMinMax;
    public List<Transform> pedestrianSpawnpointsLeftsidewalk;
    public List<Transform> pedestrianSpawnpointsRightsidewalk;
    public float pedestrianLeftsidewalkOffset = 0f;
    public float pedestrianRightsidewalkOffset = 0f;
    public float pedestrianPositionRandomness = 0.15f;
    public float pedestrianRotationRandomness = 2f;

    public List<GameObject> cyclistPrefabs;
    public Vector2 cyclistCooldownMinMax;
    public List<Transform> cyclistSpawnpointsLeftbikelane;
    public List<Transform> cyclistSpawnpointsRightbikelane;
    public float cyclistLeftbikelaneOffset = 0f;
    public float cyclistRightbikelaneOffset = 0f;
    public float cyclistPositionRandomness = 0.15f;
    public float cyclistRotationRandomness = 2f;

    public List<GameObject> jaywalkerPrefabs;
    public Vector2 jaywalkCooldownMinMax;
    public List<Transform> jaywalkSpawnpoints;
    public float jaywalkerRotationRandomness = 5f;

    public List<GameObject> cyclistOnSidewalkPrefabs;
    public Vector2 cyclistOnSidewalkCooldownMinMax;

    public GameObject baseModel;
    public GameObject shadowReceiver;
    public bool transparent = false;


    List<GameObject> spawnedObjects;
    float timeUntilCarSpawn = 0f;
    float timeUntilJaywalkerSpawn = 0f;
    float timeUntilPedestrianSpawn = 0f;
    float timeUntilCyclistSpawn = 0f;
    float timeUntilCyclistOnSidewalkSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new List<GameObject>();
        TryLoadSettingsFromMenu();
        //ResetGhostDrivingCooldown();
        //ResetJaywalkingCooldown();
        //ResetPedestrianCooldown();
        //ResetCyclistCooldown();
        //ResetCyclistOnSidewalkCooldown();
    }

    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null) { return; }
        transform.position = settings.roadPosition;
        jaywalkCooldownMinMax *= (100 / settings.jaywalkFrequency);
        cyclistOnSidewalkCooldownMinMax *= (100 / settings.cyclistOnSidewalkFrequency);
        carCooldownMinMax *= (100 / settings.carDensity);
        pedestrianCooldownMinMax *= (100 / settings.pedestrianFrequency);
        cyclistCooldownMinMax *= (100 / settings.bikeFrequency);
        carLeftOffset = settings.caroffsetleft;
        carRightOffset = settings.caroffsetright;
        pedestrianLeftsidewalkOffset = settings.pedestrianoffsetleft;
        pedestrianRightsidewalkOffset = settings.pedestrianoffsetright;
        cyclistLeftbikelaneOffset = settings.bikeoffsetleft;
        cyclistRightbikelaneOffset = settings.bikeoffsetright;
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulationActive)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[0] != null)
                {
                    Destroy(spawnedObjects[0]);
                }

                spawnedObjects.RemoveAt(0);
            }
            return;
        }

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
        timeUntilCyclistSpawn -= Time.deltaTime;
        timeUntilCyclistOnSidewalkSpawn -= Time.deltaTime;
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
        if (timeUntilCyclistSpawn <= 0)
        {
            SpawnCyclist();
            ResetCyclistCooldown();
        }
        if (timeUntilCyclistOnSidewalkSpawn <= 0)
        {
            SpawnCyclistOnSidewalk();
            ResetCyclistOnSidewalkCooldown();
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

    void ResetCyclistCooldown()
    {
        timeUntilCyclistSpawn = Random.Range(cyclistCooldownMinMax.x, cyclistCooldownMinMax.y);
    }

    void ResetCyclistOnSidewalkCooldown()
    {
        timeUntilCyclistOnSidewalkSpawn = Random.Range(cyclistOnSidewalkCooldownMinMax.x, cyclistOnSidewalkCooldownMinMax.y);
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
        spawnedObjects.Add(jaywalker);
    }

    void SpawnPedestrian(GameObject pedestrianPrefab = null)
    {
        if (pedestrianPrefab == null)
        {
            pedestrianPrefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Count)];
        }

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
        spawnedObjects.Add(pedestrian);
    }

    void SpawnCyclistOnSidewalk()
    {
        SpawnPedestrian(cyclistOnSidewalkPrefabs[Random.Range(0, cyclistOnSidewalkPrefabs.Count)]);
    }

    void SpawnCyclist()
    {
        GameObject cyclistPrefab = cyclistPrefabs[Random.Range(0, cyclistPrefabs.Count)];
        Transform cyclistSpawnpoint;
        Vector3 cyclistPos;

        bool rightBikelane = false;
        if (Random.Range(0, 1f) > 0.5f) { rightBikelane = true; }
        if (rightBikelane)
        {
            cyclistSpawnpoint = cyclistSpawnpointsRightbikelane[Random.Range(0, cyclistSpawnpointsRightbikelane.Count)];
            cyclistPos = cyclistSpawnpoint.position;
            cyclistPos.x += cyclistRightbikelaneOffset;
        }
        else
        {
            cyclistSpawnpoint = cyclistSpawnpointsLeftbikelane[Random.Range(0, cyclistSpawnpointsLeftbikelane.Count)];
            cyclistPos = cyclistSpawnpoint.position;
            cyclistPos.x += cyclistLeftbikelaneOffset;
        }

        cyclistPos.x += Random.Range(-cyclistPositionRandomness, cyclistPositionRandomness);

        GameObject cyclist = Instantiate(cyclistPrefab, cyclistPos, Quaternion.identity);
        cyclist.transform.forward = cyclistSpawnpoint.forward;
        var rotation = cyclist.transform.localEulerAngles;
        rotation.y += Random.Range(-cyclistRotationRandomness, cyclistRotationRandomness);
        cyclist.transform.localEulerAngles = rotation;
        spawnedObjects.Add(cyclist);
    }

    void SpawnCar()
    {
        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        Vector3 carPos;
        Vector3 carForward;
        List<Transform> spawnPoints = new List<Transform>();
        spawnPoints.Add(carSpawnPointLeftLane);
        spawnPoints.Add(carspawnPointRightLane);

        if (Random.Range(0, 1f) < 0.5f)
        {
            carPos = spawnPoints[0].position;
            carPos.x += carLeftOffset;
            carForward = spawnPoints[0].forward;
        }
        else
        {
            carPos = spawnPoints[1].position;
            carPos.x += carRightOffset;
            carForward = spawnPoints[1].forward;
        }

        carPos.x += Random.Range(-carPositionRandomness, carPositionRandomness);
        GameObject car = Instantiate(carPrefab, carPos, Quaternion.identity);
        car.transform.forward = carForward;
        var rotation = car.transform.localEulerAngles;
        rotation.y += Random.Range(-carRotationRandomness, carRotationRandomness);
        car.transform.localEulerAngles = rotation;
        spawnedObjects.Add(car);
    }
}
