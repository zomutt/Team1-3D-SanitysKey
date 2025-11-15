using UnityEngine;

public class CrosshairInitializer : MonoBehaviour
{
    void Start()
    {
        CrosshairController controller = GetComponent<CrosshairController>();
        if (controller != null)
        {
            controller.SetState(CrosshairState.Default);
        }
    }
}
