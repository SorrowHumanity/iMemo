namespace Assets.Scripts.UI
{
    using UnityEngine;

    public class ScreenFader : MonoBehaviour
    {
        public void FadingOut()
        {
            GameObject.Find("ScreenFader").GetComponent<Animation>().Play("ToBlack");
        }

        public void FadingIn()
        {
            GameObject.Find("ScreenFader").GetComponent<Animation>().Play("ToClear");
        }
    }
}
