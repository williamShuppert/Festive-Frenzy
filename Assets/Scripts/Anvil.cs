using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Anvil : MonoBehaviour
{
    [SerializeField] float itemSpeed = 5.5f;
    [SerializeField] float rotationSpeed = .9f;
    [SerializeField] float fixItemRotatationSpeed = 10f;
    [SerializeField] float radius;

    public UnityEvent OnComplete;

    Item item;
    Paper paper;
    GameObject bow;

    [SerializeField] List<GameObject> presentPrefabs;

    private void Update()
    {
        if (item)
        {
            item.transform.position = Vector2.MoveTowards(
                item.transform.position,
                transform.position + CalcNewPos(0),
                itemSpeed * Time.deltaTime);

            item.transform.localEulerAngles = Vector3.MoveTowards(item.transform.localEulerAngles, Vector3.zero, fixItemRotatationSpeed * Time.deltaTime);
        }

        if (paper)
        {
            paper.transform.position = Vector2.MoveTowards(
                paper.transform.position,
                transform.position + CalcNewPos(1),
                itemSpeed * Time.deltaTime);

            paper.transform.localEulerAngles = Vector3.MoveTowards(paper.transform.localEulerAngles, Vector3.zero, fixItemRotatationSpeed * Time.deltaTime);
        }

        if (bow)
        {
            bow.transform.position = Vector2.MoveTowards(
                bow.transform.position,
                transform.position + CalcNewPos(2),
                itemSpeed * Time.deltaTime);

            bow.transform.localEulerAngles = Vector3.MoveTowards(bow.transform.localEulerAngles, Vector3.zero, fixItemRotatationSpeed * Time.deltaTime);
        }

        if (item && paper && bow)
        {

            if (Vector2.Distance(item.transform.position, transform.position) > .01f
                || Vector2.Distance(paper.transform.position, transform.position) > .01f
                || Vector2.Distance(bow.transform.position, transform.position) > .01f
                ) return;


            var size = item.size;
            var color = paper.color;

            Destroy(item.gameObject);
            Destroy(paper.gameObject);
            Destroy(bow);
            item = null;
            paper = null;
            bow = null;

            var present = Instantiate(presentPrefabs[(int)size * 3 + (int)color]);

            present.transform.position = transform.position;

            OnComplete.Invoke();

            return;
        }
    }

    private Vector3 CalcNewPos(int segmentOffset)
    {
        var radius = (item && paper && bow) ? 0 : this.radius;

        float angle = segmentOffset * (360f / 3f);
        float radians = Mathf.Deg2Rad * angle + Time.timeSinceLevelLoad * rotationSpeed;
        float x = radius * Mathf.Cos(radians);
        float y = radius * Mathf.Sin(radians);

        return new Vector3(x, y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!item)
        {
            var item = collision.GetComponent<Item>();

            if (item)
            {
                this.item = item;
                collision.enabled = false;
                Destroy(collision.attachedRigidbody);
                collision.transform.localScale *= .8f;
                return;
            }
        }

        if (!paper)
        {
            var paper = collision.GetComponent<Paper>();

            if (paper)
            {
                this.paper = paper;
                collision.enabled = false;
                Destroy(collision.attachedRigidbody);
                collision.transform.localScale *= .8f;
                return;
            }
        }

        if (!bow)
        {
            if (collision.CompareTag("Bow"))
            {
                this.bow = collision.gameObject;
                collision.enabled = false;
                Destroy(collision.attachedRigidbody);
                collision.transform.localScale *= .8f;
                return;
            }
        }
    }
}
