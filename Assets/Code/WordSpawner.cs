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

    private IEnumerator _Coroutine;

    private void Awake()
    {
        while(_SpawnPoints.Count > 0)
        {
            var clone = Instantiate<SpawnableWord>(_Word_Prefab);
            clone.transform.SetParent(_Canvas.transform);
            var spawnPoint = _SpawnPoints[0];

            clone.transform.localPosition = spawnPoint.localPosition;
            clone.gameObject.SetActive(false);

            _Words.Add(clone);
            _SpawnPoints.RemoveAt(0);
        }
    }

    public void StartSpawn()
    {
        if(_Coroutine != null)
        {
            StopCoroutine(_Coroutine);
            _Coroutine = null;
        }

        _Coroutine = StartWords();
        StartCoroutine(_Coroutine);
    }

    public void StopSpawn()
    {
        if(_Coroutine != null)
        {
            StopCoroutine(_Coroutine);
            _Coroutine = null;
        }
        
        for (int i = 0; i < _Words.Count; i++)
        {
            _Words[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator StartWords()
    {
        for (int i = 0; i < _Words.Count; i++)
        {
            var words = _Words[i];

            words.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            words.Init();
        }
    }

    private List<SpawnableWord> _Words = new List<SpawnableWord>();
    private RNG _RNG = new RNG();
}
