using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For test uses only!!! 
public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("Cube");
        Debug.Log(transform.InverseTransformDirection(gameObject.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
