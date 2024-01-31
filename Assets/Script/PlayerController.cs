using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed;

    [SerializeField] float _maxHeight, _minHeight;

    float _getInput;
    Vector3 _playerPos;

    int _direction;

    [SerializeField] string _axisName;

    void Update()
    {
        _getInput = Input.GetAxisRaw(_axisName);

        _direction =(int) _getInput;

        _playerPos = transform.position + Vector3.up * _getInput * _speed * Time.deltaTime;
        _playerPos.y = Mathf.Clamp(_playerPos.y, _minHeight, _maxHeight);

        transform.position = _playerPos;
    }

    public int GetDirection()
    {
        return _direction;
    }
}
