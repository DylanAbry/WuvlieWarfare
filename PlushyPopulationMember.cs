using UnityEngine;

public class PlushyPopulationMember : MonoBehaviour
{
    private PlushyManager plushyManager;

    public void Initialize(PlushyManager manager)
    {
        plushyManager = manager;
    }

    private void OnDestroy()
    {
        if (plushyManager != null)
        {
            plushyManager.RemovePlushy(gameObject);
        }
    }
}
