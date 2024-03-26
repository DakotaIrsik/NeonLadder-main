using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [SerializeField]
    private IconIndicator iconIndicator; // Ensure this is linked in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the "Player" tag
        {
            iconIndicator.gameObject.SetActive(true); // Activate the GameObject
            iconIndicator.ActivateAnimation(); // Start the animation
        }
    }
}
