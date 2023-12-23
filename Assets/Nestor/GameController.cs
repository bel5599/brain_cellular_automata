using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Vars for celulas
    public GameObject StandardCelulaPrefb;
    public GameObject BadCelulaPrefb;

    public int matrixDimension = 10;
    public float PositionScale = 1;
    public Vector3 InitialObjectsPosition = new Vector3(0, 1, 0);

    List<Celula> celulas = new List<Celula>();

    //Vars for vasos
    public GameObject EmptyObjPrefb;
    public GameObject VasoPrefb;
    

    // Start is called before the first frame update
    void Start()
    {
        //Set celulas in the scene
        int[,,] matrix = new int[matrixDimension, matrixDimension, matrixDimension];

        for (int x = 0; x < matrixDimension; x++)
        {
            for (int y = 0; y < matrixDimension; y++)
            {
                for (int z = 0; z < matrixDimension; z++)
                {
                    var celula = new Celula { Model = Instantiate(StandardCelulaPrefb), StartPosition = new Vector3(x, y, z) };
                    celula.Model.transform.position = PositionScale * celula.StartPosition;

                    celulas.Add(celula);
                }
            }
        }

        //Set some vasos
        var p1 = Instantiate(StandardCelulaPrefb);
        p1.transform.position = new Vector3(0, 1, 1);
        var p2 = Instantiate(StandardCelulaPrefb);
        p2.transform.position = new Vector3(0, 1, 7);
        var vaso1 = Instantiate(VasoPrefb);
        var script = vaso1.GetComponent<CylinderController>();
        script.point1 = p1;
        script.point2 = p2;

    }

    // Update is called once per frame
    void Update()
    {
        //This is to be able to dynamically move the whole simulation
        foreach (var c in celulas)
        {
            c.Model.transform.position = PositionScale * c.StartPosition;

            c.Model.transform.Translate(InitialObjectsPosition);
        }
    }
}

public class Celula
{
    public GameObject Model;
    public Vector3 StartPosition;
}

public class Vaso
{
    public GameObject Model;
    public GameObject point1;
    public GameObject point2;
}
