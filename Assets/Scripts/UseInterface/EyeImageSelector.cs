using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.ComponentModel.Composition;


public class EyeImageSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform selectionBoxUI;
    private RectTransform imageRectTransform;
    private Vector2 startMousePos;
    private Vector2 endMousePos;
    private bool isSelecting = false;
    private bool isCroped = false; // Flag to check if the image is cropped
    [SerializeField] private GuiHub guiHub; // Padding around the selection box

    [SerializeField] private string eyeSide;

    // Normalized coordinates (0-1) of the selected area (min/max)

    void Start()
    {
        imageRectTransform = GetComponent<RectTransform>();

        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, eventData.position, eventData.pressEventCamera, out startMousePos);
        isSelecting = true;

        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isSelecting)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, eventData.position, eventData.pressEventCamera, out endMousePos);
        UpdateSelectionBox();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isSelecting)
            return;

        isSelecting = false;

        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(false);
        }

        CalculateNormalizedCoordinates();
    }

    void UpdateSelectionBox()
    {
        if (selectionBoxUI == null)
            return;

        Vector2 size = endMousePos - startMousePos;
        selectionBoxUI.anchoredPosition = startMousePos + size / 2;
        selectionBoxUI.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
    }

    void CalculateNormalizedCoordinates()
    {
        Vector2 imgSize = imageRectTransform.rect.size;

        Vector2 min = new Vector2(Mathf.Min(startMousePos.x, endMousePos.x), Mathf.Min(startMousePos.y, endMousePos.y));
        Vector2 max = new Vector2(Mathf.Max(startMousePos.x, endMousePos.x), Mathf.Max(startMousePos.y, endMousePos.y));

        List<List<float>> normalizedCoordinates = new List<List<float>>();

        if (eyeSide == "Left"){
            normalizedCoordinates = new List<List<float>>
            {
                new List<float>() { RoundToThreeDecimals((min.y + imgSize.y / 2) / imgSize.y) / 2, RoundToThreeDecimals((max.y + imgSize.y / 2) / imgSize.y) / 2},
                new List<float>() { RoundToThreeDecimals((min.x + imgSize.x / 2) / imgSize.x), RoundToThreeDecimals((max.x + imgSize.x / 2) / imgSize.x)}
            };
        }
        else if (eyeSide == "Right"){
            normalizedCoordinates = new List<List<float>>
            {
                new List<float>() { RoundToThreeDecimals((min.y + imgSize.y / 2) / imgSize.y) / 2 + 0.5f, RoundToThreeDecimals((max.y + imgSize.y / 2) / imgSize.y) / 2 + 0.5f},
                new List<float>() { RoundToThreeDecimals((min.x + imgSize.x / 2) / imgSize.x), RoundToThreeDecimals((max.x + imgSize.x / 2) / imgSize.x)}
            };
        }
        else
            UnityEngine.Debug.LogError($"Wrong side assigned to ImageSelector: {eyeSide}");

        sendCrop(normalizedCoordinates);
    }

    // Optional: Helper to get crop area in relative format
    private void sendCrop(List<List<float>> normalizedCoordinates, bool force = false)
    {
        if (isCroped) // Check if crop is already applied
            return; // If crop is already applied, do not send again
        if (eyeSide == "Left")
            guiHub.SendConfig("tracker_config crop_left", normalizedCoordinates);
        else if (eyeSide == "Right")
            guiHub.SendConfig("tracker_config crop_right", normalizedCoordinates);
        else
            UnityEngine.Debug.LogError($"Wrong side assigned to ImageSelector: {eyeSide}");
        
        if (force == false) // If force is true, send the crop even if it is already applied
            isCroped = true; // Set the flag to true when crop is applied
      
    }

    public void resetScale(){
        List<List<float>> normalizedCoordinates = new List<List<float>>();
        if (eyeSide == "Left"){
            normalizedCoordinates = new List<List<float>>
        {
            new List<float>() {0f, 0.5f},
            new List<float>() {0f, 1f}
        };
        }
        else if (eyeSide == "Right"){
            normalizedCoordinates = new List<List<float>>
            {
                new List<float>() {0.5f, 1f},
                new List<float>() {0, 1f}
            };
        }
        else
            UnityEngine.Debug.LogError($"Wrong side assigned to ImageSelector: {eyeSide}");
        isCroped = false; // Reset the flag when crop is removed
        sendCrop(normalizedCoordinates, true);
    }

    float RoundToThreeDecimals(float value){
        return Mathf.Round(value * 1000f) / 1000f;
    }
}
