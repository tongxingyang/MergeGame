using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ViewCanvasShop : ViewCanvas
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button[] _navigation;
    [SerializeField] private GameObject[] _pricePanel;
    [SerializeField] private GameObject[] _open;
    [SerializeField] private GameObject[] _close;

    private UnityAction<int> _mainNav;

    private void Start() {
        foreach(var nav in _navigation) {
            nav.onClick.AddListener(() => { _mainNav.Invoke(nav.transform.GetSiblingIndex()); });
            _mainNav += (index) => {
                foreach(var price in _pricePanel) {
                    int tempIdx = price.transform.GetSiblingIndex();
                    price.SetActive(index == tempIdx);
                    _open[tempIdx].SetActive(index == price.transform.GetSiblingIndex());
                    _close[tempIdx].SetActive(index != price.transform.GetSiblingIndex());
                }
            };
        }
    }
}
