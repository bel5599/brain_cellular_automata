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
    }



    void Update()
    {
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

            script.UpdateCanvas(EnvironmentLogic.ca);
        }



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


    Dictionary<Pos, List< Vector3>> ajuste;


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
        foreach (var item in EnvironmentLogic.ca.vessel_segment_list)
        {
            var vessel = Instantiate(ArteryPrefab);
            var artery1 = item.blood_vessel1;
            vessel.transform.localPosition = new Vector3(artery1.pos.X, artery1.pos.Y, artery1.pos.Z);
            vessel.transform.position = vessel.transform.localPosition * PositionScale;

            var vessel2 = Instantiate(ArteryPrefab);
            var artery2 = item.blood_vessel2;
            vessel2.transform.localPosition = new Vector3(artery2.pos.X, artery2.pos.Y, artery2.pos.Z);
            vessel2.transform.position = vessel2.transform.localPosition * PositionScale;

            var vaso1 = Instantiate(VasoPrefb);
            var script = vaso1.GetComponent<CylinderController>();
            script.point1 = vessel;
            script.point2 = vessel2;

            //Scale to better look
            vaso1.transform.localScale = new Vector3(0.2f, vaso1.transform.localScale.y, 0.2f);
            script.point1.transform.localScale *= 0.2f;
            script.point2.transform.localScale *= 0.2f;


        }
    }


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
