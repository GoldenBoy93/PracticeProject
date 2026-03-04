using Unity.VisualScripting;
using UnityEngine;

public class Pruit : MonoBehaviour
{
    [SerializeField] private int _level = 0;
    public int Level => _level;
    private bool _isPlused = false;

    public bool IsPlused
    {
        get { return _isPlused; }
        set { _isPlused = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            LayerMask myLayer = this.gameObject.layer;
            LayerMask otherLayer = collision.gameObject.layer;

            if (myLayer == otherLayer) 
            {
                Debug.Log($"[Pruit] 레이어 일치.");

                Pruit myPruit = this;
                Pruit otherPruit = collision.gameObject.GetComponent<Pruit>();

                WaterMelonMaker.Instance.GameLogic(myPruit, otherPruit);
            }
        }
    }
}