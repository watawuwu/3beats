using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutCube : MonoBehaviour
{
    public GameObject effectPrefab;
    public GameObject note;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rod")
        {
            gameObject.transform.root.gameObject.SetActive(false); 
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2.0f);
        }
    }
}
