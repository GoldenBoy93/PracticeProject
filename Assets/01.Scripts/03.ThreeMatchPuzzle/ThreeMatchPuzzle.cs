using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None,
    A,
    B,
    C,
    D,
    E,
}

public class Tile
{
    public TileType _type;
    public Vector2Int _boardPosition;
    public TileView _tileView;
}

public class ThreeMatchPuzzle : MonoBehaviour
{
    private static ThreeMatchPuzzle _instance;
    public static ThreeMatchPuzzle Instance => _instance;

    [SerializeField] Camera _camera;
    [SerializeField] Tile[,] _board;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(6, 7);
    [SerializeField] private GameObject _boardObject;
    [SerializeField] private GameObject _tilePrefab;

    [SerializeField] private TileView[] selectTiles = new TileView[2];

    // 파괴 할 타일을 저장할 해쉬셋 (중복방지기능 제공)
    private HashSet<Tile> _tilesHashSet = new HashSet<Tile>();

    // 매칭검사용 임시 리스트
    private List<Tile> _tiles = new List<Tile>();

    TileType[] _validTypes = { TileType.A, TileType.B, TileType.C, TileType.D, TileType.E };

    private bool isdirty = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (_camera == null)
        {  
            _camera = Camera.main;
        }

        Debug.Log($"[ThreeMatchPuzzle] GameManager 초기화 완료");

        // 보드 위치 정중앙으로 정렬
        float offsetX = - ( _boardSize.x - 1 ) / 2f;
        float offsetY = - ( ( _boardSize.y - 1 ) / 2f + (_boardSize.y - 1) % 2f );

        Vector2 center = new Vector2(offsetX, offsetY);
        _boardObject.gameObject.transform.position = center;

        // 보드 생성
        _board = new Tile[_boardSize.x, _boardSize.y];

        Debug.Log($"[ThreeMatchPuzzle] Board 생성 완료. Board Size : {_boardSize.x}x{_boardSize.y}");

