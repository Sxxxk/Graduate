using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Move : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 5f * Time.deltaTime, Space.Self);
    }

}
