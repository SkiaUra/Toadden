using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public class CanvasManager : MonoBehaviour
{
	[Required]
	public BattleManager battleManager;

	public Camera mainCam;

	public RectTransform healthBarGroup;

	[Required]
	public Button buttonStartBattle;
	[Required]
	public Button buttonRestart;

	// Start is called before the first frame update
	void Start()
	{
		buttonStartBattle
			.onClick
			.AddListener(delegate ()
			{
				Debug.Log("Start the battle");
				battleManager.StartBattle();
				buttonStartBattle.interactable = false;
			});
		buttonRestart
			.onClick
			.AddListener(delegate ()
			{
				Debug.Log("Restart the battle");
				battleManager.SetupBattle();
				buttonStartBattle.interactable = true;
				buttonRestart.interactable = false;
			});
		buttonRestart.interactable = false;
	}
}
