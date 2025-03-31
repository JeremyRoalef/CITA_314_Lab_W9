using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteAlways]
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
    List<GeneratedColumn> generatedColumn;

    [SerializeField]
    float cubeSpacing;

    [SerializeField]
    bool buildWall;

    [SerializeField]
    bool deleteWall;

    [SerializeField]
    bool destroyWall;

    Vector3 cubeSize;
    Vector3 spawnPos;
    GameObject[] wallCubes;
    XRSocketInteractor wallSocket;

    void Start()
    {

    }

    void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        spawnPos = transform.position;
        int socketedColumn = Random.Range(0, columns);

        for (int i = 0; i < columns; i++)
        {
            if (i == socketedColumn)
            {
                GenerateColumn(rows, true);
            }
            else
            {
                GenerateColumn(rows, false);
            }
            
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

    void AddSocketWall(GeneratedColumn socketedColumn)
    {
        if (wallCubes[socketWallPosition] != null)
        {
            Vector3 pos = wallCubes[socketWallPosition].transform.position;
            DestroyImmediate(wallCubes[socketWallPosition]);

            wallCubes[socketWallPosition] = Instantiate(socketWallCubePrefab, pos, transform.rotation);
            socketedColumn.SetCube(wallCubes[socketWallPosition]);

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

    void GenerateColumn(int height, bool socketable)
    {
        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializeColumn(transform, height, socketable);

        spawnPos.y = transform.position.y;
        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPos, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            
            spawnPos.y += cubeSize.y + cubeSpacing;
        }

        if (socketable && socketWallCubePrefab != null)
        {
            if (socketWallPosition < 0 || socketWallPosition >= height)
            {
                socketWallPosition = 0;
            }

            AddSocketWall(tempColumn);
        }

        generatedColumn.Add(tempColumn);
    }

    private void SetObjectToKinematic(GameObject obj, bool enableKinematic)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = enableKinematic;
    }

    private void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }

        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < generatedColumn.Count; i++)
             {
                generatedColumn[i].DeleteColumn();
             }
            
            if (generatedColumn.Count >= 1)
            {
                generatedColumn.Clear();
            }
        }
    }
}

[System.Serializable]
public class GeneratedColumn
{
    [SerializeField]
    GameObject[] wallCubes;

    [SerializeField]
    bool isSocketed;

    Transform parentObj;

    const string COLUMN_NAME = "column";
    bool isParented;
    Transform columnObj;

    public void InitializeColumn(Transform parent, int rows, bool socketed)
    {
        parentObj = parent;
        wallCubes = new GameObject[rows];
        isSocketed = socketed;
    }

    public void SetCube(GameObject cube)
    {

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (!isParented)
            {
                isParented = true;
                cube.name = COLUMN_NAME;
                cube.transform.SetParent(parentObj);
                columnObj = cube.transform;
            }
            else
            {
                cube.transform.SetParent(columnObj);
            }

            if (wallCubes[i] == null)
            {
                wallCubes[i] = cube;
                break;
            }
        }
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; ++i)
        {
            if (wallCubes[i] != null)
            {
                Object.DestroyImmediate(wallCubes[i]);
            }
        }

        wallCubes = new GameObject[0];
    }
}
