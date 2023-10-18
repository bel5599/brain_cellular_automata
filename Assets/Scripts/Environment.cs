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

    Dictionary<Cell, Transform> stem_cell_dict;

    Dictionary<Cell, Transform> astrocyte_list;
    Dictionary<Cell, Transform> neuron_list;
    Dictionary<Artery, Transform> artery_list;


    Dictionary<Cell, Transform> tumoral_cell_dict;
    Dictionary<Sphere, Transform> sphere_list;
    //List<Transform> cell_list;
    //List<Transform> tumoral_cell_list;
    Transform tumoralCell;

    void Awake()
    {
        //Aqui es donde se inicializa el array de movimiento en el espacio 3d
        Utils.InitializeVariables();

        //aqui es donde se crean todas las celulas junto con la clase tumor
        EnvironmentLogic.StartCellularLifeInTheBrain();

        //se crea el transform de la celula cancerigena inicial
        InitialTumoralCell();

        //aqui se inicializa todas las listas que se utilizan
        InitializeLists();
        
        //creacion de los transform de las arterias
        CreateArteryTransform();

        //creacion de los transform de las celulas madres, astrocitos y neuronas
        CreateCellTransform(EnvironmentLogic.stem_cell_list, StemCellPrefab, stem_cell_dict);
        CreateCellTransform(EnvironmentLogic.astrocyte_cell_list, AstrocytePrefab, astrocyte_list);
        CreateCellTransform(EnvironmentLogic.neuron_cell_list, NeuronPrefab, neuron_list);

        //estos son los metodos que se utilizan para buscar la posicion cercana a la celula tumoral
        // y los vasos sanguineos respectivamente
        EnvironmentLogic.GetCellsThatSenseTheTumorSubstance();
        EnvironmentLogic.PathFromCellsToTumorCell();
        EnvironmentLogic.MoveAstrocyteToVessels();

        //aqui se agregan los transform de las nuevas celulas madres
        UpdateTumorCellTransforms();
        //UpdateAstrocyteTransforms();
    }

    void Update()
    {
        if (!is_over)
        {
            //en estos metodos se realiza el movimiento de las celulas a sus respectivas posiciones
            MoveCellsToTumor();
            MoveAstrocyteToVessel();
        }

        is_over = true;
        if(!VerificarMovCell(tumoral_cell_dict) || !VerificarMovCell(astrocyte_list))
            is_over = false;

        if (is_over)
        {
            if (!is_over_conver)
            {
                EnvironmentLogic.StemCellConvertToTumoralCell();
                EnvironmentLogic.UpdateAstrocytePosition();
                is_over_conver = true;
            }
            EnvironmentLogic.UpdateActions();
            UpdateTransforms();

            EnvironmentLogic.ExecuteActions();

            EnvironmentLogic.UpdateTumorState();

            //    SphereDraw();

            //    CellDraw();
        }


    }

    void InitializeLists()
    {
        stem_cell_dict = new Dictionary<Cell, Transform>();
        tumoral_cell_dict = new Dictionary<Cell, Transform>();
        sphere_list = new Dictionary<Sphere, Transform>();
        astrocyte_list = new Dictionary<Cell, Transform>();
        neuron_list = new Dictionary<Cell, Transform>();
        artery_list = new Dictionary<Artery, Transform>();
    }

    void InitialTumoralCell()
    {
        tumoralCell = Instantiate(TumoralCellPrefab);
        tumoralCell.localPosition = new Vector3(EnvironmentLogic.tumor.ini_cell.pos.X, EnvironmentLogic.tumor.ini_cell.pos.Y, EnvironmentLogic.tumor.ini_cell.pos.Z);
    }

    void VerificarMovCell(Dictionary<Cell, Transform> cell_transf_dict)
    {
        foreach (var item in cell_transf_dict)
        {
            Pos pos1 = null;

            if (item.Key.des_pos != null)
                pos1 = item.Key.des_pos;
            else
                pos1 = item.Key.pos;

            var pos2 = item.Value.localPosition;

            if (pos1.X != pos2.x || pos1.Y != pos2.y || pos1.Z != pos2.z)
                return false;
        }
        return true;
    }

    void CreateArteryTransform()
    {
        foreach (Artery artery in EnvironmentLogic.artery_list)
        {
            //Debug.Log(artery.pos.X);
            //Debug.Log(artery.pos.Y);
            //Debug.Log(artery.pos.Z);
            //Debug.Log("finito");
            Transform artery_transform = Instantiate(ArteryPrefab);
            artery_transform.localPosition = new Vector3(artery.pos1.X, artery.pos1.Y, artery.pos1.Z);
            artery_transform.localRotation = Quaternion.Euler(0, 0, 90);
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
        foreach (Cell cell in EnvironmentLogic.tumor.cell_list)
        {
            if (stem_cell_dict.ContainsKey(cell))
                tumoral_cell_dict.Add(cell, stem_cell_dict[cell]);
            else
            {
                Transform t = Instantiate(TumoralCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_dict.Add(cell, t);
            }
        }
    }

    void UpdateAstrocyteTransforms()
    {
        foreach (Cell cell in EnvironmentLogic.astrocyte_cell_list)
        {
            if (astrocyte_list.ContainsKey(cell))
                astrocyte_list.Add(cell, astrocyte_list[cell]);
            else
            {
                Transform t = Instantiate(AstrocytePrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                astrocyte_list.Add(cell, t);
            }
        }
    }

    void MoveCellsToTumor()
    {
        float step = speed * Time.deltaTime;
        //tumoralCell.position = Vector3.MoveTowards(tumoralCell.position, new Vector3(30, 21, 5), step);
        foreach (var key_value in tumoral_cell_dict)
        {
            //Debug.Log(key_value);

            Pos pos = key_value.Key.des_pos;

            //Debug.Log(pos);
            key_value.Value.position = Vector3.MoveTowards(key_value.Value.position, new Vector3(pos.X, pos.Y, pos.Z), step);
        }
    }

    void MoveAstrocyteToVessel()
    {
        Debug.Log("Estoy aqui");
        float step = speed * Time.deltaTime;
        //tumoralCell.position = Vector3.MoveTowards(tumoralCell.position, new Vector3(30, 21, 5), step);

        Debug.Log(astrocyte_list.Count);
        foreach (var key_value in astrocyte_list)
        {
            if (key_value.Key.des_pos != null)
            {
                //Debug.Log(key_value);

                Pos pos = key_value.Key.des_pos;

                //Debug.Log(pos);
                Debug.Log(key_value.Value);
                key_value.Value.position = Vector3.MoveTowards(key_value.Value.position, new Vector3(pos.X, pos.Y, pos.Z), step);
            }
        }
    }

    void UpdateTransform()
    {
        foreach (Cell cell in EnvironmentLogic.tumor.cell_list)
        {
            if(!tumoral_cell_dict.ContainsKey(cell))
            {
                Transform t = Instantiate(TumoralCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_dict.Add(cell, t);
            }
            if (cell.actual_action == Contaminate)
            {
                

                Pos pos = cell.actual_division.pos;
                Cell cell_temp = cell.actual_division.cell;
                Cell cell_temp2 = cell.actual_division.cont_cell;
            }
            else if(cell.actual_action == Division)
            {
                Pos pos = cell.actual_division.pos;
                Cell cell_temp = cell.actual_division.cell;

                if (tumoral_cell_dict.ContainsKey())
            }
            else if(cell.actual_action == Nothing)
            {

            }
        }
    }

    void CellDraw()
    {
        Debug.Log("Estoy en Cell Draw");
        Debug.Log(EnvironmentLogic.cells_without_sphere.Count);
        Debug.Log(tumoral_cell_dict.Count);
        foreach (Cell cell in EnvironmentLogic.cells_without_sphere)
        {
            if (!tumoral_cell_dict.ContainsKey(cell))
            {
                Transform t = Instantiate(StemCellPrefab);
                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
                tumoral_cell_dict.Add(cell, t);
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
        if (tumoral_cell_dict.ContainsKey(cell))
        {
            Destroy(tumoral_cell_dict[cell].gameObject);
            tumoral_cell_dict.Remove(cell);
        }

        foreach (Cell item in cell_list)
        {
            if (tumoral_cell_dict.ContainsKey(item))
            {
                Destroy(tumoral_cell_dict[item].gameObject);
                tumoral_cell_dict.Remove(item);
            }
        }
    }
}
