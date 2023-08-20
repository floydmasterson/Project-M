using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI loading => gameObject.GetComponent<TextMeshProUGUI>();
    private const string load0 = "Loading";
    private const string load1 = "Loading.";
    private const string load2 = "Loading..";
    private const string load3 = "Loading...";
    private IEnumerator LoadingTextChange()
    {
        loading.text = load1;
        yield return new WaitForSecondsRealtime(.3f);
        loading.text = load2;
        yield return new WaitForSecondsRealtime(.3f);
        loading.text = load3;
        yield return new WaitForSecondsRealtime(.3f);
        loading.text = load0;
        yield return new WaitForSecondsRealtime(.3f);
        StartCoroutine(LoadingTextChange());
    }
    private void Awake()
    {
        StartCoroutine(LoadingTextChange());
    }
}
