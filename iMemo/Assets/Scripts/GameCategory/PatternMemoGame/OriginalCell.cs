namespace Assets.Scripts.GameCategory.PatternMemoGame
{
    using UnityEngine;

    public class OriginalCell : MonoBehaviour
    {
        [SerializeField] public PatternMemoSceneController PatternMemoSceneController;

        public Material Material
        {
            get
            {
                return GetComponent<Renderer>().material;
            }
        }

        /// <summary>
        ///     Create colored material used for selected/clicked cells.
        /// </summary>
        public Material ColoredMaterial
        {
            get { return new Material(Shader.Find("Diffuse")) { color = new Color(255, 255, 0) }; }
        }

        public static bool CellsHidden { get; set; }

        private void OnMouseDown()
        {
            if (CellsHidden)
            {
                PatternMemoSceneController.ExecuteClickCheck(this);
            }
        }

        /// <summary>
        ///    Change cell material to the given material.
        ///     Used when creating and hiding cells annd when the user clicks on a cell.
        /// </summary>
        /// <param name ="material">Material to be set as the new cell material.</param>
        public void ChangeCellMaterial(Material material)
        {
            GetComponent<Renderer>().material = material;
        }
    }
}