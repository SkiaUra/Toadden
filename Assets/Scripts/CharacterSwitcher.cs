using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSwitcher : MonoBehaviour {
    public CanvasManager canvasManager;
    public Button switcherButtonRef;

    public float openingSpeed, closingSpeed;
    public Vector3 offset;

    void Start() {
        this.gameObject.SetActive(false);
        SetupSwitcher();
    }

    public void SetupSwitcher() {
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }

        var gameProperties = canvasManager.battleManager.gameProperties;
        for (int i = 0; i < gameProperties.creatureAssets.Length; i++) {
            SwitcherButtonController buttonCtrl = Instantiate(switcherButtonRef, this.transform).GetComponent<SwitcherButtonController>();
            buttonCtrl.characterSwitcher = this;
            buttonCtrl.SetupButton(gameProperties.creatureAssets[i].species);
        }
    }

    public void Show(LivingEntity _entity) {
        var rect = (RectTransform) transform;
        Vector3 pos = canvasManager.WorldSpaceToCanvasSpace(_entity.transform.position);
        this.gameObject.SetActive(true);
        rect.DOKill();
        rect.localScale = Vector3.zero;
        rect.position = pos + offset;

        gameObject.SetActive(true);
        rect.DOScale(Vector3.one, openingSpeed).SetEase(Ease.OutBack);
    }

    public void Hide() {
        var rect = (RectTransform) transform;
        rect.DOKill();
        rect.DOScale(Vector3.zero, closingSpeed).SetEase(Ease.InBack).OnComplete(() => { this.gameObject.SetActive(false); });
    }
}