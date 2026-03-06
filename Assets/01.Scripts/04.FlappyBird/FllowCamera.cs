using UnityEngine;

public class FllowCamera : MonoBehaviour
{
    [SerializeField] private GameObject _bird;

    public void Initialize(GameObject bird)
    {
        _bird = bird;
    }

    private void Update()
    {
        // 자신의 위치를 bird와 동기화
        if (_bird != null)
        {
            Vector3 birdPos = _bird.transform.position;
            Vector3 newPos = new Vector3(birdPos.x, 0, -10); // x축만 동기화
            transform.position = newPos;
        }
    }
}