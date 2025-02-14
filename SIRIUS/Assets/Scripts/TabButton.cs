using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Handles logic for tab buttons
/// </summary>

// Ensures script can only be used when image component exists on its game object
[RequireComponent(typeof(Image))]

public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Game object with tab group script")]
    public TabGroup tabGroup;

    [Tooltip("Background Image of UI Element. Will set automatically if not set")]
    public Image background;
    
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    void Start()
    {
        // Finding image component in this game object
        background = GetComponent<Image>();

        // Add button logic through tab group
        tabGroup.Subscribe(this);
    }

    /// <summary>
    /// Called when tab rect transform is clicked
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    /// <summary>
    /// Called when tab rect transform is hovered over
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    /// <summary>
    /// Called when tab rect transform stopped being hovered over
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    /// <summary>
    /// Triggers on tab selected event
    /// </summary>
    public void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }

    /// <summary>
    /// Triggers on tab deselected event
    /// </summary>
    public void Deselect()
    {
        onTabDeselected.Invoke();
    }
}
