using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Concentration : MonoBehaviour
{
    private static Concentration _instance;
    public static Concentration Instance => _instance;

    private Camera _camera;

    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private FlipCard _card1;
    [SerializeField] private FlipCard _card2;
    [SerializeField] private float _cardCheckWaitTime = 1.5f;
    [SerializeField] private float _spreadcardWaitTime = 0.5f;

    [SerializeField] private Transform _deckTransform;
    [SerializeField] private List<GameObject> _cardPrefabs;
    [SerializeField] private List<GameObject> _cardTable;
    [SerializeField] private List<GameObject> _playCards;

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

        // 덱 세팅
        SettingDeck();
    }

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

                // 레이어 불일치시 종료
                if (_targetLayer != hitLayer) return;

                Debug.Log($"[Concentration] RaycastHit : {hit2D.collider.gameObject.name}");

                // 카드 정의
                FlipCard card = hit2D.collider.gameObject.GetComponent<FlipCard>();

                // 이미 뒤집힌 상태라면 종료
                if (card.IsFlip) return;

                // 카드 뒤집기
                card.CardFlip();

                // 카드정보 저장
                if (_card1 == null)
                {
                    _card1 = card;
                }
                else
                {
                    _card2 = card;

                    // 한쌍이 확인됐으니 매칭여부 확인
                    CheckedMatch(_card1, _card2).Forget();
                }
            }
        }
    }

    private void SettingDeck()
    {
        // Deck 생성
        InitDeck();

        // Deck 섞기
        Shuffle(_playCards);

        // 카드 배치
        SpreadCards().Forget();
    }

    private void InitDeck()
    {
        Debug.Log($"[Concentration] 1.덱 생성");

        _playCards = new List<GameObject>();

        // 카드 최대 장수
        int maxCount = 12;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < maxCount / 2; j++)
            {
                GameObject card = Instantiate(_cardPrefabs[j], _deckTransform);
                card.transform.position = _deckTransform.position;
                _playCards.Add(card);
                Debug.Log($"[Concentration] {j}({i})번째 카드 추가.");
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

    private async UniTask CheckedMatch(FlipCard card1, FlipCard card2)
    {
        int idx1 = card1.CardIdx;
        int idx2 = card2.CardIdx;

        // 카드 확인 대기
        await UniTask.WaitForSeconds(_cardCheckWaitTime);

        if (idx1 == idx2)
        {
            Destroy(card1.gameObject);
            Destroy(card2.gameObject);

            Debug.Log($"[Concentration] 카드 일치");
        }
        else
        {
            card1.CardFlip();
            card2.CardFlip();

            Debug.Log($"[Concentration] 카드 불일치");
        }

        // 임시 카드정보 정리
        _card1 = null;
        _card2 = null;
    }
}