using UnityEngine;

// a Generic Singleton class

namespace HPC
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T m_ins;

        protected virtual void Awake()
        {
            MakeSingleton(true);
        }

        public static T Ins
        {
            get
            {
                return m_ins;
            }
        }

        public void MakeSingleton(bool destroyOnload)
        {
            if (m_ins == null)
            {
                m_ins = this as T;

                if (!destroyOnload) return;

                var root = transform.root;

                if (root != transform)
                {
                    DontDestroyOnLoad(root);
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}