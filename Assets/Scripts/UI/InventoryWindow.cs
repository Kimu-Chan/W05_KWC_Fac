using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : WindowUIManager
{
    public List<InventoryItemUI> itemUIs;
    public Dictionary<ResourceType, InventoryItemUI> itemUIRefs;
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    public void UpdateInventory()
    {

    }
}
