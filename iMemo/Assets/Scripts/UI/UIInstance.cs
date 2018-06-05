namespace Assets.Scripts.UI
{
    using UnityEngine;

    public class UIInstance : MonoBehaviour
    {
        private static UIInstance instance;

        [SerializeField] public GameObject Ui;

        private UIInstance()
        {
            Instantiate(Ui, Vector2.zero, Quaternion.identity);
        }

        public static UIInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIInstance();
                }
                return instance;
            }
        }
    }
}
