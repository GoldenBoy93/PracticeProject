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
} </pre>
---
