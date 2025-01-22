using UnityEngine;
using TMPro;
using UnityEngine.VFX;

public class Score_UI : MonoBehaviour
{

    [SerializeField] TMP_Text _txt;
    [SerializeField] GameObject _vfx;

    private void Start()
    {
        _vfx.SetActive(false);
    }
    public void ScoreUpdate(int i)
    {
        GetComponent<Animator>().SetTrigger("GOAL");

        PlayFX();
        _txt.text = i.ToString();
    }

    public void PlayFX()
    {
        if (_vfx.activeInHierarchy == false)
        {
            _vfx.SetActive(true);
        }

        _vfx.GetComponent<VisualEffect>().Play();
    }
}
