using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class InputManager : MonoBehaviour {
    public BattleManager battleManager;

    [Header("Input Properties")]
    public bool allowInput = true;
    public float ClickDuration = 0.4f;
    bool clicking = false;
    float totalDownTime = 0;
    int clickNumber = 0;

    public LivingEntity selectedEntity;

    async UniTask Update() {
        // ** INPUT SYSTEM ** //
        if (Input.GetMouseButtonDown(0) && allowInput == true) {// Detect the first click
            clickNumber++;
            totalDownTime = 0;
            clicking = true;
        }

        if (clicking && Input.GetMouseButton(0) && allowInput == true) {// If a first click detected, and still clicking, measure the total click time and fire an event if we exceed the duration specified
            // await LongPress();
        }

        if (clicking && Input.GetMouseButtonUp(0) && allowInput == true) {// If a first click detected, and we release before the duration, do a normal click
            await NormalClick();
        }
    }

    async UniTask NormalClick() {
        Debug.Log("Normal Click");

        LivingEntity clickedTarget = RaycastEntity();
        if (clickedTarget == null || clickedTarget.entityType == EntityType.ENEMY) return;
        if (selectedEntity == null) {
            // Add new memory
            selectedEntity = clickedTarget;
        } else {
            if (clickedTarget == selectedEntity) {
                // Clear selected
                selectedEntity = null;
            }
            if (clickedTarget != selectedEntity) {
                // Swap les deux
                battleManager.formationManager.SwitchEntities(clickedTarget, selectedEntity);
                selectedEntity = null;
            }
        }
    }

    public LivingEntity RaycastEntity() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)) {
            Debug.Log("You raycast on :" + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponentInParent<LivingEntity>()) return hit.collider.gameObject.GetComponent<LivingEntity>();
        }
        return null;
    }
}
