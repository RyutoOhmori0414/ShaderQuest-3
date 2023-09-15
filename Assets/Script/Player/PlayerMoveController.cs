using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveController : MonoBehaviour
{
    [SerializeField, Tooltip("Playerの通常移動スピード")]
    private float _walkSpeed = 5F;

    [SerializeField, Tooltip("Playerの走るスピード")]
    private float _runSpeed = 10F;
    
    /// <summary>移動が終わった際に建てるフラグ</summary>
    private bool _moveCanceled = false;

    /// <summary>現在のスピードを監視</summary>
    private ReactiveProperty<float> _currentSpeedReactiveProperty = new ReactiveProperty<float>(0.0F);

    /// <summary>スピードの値を監視</summary>
    public IReadOnlyReactiveProperty<float> CurrentSpeedReactiveProperty => _currentSpeedReactiveProperty;

    /// <summary>走っているかどうかを監視する</summary>
    private ReactiveProperty<bool> _isRunningReactiveProperty = new ReactiveProperty<bool>(false);

    /// <summary>走っているかどうかを監視</summary>
    public IReadOnlyReactiveProperty<bool> IsRunningReactiveProperty => _isRunningReactiveProperty;
    
    /// <summary>移動方向</summary>
    private Vector2 _moveVector = Vector2.zero;
    
    /// <summary>メインカメラ</summary>
    private Camera _mainCamera = default;

    private Rigidbody _rb = default;

    private void Awake()
    {
        var playerAction= GameManager.Instance.InputActionManager.PlayerController.PlayerControl;
        
        // 移動のためのパラメータを受け取る関数を登録している
        playerAction.Move.started += OnMoveInput;
        playerAction.Move.performed += OnMoveInput;
        playerAction.Move.canceled += OnMoveCanceled;

        playerAction.Run.started += OnRunStart;
        playerAction.Run.canceled += OnRunEnd;

        _rb = GetComponent<Rigidbody>();
        
        _mainCamera = Camera.main;
    }
    
    /// <summary>InputActionに登録する関数</summary>
    /// <param name="context">コールバック</param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }

    /// <summary>移動を終えた際に呼ばれる関数</summary>
    /// <param name="context">コールバック</param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMoveInput(context);

        _moveCanceled = true;
    }

    /// <summary>走る入力が行われた際に呼ばれる関数</summary>
    /// <param name="context">コールバック</param>
    private void OnRunStart(InputAction.CallbackContext context)
    {
        _isRunningReactiveProperty.Value = true;
    }

    /// <summary>走る入力が終わった際に呼ばれる関数</summary>
    /// <param name="context">コールバック</param>
    private void OnRunEnd(InputAction.CallbackContext context)
    {
        _isRunningReactiveProperty.Value = false;
    }
    
    private void FixedUpdate()
    {
        if (_moveVector != Vector2.zero)
        {
            var moveDir = _mainCamera.transform.TransformDirection(new Vector3(_moveVector.x, 0.0F, _moveVector.y));
            moveDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;
            
            // 方向転換
            this.transform.rotation = Quaternion.LookRotation(moveDir);

            if (_isRunningReactiveProperty.Value)
            {
                Move(_runSpeed);
            } // 走っているときの処理
            else
            {
                Move(_walkSpeed);
            } // 歩いているときの処理
        }
        else if (_moveCanceled)
        {
            // ストップ
            _rb.velocity = new Vector3(0.0F, _rb.velocity.y, 0.0F);
        }
    }

    /// <summary>移動処理</summary>
    /// <param name="speed">スピード</param>
    private void Move(float speed)
    {
        // 移動
        var playerForward = this.transform.forward * speed;
        _rb.velocity = new Vector3(playerForward.x, _rb.velocity.y, playerForward.z);

        // Animationに使用する値の更新
        _currentSpeedReactiveProperty.Value = speed;
    }
}
