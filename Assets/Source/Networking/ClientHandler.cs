using UnityEngine;

namespace Hotel.Networking
{
    [RequireComponent(typeof(IClient))]
    public class ClientHandler : MonoBehaviour
    {
        public IClient Client { get; private set; }

        private void Awake()
        {
            Client = GetComponent<IClient>();
        }
    }
}