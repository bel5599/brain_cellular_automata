using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataCelularLogic;
using System;

public class Environment : MonoBehaviour
{
    [SerializeField]
    Transform ProliferativeTumoralCellPrefab, NecroticTumoralCellPrefab, QuiescentTumorCellPrefab, StemCellPrefab, TumoralSpherePrefab,
        NeuronPrefab, AstrocytePrefab, /*ArteryPrefab*/ /*ArteriolePrefab, CapillaryPrefab,*/ MigratoryPrefab/*, SegmentPrefab, CamaraPrefab*/, informationPrefab;

    public GameObject VasoPrefb;
    public GameObject ArteryPrefab;

    public GameObject NeoVasoPrefb;
    public GameObject EndothelialPrefab;

    public FrameRateCounter script;

    [SerializeField]
    public LineRenderer linePrefab;

    private LineRenderer lineRenderer;

    //Transform target;
    //public float speed;

    float secondsCounter = 0;
    float secondsToCount = 2;

    bool is_over_conver = false;
    bool is_over = false;
    int count = 5;

    int actual_count = 0;

    //int PositionScale = 6;
    //int PositionScale = 5;

    //float scale_factor_1 = 100f;
    //float scale_factor_2 = 50;
    //float scale_factor_3 = 25f;

    public float PositionScale = 1;

    Dictionary<Cell, Transform> stem_cell_dict;

    Dictionary<Cell, Transform> astrocyte_dict;
    Dictionary<Cell, Transform> neuron_dict;
    List<Tuple<Cell, GameObject>> artery_dict;

    //Dictionary<Pos, Transform> pos

    //Dictionary<Arteriole, Transform> arteriole_dict;
    //Dictionary<Capillary, Transform> capillary_dict;

    Dictionary<Cell, Transform> proliferative_dict;
    Dictionary<Cell, Transform> migratory_dict;
    Dictionary<Cell, Transform> necrotic_cell_dict;
    Dictionary<Cell, Transform> quiescent_cell_dict;
    Dictionary<Cell, Transform> tumoral_cell_dict;
    Dictionary<Sphere, Transform> sphere_list;

    Dictionary<AutomataCelularLogic.BloodVessel, GameObject> neo_artery_dict;
    //List<Transform> cell_list;
    //List<Transform> tumoral_cell_list;
    Transform tumoralCell;

    void Awake()
    {
        EnvironmentLogic.Simulation();
        Debug.Log("Termine Simulation");
        ////Aqui es donde se inicializa el array de movimiento en el espacio 3d
        //Utils.InitializeVariables();

        ////aqui es donde se crean todas las celulas junto con la clase tumor
        //EnvironmentLogic.StartCellularLifeInTheBrain();

        ////se crea el transform de la celula cancerigena inicial
        InitialTumoralCell();

        ////aqui se inicializa todas las listas que se utilizan
        InitializeLists();

        ////creacion de los transform de las arterias
        //CreateArteryTransform();
        //Mixto();

        //UpdateBloodVessels();
        var info = Instantiate(informationPrefab);
        var script = info.GetComponent<FrameRateCounter>();

        //script = GetComponent<FrameRateCounter>();
        if(script!= null)
        {
            Debug.Log("No es null");
            script.UpdateCanvas(EnvironmentLogic.ca);
        }

        BloodVesselsLine();
        Rellenar();

        Debug.Log(migratory_dict.Count);
        Debug.Log(necrotic_cell_dict.Count);
        Debug.Log(tumoral_cell_dict);


        Debug.Log("Termine de rellenar");
        ////creacion de los transform de las celulas madres, astrocitos y neuronas
        //CreateCellTransform(EnvironmentLogic.stem_cell_list, StemCellPrefab, stem_cell_dict);
        //CreateCellTransform(EnvironmentLogic.astrocyte_cell_list, AstrocytePrefab, astrocyte_list);
        //CreateCellTransform(EnvironmentLogic.neuron_cell_list, NeuronPrefab, neuron_list);

        ////estos son los metodos que se utilizan para buscar la posicion cercana a la celula tumoral
        //// y los vasos sanguineos respectivamente
        //EnvironmentLogic.GetCellsThatSenseTheTumorSubstance();
        //EnvironmentLogic.PathFromCellsToTumorCell();
        //EnvironmentLogic.MoveAstrocyteToVessels();

        ////aqui se agregan los transform de las nuevas celulas madres
        //UpdateTumorCellTransforms();
        //UpdateAstrocyteTransforms();
    }

    

