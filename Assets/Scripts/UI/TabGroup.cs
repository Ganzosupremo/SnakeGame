using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.UI
{
    [DisallowMultipleComponent]
    public class TabGroup : MonoBehaviour
    {
        private List<Tab> tabButtons;
        public List<GameObject> ObjectsToSwap;

        public Sprite TabIdle;
        public Sprite TabActive;
        public Sprite TabHover;
        public Tab SelectedTab { get { return selectedTab; } set { selectedTab = value; } }
        private Tab selectedTab;

        public void Subscribe(Tab button)
        {
            tabButtons ??= new List<Tab>();
            tabButtons.Add(button);
        }

        public void OnTabEnter(Tab button)
        {
            ResetTabs();
            if (selectedTab == null || button != selectedTab)
                button.Background.sprite = TabHover;
        }

        public void OnTabExit(Tab button)
        {
            ResetTabs();
            button.Background.sprite = TabIdle;
        }

        public void OnTabSelected(Tab button)
        {
            if (selectedTab != null)
            {
                selectedTab.Deselect();
            }

            selectedTab = button;
            selectedTab.Select();
            ResetTabs();
            button.Background.sprite = TabActive;
            int index = button.transform.GetSiblingIndex();

            for (int i = 0; i < ObjectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    ObjectsToSwap[i].SetActive(true);
                }
                else
                {
                    ObjectsToSwap[i].SetActive(false);
                }
            }
        }

        public void ResetTabs()
        {
            foreach (Tab tab in tabButtons)
            {
                if (selectedTab != null && tab == selectedTab) continue;
                tab.Background.sprite = TabIdle;
            }
        }
    }
}
