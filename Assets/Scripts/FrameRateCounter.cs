using UnityEngine;
using TMPro;
using AutomataCelularLogic;

public class FrameRateCounter : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI display;

	public enum DisplayMode { FPS, MS }

	[SerializeField]
	DisplayMode displayMode = DisplayMode.FPS;

	[SerializeField, Range(0.1f, 2f)]
	float sampleDuration = 1f;

	int frames;

	float duration, bestDuration = float.MaxValue, worstDuration;

	float time, simulationStep = 0;
	float crecimiento = 0;



	float proliferatives_count, necrotic_count, migratory_count, quiescent_count, stem_count, neuron_count, astrocyte_count, endothelial_count,
		vessel_count, neo_vessel = 0;

	public void Update () {
		
		float frameDuration = Time.unscaledDeltaTime;

		

		frames += 1;
		duration += frameDuration;

		string format = "Gliobastoma Multiforme \n SimulationStep: "+simulationStep+ "\n Time:"+ time+ "\nVerhuslt Growth:" +crecimiento+ " \n Proliferative Cells <color=#8B0000>■</color>:" + proliferatives_count+ " \n Quiescent Cells <color=\"yellow\">■</color>:" + quiescent_count+ " \n Migratory Cells <color=\"purple\">■</color>:" + migratory_count +
			" \n Necrotic Cells <color=\"black\">■</color>:" + necrotic_count+ "\n Neuron Cells <color=\"blue\">■</color>: " + neuron_count+ " \n Stem Cells <color=\"orange\">■</color>: " + stem_count+ " \n Astrocyte Cells <color=#32CD32>■</color>: " + astrocyte_count+ " \n Endothelial Cells <color=#FFC0CB>■</color>: " + endothelial_count+ " \n Blood Vessels <color=\"red\">■</color>: " + vessel_count+ " \n Neo Blood Vessels Cells <color=#FF6347>■</color>: " + neo_vessel;
		display.SetText(format);


	}
	public void UpdateCanvas(CellularAutomaton automata, int simulation_step)
    {
		time = (float)automata.tumor.time;
		simulationStep = (float)simulation_step;
		crecimiento = (float)automata.tumor.new_cells_count;
		proliferatives_count= (float)automata.proliferation_cells.Count;
		necrotic_count= (float)automata.necrotic_cell_list.Count;
		quiescent_count= (float)automata.quiescent_cell_list.Count;
		migratory_count= (float)automata.migratory_cells_actual.Count;
		stem_count = (float)automata.stem_cell_list.Count;
		neuron_count= (float)automata.neuron_list.Count;
		astrocyte_count= (float)automata.astrocyte_list.Count;

		endothelial_count = (float)automata.endothelial_cells.Count;


		neo_vessel = (float)automata.pos_neo_vessel_dict.Count;
		vessel_count = (float)automata.pos_vessel_dict.Count;


	}
}
