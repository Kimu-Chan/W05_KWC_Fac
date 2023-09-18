using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

public class ItemInfo : MonoBehaviour {
    public Image itemImage;
    public TextMeshProUGUI itemTitleText;
    public TextMeshProUGUI itemAbstractText;
    public Transform detailTransform;

    public MaterialListInfo materialInfoPrefab;
    public GeneratorListInfo generatorInfoPrefab;
    public DetailInfo detailInfoPrefab;

    public string defaultAbstract;
    public Sprite defaultImageSprite;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void InitInfo() {
        this.SetAbstractText(defaultAbstract)
            .SetImage(defaultImageSprite)
            .SetTitle("-");

        Transform[] detailInfoChildren = detailTransform.gameObject.GetComponentsInChildren<Transform>();

        if (detailInfoChildren != null && detailInfoChildren.Length > 1) {
            for (int i = 1; i < detailInfoChildren.Length; i++) {
                if (detailInfoChildren[i] != detailTransform) {
                    Destroy(detailInfoChildren[i].gameObject);
                }
            }
        }
    }   

    public ItemInfo SetTitle(string title) {
        itemTitleText.text = title;

        return this;
    }

    public ItemInfo SetImage(Sprite sprite) {
        itemImage.sprite = sprite;

        return this;
    }

    public ItemInfo SetAbstractText(string text) {
        itemAbstractText.text = text;

        return this;
    }

    public ItemInfo SetMaterialInfo(JsonData data, DictionaryManager dictionaryManager) {
        MaterialListInfo instance = Instantiate<MaterialListInfo>(materialInfoPrefab);
        instance.transform.SetParent(detailTransform);
        //instance.transform.SetSiblingIndex(0);

        instance.SetMaterialList(data, dictionaryManager);
        
        return this;
    }

    public ItemInfo SetGeneratorInfo(JsonData data, DictionaryManager dictionaryManager) {
        GeneratorListInfo instance = Instantiate<GeneratorListInfo>(generatorInfoPrefab);
        instance.transform.SetParent(detailTransform);
        //instance.transform.SetSiblingIndex(0);

        instance.SetGeneratorList(data, dictionaryManager);
        
        return this;
    }

    public ItemInfo SetDetailInfo(JsonData data) {
        foreach (JsonData d in data as IList) {
            DetailInfo instance = Instantiate<DetailInfo>(detailInfoPrefab);
            instance.transform.SetParent(detailTransform);

            instance.SetDetailInfo(d["title"].ToString(), d["detail"].ToString());
        }

        return this;
    }
}
