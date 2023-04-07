using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEvents : BaseMonoBehaviour
{
    public delegate void TriggerEvent(Collider2D collider);

    public delegate void CollisionEvent(Collision2D collision);

    public bool debug;

    public bool Performance;

    public bool DestroyOnComplete;

    private List<Collider2D> OnTriggerEnterColList;

    private int CountPerFrame = 2;

    public event TriggerEvent OnTriggerEnterEvent;

    public event TriggerEvent OnTriggerExitEvent;

    public event TriggerEvent OnTriggerStayEvent;

    public event CollisionEvent OnCollisionEnterEvent;

    public event CollisionEvent OnCollisionExitEvent;

    public event CollisionEvent OnCollisionStayEvent;

    private void Awake()
    {
        OnTriggerEnterColList = new List<Collider2D>();
    }

    public void BeginTriggerEnter(float time)
    {
        StartCoroutine(StartTriggerEnter(time / (float)OnTriggerEnterColList.Count));
    }

    private IEnumerator StartTriggerEnter(float time)
    {
        if (OnTriggerEnterColList.Count > 0)
        {
            int i = 0;
            while (i < OnTriggerEnterColList.Count)
            {
                for (int j = 0; j < CountPerFrame; j++)
                {
                    if (i >= OnTriggerEnterColList.Count)
                    {
                        i = 0;
                    }
                    if (OnTriggerEnterColList[i] != null)
                    {
                        try
                        {
                            this.OnTriggerEnterEvent?.Invoke(OnTriggerEnterColList[i]);
                        }
                        catch
                        {
                            i++;
                            continue;
                        }
                    }
                    i++;
                }
                yield return new WaitForSeconds(time);
            }
        }
        if (DestroyOnComplete)
        {
            Object.Destroy(base.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Performance)
        {
            OnTriggerEnterColList.Add(collider);
        }
        else
        {
            this.OnTriggerEnterEvent?.Invoke(collider);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        this.OnTriggerExitEvent?.Invoke(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        this.OnTriggerStayEvent?.Invoke(collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.OnCollisionEnterEvent?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        this.OnCollisionExitEvent?.Invoke(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        this.OnCollisionStayEvent?.Invoke(collision);
    }

    public void SetActive(bool active)
    {
        if (debug)
        {
            Debug.Log($"ColliderEvents on {base.gameObject.name} active: {active}", base.gameObject);
        }
        if (base.gameObject.activeSelf != active)
        {
            base.gameObject.SetActive(active);
        }
    }
}
