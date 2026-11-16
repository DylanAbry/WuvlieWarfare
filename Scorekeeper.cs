using UnityEngine;
using TMPro;

public class Scorekeeper : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreDisplay;

    public int mapNumber = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreDisplay.text = string.Format(score.ToString());
    }
}
