# [WaterMelonMaker]

![WaterMelonMaker-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/67cae375-6d94-4ba1-9831-7346fbb707e5)
## 주요 코드
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
---

# [Concentration]

![Concentration-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/88738b75-5e20-4f33-8102-3f55fd74f274)
## 주요코드
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
---
