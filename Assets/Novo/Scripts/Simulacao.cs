using UnityEngine;

public class Simulacao : MonoBehaviour
{
    public struct Esfera
    {
        public Vector3 posicao;
        public Color cor;
        public float massa;
        public float velocidade;
        public int estaNoChao;
    }

    [SerializeField] private int iteracoes;
    [SerializeField] ComputeShader corShader;
    [SerializeField] ComputeShader fisicaShader;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int nObjetos;

    private bool isCPU;
    private GameObject[] esferas;
    private Esfera[] dados;
    [SerializeField] private Vector3 posMin = new Vector3(-15, 5, -15);
    [SerializeField] private Vector3 posMax = new Vector3(15, 50, 0);
    [SerializeField] private float massaMin = 1;
    [SerializeField] private float massaMax = 3;

    [SerializeField] private float velMin = 5;
    [SerializeField] private float velMax = 10;

    private int nRodou = 0;
    private float timer;
    private float media;

    private void FixedUpdate()
    {
        nRodou++;
        timer = Time.realtimeSinceStartup;

        if (esferas != null)
        {
            if (isCPU)
            {
                for (int i = 0; i < nObjetos; i++)
                {
                    if (dados[i].estaNoChao == 0 && esferas[i].transform.position.y > dados[i].massa / 2)
                    {
                        float velFinal = -dados[i].velocidade + (9.8f / dados[i].massa) * Time.deltaTime;
                        float posFinal = dados[i].posicao.y + velFinal * Time.deltaTime;
                        esferas[i].transform.position = new Vector3(esferas[i].transform.position.x, posFinal, esferas[i].transform.position.z);
                        dados[i].posicao = esferas[i].transform.position;
                    }
                    else if (dados[i].estaNoChao == 0)
                    {
                        Material mat = esferas[i].GetComponent<MeshRenderer>().material;
                        for (int j = 0; j < iteracoes; j++)
                        {
                            mat.SetColor("_Color", Random.ColorHSV());
                        }
                        Rigidbody rb = esferas[i].AddComponent<Rigidbody>();
                        rb.mass = dados[i].massa;

                        dados[i].estaNoChao = 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < nObjetos; i++)
                {
                    if (dados[i].estaNoChao == 0 && esferas[i].transform.position.y > dados[i].massa / 2)
                    {
                        //ShaderFisica();
                        esferas[i].transform.position = dados[i].posicao;
                    }
                    else if (dados[i].estaNoChao == 0)
                    {
                        ShaderCor();
                        Color _cor = dados[i].cor;
                        esferas[i].GetComponent<MeshRenderer>().material.SetColor("_Color", _cor);
                        dados[i].cor = _cor;

                        Rigidbody rb = esferas[i].AddComponent<Rigidbody>();
                        rb.mass = dados[i].massa;

                        dados[i].estaNoChao = 1;
                    }
                }
            }
            timer = Time.realtimeSinceStartup - timer;
            media += timer;

            for (int i = 0; i < nObjetos; i++)
            {
                if (dados[i].estaNoChao == 0)
                {
                    return;
                }
            }
            MediaDeExecucao();
            Destroy(gameObject);
        }
    }

    private void OnGUI()
    {
        if (esferas == null)
        {
            if (GUI.Button(new Rect(5, 5, 75, 30), "CPU"))
            {
                isCPU = true;
                criarEsferas();
            }

            if (GUI.Button(new Rect(85, 5, 75, 30), "GPU"))
            {
                isCPU = false;
                criarEsferas();
            }
        } else
        {
            if (GUI.Toggle(new Rect(5, 5, 75, 30), true, ""))
            {
                ShaderFisica();
            }
        }
    }

    private void criarEsferas()
    {
        dados = new Esfera[nObjetos];
        esferas = new GameObject[nObjetos];

        for (int i = 0; i < nObjetos; i++)
        {
            Color cor = Random.ColorHSV();
            Vector3 pos = new Vector3(Random.Range(posMin.x, posMax.x), Random.Range(posMin.y, posMax.y), Random.Range(posMin.z, posMax.z));
            float massa = Random.Range(massaMin, massaMax);

            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            go.transform.localScale = new Vector3(massa, massa, massa);
            esferas[i] = go;
            dados[i] = new Esfera();
            dados[i].posicao = pos;
            dados[i].cor = cor;
            dados[i].massa = massa;
            dados[i].velocidade = Random.Range(velMin, velMax);
            dados[i].estaNoChao = 0;
        }
    }

    private void ShaderCor()
    {
        int tamanhoTotal = 9 * sizeof(float) + sizeof(int);
        ComputeBuffer computeBuffer = new ComputeBuffer(dados.Length, tamanhoTotal);
        computeBuffer.SetData(dados);

        corShader.SetBuffer(0, "esferas", computeBuffer);
        corShader.SetInt("iteracoes", iteracoes);
        corShader.Dispatch(0, dados.Length / 10, 1, 1);
        
        computeBuffer.GetData(dados);

        computeBuffer.Dispose();
    }

    private void ShaderFisica()
    {
        int tamanhoTotal = 9 * sizeof(float) + sizeof(int);
        ComputeBuffer computeBuffer = new ComputeBuffer(dados.Length, tamanhoTotal);
        computeBuffer.SetData(dados);

        fisicaShader.SetBuffer(0, "esferas", computeBuffer);
        fisicaShader.SetFloat("deltaTime", Time.deltaTime);
        fisicaShader.Dispatch(0, dados.Length / 10, 1, 1);

        computeBuffer.GetData(dados);

        computeBuffer.Dispose();
    }
     
    public void MediaDeExecucao()
    {
        media /= nRodou;
        Debug.Log(media);
    }
}
