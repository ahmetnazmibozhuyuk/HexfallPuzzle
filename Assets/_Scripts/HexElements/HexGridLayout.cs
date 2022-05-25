using UnityEngine;

namespace Hexfall.HexElements
{
    //odd q vertical layout
    public class HexGridLayout : MonoBehaviour
    {
        //not: eğer x %2 = 1 ise y'si altta demektir koordinatta x 1 arttığında sağ üste erişir.
        //collider'la almayı düşünebilirsin


        private const float SQRT_3 = 1.732050808f;

        [SerializeField] private Vector2 gridSize;
        [SerializeField] private GameObject hexagon;

        [SerializeField] private float distanceMultiplier;

        private void Start()
        {
            LayoutGrid();
        }
        private void LayoutGrid()
        {
            for(int y = 0; y <  gridSize.y; y++)
            {
                for(int x = 0; x < gridSize.x; x++)
                {
                    Hexagon newTile = Instantiate(hexagon, HexPosition(x, y) * distanceMultiplier, transform.rotation, transform).GetComponent<Hexagon>();
                    newTile.Initialize(Color.yellow, new Vector2(x, y));
                }
            }
        }
       private Vector2 HexPosition(float x, float y)
        {
            if(x %2 == 1)
            {
                return new Vector2(x*3,y*2* SQRT_3 - SQRT_3);
            }
            else
            {
                return new Vector2(x*3, y*2 * SQRT_3) ;
            }

        }
    }
    public enum TileType
    {

    }
}
