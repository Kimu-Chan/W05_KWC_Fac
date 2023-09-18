using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WindowUIManager : MonoBehaviour
{
    public string uiName;
    public Button closeButton;

    protected virtual void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseWindow);
        }
        CloseWindow();
        Debug.Assert(UIManager.Instance != null);
        Debug.Log($"UI add: {uiName}");

        UIManager.Instance.windows.Add(uiName, this);
    }

    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
        GlobalLocks.UnLock(ref GlobalLocks.uiLock);
    }

    public virtual bool TryOpenWindow()
    {
        if (GlobalLocks.TryLock(ref GlobalLocks.uiLock))
        {
            gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWindow();
        }
    }
}

public abstract class BuildingWindowUIManager : WindowUIManager
{
    public abstract bool OpenWindowCallback(FactoryBuilding factoryBuilding);
    public abstract void CloseWindowCallback();
    public bool TryOpenBuildingWindow(FactoryBuilding factoryBuilding)
    {
        if (GlobalLocks.TryLock(ref GlobalLocks.uiLock))
        {
            OpenWindowCallback(factoryBuilding);
            gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        CloseWindowCallback();
    }
    public override bool TryOpenWindow()
    {
        return false;
    }
}