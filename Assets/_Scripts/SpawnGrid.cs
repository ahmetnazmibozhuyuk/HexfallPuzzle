using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexfall.HexElements
{
    public class SpawnGrid : MonoBehaviour
    {
        private HexGridLayout _hexGridLayout;
        // Start is called before the first frame update

        //Grid burdan instantiate olur; variablelar vs burdan verilir, gridlayout listesi gridout içinde initialize edilirken
        //referans olarak gamemanager'a iletilir
        void Start()
        {
            //_hexGridLayout = new HexGridLayout();
            //_hexGridLayout.Initialize()
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
