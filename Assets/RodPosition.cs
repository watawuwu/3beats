using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodPosition : MonoBehaviour
{
    public GameObject rod;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rod.transform.localPosition = Vector3.zero;
        rod.transform.localRotation = Quaternion.identity;
        rod.GetComponent<Rigidbody>().velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}
