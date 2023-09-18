using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DetailInfo : MonoBehaviour {
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI detailText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetDetailInfo(string title, string detail) {
        titleText.text = title;
        detailText.text = detail;
    }
}
