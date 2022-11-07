using UnityEngine;
using UnityEngine.Events;

public class ResetGame : MonoBehaviour
{
   [SerializeField] private UnityEvent callback;

   protected void Start()
   {
      callback.Invoke();
   }
}
