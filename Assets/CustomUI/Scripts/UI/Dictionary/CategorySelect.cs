using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CategorySelect : MonoBehaviour {
    public CategoryButton categoryButtonPrefab;
   
    private Dictionary<CategoryType, CategoryButton> _buttonDict = new Dictionary<CategoryType, CategoryButton>();
    private CategoryType _currentCat = CategoryType.NONE;

    void Awake() {
        _currentCat = CategoryType.NONE;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void AddButton(CategoryType category, string name=null) {
        CategoryButton instance = Instantiate<CategoryButton>(categoryButtonPrefab);
        instance.transform.SetParent(transform);

        instance.InitButton(name==null ? Enum.GetName(typeof(CategoryType), category) : name, category, 210 * _buttonDict.Count + 10);
        _buttonDict.Add(category, instance);
    }

    public void RegisterOnSelectCallback(CategoryButton.OnCategorySelected callback) {
        foreach (var button in _buttonDict) {
            button.Value.RegisterOnClickCallback(delegate(CategoryType c) {
                if (c == _currentCat) {
                    _currentCat = CategoryType.NONE;
                    callback(CategoryType.NONE);
                } else {
                    _currentCat = c;
                    callback(c);
                }
            });
        }
    }
}
