using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Xml.Serialization;
using UnityEngine.Events;

namespace SnakeGame.UI
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public TabGroup TabGroup;
        public Image Background;
        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeselected;

        private void Start()
        {
            Background = GetComponent<Image>();
            TabGroup.Subscribe(this);
        }

        public void Select()
        {
            OnTabSelected?.Invoke();
        }

        public void Deselect()
        {
            OnTabSelected?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TabGroup.OnTabExit(this);
        }
    }
}
