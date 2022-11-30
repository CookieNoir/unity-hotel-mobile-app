using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hotel.Models
{
    public class RoleMatcher : MonoBehaviour
    {
        [SerializeField] private List<Role> _matchingRoles;
        [SerializeField] private UnityEvent<bool> _Action;

        public void Match(Role role)
        {
            _Action.Invoke(_matchingRoles.Contains(role));
        }
    }
}