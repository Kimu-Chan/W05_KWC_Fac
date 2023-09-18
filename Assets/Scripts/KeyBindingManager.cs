using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public enum KeyAction
{

}

public class KeyBindingManager : MonoBehaviour
{
    public Dictionary<KeyAction, KeyCode> keyCodes = new Dictionary<KeyAction, KeyCode>();
    public Dictionary<KeyAction, Action> callbacks = new Dictionary<KeyAction, Action>();

    // private void Update()
    // {
    //     foreach (var)
    // }



}