    void Update()
    {
        //if (actual_count < count)
        //{
            secondsCounter += Time.deltaTime;
        if (secondsCounter >= secondsToCount)
        {
            Debug.Log("Estoy aqui");
            secondsCounter = 0;
            EnvironmentLogic.ca.Update();

            if (EnvironmentLogic.ca.tumoral_angiogenic_factor > 0.5)
            {
                Debug.Log("Ya deberian salir las celulas migratorias");
            }

            UpdatePosCellDict(neuron_dict);
            UpdatePosCellDict(stem_cell_dict);
            UpdatePosCellDict(tumoral_cell_dict);
            UpdatePosCellDict(astrocyte_dict);
            UpdatePosCellDict(proliferative_dict);
            UpdatePosCellDict(migratory_dict);

            UpdateCellTransformsDict();
            UpdateVessels();

            //script = GetComponent<FrameRateCounter>();

            ////script.Update();

            //if (script != null)
            //{
                script.UpdateCanvas(EnvironmentLogic.ca);
            //}

            //UpdateBloodVessels();
        }
        actual_count++;
        //}

            //if (!is_over)
            //{
            //    //en estos metodos se realiza el movimiento de las celulas a sus respectivas posiciones
            //    MoveCellsToTumor();
            //    MoveAstrocyteToVessel();
            //}

            //is_over = true;
            //if(!VerificarMovCell(tumoral_cell_dict) || !VerificarMovCell(astrocyte_list))
            //    is_over = false;

            //if (is_over)
            //{
            //    if (!is_over_conver)
            //    {
            //        EnvironmentLogic.StemCellConvertToTumoralCell();
            //        EnvironmentLogic.UpdateAstrocytePosition();
            //        is_over_conver = true;
            //    }
            //    EnvironmentLogic.UpdateActions();
            //    UpdateTransforms();

            //    EnvironmentLogic.ExecuteActions();

            //    EnvironmentLogic.UpdateTumorState();

            //    //    SphereDraw();

            //    //    CellDraw();
            //}


        }

    void InitializeLists()
    {
        stem_cell_dict = new Dictionary<Cell, Transform>();
        tumoral_cell_dict = new Dictionary<Cell, Transform>();
        sphere_list = new Dictionary<Sphere, Transform>();
        astrocyte_dict = new Dictionary<Cell, Transform>();
        neuron_dict = new Dictionary<Cell, Transform>();
        artery_dict = new List<Tuple<Cell, GameObject>>();
        necrotic_cell_dict = new Dictionary<Cell, Transform>();
        quiescent_cell_dict = new Dictionary<Cell, Transform>();
        proliferative_dict = new Dictionary<Cell, Transform>();
        //arteriole_dict = new Dictionary<Arteriole, Transform>();
        //capillary_dict = new Dictionary<Capillary, Transform>();
        migratory_dict = new Dictionary<Cell, Transform>();
    }

    void InitialTumoralCell()
    {
        tumoralCell = Instantiate(ProliferativeTumoralCellPrefab);
        tumoralCell.localPosition = new Vector3(EnvironmentLogic.ca.tumor_stem_cell.pos.X * PositionScale, EnvironmentLogic.ca.tumor_stem_cell.pos.Y * PositionScale, EnvironmentLogic.ca.tumor_stem_cell.pos.Z* PositionScale);
    }

    #region metodos_que_no_se_utilizan_ahora
    //void VerificarMovCell(Dictionary<Cell, Transform> cell_transf_dict)
    //{
    //    foreach (var item in cell_transf_dict)
    //    {
    //        Pos pos1 = null;

    //        if (item.Key.des_pos != null)
    //            pos1 = item.Key.des_pos;
    //        else
    //            pos1 = item.Key.pos;

    //        var pos2 = item.Value.localPosition;

    //        if (pos1.X != pos2.x || pos1.Y != pos2.y || pos1.Z != pos2.z)
    //            return false;
    //    }
    //    return true;
    //}

    //void CreateArteryTransform()
    //{
    //    foreach (Artery artery in EnvironmentLogic.artery_list)
    //    {
    //        //Debug.Log(artery.pos.X);
    //        //Debug.Log(artery.pos.Y);
    //        //Debug.Log(artery.pos.Z);
    //        //Debug.Log("finito");
    //        Transform artery_transform = Instantiate(ArteryPrefab);
    //        artery_transform.localPosition = new Vector3(artery.pos1.X, artery.pos1.Y, artery.pos1.Z);
    //        artery_transform.localRotation = Quaternion.Euler(0, 0, 90);
    //        artery_list.Add(artery, artery_transform);
    //    }
    //}

    //void CreateCellTransform(List<Cell> cell_list, Transform prefab, Dictionary<Cell, Transform> cell_transform_dict)
    //{
    //    foreach (Cell cell in cell_list)
    //    {
    //        Transform stemCell = Instantiate(prefab);
    //        stemCell.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    //        cell_transform_dict.Add(cell, stemCell);
    //    }
    //}

    //void UpdateTumorCellTransforms()
    //{
    //    foreach (Cell cell in EnvironmentLogic.tumor.cell_list)
    //    {
    //        if (stem_cell_dict.ContainsKey(cell))
    //            tumoral_cell_dict.Add(cell, stem_cell_dict[cell]);
    //        else
    //        {
    //            Transform t = Instantiate(TumoralCellPrefab);
    //            t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    //            tumoral_cell_dict.Add(cell, t);
    //        }
    //    }
    //}

