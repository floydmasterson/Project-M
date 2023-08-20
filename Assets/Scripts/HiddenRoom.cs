using UnityEngine;

public class HiddenRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject walls;
    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private GameObject[] replacedWalls;
    [SerializeField]
    private GameObject[] replacedFloors;
  [SerializeField]
    private WeightedRandomList<GameObject> PossibleRooms;
    private const string lifeWell = "Life Well";
    public void Selected()
    {
        walls.SetActive(true);
        floor.SetActive(true);
        foreach(GameObject wall in replacedWalls)
        {
            wall.SetActive(false);
        }
        GameObject selectedRoom = PossibleRooms.GetRandom();
        selectedRoom.SetActive(true);
        if(selectedRoom.name == lifeWell)
        {
            foreach (GameObject floor in replacedFloors)
            {
                
                floor.SetActive(false);
            }
        }
    }
   
}
