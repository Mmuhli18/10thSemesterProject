using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Road road;
    public Vector2 velocityMinMax = new Vector2() { x = 2.7f, y = 5.4f };

    float velocity;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        velocity = Random.Range(velocityMinMax.x, velocityMinMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (road.paused) 
        { 
            if(animator != null) { animator.enabled = false; }
            return; 
        }
        animator.enabled = true;
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
