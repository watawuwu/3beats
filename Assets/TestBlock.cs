using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlock : MonoBehaviour
{
    public GameObject effectPrefab;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Rod")
        {
            Destroy(gameObject);
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2.0f);
        }
    }
}
