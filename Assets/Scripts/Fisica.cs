using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fisica : MonoBehaviour
{
    public float vel;

    [SerializeField] private float massa;
    private bool estaNoChao = false;

    private double timer = 0;
    private float nRodou = 0;
    private double mediaTimer = 0;

    void Start()
    {
        massa = transform.localScale.x;
    }

    void Update()
    {
        timer = Time.realtimeSinceStartupAsDouble;

        if (!estaNoChao && transform.position.y > transform.localScale.y / 2)
        {
            Cair();

            nRodou++;
            timer = Time.realtimeSinceStartupAsDouble - timer;
            mediaTimer += timer;
        }
        else if (!estaNoChao)
        {
            //Debug.Log("Cont media individual: " + mediaTimer);
            estaNoChao = true;
            GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
            mediaTimer /= nRodou;
            GerarObjetos.MediaDeExecucao(mediaTimer);
        }
    }

    private void Cair()
    {
        transform.Translate(vel * Vector3.down * Time.deltaTime, Space.World);
    }
}
