using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class IconIndicator : MonoBehaviour
{
    [SerializeField]
    private float IconIndicatorDuration = Constants.IconAnimationDefaultDuration;
    public int loopCount = 3;
    public float IconDistance = 1.0f;
    private bool isAnimating = false;


    // Reference to Mesh assets instead of MeshFilter components
    [SerializeField]
    private Mesh upMesh;
    [SerializeField]
    private Mesh downMesh;
    [SerializeField]
    private Mesh leftMesh;
    [SerializeField]
    private Mesh rightMesh;

    // Enum for direction
    public enum IconDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    [SerializeField]
    private IconDirection Direction = IconDirection.None;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            Debug.LogError("MeshFilter component not found on the object.");
            return;
        }

        switch (Direction)
        {
            case IconDirection.Up:
                meshFilter.mesh = upMesh;
                break;
            case IconDirection.Down:
                meshFilter.mesh = downMesh;
                break;
            case IconDirection.Left:
                meshFilter.mesh = leftMesh;
                break;
            case IconDirection.Right:
                meshFilter.mesh = rightMesh;
                break;
            case IconDirection.None:
                // Handle no direction
                break;
        }
    }

    public void ActivateAnimation()
    {
        if (!isAnimating) // Prevents reactivation before the current animation ends
        {
            
            isAnimating = true;
            StartCoroutine(MoveIndicator());
        }
    }

    public void Update()
    {
        MoveIndicator();
    }

    private IEnumerator MoveIndicator()
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 direction = Vector3.zero; // Direction vector initialized to zero

        // can I use set enums for this?
        if (Direction == IconDirection.Up) direction += Vector3.up;
        if (Direction == IconDirection.Down) direction += Vector3.down;
        if (Direction == IconDirection.Left) direction += Vector3.left;
        if (Direction == IconDirection.Right) direction += Vector3.right;

        Vector3 endPosition = startPosition + (direction.normalized * IconDistance); // Calculate endPosition

        if (direction == Vector3.zero)
        {
            Debug.LogError("No direction specified for the icon indicator");
            yield break; // Exit the coroutine early if no direction is set
        }

        for (int loop = 0; loop < loopCount; loop++)
        {
            while (elapsedTime < IconIndicatorDuration)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / IconIndicatorDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset for next loop if necessary
            elapsedTime = 0;
            transform.position = startPosition; // Optionally reset position to start each loop from the initial position
        }

        gameObject.SetActive(false); // Deactivate the GameObject after all loops are complete
        isAnimating = false; // Mark animation as complete
    }
}
