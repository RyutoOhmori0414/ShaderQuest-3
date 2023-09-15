using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField, Tooltip("AnimatorのスピードParameter名")]
    private string _speedParamName = "Speed";
    [SerializeField, Tooltip("AnimatorのスピードParameter名")]
    private string _runParamName = "Run";

    private int _speedParamID = default;
    private int _runParamID = default;
    
    private Animator _animator = default;
    private PlayerMoveController _playerMoveController = default;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMoveController = GetComponent<PlayerMoveController>();

        // Hashをキャッシュしておく
        _speedParamID = Animator.StringToHash(_speedParamName);
        _runParamID = Animator.StringToHash(_runParamName);
        
        
    }
}
