using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DetailedCreditsScript : MonoBehaviour
{
    [SerializeField] GameObject[] array;
    [SerializeField] TextMeshProUGUI[] text;
    [SerializeField] AudioSource audioSource;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        array[index].gameObject.SetActive(true);
        text[index].text = (index+1) + " out of " + array.Length.ToString();
        for(int i = index +1; i < array.Length; i++)
        {
            array[i].gameObject.SetActive(false);
            text[i].text = (i + 1) + " out of " + array.Length.ToString();
        }

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }
    public void NextPage()
    {
        audioSource.Play();
        array[index].gameObject.SetActive(false);
        index = (index+ 1) % array.Length;
        array[index].gameObject.SetActive(true);
    }

    public void PrevPage()
    {
        audioSource.Play();
        array[index].gameObject.SetActive(false);
        index--;
        if(index < 0) 
            index = array.Length - 1;

        array[index].gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        audioSource.Play();
        SceneManager.LoadScene("Main Menu");
    }
}
