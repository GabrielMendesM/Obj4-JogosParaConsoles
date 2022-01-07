using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerarObjetos : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int nObjetos;
    private List<GameObject> gameObjects = new List<GameObject>();
    [SerializeField] private Vector3 posMin = new Vector3(-15, 5, -15);
    [SerializeField] private Vector3 posMax = new Vector3(15, 50, 0);
    [SerializeField] private float scaleMin = 1;
    [SerializeField] private float scaleMax = 3;

    [SerializeField] private float velMin = 5;
    [SerializeField] private float velMax = 10;

    private static int cont = 0;

    private static double contMedia = 0;
    private static double media = 0;

    private void Start()
    {
        for (int i = 0; i < nObjetos; i++)
        {
            Vector3 pos = new Vector3(Random.Range(posMin.x, posMax.x), Random.Range(posMin.y, posMax.y), Random.Range(posMin.z, posMax.z));
            float scale = Random.Range(scaleMin, scaleMax);

            if (gameObjects.Count > 0)
            {
                criarEsfera(pos, scale);
            } else
            {
                criarEsfera(pos, scale);
            }
        }
    }
    
    private void criarEsfera(Vector3 pos, float scale)
    {
        gameObjects.Add(Instantiate(prefab));
        gameObjects[cont].transform.position = pos;
        gameObjects[cont].transform.localScale = new Vector3(scale, scale, scale);
        gameObjects[cont].GetComponent<Fisica>().vel = Random.Range(velMin, velMax);

        cont++;
    }

    public static void MediaDeExecucao(double tempo)
    {
        contMedia++;
        media += tempo;
        if (contMedia >= cont)
        {
            media /= contMedia;
            Debug.Log("Média total: " + (media * 10000000));
        }
    }
}
