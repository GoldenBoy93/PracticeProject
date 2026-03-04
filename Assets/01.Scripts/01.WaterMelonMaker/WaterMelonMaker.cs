using System.Collections.Generic;
using UnityEngine;

public class WaterMelonMaker : MonoBehaviour
{
    private static WaterMelonMaker _instance;
    public static WaterMelonMaker Instance => _instance;

    private Camera _camera;
    [SerializeField] private List<Pruit> _pruits;

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

        if( _camera == null)
        {
            _camera = Camera.main;
        }
    }

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

    public void GameLogic(Pruit pruit1, Pruit pruit2)
    {
        if (pruit1.Level != pruit2.Level) return;
        if (pruit1.IsPlused == true || pruit2.IsPlused == true) return;

        int currentLevel = pruit1.Level;
        int nextLevel = pruit2.Level + 1;

        Vector2 pos1 = pruit1.transform.position;
        Vector2 pos2 = pruit2.transform.position;
        Vector2 newPos = (pos1 + pos2) / 2;

        pruit1.IsPlused = true;
        pruit2.IsPlused = true;

        Destroy(pruit1.gameObject);
        Destroy(pruit2.gameObject);

        GameObject nextPruit = Instantiate(_pruits[nextLevel].gameObject);
        nextPruit.transform.position = newPos;
    }
}