using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngreInven: MonoBehaviour
{
    [Tooltip("아이템 데이터 베이스")]
    [SerializeField]
    private ItemDatabase itemDatabase;
    public ItemDatabase ItemDatabase { get { return itemDatabase; } }

    public GameObject eee;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
