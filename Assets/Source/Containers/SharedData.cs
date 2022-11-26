using UnityEngine;
using Hotel.Models;

namespace Hotel.Containers
{
    public class SharedData : MonoBehaviour
    {
        public User User { get; set; }
        public Room Room { get; set; }
    }
}