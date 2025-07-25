using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class AbilityOnHudModel : MonoBehaviour, IEndDragHandler, IDragHandler
{
    
    [SerializeField] private Image bgMaskFrame;
    [SerializeField] private Image bgMaskIcon;
    
    [SerializeField] private Image bgFrame;
    [SerializeField] private Image bgIcon;
    
    
    [SerializeField] private Image fgFrame;
    [SerializeField] private Image fgIcon;
    
    [SerializeField] private Button iconButton;
    [SerializeField] private float fillDuration = 0.5f;

    public AbilityData localAbilityData;
    
    private HexDirection direction;
    private int maxChargeStepsCount;
    private int chargedSteps = 0;
    private AbilityColorType abilityColor;
    private bool fullyCharged;
    public bool FullyCharged
    {
        get { return fullyCharged; }
        set { fullyCharged = value; }
        
    }
    private Action onSelectAbility;
    public float currentZRotation;


    public List<HexCell> currentPattern;
    public void Init(HexDirection hexDirection, AbilityData ad, Action OnSelectAbility)
    {
        this.localAbilityData = ad;
        this.direction = hexDirection;
        this.maxChargeStepsCount = ad.PrerequisiteChargeSteps;
        
        bgMaskIcon.sprite = ad.Icon;
        bgIcon.sprite = ad.Icon;
        fgIcon.sprite = ad.Icon;

        Color abilityColor = AbilityColorHelper.GetAbilityColor(ad.ColorType);
        bgMaskFrame.color = abilityColor;
        bgMaskIcon.color = abilityColor;
        fgFrame.color = abilityColor;
        
        //transparent and darken for bgbase 
        Color bgColor = AbilityColorHelper.DarkenColor(abilityColor);
        bgFrame.color = bgColor;
        bgIcon.color = bgColor;
        
        onSelectAbility = OnSelectAbility;
        Reset();
    }

    public void Start()
    {
        currentZRotation = transform.rotation.z;
        iconButton.onClick.AddListener(OnAbilityUIButtonClick);
    }
    void OnAbilityUIButtonClick()
    {
        if (!fullyCharged)
        {
            BattleManager.Instance.InputHandler.SelectedAbilityDirection = HexDirection.NONE;
            return;
        }
        BattleManager.Instance.InputHandler.SelectedAbilityDirection = this.direction;
        onSelectAbility?.Invoke();
        ShowAttackPattern();
        BattleManager.Instance.InputHandler.SetInputState(InputState.CastingAbility);
    }

    public void NotifyUpdate(int addOnSteps)
    {
        if (fullyCharged) return;

        chargedSteps = Math.Clamp(chargedSteps + addOnSteps, 0, maxChargeStepsCount);
        float targetFill = (float)chargedSteps / maxChargeStepsCount;


        DOTween.Kill(bgMaskIcon);
        bgMaskIcon.DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad).OnComplete((() =>
            {
                if (chargedSteps >= maxChargeStepsCount)
                {
                    fullyCharged = true;
                    iconButton.interactable = true;
                }
            }));
        
        DOTween.Kill(bgMaskFrame);
        bgMaskFrame.DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad).OnComplete((() =>
            {
                if (chargedSteps >= maxChargeStepsCount)
                {
                    fullyCharged = true;
                    iconButton.interactable = true;
                }
            }));
        
    }
    #region ChargeSteps
    public void Reset()
    {
        chargedSteps = 0;
        bgMaskFrame.fillAmount = 0;
        bgMaskIcon.fillAmount = 0;
        fullyCharged = false;
        DOTween.Kill(bgMaskIcon);
        DOTween.Kill(bgMaskFrame);
        iconButton.interactable = false;
    }
    
    public int GetChargedSteps()
    {
        return chargedSteps;
    }
    
    public void RestoreChargeSteps(int steps)
    {
        chargedSteps = Math.Clamp(steps, 0, maxChargeStepsCount);
        float targetFill = (float)chargedSteps / maxChargeStepsCount;
    
        DOTween.Kill(bgMaskIcon);
        DOTween.Kill(bgMaskFrame);
        
        bgMaskFrame.fillAmount = targetFill;
        bgMaskIcon.fillAmount = targetFill;
    
        if (chargedSteps >= maxChargeStepsCount)
            fullyCharged = true;
    }
    public void SetFullyCharged()
    {
        fullyCharged = true;
        chargedSteps = maxChargeStepsCount;
        bgMaskFrame.fillAmount = 1.0f;
        bgMaskIcon.fillAmount = 1.0f;
        iconButton.interactable = true;
    }
    #endregion

    #region Dragging

    public void ShowAttackPattern()
    {
        currentPattern = localAbilityData.SelectablePattern.GetPattern(BattleManager.Instance.PlayerCell.CellData).ToList();
        BattleManager.Instance.abilityPreviewController.Show(currentPattern);
    }
    public void UseAbility(HexDirection abiltyDirection, HexCellComponent castCell)
    {
        if(abiltyDirection == direction) print("ability dir is synced");
        if (!fullyCharged) return;
        BattleManager.Instance.PlayerActorInstance.ExecuteCastAction(abiltyDirection, castCell);
        Reset();
        UnshownAttackPattern();
        BattleManager.Instance.UpdateValidMoveRange();
    }
    
    public void UnshownAttackPattern()
    {
        BattleManager.Instance.abilityPreviewController.Unshow();
        currentPattern.Clear();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // Snap back to the original position
        UpdateRotation(PlayerActionHudController.Instance.rotationZs[(int)direction]);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // If this is a Canvas in Screen Space - Overlay or Screen Space - Camera mode
        RectTransform hudTransform = PlayerActionHudController.Instance.transform as RectTransform;
        Vector2 localMousePos;
    
        // Convert screen position to local position within the UI canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hudTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localMousePos);
    
        // Calculate direction in UI local space
        Vector2 dir = Vector2.zero - localMousePos; // Assuming hub center is at local 0,0
    
        // Calculate angle in UI local space 
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        angle = ToPositiveAngle(angle);
    
        // Apply rotation (in local space)
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Determine which hex direction this angle corresponds to
        int newDirectionIndex = DetermineHexDirectionFromAngle(angle);
        HexDirection newDirection =
            (HexDirection)((newDirectionIndex + PlayerActionHudController.Instance.cameraRotationCnt) % 6);
    
        // Only swap if the direction has actually changed
        if (newDirection != direction)
        {
            PlayerActionHudController.Instance.SwapAbilitySlot(direction, newDirection);
            // Update the current rotation after swapping
            //currentZRotation = PlayerActionHudController.Instance.rotationZs[(int)direction];
        }
    }

    private int DetermineHexDirectionFromAngle(float angle)
    {
        // Get reference to the rotationZs array
        int[] rotations = PlayerActionHudController.Instance.rotationZs;
    
        // Find the closest rotation angle
        int closestIndex = 0;
        float minDifference = float.MaxValue;
    
        for (int i = 0; i < rotations.Length; i++)
        {
            float rotationAngle = rotations[i];
            float difference = CalculateAngleDifference(angle, rotationAngle);
        
            if (difference < minDifference)
            {
                minDifference = difference;
                closestIndex = i;
            }
        }
    
        return closestIndex;
    }

    public void UpdateRotation(int newZRotation)
    {
        var rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0, 0, newZRotation);
        transform.DOLocalRotateQuaternion(rotation, 0.1f);
        // transform.rotation = rotation;
    }

    public void UpdateDirection(HexDirection newDirection)
    {
        currentZRotation = PlayerActionHudController.Instance.rotationZs[(int)newDirection];
        direction = newDirection;
    }

    float ToPositiveAngle(float angle)
    {
        //same code in input handler
        // Normalize the angle to the range [0, 360)
        angle = angle % 360; // Reduce to [-360, 360]
        if (angle < 0)
        {
            angle += 360; // Convert negative angles to positive
        }
        return angle;
    }
    float CalculateAngleDifference(float angle1, float angle2)
    {
        // Normalize the angles to be within 0-360 degrees
        angle1 = ToPositiveAngle(angle1);
        angle2 = ToPositiveAngle(angle2);

        // Calculate the raw difference
        float difference = Mathf.Abs(angle1 - angle2);

        // If the difference is greater than 180 degrees, take the smaller angle (360 - difference)
        if (difference > 180)
        {
            difference = 360 - difference;
        }

        return difference;
    }

    #endregion
}