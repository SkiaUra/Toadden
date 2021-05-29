using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SwitcherButtonController : MonoBehaviour {

    public CreatureSpecies species;
    public CharacterSwitcher characterSwitcher;

    [Header("Bindings")]
    [Required]
    public Image image;
    [Required]
    public Button button;

    public void SetupButton(CreatureSpecies _species) {
        // setup the new button
        species = _species;
        image.sprite = characterSwitcher.canvasManager.battleManager.gameProperties.GetCreatureSprite(_species);
        button
        .onClick
        .AddListener(delegate () {
            var selectedEntity = characterSwitcher.canvasManager.battleManager.inputManager.selectedEntity;
            characterSwitcher.canvasManager.battleManager.SpawnNewCreatureOnBattlePos(species, selectedEntity.linkedPos);
        });
    }
}