    //void UpdateAstrocyteTransforms()
    //{
    //    foreach (Cell cell in EnvironmentLogic.astrocyte_cell_list)
    //    {
    //        if (astrocyte_list.ContainsKey(cell))
    //            astrocyte_list.Add(cell, astrocyte_list[cell]);
    //        else
    //        {
    //            Transform t = Instantiate(AstrocytePrefab);
    //            t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    //            astrocyte_list.Add(cell, t);
    //        }
    //    }
    //}

    //void MoveCellsToTumor()
    //{
    //    float step = speed * Time.deltaTime;
    //    //tumoralCell.position = Vector3.MoveTowards(tumoralCell.position, new Vector3(30, 21, 5), step);
    //    foreach (var key_value in tumoral_cell_dict)
    //    {
    //        //Debug.Log(key_value);

    //        Pos pos = key_value.Key.des_pos;

    //        //Debug.Log(pos);
    //        key_value.Value.position = Vector3.MoveTowards(key_value.Value.position, new Vector3(pos.X, pos.Y, pos.Z), step);
    //    }
    //}

    //void MoveAstrocyteToVessel()
    //{
    //    Debug.Log("Estoy aqui");
    //    float step = speed * Time.deltaTime;
    //    //tumoralCell.position = Vector3.MoveTowards(tumoralCell.position, new Vector3(30, 21, 5), step);

    //    Debug.Log(astrocyte_list.Count);
    //    foreach (var key_value in astrocyte_list)
    //    {
    //        if (key_value.Key.des_pos != null)
    //        {
    //            //Debug.Log(key_value);

    //            Pos pos = key_value.Key.des_pos;

    //            //Debug.Log(pos);
    //            Debug.Log(key_value.Value);
    //            key_value.Value.position = Vector3.MoveTowards(key_value.Value.position, new Vector3(pos.X, pos.Y, pos.Z), step);
    //        }
    //    }
    //}

    ////void UpdateTransform()
    ////{
    ////    foreach (Cell cell in EnvironmentLogic.tumor.cell_list)
    ////    {
    ////        if(!tumoral_cell_dict.ContainsKey(cell))
    ////        {
    ////            Transform t = Instantiate(TumoralCellPrefab);
    ////            t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    ////            tumoral_cell_dict.Add(cell, t);
    ////        }
    ////        if (cell.actual_action == Contaminate)
    ////        {


    ////            Pos pos = cell.actual_division.pos;
    ////            Cell cell_temp = cell.actual_division.cell;
    ////            Cell cell_temp2 = cell.actual_division.cont_cell;
    ////        }
    ////        else if(cell.actual_action == Division)
    ////        {
    ////            Pos pos = cell.actual_division.pos;
    ////            Cell cell_temp = cell.actual_division.cell;

    ////            if (tumoral_cell_dict.ContainsKey())
    ////        }
    ////        else if(cell.actual_action == Nothing)
    ////        {

    ////        }
    ////    }
    ////}

    //void CellDraw()
    //{
    //    Debug.Log("Estoy en Cell Draw");
    //    Debug.Log(EnvironmentLogic.cells_without_sphere.Count);
    //    Debug.Log(tumoral_cell_dict.Count);
    //    foreach (Cell cell in EnvironmentLogic.cells_without_sphere)
    //    {
    //        if (!tumoral_cell_dict.ContainsKey(cell))
    //        {
    //            Transform t = Instantiate(StemCellPrefab);
    //            t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    //            tumoral_cell_dict.Add(cell, t);
    //        }
    //    }
    //}

    ////void SphereDraw()
    ////{
    ////    Utils.CellMove();

    ////    if (EnvironmentLogic.sphere_cell_dict.Count > 0)
    ////    {
    ////        Debug.Log("Estoy aqui");
    ////        Debug.Log(EnvironmentLogic.sphere_cell_dict.Count);

    ////        foreach (Cell cell in EnvironmentLogic.sphere_cell_dict.Keys)
    ////        {
    ////            Sphere sphere = EnvironmentLogic.sphere_cell_dict[cell];
    ////            if (!sphere_list.ContainsKey(sphere))
    ////            {
    ////                Transform t = Instantiate(TumoralSpherePrefab);
    ////                t.localPosition = new Vector3(cell.pos.X, cell.pos.Y, cell.pos.Z);
    ////                t.localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);
    ////                sphere_list.Add(sphere, t);
    ////            }
    ////            else
    ////                sphere_list[sphere].localScale = new Vector3(sphere.radio, sphere.radio, sphere.radio);

