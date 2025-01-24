using System;
using UnityEngine;

enum Players
{
    None,
    PLAYER_ONE,
    PLAYER_TWO,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] Players _player = Players.None;
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

    #region -- TouchConfig --

    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private Vector2 touchDeltaPosition;
    private bool isDragging = false;
    #endregion



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

#if PLATFORM_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenWidth = Screen.width;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:

                    if (touch.position.x < screenWidth * .2f && _player == Players.PLAYER_ONE)
                    {
                        touchEndPosition = touch.position;
                        touchDeltaPosition = touchEndPosition - touchStartPosition;
                        //_playerPos = new Vector2(transform.position.x, transform.position.y + touchDeltaPosition.y * Time.deltaTime * _speed);  
                        _playerPos = new Vector2(transform.position.x, transform.position.y + _speed * touchDeltaPosition.y * Time.deltaTime);
                    }

                    if (touch.position.x > screenWidth * .8f && _player == Players.PLAYER_TWO)
                    {
                        touchEndPosition = touch.position;
                        touchDeltaPosition = touchEndPosition - touchStartPosition;
                        //_playerPos = new Vector2(transform.position.x, transform.position.y + touchDeltaPosition.y * Time.deltaTime * _speed);
                        _playerPos = new Vector2(transform.position.x, transform.position.y + _speed * touchDeltaPosition.y * Time.deltaTime);
                    }
                    touchStartPosition = touchEndPosition;
                    break;
                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
#endif

        _playerPos.y = Mathf.Clamp(_playerPos.y, _minHeight, _maxHeight);

#if PLATFORM_ANDROID
        }
#endif
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
