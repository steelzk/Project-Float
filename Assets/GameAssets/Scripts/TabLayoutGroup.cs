using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabLayoutGroup : MonoBehaviour
{
    public List<GameObject> tabs = new List<GameObject>();

    private int currentTab = 0;

    public void ChangeTab(int index)
    {
        currentTab = index;
        RefreshTabs();
    }

    private void RefreshTabs()
    {
        for (int i=0; i<tabs.Count; i++)
        {
            if (i == currentTab)
                tabs[i].SetActive(true);
            else
                tabs[i].SetActive(false);
        }
    }
    
}
