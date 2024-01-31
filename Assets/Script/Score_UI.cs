using UnityEngine;
using TMPro;

public class Score_UI : MonoBehaviour
{
    [SerializeField] TMP_Text _txt;
    public void ScoreUpdate(int i)
    {
        GetComponent<Animator>().SetTrigger("GOAL");

        _txt.text = i.ToString();
    }
}
