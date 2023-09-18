using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class DictionaryData : MonoBehaviour {
    private Dictionary<CategoryType, JsonData> dictionaryData = new Dictionary<CategoryType, JsonData>();

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public DictionaryData LoadData(CategoryType category, string filePath) {
        TextAsset dataFile = Resources.Load<TextAsset>(filePath);
        JsonData data = JsonMapper.ToObject(dataFile.text);

        if (dictionaryData.ContainsKey(category)) {
            dictionaryData[category] = data;
        } else {
            dictionaryData.Add(category, data);
        }
        
        return this;
    }

    public JsonData Data(CategoryType category) {
        return dictionaryData[category];
    }
}
