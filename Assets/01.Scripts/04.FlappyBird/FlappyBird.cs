using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlappyBird : MonoBehaviour
{
    private static FlappyBird _instance;
    public static FlappyBird Instance => _instance;

    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _flappyBirdPrefab;
    [SerializeField] private List<GameObject> _obstaclePrefabs;
    [SerializeField] private GameObject _obstacleDeck;
    [SerializeField] private Vector2 _centerPosition = new Vector2(0,0);
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _reStartButton;

    [SerializeField] private Bird _bird;
    [SerializeField] private bool _isPlaying = false;
    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        // SingleTon
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 게임 초기 설정
        _isPlaying = false;

        // 버드 프리펩 생성
        if (_bird == null && _flappyBirdPrefab != null)
        {
            GameObject birdPrefab = Instantiate(_flappyBirdPrefab);
            birdPrefab.transform.position = _centerPosition;
            _bird = birdPrefab.GetComponent<Bird>();
        }

        // Fllow Camera 세팅
        if (_camera == null)
        {
            _camera = Camera.main;
            FllowCamera fllowCamera = _camera.AddComponent<FllowCamera>();
            fllowCamera.Initialize(_bird.gameObject);
        }

        // Start Button 이벤트 연결
        if (_startButton != null)
        {
            // 버튼에 호출할 함수 연결
            _startButton.onClick.AddListener(StartGame);
            _reStartButton.onClick.AddListener(StartGame);
            _reStartButton.gameObject.SetActive(false);
        }
    }

    // 게임스타트 버튼을 누를시 시작
    public void StartGame()
    {
        Debug.Log($"게임시작");

        // '게임중'으로 설정 전환
        _isPlaying = true;

        // bird 원위치
        _bird.gameObject.transform.position = _centerPosition;

        // 버드에게 중력 적용
        _bird.RB.gravityScale = 1;

        // 버튼 비활성화
        _startButton.gameObject.SetActive(false);
        _reStartButton.gameObject.SetActive(false);

        // 최초 장애물 생성
        InitObstacles();
    }

    public void GameOver()
    {
        Debug.Log($"게임종료");

        // '비게임중'으로 설정 전환
        _isPlaying = false;
        
        // 버드에게 중력 중지
        _bird.RB.gravityScale = 0;

        // 다시 시작 버튼 활성화
        _reStartButton.gameObject.SetActive(true);
    }

    // Obstacle의 CollisionEnter2D에서 호출할 함수
    public void InitObstacles()
    {
        // 랜덤 obstacle 선택
        int randomIdx = Random.Range(0, _obstaclePrefabs.Count);

        // 랜덤 높이 선택 (obstacle이 설치될 위아래 높이)
        int randomHeight = Random.Range(-2, 2);

        // bird 위치를 가져와서 obstacle 생성 위치를 계산
        Vector2 birdPos = _bird.transform.position;
        Vector2 initPos = birdPos + new Vector2(8, randomHeight);

        // obstacle 생성
        GameObject newObstacle = Instantiate(_obstaclePrefabs[randomIdx], _obstacleDeck.transform);
        newObstacle.transform.position = initPos;
    }
}