using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class GeneratorListInfo : MonoBehaviour
{
    public GeneratorInfo generatorInfoPrefab;
    public Transform generatorListTransform;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetGeneratorList(JsonData data, DictionaryManager dictionaryManager) {
        foreach (JsonData d in data as IList) {
            GeneratorInfo instance = Instantiate<GeneratorInfo>(generatorInfoPrefab);
            instance.transform.SetParent(generatorListTransform);

            instance.SetGeneratorInfo(dictionaryManager, d);
        }
    }

    public void InitList() {
        Transform[] materialListChildren = generatorListTransform.gameObject.GetComponentsInChildren<Transform>();

        if (materialListChildren != null && materialListChildren.Length > 1) {
            for (int i = 1; i < materialListChildren.Length; i++) {
                if (materialListChildren[i] != generatorListTransform) {
                    Destroy(materialListChildren[i].gameObject);
                }
            }
        }
    }
}
