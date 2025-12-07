using UnityEngine;

public class FinalPuzzle : MonoBehaviour
{
    public int finalCandleCount;
    public bool allCandlesLit;
    public bool rosePlaced;
    public GameObject roses;
    public bool watchPlaced;
    public GameObject watch;
    public bool toyPlaced;
    public GameObject toy;
    public bool pendantPlaced;
    public GameObject pendant;
    private void Awake()
    {
        finalCandleCount = 0;
        allCandlesLit = false;
    }

    private void Update()
    {
        if (finalCandleCount >= 6) { allCandlesLit = true; }    
    }
}

