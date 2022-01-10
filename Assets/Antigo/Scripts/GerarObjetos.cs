using System.Collections.Generic;
using UnityEngine;

public class GerarObjetos : MonoBehaviour
{
    public struct Esfera
    {
        public Vector3 posicao;
        public Color cor;
    }

    [SerializeField] private int iteracoes;
    [SerializeField] ComputeShader computeShader;
    [SerializeField] private GameObject prefabCPU;
    [SerializeField] private GameObject prefabGPU;
    [SerializeField] private int nObjetos;
    private List<GameObject> gameObjects = new List<GameObject>();
    public static Esfera[] dados;
    [SerializeField] private Vector3 posMin = new Vector3(-15, 5, -15);
    [SerializeField] private Vector3 posMax = new Vector3(15, 50, 0);
    [SerializeField] private float massaMin = 1;
    [SerializeField] private float massaMax = 3;

    [SerializeField] private float velMin = 5;
    [SerializeField] private float velMax = 10;

    private static int cont = 0;

    private static int contMedia;
    private static float media;

    private void Start()
    {
        dados = new Esfera[nObjetos];
    }

    private void Update()
    {
    }

    private void OnGUI()
    {
        if (gameObjects.Count <= 0)
        {
            if (GUI.Button(new Rect(5, 5, 75, 30), "CPU"))
            {
                for (int i = 0; i < nObjetos; i++)
                {
                    Vector3 pos = new Vector3(Random.Range(posMin.x, posMax.x), Random.Range(posMin.y, posMax.y), Random.Range(posMin.z, posMax.z));
                    float scale = Random.Range(massaMin, massaMax);

                    criarEsferaCPU(pos, scale);
                }
            }

            if (GUI.Button(new Rect(85, 5, 75, 30), "GPU"))
            {
                for (int i = 0; i < nObjetos; i++)
                {
                    Vector3 pos = new Vector3(Random.Range(posMin.x, posMax.x), Random.Range(posMin.y, posMax.y), Random.Range(posMin.z, posMax.z));
                    float scale = Random.Range(massaMin, massaMax);

                    criarEsferaGPU(pos, scale);
                }
                configurarShader();
            }
        }/*
        else
        {
            if (GUI.Button(new Rect(5, 5, 120, 30), "Sortear cores"))
            {
                sortearCoresGPU();
            }
        }*/
    }

    private void criarEsferaCPU(Vector3 pos, float scale)
    {
        gameObjects.Add(Instantiate(prefabCPU, pos, Quaternion.identity));
        gameObjects[cont].transform.localScale = new Vector3(scale, scale, scale);
        gameObjects[cont].GetComponent<Fisica>().vel = Random.Range(velMin, velMax);
        gameObjects[cont].GetComponent<Fisica>().iteracoes = iteracoes;

        cont++;
    }

    private void criarEsferaGPU(Vector3 pos, float scale)
    {
        gameObjects.Add(Instantiate(prefabGPU, pos, Quaternion.identity));
        gameObjects[cont].transform.localScale = new Vector3(scale, scale, scale);
        FisicaGPU fisicaGPU = gameObjects[cont].GetComponent<FisicaGPU>();
        fisicaGPU.vel = Random.Range(velMin, velMax);
        fisicaGPU.id = cont;
        fisicaGPU.computeShader = computeShader;
        fisicaGPU.iteracoes = iteracoes;

        dados[cont] = new Esfera();
        dados[cont].posicao = pos;
//        dados[cont].cor = Random.ColorHSV();

        cont++;
    }

    private void configurarShader()
    {
        int tamanhoTotal = 4 * sizeof(float) + 3 * sizeof(float);
        ComputeBuffer computeBuffer = new ComputeBuffer(dados.Length, tamanhoTotal);
        computeBuffer.SetData(dados);

        computeShader.SetBuffer(0, "esferas", computeBuffer);
        computeShader.SetInt("iteracoes", iteracoes);

        computeShader.Dispatch(0, dados.Length / 10, 1, 1);

        computeBuffer.GetData(dados);
        
        for (int i = 0; i < gameObjects.Count; i++)
        {
            dados[i].cor = Random.ColorHSV();
        }

        computeBuffer.Dispose();
    }

    public static void MediaDeExecucao(float tempo)
    {
        contMedia++;
        media += tempo;
        if (contMedia >= cont)
        {
            media /= contMedia;
            Debug.Log("Média total: " + media * 1000);
        }
    }
}
