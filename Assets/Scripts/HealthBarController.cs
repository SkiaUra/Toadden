using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class HealthBarController : MonoBehaviour {
    public LivingEntity connectedEntity;
    public RectTransform _rectTransform;
    public CanvasManager canvasManager;
    public Vector3 BarOffset;

    [BoxGroup("Prefab Configs")]
    public List<Image> Hearts = new List<Image>();

    [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]
    public void RefreshVisuals(int _healthCurrent, int _healthMax) {
        // update Hearts
        for (int i = 0; i < Hearts.Count; i++) {
            if (i < _healthMax) // coeur actif
            {
                Hearts[i].gameObject.SetActive(true);

                if (i < _healthCurrent) // coeur vivant => on cache rien
                {
                    Hearts[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                } else // coeur perdu => on cache le coeur, on active le conteneur 
                  {
                    Hearts[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
            } else // coeur Inactif => Cacher tout
              {
                Hearts[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdatePosition(Vector3 _position) {
        _rectTransform.position = Camera.main.WorldToScreenPoint(_position + BarOffset);
    }
}
