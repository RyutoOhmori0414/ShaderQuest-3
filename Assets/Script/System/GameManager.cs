using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    /// <summary>インスタンス</summary>
    private static GameManager _instance = null;

    /// <summary>シングルトンのInstance</summary>
    public static GameManager Instance
    {
        get
        {
            _instance ??= new GameManager();
            return _instance;
        }
    }
    
    // メンバー
    private InputActionManager _inputActionManager = new InputActionManager();
    
    // プロパティ
    public InputActionManager InputActionManager => _inputActionManager;
}
