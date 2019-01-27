using System;
using UnityEngine;

namespace Game
{
    public class CharacterInstantiator : PlayerBehaviour
    {
        [SerializeField]
        private GameObject Cat;
        [SerializeField]
        private GameObject Dog;
        [SerializeField]
        private GameObject Bird;

        [SerializeField]
        private Transform Parent;
        
        public void Create()
        {
            switch (Player.NetworkPlayer.CharacterType)
            {
                case CharacterTypes.Unassigned:
                    throw new NotImplementedException("You can fuck right off mate.");
                case CharacterTypes.Cat:
                    Instantiate(Cat, Parent);
                    break;
                case CharacterTypes.Dog:
                    Instantiate(Dog, Parent);
                    break;
                case CharacterTypes.Bird:
                    Instantiate(Bird, Parent);
                    break;
            }


        }
    }
}