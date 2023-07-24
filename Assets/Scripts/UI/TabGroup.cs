using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    [DisallowMultipleComponent]
    public abstract class TabGroup : MonoBehaviour
    {
        public List<GameObject> ObjectsToSwap = new List<GameObject>();

        private static List<Tab> _TabButtonsList;

        [Space(5)]
        public SoundEffectSO TabSelectedSoundEffect;

        [Space(5)]
        public Sprite TabIdle;
        public Sprite TabActive;
        public Sprite TabHover;

        private Tab _selectedTab;

        public static void Subscribe(Tab button)
        {
            _TabButtonsList ??= new List<Tab>();
            _TabButtonsList.Add(button);
        }

        public void OnTabEnter(Tab button)
        {
            ResetTabs();
            if (_selectedTab == null || button != _selectedTab)
                button.Background.sprite = TabHover;
        }

        public void OnTabExit(Tab button)
        {
            ResetTabs();
            button.Background.sprite = TabIdle;
        }

        public void OnTabSelected(Tab tabButton)
        {
            if (_selectedTab != null)
                _selectedTab.Deselect();

            _selectedTab = tabButton;
            _selectedTab.Select();

            ResetTabs();
            tabButton.Background.sprite = TabActive;

            int index = tabButton.transform.GetSiblingIndex();

            for (int i = 0; i < ObjectsToSwap.Count; i++)
            {
                if (i == index)
                    ObjectsToSwap[i].SetActive(true);
                else
                    ObjectsToSwap[i].SetActive(false);
            }
        }

        /// <summary>
        /// Sets the images of all tabs to idle
        /// </summary>
        public void ResetTabs()
        {
            foreach (Tab tab in _TabButtonsList)
            {
                if (_selectedTab != null && tab == _selectedTab) continue;

                tab.Background.sprite = TabIdle;
            }
        }

        public void PlayAudio()
        {
            if (TabSelectedSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(TabSelectedSoundEffect);
        }
    }
}
