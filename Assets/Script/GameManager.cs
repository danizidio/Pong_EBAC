using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Action<int, int> OnUpdatePoints;

    [SerializeField] EnumStates _currentState, _nextState;
    public EnumStates currentState { get { return _currentState; } }

    [Space(10)]

    [SerializeField] Transform _playerOnePaddle;
    [SerializeField] Transform _playerTwoPaddle;
    public Transform playerOnePaddle { get { return _playerOnePaddle; }}
    public Transform playerTwoPaddle { get { return _playerTwoPaddle; }}

    [SerializeField] Color[] _playerColor;

    [Space(10)]

    [SerializeField] BallController _ballController;

    [Space(10)]
    [SerializeField] List<VisualEffect> _confetFX;
    [Space(10)]

    [SerializeField] TMP_Text _context;
    [SerializeField] TMP_Text _lastWinner;
    [SerializeField] TMP_Text _playerOneText;
    [SerializeField] TMP_Text _playerTwoText;
    [SerializeField] TMP_Text _pressSomething;
    [SerializeField] GameObject _goalText;
    [SerializeField] Score_UI[] _scoreText;
    [SerializeField] TMP_InputField _inputTxtPlayerOne;
    [SerializeField] TMP_InputField _inputTxtPlayerTwo;

    [Space(10)]

    int scoreA, scoreB, _maxScore;
    int _playersReady, _howMany;

    string _playerOneName, _playerTwoName, _gameWinner;

    float t;

    [SerializeField] GameObject _gameAssets, _title, _menuScoreRule, _gameplayScore, _menuPlayers, _colorsOne, _colorsTwo, _menuPause, _clearData;

    private void Awake()
    {
        Application.targetFrameRate = 61;    
        Instance = this;
    }

    void Start()
    {

        if(PlayerPrefs.HasKey("LAST_WINNER") && PlayerPrefs.GetString("LAST_WINNER") != "")
        {
            _lastWinner.gameObject.SetActive(true);

            _lastWinner.text = "The Last Triumphant - " + PlayerPrefs.GetString("LAST_WINNER");

            _clearData.SetActive(true);
        }
        else
        {
            _lastWinner.gameObject.SetActive(false);

            _clearData.SetActive(false);
        }

#if PLATFORM_ANDROID
        _pressSomething.text = "Touch to Start";
#endif
#if PLATFORM_STANDALONE_WIN
                    _pressSomething.text = "Press Enter to Start";
#endif

        _menuScoreRule.SetActive(false);

        ChangeState(EnumStates.TITLE);
    }

    private void Update()
    {
        StateMachine(_currentState);

        _currentState = _nextState;
    }

    public EnumStates ChangeState(EnumStates state)
    {
        return _nextState = state;
    }

    void StateMachine(EnumStates state)
    {
        switch(state) 
        {
            case EnumStates.TITLE:
                {
                    _title.SetActive(true);
                    _gameAssets.SetActive(false);
                    _scoreText[0].gameObject.SetActive(false);
                    _scoreText[1].gameObject.SetActive(false);
                    _menuPlayers.SetActive(false);

                    _gameplayScore.SetActive(false);
                    _colorsOne.SetActive(false);
                    _colorsTwo.SetActive(false);

                    _playerOneName = _inputTxtPlayerOne.text;

                    _playerTwoName = _inputTxtPlayerTwo.text;

                    _context.text = "";

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        EnterMenu();
                    }

                    break;
                }
            case EnumStates.MENU:
                {
                    _lastWinner.gameObject.SetActive(false);
                    _title.SetActive(false);
                    _gameAssets.SetActive(false);

                    break;
                }
            case EnumStates.BEGIN:
                {
                    _gameAssets.SetActive(true);

                    _context.gameObject.SetActive(true);

#if PLATFORM_ANDROID
                    _context.text = "Touch to Start";
#endif
#if PLATFORM_STANDALONE_WIN
                    _context.text = "Press any Key to Start";
#endif
                    _scoreText[0].gameObject.SetActive(true);
                    _scoreText[1].gameObject.SetActive(true);

                    ResetGame();

                    if (_playerOnePaddle.GetComponent<PlayerController>().cpu && _playerTwoPaddle.GetComponent<PlayerController>().cpu)
                    {
                        t += Time.unscaledDeltaTime;
                    }

                    if (Input.anyKeyDown || t >= 1f)
                    {
                        t = 0;

                        _context.text = "";

                        _context.gameObject.SetActive(false);

                        ChangeState(EnumStates.GAMEPLAY);
                    }

                    Time.timeScale = 0;

                    break;
                }
            case EnumStates.GAMEPLAY:
                {
                    Time.timeScale = 1;

                    _menuPause.SetActive(false);
                    _gameplayScore.SetActive(true);
                    _context.gameObject.SetActive(false);
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ChangeState(EnumStates.PAUSE);
                    }

                    break;
                }
            case EnumStates.GOAL:
                {
                    t += Time.deltaTime;

                    _ballController.gameObject.SetActive(false);

                    if (t >= 3.5f)
                    {
                        t = 0;

                        if (scoreA >= _maxScore)
                        {
                            _gameWinner = _playerOneName;

                            ChangeState(EnumStates.ENDMATCH);
                        }
                        else if(scoreB >= _maxScore)
                        {
                            _gameWinner = _playerTwoName;

                            ChangeState(EnumStates.ENDMATCH);
                        }
                        else
                        {
                            ChangeState(EnumStates.BEGIN);
                        }
                    }
                    break;
                }
            case EnumStates.PAUSE:
                {
                    Time.timeScale = 0;

                    _menuPause.SetActive(true);
                    _context.gameObject.SetActive(true);
                    _gameplayScore.SetActive(false);

#if PLATFORM_STANDALONE_WIN
                    _context.text = "Press esc to Return to Game \n Press Enter to Return to Title Screen";
#endif

#if PLATFORM_ANDROID
                    _context.text = "Touch the pause button again to Return to Game \n Touch the X button to Return to the Title Screen";
#endif
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        _context.gameObject.SetActive(false);
                        _context.text = "";

                        ChangeState(EnumStates.GAMEPLAY);
                    }

                    if(Input.GetKeyDown(KeyCode.Return))
                    {
                        _context.text = "";

                        SceneManager.LoadScene("EBAC_Pong");
                    }

                    break;
                }
            case EnumStates.ENDMATCH:
                {
                    _context.gameObject.SetActive(true);

                    foreach(var v in _confetFX)
                    {
                        v.Play();
                    }

                    PlayerPrefs.SetString("LAST_WINNER", _gameWinner + " with a score: " + scoreA + " x " + scoreB);

#if PLATFORM_STANDALONE_WIN
                    _context.text = "Match is over! \n" + _gameWinner + " Wins!" + "\n press any key to return to title";
#endif

#if PLATFORM_ANDROID
                    _context.text = "Match is over! \n" + _gameWinner + " Wins!" + "\n Touch the screen to return to the title";
#endif

                    if (Input.anyKeyDown)
                    {
                        _context.text = "";

                        SceneManager.LoadScene("EBAC_Pong");
                    }

                    break;
                }
        } 
    }

    public void EnterMenu()
    {
        _menuScoreRule.SetActive(true);
        ChangeState(EnumStates.MENU);
    }

    void ResetGame()
    {
        _playerOnePaddle.position = new Vector3(-8f, 0f, 0f);
        _playerTwoPaddle.position = new Vector3(8f, 0f, 0f);

        if (!_ballController.gameObject.activeSelf)
        {
            _ballController.gameObject.SetActive(true);
        }
        _ballController.ResetBall();
    }

    void UpdatePoints(int a, int b)
    {
        scoreA = scoreA + a;
        scoreB = scoreB + b;

        if (a > 0)
        {
            _scoreText[0].GetComponent<Score_UI>().ScoreUpdate(scoreA);
        }

        if(b > 0)
        {
            _scoreText[1].GetComponent<Score_UI>().ScoreUpdate(scoreB);
        }

        _goalText.GetComponent<Animator>().SetTrigger("GOAL");

        ChangeState(EnumStates.GOAL);
    }

    public void SetMatchRule(int i)
    {
        _maxScore = i;

        _menuPlayers.SetActive(true);
        _menuScoreRule.SetActive(false);
    }

    public void SetMatchPlayer(int i)
    {
        _menuPlayers.SetActive(false);
        _menuScoreRule.SetActive(false);

        switch(i)
        {
            case 0:
                {
                    _playerOnePaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerOnePaddle.GetComponent<PlayerController>().PlayerCommands;

                    _playerOnePaddle.GetComponent<PlayerController>().cpu = false;

                    _playerTwoPaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerTwoPaddle.GetComponent<PlayerController>().PlayerCommands;

                    _playerTwoPaddle.GetComponent<PlayerController>().cpu = false;

                    _colorsOne.SetActive(true);
                    _colorsTwo.SetActive(true);

                    _howMany = 2;

                    break;
                }
            case 1:
                {
                    _playerOnePaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerOnePaddle.GetComponent<PlayerController>().PlayerCommands;

                    _playerOnePaddle.GetComponent<PlayerController>().cpu = false;

                    _playerTwoPaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerTwoPaddle.GetComponent<PlayerController>().IACommands;

                    _playerTwoPaddle.GetComponent<PlayerController>().cpu = true;

                    _colorsOne.SetActive(true);
                    _colorsTwo.SetActive(false);

                    EditNamePlayerTwo("CPU");

                    SetPlayerTwoColors(UnityEngine.Random.Range(0, _playerColor.Length));

                    _howMany = 1;

                    break;
                }
            case 2:
                {
                    _playerOnePaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerOnePaddle.GetComponent<PlayerController>().IACommands;

                    _playerOnePaddle.GetComponent<PlayerController>().cpu = true;

                    _playerTwoPaddle.GetComponent<PlayerController>().OnDefinePlayerType +=
                    _playerTwoPaddle.GetComponent<PlayerController>().IACommands;

                    _playerTwoPaddle.GetComponent<PlayerController>().cpu = true;

                    SetPlayerOneColors(UnityEngine.Random.Range(0, _playerColor.Length));
                    SetPlayerTwoColors(UnityEngine.Random.Range(0, _playerColor.Length));

                    EditNamePlayerOne("CPU 1");
                    EditNamePlayerTwo("CPU 2");

                    AllPlayersReady();

                    break;
                }
        }

        _menuPlayers.SetActive(false);
    }

    public void SetPlayerOneColors(int i)
    {
        _playerOnePaddle.GetComponent<SpriteRenderer>().color = _playerColor[i];
    }

    public void SetPlayerTwoColors(int i)
    {
        _playerTwoPaddle.GetComponent<SpriteRenderer>().color = _playerColor[i];
    }

    public void EditNamePlayerOne(string s)
    {
        _playerOneName = s;
    }

    public void EditNamePlayerTwo(string s)
    {
        _playerTwoName = s;
    }
    public void AllPlayersReady()
    {
        _playersReady++;

        _playerOneText.text = _playerOneName;

        _playerTwoText.text = _playerTwoName;

        if (_playersReady >= _howMany)
        {
            _colorsOne.SetActive(false);
            _colorsTwo.SetActive(false);

            ChangeState(EnumStates.BEGIN);
        }
    }

    private void OnEnable()
    {
        OnUpdatePoints = UpdatePoints;
    }
    private void OnDisable()
    {
        OnUpdatePoints = null;
    }
}
