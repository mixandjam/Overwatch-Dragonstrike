using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonMovement : MonoBehaviour
{

    public float speed = 5;

    [Space]
    [Header("Shader Settings")]
    public float initialDissolveValue;
    public float finalDissolveValue;
    public float dissolveSpeed = 10;
    public float firstDragonOffset = 3;

    [Space]
    public float destroyTime = 5;

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        foreach(Material m in renderers[0].materials)
        {
            m.SetFloat("_TimeOffset", firstDragonOffset);
        }

        foreach(Renderer r in renderers)
        {
            Material[] materials = r.materials;

            foreach (Material m in materials)
            {
                m.SetFloat("_SplitValue", initialDissolveValue);
                m.DOFloat(initialDissolveValue, "_SplitValue", 1).SetDelay(destroyTime).OnComplete(() => Destroy(gameObject));
            }
        }

    }

    void Update()
    {
        transform.localPosition += transform.forward * Time.deltaTime * speed;

        foreach (Renderer r in renderers)
        {
            Material[] materials = r.materials;
            foreach (Material m in materials)
            {
                m.SetFloat("_SplitValue", m.GetFloat("_SplitValue") + (dissolveSpeed * Time.deltaTime * speed));
            }
        }
    }
}
