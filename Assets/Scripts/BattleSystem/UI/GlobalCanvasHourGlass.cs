using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class GlobalCanvasHourGlass : MonoBehaviour 
{
    [SerializeField] private RectTransform hourGlass;
    [SerializeField] private Image sand;
    
    private Sequence currentSequence;
    private EnemyWorldCanvas parentCanvas;
    private Vector3 initialHourGlassRotation;
    private Vector3 initialSandPosition;
    
    private void Awake()
    {
        parentCanvas = GetComponentInParent<EnemyWorldCanvas>();
        initialHourGlassRotation = hourGlass.localRotation.eulerAngles;
        initialSandPosition = sand.rectTransform.localPosition;
    }

    private void OnDestroy()
    {
        currentSequence?.Kill();
    }

    public void CountTime(float duration) 
    {
        currentSequence?.Kill();
        
        // Reset state
        hourGlass.localRotation = Quaternion.Euler(initialHourGlassRotation);
        sand.rectTransform.localPosition = initialSandPosition;
        sand.color = Color.green;

        currentSequence = DOTween.Sequence();

        // Calculate the rotation based on world space
        Vector3 targetRotation = initialHourGlassRotation + new Vector3(0, 0, 180f);

        // Rotate hourglass 180 degrees in local space
        currentSequence.Join(
            hourGlass.DOLocalRotate(targetRotation, 0.5f)
            .SetEase(Ease.OutBack)
        );

        // Sand movement animation adjusted for world space
        float sandDistance = 3f; // Adjust this value based on your world space scale
        Vector3 startPos = sand.rectTransform.localPosition;
        Vector3 endPos = startPos + (Vector3.up * sandDistance);

        currentSequence.Join(
            sand.rectTransform.DOLocalMove(
                endPos,
                duration
            )
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                // Calculate progress (1 to 0)
                float currentProgress = Vector3.Distance(sand.rectTransform.localPosition, startPos) / sandDistance;
                float progress = 1 - currentProgress;

                // Update color based on progress
                Color newColor;
                if (progress >= 0.5f)
                {
                    // Green (1.0) to Yellow (0.5)
                    float t = (progress - 0.5f) * 2f;
                    newColor = Color.Lerp(Color.yellow, Color.green, t);
                }
                else
                {
                    // Yellow (0.5) to Red (0.0)
                    float t = progress * 2f;
                    newColor = Color.Lerp(Color.red, Color.yellow, t);
                }
                
                sand.color = newColor;
            })
        );

        // Optional shake at the end, adjusted for world space
        currentSequence.AppendCallback(() => {
            Vector3 originalRotation = hourGlass.localRotation.eulerAngles;
            hourGlass.DOShakeRotation(0.5f, 10f, 10, 90f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    // Reset to the target rotation after shaking
                    hourGlass.localRotation = Quaternion.Euler(targetRotation);
                });
        });
    }

    public void StopAnimation()
    {
        currentSequence?.Kill();
        hourGlass.localRotation = Quaternion.Euler(initialHourGlassRotation);
        sand.rectTransform.localPosition = initialSandPosition;
        sand.color = Color.green;
    }
}