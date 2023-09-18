using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CategoryButton : MonoBehaviour
{
    private RectTransform _rectTransform;
    private TextMeshProUGUI _buttonText;
    private Button _button;
    
    private CategoryType _category; 

    public delegate void OnCategorySelected(CategoryType category);

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitButton(string name, CategoryType category, float xPos) {
        _buttonText.text = name;
        this._category = category;

        _rectTransform.anchoredPosition = new Vector2(xPos, 0);
    }

    public void RegisterOnClickCallback(OnCategorySelected callback) {
        _button.onClick.AddListener(() => callback(_category));
    }
}
