using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadingPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowLoadPanel() {
        gameObject.SetActive(true);
    }

    public void HideLoadPanel() {
        gameObject.SetActive(false);
    }

    public IEnumerator ShowLoadPanelUntil(Func<bool> deactivateIf) {
        gameObject.SetActive(true);
        yield return new WaitUntil(deactivateIf);
        gameObject.SetActive(false);
    }
}
