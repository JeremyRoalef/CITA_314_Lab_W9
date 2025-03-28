using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TheWall : MonoBehaviour
{
    [SerializeField]
    XRSocketInteractor wallSocket;

    [SerializeField]
    GameObject[] wallCubes;

    // Start is called before the first frame update
    void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(WallSocket_OnSelectEntered);
            wallSocket.selectExited.AddListener(WallSocket_OnSelectExited);
        }
    }


    private void WallSocket_OnSelectEntered(SelectEnterEventArgs arg0)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                SetObjectToKinematic(wallCubes[i], false);
            }
        }
    }

    private void WallSocket_OnSelectExited(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                SetObjectToKinematic(wallCubes[i], true);
            }
        }
    }

    private void SetObjectToKinematic(GameObject obj, bool enableKinematic)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = enableKinematic;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
