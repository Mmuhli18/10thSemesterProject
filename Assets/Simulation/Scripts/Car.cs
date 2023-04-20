using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Road road;
    public Vector2 velocityMinMax = new Vector2() { x = 4, y = 7 };
    public LayerMask carLayerMask;

    float velocity;
    // Start is called before the first frame update
    void Start()
    {
        velocity = Random.Range(velocityMinMax.x, velocityMinMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (road.paused) { return; }
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, carLayerMask))
        {
            //Debug.Log("Hit car with distance of: " + hit.distance);
            if(hit.distance < 7f)
            {
                velocity = map(hit.distance, 2f, 7f, velocityMinMax.x, velocityMinMax.y);
                //Debug.Log("Set velocity to: " + velocity);
            }
        }
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}