using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles logic for tab groups
/// </summary>

public class TabGroup : MonoBehaviour
{
    public Color tabHover;
    public Color tabIdle;
    public Color tabSelected;

    [Tooltip("Gameobject with tab button script")]
    public List<TabButton> tabButtons;

    [Tooltip("Gameobject holding UI pages or panels")]
    public List<GameObject> objectsToSwap;

    private TabButton selectedTab;

    /// <summary>
    /// Add tab button to tab group
    /// </summary>
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    /// <summary>
    /// Holds logic for hovering over a tab button
    /// </summary>
    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = tabHover;
        }
    }

    /// <summary>
    /// Holds logic for exiting hover of tab button
    /// </summary>
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    /// <summary>
    /// Set panel to active from tab selected based on index in layout group
    /// </summary>
    public void OnTabSelected(TabButton button)
    {
        // Deselect previously selected tab
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        // Set selected tab to current button
        selectedTab = button;

        // Invoke selected tab event
        selectedTab.Select();

        // Set color for unselected tabs
        ResetTabs();

        button.background.color = tabSelected;

        // Get index button gameobject is in order of siblings
        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i== index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Change background color of unselected tabs to idle color
    /// </summary>
    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.color = tabIdle;

        }
    }
}
