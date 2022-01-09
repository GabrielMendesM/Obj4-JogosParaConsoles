using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisicaGPU : MonoBehaviour
{
    public float vel;
    public int id;
    public ComputeShader computeShader;

    private bool noChao = false;
    [SerializeField] private float massa;

    private int nRodou;
    private float timer;
    private float media;

    void Start()
    {
        massa = transform.localScale.x;
    }

    private void FixedUpdate()
    {
        Cair();
    }

    private void Cair()
    {
        nRodou++;
        timer = Time.realtimeSinceStartup;
        if (!noChao && transform.position.y > transform.localScale.y / 2)
        {
            float velocidadeFinal = -vel + (9.8f / massa) * Time.deltaTime;
            float posicaoFinal = transform.position.y + velocidadeFinal * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, posicaoFinal, transform.position.z);

            timer = Time.realtimeSinceStartup - timer;
            media += timer;
        }
        else if (!noChao)
        {
            sortearCor();
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = massa;

            timer = Time.realtimeSinceStartup - timer;
            media += timer;

            media /= nRodou;

            GerarObjetos.MediaDeExecucao(media);

            noChao = true;

        }
        else if (noChao && transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void sortearCor()
    {
        GerarObjetos.Esfera dado = GerarObjetos.dados[id];
        if (dado.cor.r == 0 && dado.cor.g == 0 && dado.cor.b == 0)
        {
            dado.cor = Random.ColorHSV();
        }

        Color _cor = dado.cor;
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", _cor);
        dado.cor = _cor;
    }
}
