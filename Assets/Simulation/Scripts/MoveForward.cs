using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Vector2 velocityMinMax = new Vector2() { x = 2.7f, y = 5.4f };

    float velocity;
    // Start is called before the first frame update
    void Start()
    {
        velocity = Random.Range(velocityMinMax.x, velocityMinMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
