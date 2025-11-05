using SBN.UITool.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SBN.UITool.Core.Elements.Windows
{
    /// <summary>
    /// Base class for any specific UI window you want to able to show
    /// on screen. 
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class UIWindow : UIElement
    {
        [Header("Components")]
        [Tooltip("A default back button which will trigger to go back in window history. Can be null if no back button exists for this window.")]
        [SerializeField] private Button defaultBackButton;

        [Tooltip("A default button to be selected when UI input changes to gamepad.")]
        [SerializeField] private Button defaultSelectedButton;

        private UIElement[] uiElements;

        public Scene OwnerScene
        {
            get;
            private set;
        }
        public Button DefaultSelectedButton 
        { 
            get => defaultSelectedButton; 
            set => defaultSelectedButton = value; 
        }

        protected virtual void OnEnable()
        {
            if (defaultBackButton != null)
                defaultBackButton.onClick.AddListener(OnBackButtonClick);
        }

        protected virtual void OnDisable()
        {
            if (defaultBackButton != null)
                defaultBackButton.onClick.RemoveListener(OnBackButtonClick);
        }

        public void Setup(UIManager uiManager, Scene ownerScene)
        {
            base.Setup(uiManager);

            OwnerScene = ownerScene;
            uiElements = GetComponentsInChildren<UIElement>();

            // Start at index 1 because 0 is this UIWindow object
            // since GetComponentsInChildren also gets on the object itself
            for (int i = 1; i < uiElements.Length; i++)
                uiElements[i].Setup(uiManager);
        }

        protected virtual void OnBackButtonClick()
        {
            UIManager.GoBack();
        }
    }
}