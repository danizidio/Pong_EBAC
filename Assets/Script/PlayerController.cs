using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public event Action OnDefinePlayerType;

    [SerializeField] int _playerNumber;

    [SerializeField] float _speed;

    float _iaSpeedVariation = 5f;

    [SerializeField] float _maxHeight, _minHeight;

    float _getInput;
    Vector3 _playerPos;
    Vector2 _iaPos;
    int _direction;

    GameObject _ball;

    [SerializeField] string _axisName;

    bool _cpu;
    public bool cpu { get { return _cpu; } set { _cpu = value; } }

    private void Start()
    {
        _ball = GameObject.FindGameObjectWithTag("BALL");
    }

    void Update()
    {
        OnDefinePlayerType?.Invoke();

        PlayerMove();
    }

    void PlayerMove()
    {
        if (!_cpu)
        {
            _getInput = Input.GetAxisRaw(_axisName);

            _direction = (int)_getInput;

            transform.position = _playerPos;
        }
    }

    public void PlayerCommands()
    {
        _playerPos = transform.position + Vector3.up * _getInput * _speed * Time.deltaTime;
        _playerPos.y = Mathf.Clamp(_playerPos.y, _minHeight, _maxHeight);
    }

    public void IACommands()
    {
        if (_ball != null)
        {
            float targetY = Mathf.Clamp(_ball.transform.position.y, _minHeight, _maxHeight);

            Vector2 targetPosition = new Vector2(transform.position.x, targetY);

            if (_playerNumber == 1 && _ball.transform.position.x < -.3f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * _iaSpeedVariation);
            }

            if (_playerNumber == 2 && _ball.transform.position.x > .3f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * _iaSpeedVariation);
            }
        }
    }

    public int GetDirection()
    {
        return _direction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("BALL"))
        {
            _iaSpeedVariation = UnityEngine.Random.Range(4, 6.5f);
        }
    }


    private void OnDisable()
    {
        OnDefinePlayerType = null;
    }
}
