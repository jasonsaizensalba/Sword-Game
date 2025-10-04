using UnityEngine;
using UnityEngine.SceneManagement;
public class playGame : MonoBehaviour
{
        public void LoadSpecificScene(string SampleScene)
        {
            SceneManager.LoadScene(SampleScene);
        }
}