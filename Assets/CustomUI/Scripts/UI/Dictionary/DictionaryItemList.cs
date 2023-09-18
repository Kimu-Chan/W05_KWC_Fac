using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class DictionaryItemList : MonoBehaviour {
    public ListItem itemPrefab;
    
    public GameObject warnText;
    public Transform contentTransform;
    
    private bool _isShowingList = false;
    private List<ListItem> _itemList;

    void Awake() {
        _isShowingList = false;

        if (warnText == null) {
            warnText = transform.Find("Text When Null").gameObject;
        }
        warnText.SetActive(true);

        if (contentTransform == null) {
            contentTransform = transform.Find("Content");
        }

        _itemList = new List<ListItem>();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetList(CategoryType c, JsonData data, ListItem.OnItemSelected callback, DictionaryManager dictionaryManager) {
        if (_itemList.Count > 0) {
            foreach (var i in _itemList) {
                Destroy(i.gameObject);
            }
            _itemList = new List<ListItem>();
        }

        _isShowingList = true;
        warnText.SetActive(false);

        foreach (JsonData d in data as IList) {
            ListItem instance = Instantiate<ListItem>(itemPrefab);
            instance.transform.SetParent(contentTransform);
            _itemList.Add(instance);
            
            instance.InitListItem(c, d, _itemList.Count - 1, callback, dictionaryManager);
        }
    }

    public void SetDefault() {
        if (_itemList.Count > 0) {
            foreach (var i in _itemList) {
                Destroy(i.gameObject);
            }
            _itemList = new List<ListItem>();
        }

        _isShowingList = false;
        warnText.SetActive(true);
    }
}
