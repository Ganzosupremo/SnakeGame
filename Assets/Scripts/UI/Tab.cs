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

        public Button uiButton;
        public Image uiImage;
        public LayoutElement uiLayoutElement;

        private void Awake()
        {
            if (uiButton == null)
                uiButton = GetComponent<Button>();
            if (uiImage == null)
                uiImage = GetComponent<Image>();
            if (uiLayoutElement == null)
                uiLayoutElement = GetComponent<LayoutElement>();
        }

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
            OnTabDeselected?.Invoke();
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
