using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using TMPro;
using LitJson;

public class ListItem : MonoBehaviour
{
    public Image _itemImage;

    private TextMeshProUGUI _itemNameText;
    private Button _itemButton;

    private JsonData _itemData;
    private CategoryType _category;

    public delegate void OnItemSelected(int idx);

    void Awake() {
        _itemNameText = GetComponentInChildren<TextMeshProUGUI>();
        _itemButton = GetComponent<Button>();

        if (_itemImage == null) {
            _itemImage = GetComponentInChildren<Image>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitListItem(CategoryType category, JsonData data, int idx, OnItemSelected callback, DictionaryManager dictionaryManager) {
        _itemData = data;
        _category = category;

        _itemNameText.text = data["name"].ToString();
        
        _itemImage.sprite = dictionaryManager.GetImageSprite(category, idx);

        _itemButton.onClick.AddListener(() => callback(idx));
    }
}
