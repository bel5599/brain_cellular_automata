using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Vars for celulas
    public GameObject StandardCelulaPrefb;
    public GameObject BadCelulaPrefb;
    public GameObject CelulasContainer;
    Entity[,,] matrix;

    public int matrixDimension = 10;
    public float PositionScale = 1;
    //To translade 1 or more unitys in Y exe.
    public Vector3 InitialObjectsPosition = new Vector3(0, 1, 0);

    List<Celula> celulas = new List<Celula>();

    //Vars for vasos
    public GameObject VasoPrefb;
    public int MaxChildCount = 3;
    public int MaxStepSize = 2;
    public int TreeCount = 4;

    //Auxiliars
    static System.Random random = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        //Set celulas in the scene
        matrix = new Entity[matrixDimension, matrixDimension, matrixDimension];

        CreateCelulas();

        for (int i = 0; i < TreeCount; i++)
        {
            CreateVasosSanguineos();
        }

    }

    private void CreateVasosSanguineos()
    {
        if (matrixDimension <= 2) return;

        //Get the base point for the tree
        var basePoint = new Vector3(random.GetIntValue(2, matrixDimension-1), 0, random.GetIntValue(2, matrixDimension-1));
        var p1 = matrix[(int)basePoint.x, (int)basePoint.y, (int)basePoint.z];
        p1.Checked = true;

        //Creating the tail
        var size = random.GetIntValue(1, 2);
        var validPositions = GetValidPositions(basePoint, size);

        int selectedPos = 0;
        Entity p2;
        if (validPositions.Count > 0)
        {
            selectedPos = random.GetIntValue(0, validPositions.Count-1);
            p2 = validPositions[selectedPos];
            AddVasoSanguineo(p1, p2);
            p2.Checked = true;
            ExpandTree(p2, MaxChildCount, MaxStepSize);
        }
    }

    private void ExpandTree(Entity basePoint, int MaxChildsCount, int maxStepSize)
    {
        if (basePoint.x == matrixDimension-1 || basePoint.y == matrixDimension-1 || basePoint.z == matrixDimension-1) return;

        var validPositions = GetValidPositions(basePoint.Model.transform.position, random.GetIntValue(1, maxStepSize));
        var childsCount = random.GetIntValue(0, MaxChildsCount);

        int selectedPos = 0;
        Entity p2;

       
        if (validPositions.Count > 0)
        {
            for (int i = 0; i < childsCount && validPositions.Count > 0; i++)
            {
                selectedPos = random.GetIntValue(0, validPositions.Count-1);
                p2 = validPositions[selectedPos];
                validPositions.RemoveAt(selectedPos);
                AddVasoSanguineo(basePoint, p2);
                p2.Checked = true;
                ExpandTree(p2, MaxChildsCount, maxStepSize);
            }
        }
    }

    private void AddVasoSanguineo(Entity p1, Entity p2)
    {
        var vaso = Instantiate(VasoPrefb);
        var script = vaso.GetComponent<CylinderController>();
        script.point1 = p1.Model;
        script.point2 = p2.Model;

        //Scale to better look
        vaso.transform.localScale = new Vector3(0.3f, vaso.transform.localScale.y, 0.3f);
        p1.Model.transform.localScale *= 0.3f;
        p2.Model.transform.localScale *= 0.3f;

        p1.Model.name = "v";
        p2.Model.name = "v";
    }

    private List<Entity> GetValidPositions(Vector3 basePoint, int size)
    {
        var positions = new List<Entity>();

        int x1, y1, z1;

        if (basePoint.y + size < matrixDimension) {

            //Positive
            for (int x = 0; x <= size && basePoint.x + x < matrixDimension; x++)
                for (int z = 0; z <= size && basePoint.z + z < matrixDimension; z++)
                {
                    x1 = (int)basePoint.x + x;
                    y1 = (int)basePoint.y + size;
                    z1 = (int)basePoint.z + z;

                    if (!matrix[x1, y1, z1].Checked)
                        positions.Add(matrix[x1, y1, z1]);
                }

            //Negative
            for (int x = -1; x >= -size && basePoint.x + x >=0; x--)
                for (int z = 0; z >= -size && basePoint.z + z >=0; z--)
                {
                    x1 = (int)basePoint.x + x;
                    y1 = (int)basePoint.y + size;
                    z1 = (int)basePoint.z + z;

                    if (!matrix[x1, y1, z1].Checked)
                        positions.Add(matrix[x1, y1, z1]);
                }

        }

        return positions;
    }

    private void CreateCelulas()
    {
        for (int x = 0; x < matrixDimension; x++)
        {
            for (int y = 0; y < matrixDimension; y++)
            {
                for (int z = 0; z < matrixDimension; z++)
                {
                    var celula = new Celula { Model = Instantiate(StandardCelulaPrefb), StartPosition = new Vector3(x, y, z), x = x, y = y, z = z };
                    matrix[x, y, z] = celula;
                    celula.Model.transform.position = PositionScale * celula.StartPosition;
                    celula.Model.transform.parent = CelulasContainer.transform;

                    celulas.Add(celula);
                }
            }
        }
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

public class Entity
{
    public GameObject Model;
    public bool Checked;
    public int x, y, z;//logical positions
}
public class Celula:Entity
{
    public Vector3 StartPosition;
}

public class Vaso:Entity
{
    public GameObject point1;
    public GameObject point2;
}

public static class Extensions
{
    static System.Random r = new System.Random();
    public static int GetIntValue(this System.Random r, int min, int max)
    {
        if (min == max) return min;

        double d = Math.Abs(max - min) + 1;
        double interval = 1d / d;
        double myRandom = r.NextDouble();

        var numbers = new List<int>();
        for (int i = min; i <= max; i++) numbers.Add(i);

        double a, b;
        for (int i = 1; i <= d; i++)
        {
            a = (i - 1) * interval;
            b = i * interval;

            if (myRandom.Belongs(a, b)) return numbers[i - 1];
        }

        return 0;
    }

    public static bool Belongs(this double n, double min, double max)
    {
        return n >= min && n <= max;
    }
}
