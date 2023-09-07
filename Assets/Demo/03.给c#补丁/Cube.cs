using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix()]
public class Cube : MonoBehaviour
{
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void LogDebug()
    {
        Debug.Log("c#原生");
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            LogDebug();
            _rigidbody.AddForce(Vector3.up * 500);
        }
    }
}