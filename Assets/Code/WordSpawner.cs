using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnableWord _Word_Prefab;
    [SerializeField]
    private List<Transform> _SpawnPoints;
    [SerializeField]
    private Transform _Canvas;

    [SerializeField]
    private int _WordsToSpawn;

    private void Awake()
    {
        while(_WordsToSpawn > 0)
        {
            _WordsToSpawn--;
            var clone = Instantiate<SpawnableWord>(_Word_Prefab);
            clone.transform.SetParent(_Canvas.transform);
            
            var roll = _RNG.GetRandomInt(_SpawnPoints.Count);
            var spawnPoint = _SpawnPoints[roll];

            clone.transform.localPosition = spawnPoint.localPosition;
            clone.gameObject.SetActive(false);
            _Words.Add(clone);
        }
    }

    private void Start()
    {
        StartCoroutine(StartWords());
    }

    private IEnumerator StartWords()
    {
        for (int i = 0; i < _Words.Count; i++)
        {
            var words = _Words[i];
            var roll = _RNG.GetRandomInt(9);

            words.gameObject.SetActive(true);
            words.Init();
            yield return new WaitForSeconds(.6f);
        }
    }

    private List<SpawnableWord> _Words = new List<SpawnableWord>();
    private RNG _RNG = new RNG();
}
