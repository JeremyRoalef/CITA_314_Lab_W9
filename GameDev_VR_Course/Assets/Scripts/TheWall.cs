using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TheWall : MonoBehaviour
{
    [SerializeField]
    int columns;

    [SerializeField]
    int rows;

    [SerializeField]
    GameObject wallCubePrefab;

    [SerializeField]
    GameObject socketWallCubePrefab;

    [SerializeField]
    int socketWallPosition = 1;

    [SerializeField]
    XRSocketInteractor wallSocket;

    [SerializeField]
    GameObject[] wallCubes;

    [SerializeField]
    float cubeSpacing;

    Vector3 cubeSize;
    Vector3 spawnPos;



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
        for (int i = 0; i < columns; i++)
        {
            GenerateColumn(rows, true);
            spawnPos.x += cubeSize.x + cubeSpacing;
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

    void GenerateColumn(int height, bool socketable)
    {
        spawnPos.y = transform.position.y;
        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPos, transform.rotation);
                if (i== 0)
                {
                    wallCubes[i].name = "column";
                    wallCubes[i].transform.SetParent(transform);
                }
                else
                {
                    wallCubes[i].transform.SetParent(wallCubes[0].transform);
                }
            }
            
            spawnPos.y += cubeSize.y + cubeSpacing;
        }

        if (socketable && socketWallCubePrefab != null)
        {
            if (socketWallPosition < 0 || socketWallPosition >= height)
            {
                socketWallPosition = 0;
            }

            if (wallCubes[socketWallPosition] != null)
            {
                Vector3 pos = wallCubes[socketWallPosition].transform.position;
                DestroyImmediate(wallCubes[socketWallPosition]);

                wallCubes[socketWallPosition] = Instantiate(socketWallCubePrefab, pos, transform.rotation);
                if (socketWallPosition == 0)
                {
                    wallCubes[socketWallPosition].transform.SetParent(transform);
                }
                else
                {
                    wallCubes[socketWallPosition].transform.SetParent(wallCubes[0].transform);
                }
                
                wallSocket = wallCubes[socketWallPosition].GetComponentInChildren<XRSocketInteractor>();

                if (wallSocket != null)
                {
                    wallSocket.selectEntered.AddListener(WallSocket_OnSelectEntered);
                    wallSocket.selectExited.AddListener(WallSocket_OnSelectExited);
                }
            }
        }
    }

    private void SetObjectToKinematic(GameObject obj, bool enableKinematic)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = enableKinematic;
    }
}
