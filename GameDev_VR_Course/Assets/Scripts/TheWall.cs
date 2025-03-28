using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TheWall : MonoBehaviour
{
    [SerializeField]
    GameObject wallCubePrefab;

    [SerializeField]
    GameObject socketWallCubePrefab;

    [SerializeField]
    XRSocketInteractor wallSocket;

    [SerializeField]
    GameObject[] wallCubes;

    [SerializeField]
    float cubeSpacing;

    Vector3 cubeSize;
    Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        spawnPos = transform.position;
        BuildWall();
    }

    void BuildWall()
    {
        wallCubes = new GameObject[2];
        if (wallCubePrefab != null)
        {
            wallCubes[0] = Instantiate(wallCubePrefab, spawnPos, transform.rotation);
        }

        spawnPos.y += cubeSize.y + cubeSpacing;

        if (socketWallCubePrefab != null)
        {

            wallCubes[1] = Instantiate(socketWallCubePrefab, spawnPos, transform.rotation);

            wallSocket = wallCubes[1].GetComponentInChildren<XRSocketInteractor>();

            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(WallSocket_OnSelectEntered);
                wallSocket.selectExited.AddListener(WallSocket_OnSelectExited);
            }
        }

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                wallCubes[i].transform.SetParent(transform);
            }
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
