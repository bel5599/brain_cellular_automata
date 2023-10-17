using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataCelularLogic;

public class Environment : MonoBehaviour
{
    [SerializeField]
    Transform TumoralCellPrefab, StemCellPrefab, TumoralSpherePrefab, NeuronPrefab, AstrocytePrefab, ArteryPrefab;

    //Transform target;
    public float speed;

    bool is_over_conver = false;
    bool is_over = false;
    int count = 6;

    Dictionary<Cell, Transform> cell_dict;

    Dictionary<Cell, Transform> astrocyte_list;
    Dictionary<Cell, Transform> neuron_list;
    Dictionary<Artery, Transform> artery_list;


    Dictionary<Cell, Transform> tumoral_cell_list;
    Dictionary<Sphere, Transform> sphere_list;
    //List<Transform> cell_list;
    //List<Transform> tumoral_cell_list;
    Transform tumoralCell;

    void Awake()
    {
        //Transform point = Instantiate(pointPrefab);
        //point.localPosition = new Vector3(1, 0, 0);
        //Debug.Log(point.localPosition.x);

        //EnvironmentLogic env = new EnvironmentLogic();

        Utils.InitializeVariables();

        EnvironmentLogic.StartCellularLifeInTheBrain();


        MoveVectorsToTumoralCell();

        //Debug.Log(EnvironmentLogic.stem_cell_list.Count);


        InitializeLists();

        CreateCellTransform(EnvironmentLogic.stem_cell_list, StemCellPrefab, cell_dict);
        CreateCellTransform(EnvironmentLogic.astrocyte_cell_list, AstrocytePrefab, astrocyte_list);
        CreateCellTransform(EnvironmentLogic.neuron_cell_list, NeuronPrefab, neuron_list);

        CreateArteryTransform();

        //EnvironmentLogic.GetCellsThatSenseTheTumorSubstance();
        //EnvironmentLogic.PathFromCellsToTumorCell();


        //UpdateTumorCellTransforms();
    }

    void Update()
    {
        //if (!is_over)
        //    MoveCellsToTumor();

        //is_over = true;
        //foreach (var item in tumoral_cell_list)
        //{
        //    Pos pos1 = null;

        //    if (item.Key.des_pos != null)
        //        pos1 = item.Key.des_pos;
        //    else
        //        pos1 = item.Key.pos;

        //    var pos2 = item.Value.localPosition;

        //    if (pos1.X != pos2.x || pos1.Y != pos2.y || pos1.Z != pos2.z)
        //        is_over = false;
        //}

        //if (is_over)
        //{
        //    if (!is_over_conver)
        //    {
        //        EnvironmentLogic.StemCellConvertToTumoralCell();
        //        is_over_conver = true;
        //    }
        //    //int time = 0;
        //    //while (time != 10)
        //    //{
        //    //    time++;
        //    while (count != 0)
        //    {
        //        count--;
        //        Debug.Log("Estoy en el ciclo");
        //        Debug.Log(count);

        //        SphereDraw();

        //        CellDraw();

        //        //Invoke("SphereDraw", 7);


        //        //Invoke("CellDraw", 5); 
        //    }
        //}


    }

    void InitializeLists()
    {
        cell_dict = new Dictionary<Cell, Transform>();
        tumoral_cell_list = new Dictionary<Cell, Transform>();
        sphere_list = new Dictionary<Sphere, Transform>();
        astrocyte_list = new Dictionary<Cell, Transform>();
        neuron_list = new Dictionary<Cell, Transform>();
        artery_list = new Dictionary<Artery, Transform>();
    }

    void MoveVectorsToTumoralCell()
    {
        tumoralCell = Instantiate(TumoralCellPrefab);
        tumoralCell.localPosition = new Vector3(EnvironmentLogic.tumor_stem_cell.pos.X, EnvironmentLogic.tumor_stem_cell.pos.Y, EnvironmentLogic.tumor_stem_cell.pos.Z);
    }

    void CreateArteryTransform()
    {
        foreach (Artery artery in EnvironmentLogic.artery_list)
        {
            Transform artery_transform = Instantiate(ArteryPrefab);
            artery_transform.localPosition = new Vector3(artery.pos.X, artery.pos.Y, artery.pos.Z);
            artery_list.Add(artery, artery_transform);
        }
    }

    void CreateCellTransform(List<Cell> cell_list, Transform prefab, Dictionary<Cell, Transform> cell_transform_dict)
    {
        foreach (Cell cell in cell_list)
        {
            Transform stemCell = Instantiate(prefab);
            stemCell.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
            cell_transform_dict.Add(cell, stemCell);
        }
    }

    void UpdateTumorCellTransforms()
    {
        foreach (Cell cell in EnvironmentLogic.tumor_cell_list)
        {
            if (cell_dict.ContainsKey(cell))
                tumoral_cell_list.Add(cell, cell_dict[cell]);
            else
            {
                Transform t = Instantiate(StemCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_list.Add(cell, t);
            }
        }
    }

    void MoveCellsToTumor()
    {
        float step = speed * Time.deltaTime;
        //tumoralCell.position = Vector3.MoveTowards(tumoralCell.position, new Vector3(30, 21, 5), step);
        foreach (var key_value in tumoral_cell_list)
        {
            //Debug.Log(key_value);

            Pos pos = key_value.Key.des_pos;

            //Debug.Log(pos);
            key_value.Value.position = Vector3.MoveTowards(key_value.Value.position, new Vector3(pos.X, pos.Y, pos.Z), step);
        }
    }

    void CellDraw()
    {
        Debug.Log("Estoy en Cell Draw");
        Debug.Log(EnvironmentLogic.cells_without_sphere.Count);
        Debug.Log(tumoral_cell_list.Count);
        foreach (Cell cell in EnvironmentLogic.cells_without_sphere)
        {
            if (!tumoral_cell_list.ContainsKey(cell))
            {
                Transform t = Instantiate(StemCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_list.Add(cell, t);
            }
        }
    }

    //void SphereDraw()
    //{
    //    Utils.CellMove();

    //    if (EnvironmentLogic.sphere_cell_dict.Count > 0)
    //    {
    //        Debug.Log("Estoy aqui");
    //        Debug.Log(EnvironmentLogic.sphere_cell_dict.Count);

    //        foreach (Cell cell in EnvironmentLogic.sphere_cell_dict.Keys)
    //        {
    //            Sphere sphere = EnvironmentLogic.sphere_cell_dict[cell];
    //            if (!sphere_list.ContainsKey(sphere))
    //            {
    //                Transform t = Instantiate(TumoralSpherePrefab);
    //                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    //                t.localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);
    //                sphere_list.Add(sphere, t);
    //            }
    //            else
    //                sphere_list[sphere].localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);

    //            Destroy_Transform(sphere.cell_list, cell);
    //            //AQUI ES DONDE DEBERIA ACTUALIZAR EL RADIO DE LA ESFERA
    //        }
    //    }
    //}

    void Destroy_Transform(List<Cell> cell_list, Cell cell)
    {
        if (tumoral_cell_list.ContainsKey(cell))
        {
            Destroy(tumoral_cell_list[cell].gameObject);
            tumoral_cell_list.Remove(cell);
        }

        foreach (Cell item in cell_list)
        {
            if (tumoral_cell_list.ContainsKey(item))
            {
                Destroy(tumoral_cell_list[item].gameObject);
                tumoral_cell_list.Remove(item);
            }
        }
    }
}
