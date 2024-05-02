using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class SantaBag : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private float moveSpeed;
    [SerializeField] GameObject confettiPrefab;

    public UnityEvent OnPresentDelivered;

    private List<Present> presents = new List<Present>();

    private CircleCollider2D trigger;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (presents.Count == 0) return;

        for (int i = 0; i < presents.Count; i++)
        {
            var present = presents[i];
            var presentObj = presents[i].obj;

            var dist = Vector2.Distance(presentObj.transform.position, destination.position);

            presentObj.transform.position = Vector2.MoveTowards(
                presentObj.transform.position, 
                destination.position, 
                Time.deltaTime * moveSpeed
            );

            presentObj.transform.localScale = present.initScale * dist / present.initDist;

            if (dist < .1f)
            {
                var confetti = Instantiate(confettiPrefab);
                confetti.transform.position = destination.position;
                Destroy(confetti.gameObject, 3);

                presents.Remove(present);
                i--;
                Destroy(presentObj.gameObject);

                OnPresentDelivered.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Present")) return;

        collision.GetComponent<Collider2D>().enabled = false;
        Destroy(collision.GetComponent<Rigidbody2D>());

        presents.Add(new Present {
            obj=collision.gameObject,
            initScale=collision.transform.localScale,
            initDist=Vector2.Distance(collision.transform.position, destination.position)
        });
    }

    struct Present
    {
        public GameObject obj;
        public Vector3 initScale;
        public float initDist;
    }
}
