using System;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine;

public class RouteSelectButton : MonoBehaviour
{
    private RouteText _routeText;
    private HowToUseScreen _screen;
    private string _sumoGUI;
    private bool _next;
    private bool _canceled;

    private void Awake()
    {
        _routeText = FindObjectOfType<RouteText>();
        _screen = FindObjectOfType<HowToUseScreen>();
    }

    public void OnRouteSelect() => StartCoroutine(OnRouteSelectEnum());

    private IEnumerator OnRouteSelectEnum()
    {
        _screen.ToggleRaycasts(false);
        
        FileBrowser.SetFilters(false, new FileBrowser.Filter("SUMO-GUI", ".exe"));
        FileBrowser.SetDefaultFilter(".exe");

        do
        {
            _next = false;
            
            FileBrowser.ShowLoadDialog((paths) =>
                {
                    _sumoGUI = paths[0];
                    _next = true;
                },
                () =>
                {
                    _canceled = true;
                    _next = true;
                }, FileBrowser.PickMode.Files);
            
            yield return new WaitWhile(() => !_next);
            
            if (_canceled) break;
            if (!_sumoGUI.Contains("sumo-gui.exe")) print("TODO - Handle Error");
        } while (!_sumoGUI.Contains("sumo-gui.exe") || !_next);

        _screen.ToggleRaycasts(true);
        
        if (_canceled) yield break;

        Utils.SetExecutableRoute(_sumoGUI);
        _routeText.SetRoute(_sumoGUI);
    }
}
