using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public class SleepHandler : MonoBehaviour
{
    [SerializeField] private GameObject _confirmPanel;

    private void OnMouseDown()
    {
        _confirmPanel.SetActive(true);
        ScaleAnimator.PlayScale(_confirmPanel.transform);
    }

}
