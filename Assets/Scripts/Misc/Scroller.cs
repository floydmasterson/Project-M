using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _y;
    [SerializeField] private float _x;
    [SerializeField] private float _time;

    private bool isCoroutineExecuting = false;


    IEnumerator Timer()
    {
        if (isCoroutineExecuting)
            yield break;

        isCoroutineExecuting = true;

        yield return new WaitForSeconds(_time);


        _x = Random.Range(-.02f, .02f);
        _y = Random.Range(-.02f, .02f);


        isCoroutineExecuting = false;
        StartCoroutine(Timer());
    }
    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(Timer());
    }
    void Update()
    {
      
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}
