using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject menu;
    
    public TimerUI timerUI;
   public void ScenePlay()
   {
         UnityEngine.SceneManagement.SceneManager.LoadScene("EscenaPlay");
   }
    
   public void SceneMenu()
   {
       UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
   }
   
   public void Quit()
   {
       Application.Quit();
   }

   public void Return()
   {
       menu.SetActive(false);
       Time.timeScale = 1;
       timerUI.StartTimer();
       
   }    
}
