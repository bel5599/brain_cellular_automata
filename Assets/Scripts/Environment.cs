using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataCelularLogic;

public class Environment : MonoBehaviour
{
    [SerializeField]
    Transform TumoralCellPrefab, StemCellPrefab, TumoralSpherePrefab;

    //Transform target;
    public float speed;

    bool is_over_conver = false;
    bool is_over = false;
    int count = 6;

    Dictionary<Cell, Transform> cell_list;
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

        EnvironmentLogic.InitializeVariables();

        EnvironmentLogic.StartCellularLifeInTheBrain();
        

        tumoralCell = Instantiate(TumoralCellPrefab);
        tumoralCell.localPosition = new Vector3(EnvironmentLogic.tumor_stem_cell.pos.X, EnvironmentLogic.tumor_stem_cell.pos.Y, EnvironmentLogic.tumor_stem_cell.pos.Z);

        Debug.Log(EnvironmentLogic.stem_cell_list.Count);

        cell_list = new Dictionary<Cell, Transform>();
        tumoral_cell_list = new Dictionary<Cell, Transform>();
        sphere_list = new Dictionary<Sphere, Transform>();

        foreach (Cell cell in EnvironmentLogic.stem_cell_list)
        {
            Transform stemCell = Instantiate(StemCellPrefab);
            stemCell.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
            cell_list.Add(cell, stemCell);
        }

        EnvironmentLogic.GetCellsThatSenseTheTumorSubstance();
        EnvironmentLogic.PathFromCellsToTumorCell();

        foreach (Cell cell in EnvironmentLogic.tumor_cell_list)
        {
            if (cell_list.ContainsKey(cell))
                tumoral_cell_list.Add(cell, cell_list[cell]);
            else
            {
                Transform t = Instantiate(StemCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_list.Add(cell, t);
            }
        }

    }

    void Update()
    {
        if (!is_over)
            MoveCellsToTumor();

        is_over = true;
        foreach (var item in tumoral_cell_list)
        {
            Pos pos1 = null;

            if (item.Key.des_pos != null)
                pos1 = item.Key.des_pos;
            else
                pos1 = item.Key.pos;

            var pos2 = item.Value.localPosition;

            if (pos1.X != pos2.x || pos1.Y != pos2.y || pos1.Z != pos2.z)
                is_over = false;
        }

        if (is_over)
        {
            if (!is_over_conver)
            {
                EnvironmentLogic.StemCellConvertToTumoralCell();
                is_over_conver = true;
            }
            //int time = 0;
            //while (time != 10)
            //{
            //    time++;
            while (count != 0)
            {
                count--;
                Debug.Log("Estoy en el ciclo");
                Debug.Log(count);

                SphereDraw();

                CellDraw();

                //Invoke("SphereDraw", 7);


                //Invoke("CellDraw", 5); 
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

    void SphereDraw()
    {
        EnvironmentLogic.CellMove();

        if (EnvironmentLogic.sphere_cell_dict.Count > 0)
        {
            Debug.Log("Estoy aqui");
            Debug.Log(EnvironmentLogic.sphere_cell_dict.Count);

            foreach (Cell cell in EnvironmentLogic.sphere_cell_dict.Keys)
            {
                Sphere sphere = EnvironmentLogic.sphere_cell_dict[cell];
                if (!sphere_list.ContainsKey(sphere))
                {
                    Transform t = Instantiate(TumoralSpherePrefab);
                    t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                    t.localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);
                    sphere_list.Add(sphere, t);
                }
                else
                    sphere_list[sphere].localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);

                Destroy_Transform(sphere.cell_list, cell);
                //AQUI ES DONDE DEBERIA ACTUALIZAR EL RADIO DE LA ESFERA
            }
        }
    }

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
