using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Exposed
    [Header("Puzzle elements")]
    public GameObject _puzzlePiecePrefab;
    public GameObject _emptyPiecePrefab;
    public Transform _puzzlePanel;
    public Sprite[] _androidImages;
    public Sprite[] _iOSImages;
    [Header("Preview elements")]
    public Transform _puzzlePreviewPanel;
    public Sprite _androidCenterimage;
    public Sprite _iOSCenterimage;
    [Header("Timer parameters")]
    public int _timerInSeconds = 180;
    public Text _timerText;
    [Header("End game panels")]
    public GameObject _victoryPanel;
    public GameObject _defeatPanel;
    #endregion

    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            return _instance;
        }
    }
    #endregion

    #region Events
    public delegate void OnPieceMoved(int draggedPieceIndex, int emptyPieceIndex);
    public OnPieceMoved _onPieceMoved;
    public delegate void OnVictory();
    public OnVictory _onVictory;
    public delegate void OnDefeat();
    public OnDefeat _onDefeat;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1;
        _onPieceMoved += OnPieceMovedAction;
        _onVictory += OnVictoryAction;
        _onDefeat += OnDefeatAction;

        //**** game creation ****//
        // initialize timer
        _timerText.text = Utils.FormatTime(_timerInSeconds);

        // check the OS.
        Sprite[] puzzleImages;
        Sprite centerImage;
        #if UNITY_IOS
            puzzleImages = _iOSImages;
            centerImage = _iOSCenterimage;
        #endif
        #if UNITY_ANDROID
            puzzleImages = _androidImages;
            centerImage = _androidCenterimage;
        #endif
        /*if (Application.platform == RuntimePlatform.Android) {
            puzzleImages = _androidImages;
        } else if(Application.platform == RuntimePlatform.IPhonePlayer) {

        }*/

        // shuffle the puzzle and check if it is solvable.
        _currentGameState = ShuffleList(new List<int>{0,1,2,3,4,5,6,7,8});
        Debug.Log(string.Join(";", _currentGameState));
        while(!CheckPuzzleResolution(_currentGameState)) {
            _currentGameState = ShuffleList(_currentGameState);
            Debug.Log(string.Join(";", _currentGameState));
        }
        
        // assign the images to the panels.
        for (int i=0; i < _currentGameState.Count; i++) {

            int pieceId = _currentGameState[i];

            // id 0 is the empty piece
            if (pieceId == 0) {
                Instantiate(_emptyPiecePrefab, _puzzlePanel);
                // use puzzle prefab for Preview instantiation, because we don't want the drag/drop mechanism
                GameObject previewCenter = Instantiate(_puzzlePiecePrefab, _puzzlePreviewPanel);
                previewCenter.GetComponent<Image>().sprite = centerImage;
            } else {
                GameObject currentPiece = Instantiate(_puzzlePiecePrefab, _puzzlePanel);
                GameObject currentpreviewPiece = Instantiate(_puzzlePiecePrefab, _puzzlePreviewPanel);
                // for all other ids, modify the image
                currentPiece.GetComponent<Image>().sprite = puzzleImages[pieceId-1];
                currentpreviewPiece.GetComponent<Image>().sprite = puzzleImages[pieceId-1];
            }
        }
    }

    void Update()
    {
        // Timer update
        if (_isGameStarted) {
            _timePassed = Time.time - _startTime;

            _timerText.text = Utils.FormatTime(_timerInSeconds - _timePassed);

            // defeat condition
            if (_timePassed >= _timerInSeconds) _onDefeat.Invoke();
        }
        

    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    public void OnPieceMovedAction(int draggedPieceIndex, int emptyPieceIndex) {
        // game starts after the first move
        if (!_isGameStarted) {
            _isGameStarted = true;
            _startTime = Time.time;
        }

        // Update preview
        Transform draggedPreviewPiece = _puzzlePreviewPanel.GetChild(draggedPieceIndex);
        Transform centerPreviewPiece = _puzzlePreviewPanel.GetChild(emptyPieceIndex);
        draggedPreviewPiece.SetSiblingIndex(emptyPieceIndex);
        centerPreviewPiece.SetSiblingIndex(draggedPieceIndex);

        // Update game state
        int temp = _currentGameState[draggedPieceIndex];
        _currentGameState[draggedPieceIndex] = _currentGameState[emptyPieceIndex];
        _currentGameState[emptyPieceIndex] = temp;

        // compare current game state with the victory condition
        List<int> expectedList = new List<int>{1,2,3,4,0,5,6,7,8};
        for (int i=0; i < expectedList.Count; i++) {
            if (_currentGameState[i] != expectedList[i]) return;
        }

        // apply victory
        _onVictory.Invoke();
    }

    private List<int> ShuffleList(List<int> list) {
        for (int i = 0; i < list.Count; i++) {
            int temp = list[i];
            int randomId = Random.Range(i, list.Count);
            list[i] = list[randomId];
            list[randomId] = temp;
        }
        return list;
    }

    private bool CheckPuzzleResolution(List<int> gameState) {
        // to check if the puzzle is solvable, the number of inversions must be even.
        // there is an inversion when two numbers are not in ascending numerical order with respect to each other.

        int inversionsCount = 0;
        for (int i=0; i < gameState.Count-1; i++) {
            // ignore the empty tile
            if (gameState[i] == 0) continue;
            for (int j = i+1; j < gameState.Count; j++) {
                // ignore the empty tile
                if (gameState[j] == 0) continue;
                // check the order
                if (gameState[j] < gameState[i]) {
                    //Debug.Log("(" + gameState[i] + ";" + gameState[j] + ")");
                    inversionsCount++;
                }
            }
        }

        //Debug.Log("inversions : " + inversionsCount);
        return inversionsCount%2 == 0;
    }

    private void OnVictoryAction() {
        _isGameStarted = false;
        _victoryPanel.SetActive(true);
        // update the time displayed in victory panel
        _victoryPanel.transform.Find("Time Text").GetComponent<Text>().text += Utils.FormatTime(_timePassed);

        if (SaveSystem.LoadBestTime() == null || _timePassed < SaveSystem.LoadBestTime().bestTime) {
            SaveSystem.SaveBestTime(new BestTime(_timePassed));
            // display best time message
            _victoryPanel.transform.Find("New Best Score Text").GetComponent<Text>().enabled = true;
        }
    }

    private void OnDefeatAction() {
        _isGameStarted = false;
        _defeatPanel.SetActive(true);
    }


    #endregion

    #region Private & Protected
    private List<int> _currentGameState = new List<int>();
    private float _startTime = -1;
    private float _timePassed;
    private bool _isGameStarted;
    #endregion
}
