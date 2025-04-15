using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEditor;

public class HexCellComponent : MonoBehaviour
{
    public HexCell CellData;
    private MeshRenderer meshRenderer;
    [SerializeField] private Material sharedCellMat;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color invalidColor;
    [SerializeField] private Color validMoveRangeColor;
    [SerializeField] private Color validAttackRangeColor;
    [SerializeField] private Color objectStandingColor;

    
    [Header("Debug")]
    public TextMeshProUGUI DebugCoord;
    private Material customCellMat;
    private bool isTweening = false;


    private float originalY;
    public void Initialize(HexCell hexCell)
    {
        CellData = hexCell;
        meshRenderer = this.GetComponent<MeshRenderer>();
        customCellMat = new Material(meshRenderer.material);
        meshRenderer.material = customCellMat;
        UpdateMaterialColor();
        CellData.OnCellTypeChanged += UpdateMaterialColor;
        DebugCoord.text = CellData.Coordinates.ToString();
        DebugManager.Instance.CellsCoordGUI.Add(DebugCoord);
        originalY = transform.localPosition.y;
    }

    private void OnDestroy()
    {
        if (CellData != null)
        {
            CellData.OnCellTypeChanged -= UpdateMaterialColor;
        }
    }

    private void UpdateMaterialColor()
    {
        switch (CellData.CellType)
        {
            case CellType.Empty:
                customCellMat.color = emptyColor;
                break;
            case CellType.Invalid:
            case CellType.Enemy:
                customCellMat.color = invalidColor;
                break;
        }

        switch (CellData.CellActionType)
        {
            case CellActionType.ValidMoveCell:
                customCellMat.color = validMoveRangeColor;
                break;
            case CellActionType.ValidAttackCell:
                customCellMat.color = validAttackRangeColor;
                break;
            case CellActionType.Chest:
                customCellMat.color = Color.green;
                break;
            case CellActionType.Empty:
                UpdateCellTypeColor();
                break;
        }
        meshRenderer.material = customCellMat;
    }

    private void UpdateCellTypeColor()
    {
        switch (CellData.CellType)
        {
            case CellType.Empty:
                customCellMat.color = emptyColor;
                break;
            case CellType.Player:
                customCellMat.color = objectStandingColor;
                break;
            case CellType.Invalid:
            case CellType.Enemy:
                customCellMat.color = invalidColor;
                break;
        }
    }

    public void HighLightCell(AbilityColorType abilityColor)
    {
        if(isTweening) return;
        isTweening = true;
        print(abilityColor.ToString());
        customCellMat.color = AbilityColorHelper.GetAbilityColor(abilityColor);
        transform.DOLocalMoveY(2, 0.1f);
    }
    public void UnhighLightCell()
    {
        if(!isTweening) return;
        isTweening = false;
        UpdateMaterialColor();
        transform.DOLocalMoveY(originalY, 0.1f);
    }
    public Vector3 CalPosForAction()
    {
        return new Vector3(transform.position.x, 0.9f, transform.position.z);
    }

    public void DebugTest()
    {
        this.gameObject.transform.localPosition += Vector3.up * 10;
        foreach (var c in CellData.Neighbors)
        {
            if (c != null)
                Debug.Log($"{c.Coordinates}");
        }
    }

    public void ChangeCellColor(bool pointEnter)
    {
        if (pointEnter)
            meshRenderer.material = customCellMat;
        else
            meshRenderer.material = sharedCellMat;
    }
}