using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class MaterialListInfo : MonoBehaviour {
    public MaterialInfo materialInfoPrefab;
    public Transform materialListTransform;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetMaterialList(JsonData data, DictionaryManager dictionaryManager) {
        foreach (JsonData d in data as IList) {
            MaterialInfo instance = Instantiate<MaterialInfo>(materialInfoPrefab);
            instance.transform.SetParent(materialListTransform);

            instance.SetMaterialInfo(dictionaryManager, d);
        }
    }

    public void InitList() {
        Transform[] materialListChildren = materialListTransform.gameObject.GetComponentsInChildren<Transform>();

        if (materialListChildren != null && materialListChildren.Length > 1) {
            for (int i = 1; i < materialListChildren.Length; i++) {
                if (materialListChildren[i] != materialListTransform) {
                    Destroy(materialListChildren[i].gameObject);
                }
            }
        }
    }
}
