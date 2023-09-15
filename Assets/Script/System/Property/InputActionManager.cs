using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputActionManager
{
    private PlayerController _playerController = new PlayerController();

    public PlayerController PlayerController => _playerController;

    public InputActionManager()
    {
        _playerController.Enable();
    }
}
