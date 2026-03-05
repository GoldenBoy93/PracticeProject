# [01. WaterMelonMaker]
<details> <!-- WaterMelon 전체 접기 -->
<summary> WaterMelonMaker 펼치기 / 접기 </summary>

![WaterMelonMaker-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/67cae375-6d94-4ba1-9831-7346fbb707e5)

## 주요 코드
<details> <!-- WaterMelon 코드 접기 -->
<summary> 클릭한 위치를 가져오는 로직 </summary>

<pre>csharp
private void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"[WaterMelonMaker] Mouse Position : {mousePos}");

        GameObject pruit = Instantiate(_pruits[0].gameObject);
        pruit.transform.position = mousePos;
    }
}
</pre>

</details> <!-- WaterMelon 코드 접기 -->

</details> <!-- WaterMelon 전체 접기 -->

---

# [02. Concentration]
<details> <!-- Concentration 전체 접기 -->
<summary> Concentration 펼치기 / 접기 </summary>

![Concentration-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/88738b75-5e20-4f33-8102-3f55fd74f274)

주요코드
<details> <!-- Concentration 코드1 접기 -->
<summary> 클릭한 위치의 오브젝트 정보를 검출하는 로직 </summary>
    
<pre>csharp
// 마우스 클릭 대상 검출
private void Update()
{
    // 마우스 좌클릭 했을 때
    if (Input.GetMouseButtonDown(0))
    {
        // 마우스 위치를 가져옴
        Vector2 mousePos = Input.mousePosition;

        // 마우스 위치를 월드 위치로 변환
        Vector2 worldPos = _camera.ScreenToWorldPoint(mousePos);

        // RayCastHit 객체 정의
        RaycastHit2D hit2D;

        // 월드위치에 Raycast 발사하여 찾기
        hit2D = Physics2D.Raycast(worldPos, transform.forward);

        // 맞은 대상이 있다면
        if (hit2D)
        {
            // hit2D의 레이어 정의
            LayerMask hitLayer = 1 << hit2D.collider.gameObject.layer;
            *이하 생략
        }
    }
}
</pre>

</details> <!-- Concentration 코드1 접기 -->

<details> <!-- Concentration 코드2 접기 -->
<summary> Fisher-Yates Shuffle </summary>
    
<pre>csharp
// Fisher-Yates Shuffle
private void Shuffle<T>(List<T> list)
{
    Debug.Log($"[Concentration] 2.카드 셔플");

    for (int i = list.Count - 1; i > 0; i--)
    {
        int rand = Random.Range(0, i + 1);

        // swap
        T temp = list[i];
        list[i] = list[rand];
        list[rand] = temp;
    }
}
</pre>

</details> <!-- Concentration 코드2 접기 -->

<details> <!-- Concentration 코드3 접기 -->
<summary> 카드 뿌리기 연출 (DOTween & UniTask) </summary>
    
<pre>csharp
// DOTween & UniTask 적용
private async UniTask SpreadCards()
{
    Debug.Log($"[Concentration] 3.카드 배치");

    for (int i = 0; i < _cardTable.Count; i++)
    {
        Transform tablePos = _cardTable[i].transform;
        Transform cardPos = _playCards[i].transform;

        // 트윈 효과
        cardPos.DOMove(tablePos.position, _spreadcardWaitTime);

        // 한장씩 배치하듯이 딜레이
        await UniTask.WaitForSeconds(_spreadcardWaitTime);
    }
}
</pre>

</details> <!-- Concentration 코드3 접기 -->

</details> <!-- Concentration 전체 접기 -->

---

# [03. ThreeMatchPuzzle]
<details> <!-- ThreeMatchPuzzle 전체 접기 -->
<summary> ThreeMatchPuzzle 펼치기 / 접기 </summary>

![ThreeMatchPuzzle-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/a18962de-d42c-42bd-963d-b208c25c3bf6)


주요코드
<details> <!-- ThreeMatchPuzzle 코드1 접기 -->
<summary> 파괴 할 타일을 저장할 해쉬셋 </summary>

<pre>csharp
// 유니티기능, 중복 해쉬가 등록 될시 무시하는 기능
private HashSet<Tile> _tilesHashSet = new HashSet<Tile>();
</pre>

</details> <!-- ThreeMatchPuzzle 코드1 접기 -->

<details> <!-- ThreeMatchPuzzle 코드2 접기 -->
<summary> 선택한 타일이 인접한지 체크하는 로직 </summary>

<pre>csharp
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
</pre>

</details> <!-- ThreeMatchPuzzle 코드2 접기 -->

<details> <!-- ThreeMatchPuzzle 코드3 접기 -->
<summary> 선택한 타일 위치 바꾸는 로직 </summary>

<pre>csharp
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
</pre>

</details> <!-- ThreeMatchPuzzle 코드3 접기 -->

<details> <!-- ThreeMatchPuzzle 코드4 접기 -->
<summary> 모든 타일 매치 검사하는 로직 </summary>

<pre>csharp
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
</pre>

</details> <!-- ThreeMatchPuzzle 코드4 접기 -->

<details> <!-- ThreeMatchPuzzle 코드5 접기 -->
<summary> 비워진 곳을 채우는 로직 </summary>

<pre>csharp
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
</pre>

</details> <!-- ThreeMatchPuzzle 코드5 접기 -->

</details> <!-- ThreeMatchPuzzle 전체 접기 -->

---
