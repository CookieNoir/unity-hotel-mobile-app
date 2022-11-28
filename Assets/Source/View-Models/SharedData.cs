using UnityEngine;
using Hotel.Models;

namespace Hotel.ViewModels
{
    public class SharedData : MonoBehaviour
    {
        public User User { get; set; }
        public Room Room { get; set; }
    }
}