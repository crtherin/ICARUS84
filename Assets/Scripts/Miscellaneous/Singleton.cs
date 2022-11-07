using UnityEngine;

namespace Miscellaneous
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T Instance
        {
            get
            {
                //if (applicationIsQuitting)
                //return null;

                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                if (_instance == null)
                    _instance = new GameObject("_" + typeof(T)).AddComponent<T>();

                return _instance;
            }
        }

        private static T _instance;
    
        //private static bool applicationIsQuitting;

        public static T GetInstance()
        {
            return Instance;
        }

        public static bool HasInstance()
        {
            return _instance;
        }
    
        //protected virtual void OnDestroy()
        //{
        //Application.quitting += () => applicationIsQuitting = true;
        //}
    }
}