        // 타일 생성
        SpawnTiles();
    }

    private void Update()
    {
        // 마우스 좌클릭 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 커서 위치를 가져옴
            Vector2 mousePos = Input.mousePosition;

            // 월드 포스로 변환
            Vector2 worldPos = _camera.ScreenToWorldPoint(mousePos);

            // 객체 정의
            RaycastHit2D hit2D;

            // 레이캐스트 발사
            hit2D = Physics2D.Raycast(worldPos, Vector2.zero);

            // 맞은 대상이 있다면
            if (hit2D)
            {
                TileView targetTile = hit2D.collider.gameObject.GetComponent<TileView>();

                // 선택한 타일 배열의 첫자리가 비어있다면
                if (selectTiles[0] == null)
                {
                    selectTiles[0] = targetTile;
                    Debug.Log($"[ThreeMatchPuzzle] 첫번째 타일 좌표 : {selectTiles[0].Tile._boardPosition}");
                }
                else
                {
                    selectTiles[1] = targetTile;
                    Debug.Log($"[ThreeMatchPuzzle] 두번째 타일 좌표 : {selectTiles[1].Tile._boardPosition}");

                    // 두번째 선택한 타일이 인접한지 검사
                    if (IsAdjacentCheck())
                    {
                        // 자리 바꾸기
                        ChangeTilePosition().Forget();
                    }
                    else
                    {
                        // 선택 배열 초기화
                        selectTiles[0] = null;
                        selectTiles[1] = null;
                    }
                }
            }
            else
            {
                Debug.Log($"[ThreeMatchPuzzle] 대상을 찾지 못함");
            }
        }
    }

    private void SpawnTiles()
    {
        // 보드의 가로 길이만큼 반복
        for (int x = 0; x < _boardSize.x; x++)
        {
            // 보드의 세로 길이만큼 반복
            for (int y = 0; y < _boardSize.y; y++)
            {
                Tile tile = new Tile();
                tile._boardPosition = new Vector2Int(x, y);

                // 사용할 타입 중 랜덤 타입 할당
                tile._type = _validTypes[Random.Range(0, _validTypes.Length)];

                // 실제 타일 프리펩 생성 및 보드 오브젝트의 자식으로 생성
                GameObject tileObject = Instantiate(_tilePrefab, _boardObject.transform);
                
                // 타일 오브젝트 위치 이동
                tileObject.transform.localPosition = new Vector2(x, y);
                
                // 타일뷰 초기화
                TileView targetTile = tileObject.gameObject.GetComponent<TileView>();
                targetTile.Initialize(tile);

                // Tile에 View 정보 주입
                tile._tileView = targetTile;

                // Board에 타일을 매핑
                _board[x, y] = tile;

                Debug.Log($"[ThreeMatchPuzzle] Tile 생성 및 배치 완료");
            }
        }
    }

    private async UniTask ChangeTilePosition()
    {
        // 현재 선택 Tile
        TileView tileView0 = selectTiles[0];
        TileView tileView1 = selectTiles[1];

        Tile tile0 = tileView0.Tile;
        Tile tile1 = tileView1.Tile;

        // board 위치 임시 저장
        Vector2Int pos0 = tile0._boardPosition;
        Vector2Int pos1 = tile1._boardPosition;

        // board 배열 교환
        _board[pos0.x, pos0.y] = tile1;
        _board[pos1.x, pos1.y] = tile0;

        // Tile._boardPosition 교환
        tile0._boardPosition = pos1;
        tile1._boardPosition = pos0;

        // Transform 이동 (연출)
        tileView0.transform.DOLocalMove(new Vector2(pos1.x, pos1.y), 0.2f);
        tileView1.transform.DOLocalMove(new Vector2(pos0.x, pos0.y), 0.2f);

        // 선택 배열 초기화
        selectTiles[0] = null;
        selectTiles[1] = null;

        Debug.Log("[ThreeMatchPuzzle] 자리 변경 완료");

        await UniTask.WaitForSeconds(1.0f);

        // 매칭검사
        AllMatchedCheck();
    }

    private bool IsAdjacentCheck()
    {
        // 현재 선택 Tile
        TileView tileView0 = selectTiles[0];
        TileView tileView1 = selectTiles[1];

        Tile tile0 = tileView0.Tile;
        Tile tile1 = tileView1.Tile;

        // board 위치 가져오기
        Vector2Int pos0 = tile0._boardPosition;
        Vector2Int pos1 = tile1._boardPosition;

        int dx = Mathf.Abs(pos0.x - pos1.x);
        int dy = Mathf.Abs(pos0.y - pos1.y);

        // 합쳐서 1이거나 (인접)
        // 각각 1일때 (대각선)
        bool adjacentCheck = dx + dy == 1 || ( dx == 1 && dy == 1 );

        // 맨해튼 거리 체크 방식
        if (adjacentCheck)
        {
            Debug.Log($"[ThreeMatchPuzzle] 인접 판정");
            return true;
        }
        else
        {
            Debug.Log($"[ThreeMatchPuzzle] 비인접 판정");
            return false;
        }
    }

    // 모든 타일 매치 검사
    private void AllMatchedCheck()
    {
        int width = _board.GetLength(0);
        int height = _board.GetLength(1);

        // 가로 매칭 검사 로직
        for (int y = 0; y < height; y++) // 세로 길이만큼 반복
        {
            // 줄(Row)이 바뀔 때마다 초기화
            _tiles.Clear();
            Tile lastTile = _board[0, y]; // 현재 줄의 첫 번째 타일을 기준점으로 설정
            _tiles.Add(lastTile);         // 기준점 타일을 리스트에 미리 넣고 시작

            for (int x = 1; x < width; x++) // 첫 타일은 넣었으니 x=1부터 시작
            {
                Tile currentTile = _board[x, y];

                // 타입이 같고, None이 아닐 때만 추가
                if (currentTile != null && lastTile != null &&
                    currentTile._type == lastTile._type && currentTile._type != TileType.None)
                {
                    _tiles.Add(currentTile);
                }
                else // 타입이 다르거나 빈 공간을 만났다면 (연속이 끊김)
                {
                    // 지금까지 쌓인 게 3개 이상인지 확인
                    if (_tiles.Count >= 3)
                    {
                        // 3개이상 쌓인 _tiles를 HashSet에 추가
                        _tilesHashSet.UnionWith(_tiles);
                    }

                    // 리스트 비우고, 지금부터 다시 카운트 시작
                    _tiles.Clear();
                    _tiles.Add(currentTile);
                }

                // 다음 칸 비교를 위해 기준점을 현재 타일로 업데이트
                lastTile = currentTile;
            }

            // 한 줄 검사가 완전히 끝난 직후 마지막으로 체크
            // 보드 오른쪽 끝부분에서 매칭이 끝난 경우를 처리하기 위함
            if (_tiles.Count >= 3)
            {
                _tilesHashSet.UnionWith(_tiles);
            }

            // 정리 끝났음 선언
            isdirty = false;
        }

        // 세로 매칭 검사 로직
        for (int x = 0; x < width; x++)
        {
            _tiles.Clear();
            Tile lastTile = _board[x, 0];
            _tiles.Add(lastTile);

            for (int y = 1; y < height; y++)
            {
                Tile currentTile = _board[x, y];

                if (currentTile != null && lastTile != null &&
                    currentTile._type == lastTile._type && currentTile._type != TileType.None)
                {
                    _tiles.Add(currentTile);
                }
                else
                {
                    if (_tiles.Count >= 3)
                    {
                        _tilesHashSet.UnionWith(_tiles);
                    }

                    _tiles.Clear();
                    _tiles.Add(currentTile);
                    lastTile = currentTile;
                }
            }

            if (_tiles.Count >= 3)
            {
                _tilesHashSet.UnionWith(_tiles);
            }
        }

        Debug.Log("[ThreeMatchPuzzle] 매칭 검사 완료");

        // 매칭된 타일들 파괴
        DestroyMatchedTile();
    }

    private void DestroyMatchedTile()
    {
        // '_tilesHashSet' 목록 순회
        foreach (Tile tile in _tilesHashSet)
        {
            Vector2Int boardPos = tile._boardPosition;
            _board[boardPos.x, boardPos.y] = null;

            try
            {
                Destroy(tile._tileView.gameObject);
            }
            catch
            {
                Debug.Log($"파괴 실패");
            }
        }

        // _tilesHashSet 비우기
        _tilesHashSet.Clear();

        // 정리 완료 선언
        isdirty = false;

        // 보드 채우기
        FillEmptySpaces().Forget();
    }

    private async UniTask FillEmptySpaces()
    {
        int width = _board.GetLength(0);
        int height = _board.GetLength(1);

        // 일단 보드에 남은 타일들을 아래로 내리기
        for (int x = 0; x < width; x++) // 세로줄 검사
        {
            for (int y = 0; y < height; y++) // 아래에서 위로 검사
            {
                // 현재 검사중인 타일 좌표가 null 이라면
                if (_board[x, y] == null)
                {
                    // 위를 쭉 훑으며 타일을 찾기
                    for (int targetY = y + 1; targetY < height; targetY++)
                    {
                        // 위에 타일이 있다면
                        if (_board[x, targetY] != null)
                        {
                            // 찾은 타일 정의
                            Tile foundTile = _board[x, targetY];

                            // 빈자리에 찾은 타일 넣기
                            _board[x, y] = foundTile;

                            // 찾은 타일의 보드 좌표 값 비우기
                            _board[x, targetY] = null;

                            // 찾은 타일의 가지고 있는 좌표값 내려온 위치로 변경
                            foundTile._boardPosition = new Vector2Int(x, y);

                            // 타일 실제 오브젝트 내리기 (DOTween 연출)
                            foundTile._tileView.transform.DOLocalMove(new Vector2(x, y), 0.3f);

                            // 타일을 하나 찾아서 끌어내렸으니, 타겟 검색 루프를 멈추고 다음 y 칸 검사로 넘어감
                            break;
                        }
                    }
                }
            }
        }

        // 남은 빈 공간에 새로운 타일 생성해서 채우는 과정
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_board[x, y] == null)
                {
                    if (isdirty == false)
                    {
                        // 변한 값이 있다고 체크
                        isdirty = true;
                    }

                    Tile newTile = new Tile();
                    newTile._boardPosition = new Vector2Int(x, y);

                    newTile._type = _validTypes[Random.Range(0, _validTypes.Length)];

                    // 새 타일 생성
                    GameObject tileObject = Instantiate(_tilePrefab, _boardObject.transform);

                    // 새 타일을 보드 높이보다 위로 배치
                    tileObject.transform.localPosition = new Vector2(x, height + y);

                    TileView targetTile = tileObject.GetComponent<TileView>();
                    targetTile.Initialize(newTile);
                    newTile._tileView = targetTile;

                    // 배열에 등록
                    _board[x, y] = newTile;

                    // 새로 생성한 타일을 아래로 이동 (DOTween 사용)
                    tileObject.transform.DOLocalMove(new Vector2(x, y), 0.4f);
                }
            }
        }

        Debug.Log("[ThreeMatchPuzzle] 빈 공간 채우기 완료");

        // 잠시 기다림
        await UniTask.WaitForSeconds(1.0f);

        // 변한 값이 있다면
        if (isdirty == true)
        {
            // 매칭검사(재)
            AllMatchedCheck();
        }
    }
}