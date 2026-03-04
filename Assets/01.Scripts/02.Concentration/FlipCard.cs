using UnityEngine;

public class FlipCard : MonoBehaviour
{
    private SpriteRenderer _mySpriteRenderer;

    [SerializeField] private bool _isFlip = false;
    [SerializeField] private int _cardIdx = 0;
    [SerializeField] private Color _backColor;
    [SerializeField] private Color _frontColor;

    public bool IsFlip
    {
        get { return _isFlip; }
        set { _isFlip = value; }
    }

    public int CardIdx
    {
        get { return _cardIdx; }
        set { _cardIdx = value; }
    }

    private void Awake()
    {
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
        _mySpriteRenderer.color = _backColor;
        _isFlip = false;
    }

    public void CardFlip()
    {
        // 뒷면 (뒤집히지 않았을 때)
        if (!_isFlip)
        {
            _isFlip = true;
            _mySpriteRenderer.color = _frontColor;
            Debug.Log($"카드가 앞면이 되었음");
        }
        // 앞면 (뒤집혀 있을 때)
        else
        {
            _isFlip = false;
            _mySpriteRenderer.color = _backColor;
            Debug.Log($"카드가 뒷면이 되었음");
        }
    }
}