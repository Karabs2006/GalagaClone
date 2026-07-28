using TMPro;
using UnityEngine;

public class PrintScore : MonoBehaviour
{
    public TMP_Text text;
    
    void Start()
    {
        text.text = $"{PlayerScore.finalScore}";
    }

}
