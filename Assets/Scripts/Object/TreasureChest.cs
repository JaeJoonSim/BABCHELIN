using UnityEngine;
using UnityEngine.Events;

public class TreasureChest : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    [SerializeField] float dropRange;
    [SerializeField] GameObject[] itemsToDrop;

    private void Update()
    {

    }

    public bool OnInteract(Interactor interactor)
    {
        if (!UIManagerScript.OnUI)
        {
            onInteraction.Invoke();
            Debug.Log($"{gameObject.name} : OnInteracted with Chest");
        }
        return true;
    }

    public void OffInteract()
    {
    }

    public void DropItems()
    {
        int numDrops = Random.Range(1, 3);
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * dropRange;
            Vector3 dropPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y, transform.position.z);

            GameObject item = itemsToDrop[Random.Range(0, itemsToDrop.Length)];
            Instantiate(item, dropPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
