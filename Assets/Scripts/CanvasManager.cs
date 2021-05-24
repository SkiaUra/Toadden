using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public class CanvasManager : MonoBehaviour {
    [Required]
    public BattleManager battleManager;

    public Camera mainCam;

    public RectTransform healthBarGroup;

    [Required]
    public Button buttonStartBattle;
    [Required]
    public Button buttonRestart;

    [Required]
    public Button buttonSwapFront, buttonSwapBack;

    // Start is called before the first frame update
    void Start() {
        buttonStartBattle
            .onClick
            .AddListener(delegate () {
                Debug.Log("Start the battle");
                battleManager.StartBattle();
                // disable UI
                buttonSwapBack.gameObject.SetActive(false);
                buttonSwapFront.gameObject.SetActive(false);
                buttonStartBattle.interactable = false;
            });
        buttonRestart
            .onClick
            .AddListener(delegate () {
                Debug.Log("Restart the battle");
                battleManager.SetupBattle();
                // buttonSwapBack.gameObject.SetActive(true);
                // buttonSwapFront.gameObject.SetActive(true);
                // buttonStartBattle.interactable = true;
                // buttonRestart.interactable = false;
            });
        buttonRestart.interactable = false;

        buttonSwapFront
        .onClick
        .AddListener(delegate () {
            battleManager.formationManager.SwitchFormations(FormationPlaces.MIDLANE, FormationPlaces.FRONTLANE);
        });

        buttonSwapBack
        .onClick
        .AddListener(delegate () {
            battleManager.formationManager.SwitchFormations(FormationPlaces.MIDLANE, FormationPlaces.BACKLANE);
        });
    }
}