    ////            Destroy_Transform(sphere.cell_list, cell);
    ////            //AQUI ES DONDE DEBERIA ACTUALIZAR EL RADIO DE LA ESFERA
    ////        }
    ////    }
    ////}
    #endregion


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
    void Destroy_Transform(List<Cell> cell_list, Dictionary<Cell, Transform> cell_trans_dict)
    {
        foreach (Cell cell in cell_list)
        {
            if(cell_trans_dict.ContainsKey(cell))
            {
                Destroy(cell_trans_dict[cell].gameObject);
                cell_trans_dict.Remove(cell);
            }
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
    void Rellenar()
    {
        Dictionary<Pos, Cell> pos_cell_dict = EnvironmentLogic.ca.pos_cell_dict;
        foreach (var key_value in pos_cell_dict)
        {
            if (key_value.Value.behavior_state == CellState.ProliferativeTumoralCell)
            {
                Transform stemCell = Instantiate(ProliferativeTumoralCellPrefab);
                stemCell.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                stemCell.position = stemCell.localPosition * PositionScale;
                proliferative_dict.Add(key_value.Value, stemCell);
            }
            else if (key_value.Value.behavior_state == CellState.StemCell)
            {
                Transform stemCell = Instantiate(StemCellPrefab);
                stemCell.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                stemCell.position = stemCell.localPosition * PositionScale;
                //stemCell.localPosition = new Vector3(key_value.Key.X*PositionScale, key_value.Key.Y* PositionScale, key_value.Key.Z* PositionScale);
                stem_cell_dict.Add(key_value.Value, stemCell);
            }
            else if (key_value.Value.behavior_state == CellState.Astrocyte)
            {
                Transform astrocyte_cell = Instantiate(AstrocytePrefab);
                astrocyte_cell.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                astrocyte_cell.position = astrocyte_cell.localPosition * PositionScale;
                //astrocyte_cell.localPosition = new Vector3(key_value.Key.X* PositionScale, key_value.Key.Y* PositionScale, key_value.Key.Z* PositionScale);
                astrocyte_dict.Add(key_value.Value, astrocyte_cell);
            }
            else if (key_value.Value.behavior_state == CellState.Neuron)
            {
                Transform neuron = Instantiate(NeuronPrefab);
                neuron.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                neuron.position = neuron.localPosition * PositionScale;
                //neuron.localPosition = new Vector3(key_value.Key.X* PositionScale, key_value.Key.Y* PositionScale, key_value.Key.Z* PositionScale);
                neuron_dict.Add(key_value.Value, neuron);
            }
            else if (key_value.Value.behavior_state == CellState.NecroticTumorCell)
            {
                Transform necrotic = Instantiate(NecroticTumoralCellPrefab);
                necrotic.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                necrotic.position = necrotic.localPosition * PositionScale;
                //necrotic.localPosition = new Vector3(key_value.Key.X* PositionScale, key_value.Key.Y* PositionScale, key_value.Key.Z* PositionScale);
                necrotic_cell_dict.Add(key_value.Value, necrotic);
            }
            else if (key_value.Value.behavior_state == CellState.QuiescentTumorCell)
            {
                Transform quiescent = Instantiate(QuiescentTumorCellPrefab);
                quiescent.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                quiescent.position = quiescent.localPosition * PositionScale;
                //quiescent.localPosition = new Vector3(key_value.Key.X* PositionScale, key_value.Key.Y* PositionScale, key_value.Key.Z* PositionScale);
                quiescent_cell_dict.Add(key_value.Value, quiescent);
            }
            else if(key_value.Value.behavior_state == CellState.MigratoryTumorCell)
            {
                Transform migratory = Instantiate(MigratoryPrefab);
                migratory.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
                migratory.position = migratory.localPosition * PositionScale;
                //migratory.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
                migratory_dict.Add(key_value.Value, migratory);
            }
        }


    }

    //void Rellenar()
    //{
    //    Dictionary<Pos, Cell> pos_cell_dict = EnvironmentLogic.ca.pos_cell_dict;
    //    foreach (var key_value in pos_cell_dict)
    //    {
    //        if (key_value.Value.behavior_state == CellState.ProliferativeTumoralCell)
    //        {
    //            Transform stemCell = Instantiate(ProliferativeTumoralCellPrefab);
    //            stemCell.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            stemCell.position =
    //            proliferative_dict.Add(key_value.Value, stemCell);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.StemCell)
    //        {
    //            Transform stemCell = Instantiate(StemCellPrefab);
    //            stemCell.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            stem_cell_dict.Add(key_value.Value, stemCell);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.Astrocyte)
    //        {
    //            Transform astrocyte_cell = Instantiate(AstrocytePrefab);
    //            astrocyte_cell.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            astrocyte_dict.Add(key_value.Value, astrocyte_cell);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.Neuron)
    //        {
    //            Transform neuron = Instantiate(NeuronPrefab);
    //            neuron.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            neuron_dict.Add(key_value.Value, neuron);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.NecroticTumorCell)
    //        {
    //            Transform necrotic = Instantiate(NecroticTumoralCellPrefab);
    //            necrotic.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            necrotic_cell_dict.Add(key_value.Value, necrotic);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.QuiescentTumorCell)
    //        {
    //            Transform quiescent = Instantiate(QuiescentTumorCellPrefab);
    //            quiescent.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            quiescent_cell_dict.Add(key_value.Value, quiescent);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.MigratoryTumorCell)
    //        {
    //            Transform migratory = Instantiate(MigratoryPrefab);
    //            migratory.localPosition = new Vector3(key_value.Key.X * PositionScale, key_value.Key.Y * PositionScale, key_value.Key.Z * PositionScale);
    //            migratory_dict.Add(key_value.Value, migratory);
    //        }
    //    }


    //}

    Dictionary<Pos, List< Vector3>> ajuste;

    //public List<Tuple<Cell, GameObject>> Artery_dict { get => artery_dict; set => artery_dict = value; }

    //void Rellenar()
    //{
    //    Dictionary<Pos, Cell> pos_cell_dict = EnvironmentLogic.ca.pos_cell_dict;
    //    foreach (var key_value in pos_cell_dict)
    //    {
    //        if (key_value.Value.behavior_state == CellState.StemCell)
    //        {
    //            Transform stemCell = Instantiate(StemCellPrefab);
    //            stemCell.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            stem_cell_dict.Add(key_value.Value, stemCell);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.Astrocyte)
    //        {
    //            Transform astrocyte_cell = Instantiate(AstrocytePrefab);
    //            astrocyte_cell.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            astrocyte_dict.Add(key_value.Value, astrocyte_cell);
    //        }
    //        else if (key_value.Value.behavior_state == CellState.Neuron)
    //        {
    //            Transform neuron = Instantiate(NeuronPrefab);
    //            neuron.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            neuron_dict.Add(key_value.Value, neuron);
    //        }
    //        else if(key_value.Value.behavior_state == CellState.NecroticTumorCell)
    //        {
    //            Transform necrotic = Instantiate(NecroticTumoralCellPrefab);
    //            necrotic.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            necrotic_cell_dict.Add(key_value.Value, necrotic);
    //        }
    //        else if(key_value.Value.behavior_state == CellState.QuiescentTumorCell)
    //        {
    //            Transform quiescent = Instantiate(QuiescentTumorCellPrefab);
    //            quiescent.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            quiescent_cell_dict.Add(key_value.Value, quiescent);
    //        }
    //    }


    //}

    void UpdateVessels()
    {
        //neo_artery_dict
        foreach (var item in EnvironmentLogic.ca.growing_vessels)
        {
            var vessel = Instantiate(ArteryPrefab);
            var artery1 = item.Key;
            vessel.transform.localPosition = new Vector3(artery1.pos.X, artery1.pos.Y, artery1.pos.Z);
            vessel.transform.position = vessel.transform.localPosition * PositionScale;

            for (int i = 0; i < item.Value.Count; i++)
            {
                var vessel1 = Instantiate(EndothelialPrefab);
                var artery2 = item.Value[i];
                vessel1.transform.localPosition = new Vector3(artery2.pos.X, artery2.pos.Y, artery2.pos.Z);
                vessel1.transform.position = vessel1.transform.localPosition * PositionScale;

                var vaso1 = Instantiate(NeoVasoPrefb);
                vaso1.transform.localScale = new Vector3(0.3f, vaso1.transform.localScale.y, 0.3f);
                var script = vaso1.GetComponent<CylinderController>();
                script.point1 = vessel;
                script.point1.transform.localScale *= 0.3f;
                script.point2 = vessel1;
                script.point2.transform.localScale *= 0.3f;
            }
            
        }
    }

    void BloodVesselsLine()
    {
        ajuste = new Dictionary<Pos, List<Vector3>>();
        //int i = 0;
        float scale = 10f;
        foreach (var item in EnvironmentLogic.ca.vessel_segment_list)
        {
            
            //if (item.order == StrahlerOrder.StrahlerOrder_1)
            //    scale = scale_factor_1;
            //else if (item.order == StrahlerOrder.StrahlerOrder_1)
            //    scale = scale_factor_2;
            //else
            //    scale = scale_factor_3;

            //if (item.blood_vessel1.pos.Z == 0)
                //scale = 1;

            var vessel = Instantiate(ArteryPrefab);
            var artery1 = item.blood_vessel1;
            vessel.transform.localPosition = new Vector3(artery1.pos.X, artery1.pos.Y, artery1.pos.Z);
            vessel.transform.position = vessel.transform.localPosition * PositionScale;
                //if (ajuste.ContainsKey(artery1.pos))
                //    ajuste[artery1.pos].Add(vessel.transform.position);
                //else
                //    ajuste.Add(artery1.pos, new List<Vector3>() { vessel.transform.position });
            //    vessel.transform.position = new Vector3(artery1.pos.X * scale, artery1.pos.Y * scale, artery1.pos.Z * scale);
            //    if (ajuste.ContainsKey(artery1.pos))
            //        ajuste[artery1.pos].Add(vessel.transform.position);
            //    else
            //        ajuste.Add(artery1.pos, new List<Vector3>() { vessel.transform.position });
            //}

            //artery_dict.Add(artery1, vessel);
            //Artery_dict.Add(new Tuple<AutomataCelularLogic.BloodVessel, GameObject>(artery1, vessel));

            var vessel2 = Instantiate(ArteryPrefab);
            var artery2 = item.blood_vessel2;
            vessel2.transform.localPosition = new Vector3(artery2.pos.X, artery2.pos.Y, artery2.pos.Z);
            vessel2.transform.position = vessel2.transform.localPosition * PositionScale;
            //vessel2.transform.position = new Vector3(artery2.pos.X * scale, artery2.pos.Y * scale, artery2.pos.Z * scale);
            //if (ajuste.ContainsKey(artery2.pos))
            //    ajuste[artery2.pos].Add(vessel2.transform.position);
            //else
            //    ajuste.Add(artery2.pos, new List<Vector3>() { vessel2.transform.position });

            //artery_dict.Add(artery2, vessel2);
            //artery_dict.Add(new Tuple<AutomataCelularLogic.BloodVessel, GameObject>(artery2, vessel2));

            var vaso1 = Instantiate(VasoPrefb);
            vaso1.transform.localScale = new Vector3(0.3f, vaso1.transform.localScale.y, 0.3f);
            var script = vaso1.GetComponent<CylinderController>();
            script.point1 = vessel;
            script.point1.transform.localScale *= 0.3f;
            script.point2 = vessel2;
            script.point2.transform.localScale *= 0.3f;

        }
    }

    //    foreach (var item in EnvironmentLogic.ca.vessel_segment_list)
    //    {
    //        Transform vessel = Instantiate(CamaraPrefab);
    //        BloodVessel script = vessel.GetComponent<BloodVessel>();
    //        script.pos1 = artery_dict[item.blood_vessel1];
    //        script.pos2 = artery_dict[item.blood_vessel2];

    //        Pos pos1 = item.blood_vessel1.pos;
    //        Pos pos2 = item.blood_vessel2.pos;
    //        Debug.Log("*******************************");
    //        Debug.Log(item.blood_vessel1.pos.X + " " + item.blood_vessel1.pos.Y + " " + item.blood_vessel1.pos.Z);
    //        Debug.Log(item.blood_vessel2.pos.X + " " + item.blood_vessel2.pos.Y + " " + item.blood_vessel2.pos.Z);
    //        Debug.Log(Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2) + Math.Pow(pos1.Z - pos2.Z, 2)));
    //        Debug.Log("*******************************");

    //        script.line = vessel.GetComponent<LineRenderer>();
    //        script.Start();
    //        script.Update();
    //        //BloodVessel vessel = new BloodVessel();
    //        //vessel.pos1 = artery_dict[item.]
    //        //lineRenderer.SetPosition(i, new Vector3(item.pos1.X, item.pos1.Y, item.pos1.Z));
    //        //i++;
    //        //lineRenderer.SetPosition(i, new Vector3(item.pos2.X, item.pos2.Y, item.pos2.Z));
    //        //i++;
    //    }
    //}

    //void BloodVesselsLine()
    //{
    //    //lineRenderer = Instantiate(linePrefab);

    //    //AnimationCurve curve = new AnimationCurve();
    //    //curve.AddKey(0, 0);
    //    //curve.AddKey(.1f, .5f);
    //    //curve.AddKey(.9f, .5f);
    //    //curve.AddKey(1, 0);
    //    //lineRenderer.widthCurve = curve;

    //    //lineRenderer.positionCount = EnvironmentLogic.ca.pos_artery_dict.Count;

    //    //int i = 0;

    //    foreach (var item in EnvironmentLogic.ca.vessel_segment_list)
    //    {
    //        Transform vessel = Instantiate(CamaraPrefab);
    //        BloodVessel script = vessel.GetComponent<BloodVessel>();
    //        script.pos1 = artery_dict[item.blood_vessel1];
    //        script.pos2 = artery_dict[item.blood_vessel2];

    //        Pos pos1 = item.blood_vessel1.pos;
    //        Pos pos2 = item.blood_vessel2.pos;
    //        Debug.Log("*******************************");
    //        Debug.Log(item.blood_vessel1.pos.X+ " " +item.blood_vessel1.pos.Y+ " "+ item.blood_vessel1.pos.Z);
    //        Debug.Log(item.blood_vessel2.pos.X + " " + item.blood_vessel2.pos.Y + " " + item.blood_vessel2.pos.Z);
    //        Debug.Log(Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2) + Math.Pow(pos1.Z - pos2.Z, 2)));
    //        Debug.Log("*******************************");

    //        script.line = vessel.GetComponent<LineRenderer>();
    //        script.Start();
    //        script.Update();
    //        //BloodVessel vessel = new BloodVessel();
    //        //vessel.pos1 = artery_dict[item.]
    //        //lineRenderer.SetPosition(i, new Vector3(item.pos1.X, item.pos1.Y, item.pos1.Z));
    //        //i++;
    //        //lineRenderer.SetPosition(i, new Vector3(item.pos2.X, item.pos2.Y, item.pos2.Z));
    //        //i++;
    //    }
    //}

    void Mixto()
    {
        ////foreach (var item in EnvironmentLogic.ca.vessel_segment_list)
        ////{
        //var tem = EnvironmentLogic.ca.vessel_segment_list[0];
        //Transform vessel = Instantiate(CamaraPrefab);
        //BloodVessel script = vessel.GetComponent<BloodVessel>();

        ////var artery1 = EnvironmentLogic.ca.pos_artery_dict[tem.blood_vessel];
        //var artery1 = tem.blood_vessel1;
        //Transform artery = Instantiate(ArteryPrefab);
        //artery.localPosition = new Vector3(artery1.pos.X, artery1.pos.Y, artery1.pos.Z);
        //artery_dict.Add(artery1, artery);

        //var artery2 = tem.blood_vessel2;
        //Transform artery_2 = Instantiate(ArteryPrefab);
        //artery_2.localPosition = new Vector3(artery2.pos.X, artery2.pos.Y, artery2.pos.Z);
        //artery_dict.Add(artery2, artery_2);

        //script.pos1 = artery;
        //script.pos2 = artery_2;

        //script.Start();
        //script.Update();

        //script.line = vessel.GetComponent<LineRenderer>();

        //}
    }

    void UpdateBloodVessels()
    {
        //Dictionary<Pos, BloodVessel> pos_artery_dict = EnvironmentLogic.ca.pos_artery_dict;
        //foreach (var key_value in EnvironmentLogic.ca.pos_artery_dict)
        //{
        //    if (!artery_dict.ContainsKey(key_value.Value))
        //    {
        //        Transform artery = Instantiate(ArteryPrefab);
        //        artery.localPosition = new Vector3(key_value.Key.X*scale_factor, key_value.Key.Y* scale_factor, key_value.Key.Z* scale_factor);
        //        artery_dict.Add(key_value.Value, artery);
        //    }
        //}

        //Dictionary<Pos, Arteriole> pos_arteriole_dict = EnvironmentLogic.ca.pos_arteriole_dict;
        //foreach (var key_value in pos_arteriole_dict)
        //{
        //    if (!arteriole_dict.ContainsKey(key_value.Value))
        //    {
        //        Transform arteriole = Instantiate(ArteriolePrefab);
        //        arteriole.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
        //        arteriole_dict.Add(key_value.Value, arteriole);
        //    }
        //}

        //Dictionary<Pos, Capillary> pos_capillary_dict = EnvironmentLogic.ca.pos_capillary_dict;
        //foreach (var key_value in pos_capillary_dict)
        //{
        //    if (!capillary_dict.ContainsKey(key_value.Value))
        //    {
        //        Transform capillary = Instantiate(CapillaryPrefab);
        //        capillary.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
        //        capillary_dict.Add(key_value.Value, capillary);
        //    }
        //}
    }

    //void UpdateBloodVessels()
    //{
    //    //Dictionary<Pos, BloodVessel> pos_artery_dict = EnvironmentLogic.ca.pos_artery_dict;
    //    foreach (var key_value in EnvironmentLogic.ca.pos_artery_dict)
    //    {
    //        if (!artery_dict.ContainsKey(key_value.Value))
    //        {
    //            Transform artery = Instantiate(ArteryPrefab);
    //            artery.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //            artery_dict.Add(key_value.Value, artery);
    //        }
    //    }

    //    //Dictionary<Pos, Arteriole> pos_arteriole_dict = EnvironmentLogic.ca.pos_arteriole_dict;
    //    //foreach (var key_value in pos_arteriole_dict)
    //    //{
    //    //    if (!arteriole_dict.ContainsKey(key_value.Value))
    //    //    {
    //    //        Transform arteriole = Instantiate(ArteriolePrefab);
    //    //        arteriole.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //    //        arteriole_dict.Add(key_value.Value, arteriole);
    //    //    }
    //    //}

    //    //Dictionary<Pos, Capillary> pos_capillary_dict = EnvironmentLogic.ca.pos_capillary_dict;
    //    //foreach (var key_value in pos_capillary_dict)
    //    //{
    //    //    if (!capillary_dict.ContainsKey(key_value.Value))
    //    //    {
    //    //        Transform capillary = Instantiate(CapillaryPrefab);
    //    //        capillary.localPosition = new Vector3(key_value.Key.X, key_value.Key.Y, key_value.Key.Z);
    //    //        capillary_dict.Add(key_value.Value, capillary);
    //    //    }
    //    //}
    //}

    void UpdatePosCellDict(Dictionary<Cell, Transform> cell_dict)
    {
        List<Cell> destroy_list = new List<Cell>();
        foreach (Cell cell in cell_dict.Keys)
        {
            if (!EnvironmentLogic.ca.pos_cell_dict.ContainsKey(cell.pos))
            {
                destroy_list.Add(cell);
            }
            else if (cell.behavior_state != EnvironmentLogic.ca.pos_cell_dict[cell.pos].behavior_state)
            {
                destroy_list.Add(cell);
                var behavior_state = EnvironmentLogic.ca.pos_cell_dict[cell.pos].behavior_state;
                if (behavior_state == CellState.Neuron)
                {
                    Transform neuron = Instantiate(NeuronPrefab);
                    neuron.localPosition = new Vector3(cell.pos.X* PositionScale, cell.pos.Y* PositionScale, cell.pos.Z* PositionScale);
                    neuron_dict.Add(cell, neuron);
                }
                else if (behavior_state == CellState.Astrocyte)
                {
                    Transform astrocyte_cell = Instantiate(AstrocytePrefab);
                    astrocyte_cell.localPosition = new Vector3(cell.pos.X* PositionScale, cell.pos.Y* PositionScale, cell.pos.Z* PositionScale);
                    astrocyte_dict.Add(cell, astrocyte_cell);
                }
                else if (behavior_state == CellState.ProliferativeTumoralCell)
                {
                    Transform tumoral_cell = Instantiate(ProliferativeTumoralCellPrefab);
                    tumoral_cell.localPosition = new Vector3(cell.pos.X* PositionScale, cell.pos.Y* PositionScale, cell.pos.Z* PositionScale);
                    tumoral_cell_dict.Add(cell, tumoral_cell);
                }
                else if (behavior_state == CellState.NecroticTumorCell)
                {
                    Transform necrotic = Instantiate(NecroticTumoralCellPrefab);
                    necrotic.localPosition = new Vector3(cell.pos.X* PositionScale, cell.pos.Y* PositionScale, cell.pos.Z* PositionScale);
                    necrotic_cell_dict.Add(cell, necrotic);
                }
                else if (behavior_state == CellState.QuiescentTumorCell)
                {
                    Transform quiescent = Instantiate(QuiescentTumorCellPrefab);
                    quiescent.localPosition = new Vector3(cell.pos.X* PositionScale, cell.pos.Y* PositionScale, cell.pos.Z* PositionScale);
                    quiescent_cell_dict.Add(cell, quiescent);
                }
                else if (behavior_state == CellState.MigratoryTumorCell)
                {
                    Transform migratory = Instantiate(MigratoryPrefab);
                    migratory.localPosition = new Vector3(cell.pos.X * PositionScale, cell.pos.Y * PositionScale, cell.pos.Z * PositionScale);
                    migratory_dict.Add(cell, migratory);
                }
            }
        }
        Destroy_Transform(destroy_list, cell_dict);


    }

    void UpdateCellTransformsDict()
    {
        foreach (var item in EnvironmentLogic.ca.pos_cell_dict)
        {
            if (item.Value.behavior_state == CellState.Astrocyte && !astrocyte_dict.ContainsKey(item.Value))
            {
                Transform astrocyte_cell = Instantiate(AstrocytePrefab);
                astrocyte_cell.localPosition = new Vector3(item.Key.X* PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                astrocyte_dict.Add(item.Value, astrocyte_cell);
            }
            else if (item.Value.behavior_state == CellState.ProliferativeTumoralCell && !tumoral_cell_dict.ContainsKey(item.Value))
            {
                Transform tumoral_cell = Instantiate(ProliferativeTumoralCellPrefab);
                tumoral_cell.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                tumoral_cell_dict.Add(item.Value, tumoral_cell);
            }
            else if (item.Value.behavior_state == CellState.Neuron && !neuron_dict.ContainsKey(item.Value))
            {
                Transform neuron = Instantiate(NeuronPrefab);
                neuron.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                neuron_dict.Add(item.Value, neuron);
            }
            else if (item.Value.behavior_state == CellState.StemCell && !stem_cell_dict.ContainsKey(item.Value))
            {
                Transform stemCell = Instantiate(StemCellPrefab);
                stemCell.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                stem_cell_dict.Add(item.Value, stemCell);
            }
            else if(item.Value.behavior_state == CellState.MigratoryTumorCell && !migratory_dict.ContainsKey(item.Value))
            {
                Transform migratory_cell = Instantiate(MigratoryPrefab);
                migratory_cell.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                migratory_dict.Add(item.Value, migratory_cell);
            }
            else if(item.Value.behavior_state == CellState.NecroticTumorCell && !necrotic_cell_dict.ContainsKey(item.Value))
            {
                Transform necrotic = Instantiate(NecroticTumoralCellPrefab);
                necrotic.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                necrotic_cell_dict.Add(item.Value, necrotic);
            }
            else if(item.Value.behavior_state == CellState.QuiescentTumorCell && !quiescent_cell_dict.ContainsKey(item.Value))
            {
                Transform quiescent = Instantiate(QuiescentTumorCellPrefab);
                quiescent.localPosition = new Vector3(item.Key.X * PositionScale, item.Key.Y * PositionScale, item.Key.Z * PositionScale);
                quiescent_cell_dict.Add(item.Value, quiescent);
            }
        }
    }
}
