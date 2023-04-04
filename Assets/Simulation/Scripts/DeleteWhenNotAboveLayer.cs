using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWhenNotAboveLayer : MonoBehaviour
{
    public LayerMask layers;

    // Update is called once per frame
    void Update()
    {
        if(!Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.up * -1, out RaycastHit hit, 2f, layers))
        {
            Destroy(gameObject);
        }
    }
}
