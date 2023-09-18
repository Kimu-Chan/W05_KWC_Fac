using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using LitJson;

public class DictionaryManager : MonoBehaviour {
    public CategorySelect categorySelect;
    public DictionaryData dictionaryData;
    public DictionaryItemList dictionaryItemList;
    public ItemInfo itemInfo;
    public LoadingPanel loadingPanel;

    private CategoryType _selectedCat;
    private int _selectedItemIdx;

    private Dictionary<string, Sprite> _imageSpriteList;

    void Awake() {
        if (categorySelect == null) {
            categorySelect = GetComponentInChildren<CategorySelect>();
        }

        if (dictionaryItemList == null) {
            dictionaryItemList = GetComponentInChildren<DictionaryItemList>();
        }

        if (itemInfo == null) {
            itemInfo = GetComponentInChildren<ItemInfo>();
        }

        if (loadingPanel == null) {
            loadingPanel = GetComponentInChildren<LoadingPanel>();
        }

        if (dictionaryData == null) {
            dictionaryData = GetComponent<DictionaryData>();
        }

        _imageSpriteList = new Dictionary<string, Sprite>();
    }
    
    // Start is called before the first frame update
    void Start() {
        var categories = Enum.GetValues(typeof(CategoryType));

        foreach (CategoryType c in categories) {
            if (c != CategoryType.NONE) {
                categorySelect.AddButton(c);
            }
        }
        categorySelect.RegisterOnSelectCallback(OnCategorySelected);

        // 코루틴 이상하게 쓰고 있음
        bool loadingFinished = false;
        StartCoroutine(loadingPanel.ShowLoadPanelUntil(() => {return loadingFinished;}));
        foreach (CategoryType c in categories) {
            if (c != CategoryType.NONE) {
                dictionaryData.LoadData(c, "Json/" + Enum.GetName(typeof(CategoryType), c));
            }
        }
        loadingFinished = true;

        itemInfo.InitInfo();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnCategorySelected(CategoryType category) {
        itemInfo.InitInfo();

        if (category == CategoryType.NONE) {
            dictionaryItemList.SetDefault();
        } else {
            // 코루틴 이상하게 쓰고 있음
            bool loadingFinished = false;
            StartCoroutine(loadingPanel.ShowLoadPanelUntil(() => {return loadingFinished;}));
            dictionaryItemList.SetList(category, dictionaryData.Data(category), OnItemSelected, this);
            loadingFinished = true;
        }

        _selectedCat = category;
    }

    private void OnItemSelected(int idx) {
        JsonData data = dictionaryData.Data(_selectedCat)[idx];

        string categoryName = Enum.GetName(typeof(CategoryType), _selectedCat);

        itemInfo.InitInfo();
        itemInfo
            .SetImage(this.GetImageSprite(categoryName, idx))
            .SetAbstractText(data["abstract"].ToString())
            .SetTitle(data["name"].ToString());

        if (data.ContainsKey("materials")) {
            itemInfo.SetMaterialInfo(data["materials"], this);
        }

        if (data.ContainsKey("generator")) {
            itemInfo.SetGeneratorInfo(data["generator"], this);
        }

        if (data.ContainsKey("details")) {
            itemInfo.SetDetailInfo(data["details"]);
        }
    }

    public Sprite GetImageSprite(CategoryType category, int id) {
        JsonData data = dictionaryData.Data(category)[id];

        string categoryName = Enum.GetName(typeof(CategoryType), category);
        string imagePath = "Sprites/" + Enum.GetName(typeof(CategoryType), category) + "/" + data["name"].ToString();
        
        if (!_imageSpriteList.ContainsKey(categoryName + id)) {
            _imageSpriteList.Add(categoryName + id, Resources.Load<Sprite>(imagePath));
        }
        
        return _imageSpriteList[categoryName + id];
    }

    public Sprite GetImageSprite(string categoryName, int id) {
        CategoryType category = (CategoryType)Enum.Parse(typeof(CategoryType), categoryName);

        return this.GetImageSprite(category, id);
    }
}
