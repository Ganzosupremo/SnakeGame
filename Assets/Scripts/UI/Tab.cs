using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
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

        public virtual void Select()
        {
            OnTabSelected?.Invoke();
        }

        public virtual void Deselect()
        {
            OnTabSelected?.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabSelected(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            TabGroup.OnTabExit(this);
        }
    }
}
