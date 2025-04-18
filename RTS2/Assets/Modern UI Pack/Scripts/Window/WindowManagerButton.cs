using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(Animator))]
    public class WindowManagerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool enableMobileMode = false;
        [HideInInspector] public Animator buttonAnimator;
        public string PageToOpen;
        void OnEnable()
        {
            if (buttonAnimator == null)
            {
                buttonAnimator = gameObject.GetComponent<Animator>();
              //  this.GetComponent<Button>().onClick.AddListener(()=>this.GetComponentInParent<WindowManager>().OpenPanel(PageToOpen));

            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableMobileMode == true)
                return;

            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed")
                && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Hover");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (enableMobileMode == true)
                return;

            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed")
                && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Hover to Normal");
        }
    }
}
