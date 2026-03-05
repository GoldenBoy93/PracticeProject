using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Tile _tile;
    [SerializeField] private TileType _tileType;

    public Tile Tile => _tile;

    // 컬러 모음
    private Color _colorRed = Color.red;
    private Color _colorGreen = Color.green;
    private Color _colorBlue = Color.blue;
    private Color _colorYellow = Color.yellow;
    private Color _colorCyan = Color.cyan;

    public void Initialize(Tile tile)
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        _tile = tile;
        _tileType = tile._type;

        // 컬러 변경
        ChangeColor(_tile._type);
    }

    private void ChangeColor(TileType type)
    {
        switch(type)
        {
            case TileType.A :
                _spriteRenderer.color = _colorRed;
                return;

            case TileType.B:
                _spriteRenderer.color = _colorGreen;
                return;

            case TileType.C:
                _spriteRenderer.color = _colorBlue;
                return;

            case TileType.D:
                _spriteRenderer.color = _colorYellow;
                return;

            case TileType.E:
                _spriteRenderer.color = _colorCyan;
                return;
        }
    }
}