using Cinemachine;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{

    public float priorityUpdateDuration = 0.5f; // The duration over which the priority will interpolate.
    public float cameraTransitionEasement = 0.5f; // A value that affects the smoothing of the transition.

    private int currentPriority;
    private int targetPriority;
    private float priorityChangeStartTime;
    private CinemachineVirtualCamera targetCameraVirtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        targetCameraVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        currentPriority = targetCameraVirtualCamera.Priority;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPriority != targetPriority)
        {
            // Calculate how much time has passed since we started the priority change.
            float elapsedTime = Time.time - priorityChangeStartTime;

            // Calculate the percentage complete the transition is (0 to 1).
            float t = Mathf.Clamp01(elapsedTime / priorityUpdateDuration);

            // Optionally apply some easing to the transition.
            t = Mathf.SmoothStep(0, 1, t);

            // Set the priority by interpolating between the current and target values.
            targetCameraVirtualCamera.Priority = Mathf.RoundToInt(Mathf.Lerp(currentPriority, targetPriority, t));

            // If the transition is complete, set the current priority to the target.
            if (t >= 1)
            {
                currentPriority = targetPriority;
            }
        }
    }

    public void UpdatePriority(int priority)
    {
        targetPriority = priority;
        priorityChangeStartTime = Time.time; // Reset the start time for the interpolation.
    }
}
