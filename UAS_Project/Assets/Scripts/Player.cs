using UnityEngine;

public class Player : MonoBehaviour
{
        [SerializeField] private Camera mainCam;
        [SerializeField] private float maxDistance = 10f;

        private Ray ray;

        void Update()
        {
            //HOMEWORK
            //Control : wasd + space(ascend) + ctrl(descend) , camera direction to mouse position.
            //If you can (no pressure) try making movement more 'underwater' like slowly sliding a bit after move

            ray = mainCam.ScreenPointToRay(new Vector2(Screen.width/2f, Screen.height/2f));

                    if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                    {
                        if(hit.collider.TryGetComponent(out Interactable interactable)) //Hover
                        {
                            if(Input.GetMouseButtonDown(0)) //Click
                            {
                                interactable.TryInteract();
                            }
                        }
                    }
        }
}
