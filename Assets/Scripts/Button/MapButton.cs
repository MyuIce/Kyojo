using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapButton : MonoBehaviour
{
    [SerializeField] public GameObject MapCanvas;

    void Start()
    {
        MapCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool nextState = !MapCanvas.activeSelf;
            MapCanvas.SetActive(nextState);

            // マップ（プロフィール）を開いたとき、最初の人物を表示するようにリセット
            if (nextState)
            {
                ProfileUIManager profileUI = MapCanvas.GetComponentInChildren<ProfileUIManager>(true);
                if (profileUI != null)
                {
                    profileUI.UpdateProfileWithReset();
                }
            }
        }
    }
}
