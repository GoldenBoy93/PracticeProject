using DG.Tweening;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _obstacleLayer; // 비트단위

    [SerializeField] private float _goSpeed = 1.0f;
    [SerializeField] private float _jumpPower = 1.0f;

    public Rigidbody2D RB
    {
        get { return _rb; }
        set { _rb = value; }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    private void Update()
    {
        // 게임중일경우
        if (FlappyBird.Instance.IsPlaying)
        {
            Vector3 goDir = new Vector3(_goSpeed, 0,0);
            transform.position += goDir * Time.deltaTime;

            // 마우스 좌클릭시
            if (Input.GetMouseButtonDown(0))
            {
                Jump();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 닿은 대상의 레이어를 비트 단위로 변환
        LayerMask hitLayer = 1 << collision.gameObject.layer;

        Debug.Log($"[Bird] HitLayer : {hitLayer.value}");

        // 장애물이 아닐경우 종료
        if (hitLayer != _obstacleLayer) return;

        // 게임오버
        FlappyBird.Instance.GameOver();
    }

    public void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower);
    }
}
