using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraSidewalk : MonoBehaviour
{
    public List<Transform> spawnpoints;
    public List<GameObject> prefabs;

    public Vector2 cooldownMinMax;

    float timeUntilSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ResetCooldown();
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilSpawn -= Time.deltaTime;
        if(timeUntilSpawn <= 0)
        {
            ResetCooldown();
            Spawn();
        }
    }

    void ResetCooldown()
    {
        timeUntilSpawn = Random.Range(cooldownMinMax.x, cooldownMinMax.y);
    }

    void Spawn()
    {
        GameObject pedestrianPrefab = prefabs[Random.Range(0, prefabs.Count)];
        Transform pedestrianSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Count)];
        Vector3 pedestrianPos = pedestrianSpawnpoint.position;
        pedestrianPos.x += Random.Range(-0.1f, 0.1f);
        GameObject pedestrian = Instantiate(pedestrianPrefab, pedestrianPos, Quaternion.identity);
        pedestrian.transform.forward = pedestrianSpawnpoint.forward;
        var rotation = pedestrian.transform.localEulerAngles;
        rotation.y += Random.Range(-5, 5);
        pedestrian.transform.localEulerAngles = rotation;
    }
}
