using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

public class GeneratorInfo : MonoBehaviour
{
    public Image generatorImage;
    public TextMeshProUGUI generatorText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetGeneratorInfo(DictionaryManager dictionaryManager, JsonData data) {
        generatorImage.sprite = dictionaryManager.GetImageSprite(data["type"].ToString(), int.Parse(data["id"].ToString()));

        generatorText.text = data["name"].ToString();
    }
}
