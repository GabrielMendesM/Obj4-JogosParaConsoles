using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fisica : MonoBehaviour
{
    public float vel;

    [SerializeField] private float massa;

    void Start()
    {
        massa = transform.localScale.x;
    }

    void Update()
    {
        Cair();
    }

    private void Cair()
    {
        if (transform.position.y > transform.localScale.y / 2)
        {
            calcularPosicao(-vel, 9.8f, transform.position.y);
        } else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
            Destroy(this);
        }
    }
    
    private void calcularPosicao(float velocidadeAtual, float forca, float posicaoAtual)
    {
        float velocidadeFutura = velocidadeAtual + (forca / massa) * Time.deltaTime;
        float posicaoFutura = posicaoAtual + velocidadeFutura * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, posicaoFutura, transform.position.z);
    }
}
