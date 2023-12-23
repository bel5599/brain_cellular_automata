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

	float time =0;
	float crecimiento = 0;

	float proliferatives_count, necrotic_count, migratory_count, quiescent_count, stem_count, neuron_count, astrocyte_count, endothelial_count,
		vessel_count, neo_vessel = 0;

	public void Update () {
		
		float frameDuration = Time.unscaledDeltaTime;

		

		frames += 1;
		duration += frameDuration;

		//if (frameDuration < bestDuration) {
		//	bestDuration = frameDuration;
		//}
		//if (frameDuration > worstDuration) {
		//	worstDuration = frameDuration;
		//}

		//if (duration >= sampleDuration) {
		//if (displayMode == DisplayMode.FPS) {
		//	display.SetText(
		//		"FPS\n{0:0}\n{1:0}\n{2:0}",
		//		1f / bestDuration,
		//		frames / duration,
		//		1f / worstDuration
		//	);
		//}
		//else {
		//display.SetText(
		//	"Time\n{0:0} Verhuslt Growth\n{1:0} Proliferative Cells\n{2:0} Quiescent Cells\n{3:0} Migratory Cells\n{4:0} Necrotic Cells\n{5:0}"
		//	+ "Neuron Cells\n{6:0} Stem Cells\n{7:0}"+ "Astrocyte Cells\n{8:0}" + "Endothelial Cells\n{9:0}" + "Blood Vessels \n{10:0}" + "Neo Blood Vessels Cells\n{11:0}",
		//	time, crecimiento, proliferatives_count, quiescent_count, migratory_count,necrotic_count, neuron_count, stem_count, neuron_count, astrocyte_count, endothelial_count,
		//	vessel_count, neo_vessel
		//);
		string format = "Time:"+ time+ "\nVerhuslt Growth:" +crecimiento+ " \n Proliferative Cells(dark red sphere):" + proliferatives_count+ " \n Quiescent Cells(dark gray sphere):" +quiescent_count+ " \n Migratory Cells(purple sphere):" + migratory_count +
			" \n Necrotic Cells(black sphere):" +necrotic_count+ "\n Neuron Cells(blue sphere): " +neuron_count+ " \n Stem Cells(red sphere): "+stem_count+" \n Astrocyte Cells(green sphere): "+ astrocyte_count+ " \n Endothelial Cells(pink sphere): "+endothelial_count+ " \n Blood Vessels: "+vessel_count+ " \n Neo Blood Vessels Cells: "+neo_vessel;
		////display.Text(
		//	"Time:{0:0} \n Verhuslt Growth: {1:0} \n Proliferative Cells: {2:0} \n Quiescent Cells{3:0} \n Migratory Cells: {4:0}", time, crecimiento, proliferatives_count, quiescent_count, migratory_count, necrotic_count);
		//display.SetText(
		//    "Time:{0:0} \n Verhuslt Growth: {1:0} \n Proliferative Cells: {2:0} \n Quiescent Cells{3:0} \n Migratory Cells: {4:0} \n Necrotic Cells: { 5:0} \n Neuron Cells: {6:0} \n Stem Cells: {7:0} \n Astrocyte Cells: {8:0} \n " +
		//    "Endothelial Cells: {9:0} \n Blood Vessels: {10:0} \n Neo Blood Vessels Cells: {11:0}", time, crecimiento, proliferatives_count, quiescent_count, migratory_count, necrotic_count, neuron_count, stem_count, astrocyte_count, endothelial_count, vessel_count, neo_vessel);
		////display.SetText(
		display.SetText(format);
		////    ,

		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);
		//display.SetText(
		//	,


		//);

		//}


	}
	public void UpdateCanvas(CellularAutomaton automata)
    {
		time = (float)automata.tumor.time;
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
