using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 대상의 레이어를 비트로 변환
        LayerMask hitlayer = 1 << collision.gameObject.layer;

        // 대상인지 확인
        if (hitlayer == _targetLayer)
        {
            Debug.Log($"[Obstacle] Bird 감지.");

            // 새로운 장애물을 생성하라고 매니저에게 요청
            FlappyBird.Instance.InitObstacles();
        }
        else
        {
            Debug.Log($"[Obstacle] Bird 아님.");
        }
    }
}