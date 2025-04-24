using UnityEngine; 
using DG.Tweening;
public class CameraRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationDuration = 5f;
    [SerializeField] private float rotationAngle = 360f;
    [SerializeField] private Ease rotationEase = Ease.Linear;
    [SerializeField] private bool loopRotation = true;
    [SerializeField] private LoopType loopType = LoopType.Restart;
    
    private Tween rotationTween;

    void Start()
    {

        StartRotation();
    }

    public void StartRotation()
    {
        // Kill any existing tween to prevent conflicts
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        // Create a new rotation tween
        rotationTween = transform.DORotate(
                new Vector3(eulerAngles.x, rotationAngle, eulerAngles.z), // Rotate around Y axis only
                rotationDuration,
                RotateMode.WorldAxisAdd) // Use LocalAxisAdd to continuously add rotation
            .SetEase(rotationEase);
    
        // Set up looping if enabled
        if (loopRotation)
        {
            rotationTween.SetLoops(-1, loopType); // -1 means infinite loops
        }
    }

    public void StopRotation()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }

    public void PauseRotation()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Pause();
    }

    public void ResumeRotation()
    {
        if (rotationTween != null)
            rotationTween.Play();
    }

// Clean up when the object is destroyed
    private void OnDestroy()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }
}