using UnityEngine;
using UnityEngine.UI;

public class PlushyInfo : MonoBehaviour
{
    public int health;
    public Slider healthBar;

    public enum PlushType
    {
        Pinchypup, 
        Shellshark, 
        Hoppo, 
        Rattlemeowser
    };

    public PlushType type;

    void Update()
    {
        healthBar.value = health;
    }
}
