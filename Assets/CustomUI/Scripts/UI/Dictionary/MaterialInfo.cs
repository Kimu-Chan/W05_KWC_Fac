using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

public class MaterialInfo : MonoBehaviour {
    public Image materialImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI qtyText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetMaterialInfo(DictionaryManager dictionaryManager, JsonData data, string unit="ea") {
        materialImage.sprite = dictionaryManager.GetImageSprite(data["type"].ToString(), int.Parse(data["id"].ToString()));

        nameText.text = data["material"].ToString();
        qtyText.text = "" + data["qty"].ToString() + " " + unit;
    }
}
