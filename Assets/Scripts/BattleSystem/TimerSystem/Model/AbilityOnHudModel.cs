using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class AbilityOnHudModel : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [SerializeField] private Image iconFill;
    [SerializeField] private Image iconBg;
    [SerializeField] private Button iconButton;
    [SerializeField] private float fillDuration = 0.5f;

    private HexDirection direction;
    private int maxChargeStepsCount;
    private int chargedSteps = 0;
    private AbilityColorType abilityColor;
    private bool fullyCharged = false;
    private Vector2 playerActionHudControllerVector2;
    // This callback is provided by the HUD controller.
    // When the ability is fully charged, the controller will be notified (with the current direction).
    private Action<HexDirection> onDirectionCharged;
    private float currentZRotation;
    /// <summary>
    /// Initializes the HUD model for a specific direction.
    /// </summary>
    /// <param name="hexDirection">The direction (or slot index) of this ability.</param>
    /// <param name="requireSteps">How many steps are required to fully charge this ability.</param>
    /// <param name="iconSprite">The icon representing the ability.</param>
    /// <param name="colorType">The color type of the ability.</param>
    /// <param name="OnFullyCharged">Callback to notify when fully charged.</param>
    public void Init(HexDirection hexDirection,int requireSteps, Sprite iconSprite,AbilityColorType colorType, Action<HexDirection> OnFullyCharged)
    {
        this.direction = hexDirection;
        this.maxChargeStepsCount = requireSteps;
        this.iconFill.sprite = iconSprite;
        this.iconBg.sprite = iconSprite;

        Color abilityColor = AbilityColorHelper.GetAbilityColor(colorType);
        this.iconFill.color = abilityColor;
        
        abilityColor.a = 0.3f;
        this.iconBg.color = abilityColor;
        
        onDirectionCharged = OnFullyCharged;
        Reset();
    }

    public void Start()
    {
        playerActionHudControllerVector2 = new Vector2(PlayerActionHudController.Instance.transform.position.x,
            PlayerActionHudController.Instance.transform.position.y);
        currentZRotation = transform.rotation.z;
    }

    private void HandleChargeCompletion(Action<HexDirection> fullyChargedCallback)
    {
        if (!fullyCharged) return;
        fullyChargedCallback?.Invoke(direction);
        Reset();
    }

    /// <summary>
    /// Called externally to add charge steps. When the accumulated steps reach the required count, the ability is considered fully charged.
    /// </summary>
    /// <param name="addOnSteps">The number of steps to add.</param>
    public void NotifyUpdate(int addOnSteps)
    {
        if (fullyCharged) return;
        
        chargedSteps = Math.Clamp(chargedSteps + addOnSteps, 0, maxChargeStepsCount);
        float targetFill = (float)chargedSteps / maxChargeStepsCount;

        DOTween.Kill(iconFill);
        iconFill.DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad).OnComplete((() =>
            {
                if (chargedSteps >= maxChargeStepsCount) fullyCharged = true;
                HandleChargeCompletion(onDirectionCharged);
            }));
    }

    /// <summary>
    /// Resets the ability charge.
    /// </summary>
    public void Reset()
    {
        chargedSteps = 0;
        iconFill.fillAmount = 0;
        fullyCharged = false;
        DOTween.Kill(iconFill);
    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
        var rotation = transform.rotation;
        rotation.z = PlayerActionHudController.Instance.rotationZs[(int)direction];
        transform.rotation = rotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.up = playerActionHudControllerVector2 - eventData.position;
        if (transform.rotation.z > currentZRotation + 30)
        {
            PlayerActionHudController.Instance.SwapAbilitySlot(direction, (HexDirection)(((int)direction + 1)%6));
        }
        if (transform.rotation.z > currentZRotation - 30)
        {
            PlayerActionHudController.Instance.SwapAbilitySlot(direction, (HexDirection)(((int)direction - 1)%6));
        }
    }

    public void UpdateRotation(int newZRotation)
    {
        var rotation = transform.rotation;
        rotation.z = newZRotation;
        transform.rotation = rotation;
    }

    public void UpdateDirection(HexDirection newDirection)
    {
        currentZRotation = PlayerActionHudController.Instance.rotationZs[(int)newDirection];
        direction = newDirection;
    }
}

  