using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class HourGlass : MonoBehaviour 
{
    [SerializeField] private RectTransform hourGlass;
    [SerializeField] private Image sand;

    private Sequence currentSequence;
    
    private void Start() 
    {
        //BattleManager.Instance.OnTurnStart.AddListener<float>(CountTime); 
    }

    private void OnDestroy()
    {
        currentSequence?.Kill();
    }

    public void CountTime(float duration) 
    {
        // Kill any existing sequence
        currentSequence?.Kill();
        
        // Reset state
        hourGlass.rotation = Quaternion.identity;
        sand.rectTransform.anchoredPosition = Vector2.zero;
        sand.color = Color.green;

        currentSequence = DOTween.Sequence();

        // Rotate hourglass 180 degrees
        currentSequence.Join(
            hourGlass.DORotate(new Vector3(this.transform.rotation.x,0,180), 0.5f)
            .SetEase(Ease.OutBack)
        );

        // Sand movement animation with fixed direction
        const float sandDistance = 128f; 
        
        currentSequence.Join(
            sand.rectTransform.DOAnchorPosY(
                sandDistance, 
                duration
            )
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                // Calculate progress (1 to 0)
                float currentPos = sand.rectTransform.anchoredPosition.y;
                float progress = 1 - Mathf.Abs(currentPos / sandDistance);
                

                // Update color based on progress
                Color newColor;
                if (progress >= 0.5f)
                {
                    // Green (1.0) to Yellow (0.5)
                    float t = (progress - 0.5f) * 2f; // Scale 0.5-1.0 to 0-1
                    newColor = Color.Lerp(Color.yellow, Color.green, t);
                }
                else
                {
                    // Yellow (0.5) to Red (0.0)
                    float t = progress * 2f; // Scale 0-0.5 to 0-1
                    newColor = Color.Lerp(Color.red, Color.yellow, t);
                }
                
                sand.color = newColor;
            })
        );

        //Optional shake at the end
        currentSequence.AppendCallback(() => {
            hourGlass.DOShakeRotation(0.5f, 10f, 10, 90f)
                .SetEase(Ease.OutQuad);
        });
    }

    public void StopAnimation()
    {
        currentSequence?.Kill();
        hourGlass.rotation = Quaternion.identity;
        sand.rectTransform.anchoredPosition = Vector2.zero;
        sand.color = Color.green;
    }
}