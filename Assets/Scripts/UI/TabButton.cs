using System;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class TabButton : MonoBehaviour
    {
        public TabGroup TabGroup;

        private int m_Index = 0;


        public void SelectNextPage()
        {
            if (m_Index >= TabGroup.ObjectsToSwap.Count - 1) return;
            m_Index++;

            //ResetObjectsToSwap();
            MoveToSelectedObject(m_Index);
            

            //for (int i = 0; i < TabGroup.ObjectsToSwap.Count; i++)
            //{
            //    if (i == m_Index)
            //    {
            //        TabGroup.ObjectsToSwap[m_Index].SetActive(true);
            //    }
            //    else
            //    {
            //        TabGroup.ObjectsToSwap[m_Index].SetActive(false);
            //    }
            //}
        }

        public void SelectPreviousPage()
        {
            if (m_Index == 0) return;
            m_Index--;

            //ResetObjectsToSwap();
            MoveToSelectedObject(m_Index);
            //for (int i = 0; i < TabGroup.ObjectsToSwap.Count; i++)
            //{
            //    if (i == m_Index)
            //    {
            //        TabGroup.ObjectsToSwap[m_Index].SetActive(true);
            //    }
            //    else
            //    {
            //        TabGroup.ObjectsToSwap[m_Index].SetActive(false);
            //    }
            //}
        }

        private void MoveToSelectedObject(int index)
        {
            for (int i = 0; i < TabGroup.ObjectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    TabGroup.ObjectsToSwap[index].SetActive(true);
                }
                else
                {
                    TabGroup.ObjectsToSwap[index].SetActive(false);
                }
            }


        }

        private void ResetObjectsToSwap()
        {


            foreach (GameObject gameObject in TabGroup.ObjectsToSwap)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
