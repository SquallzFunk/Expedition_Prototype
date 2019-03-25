﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionPanel : MonoBehaviour
{
    public Text Title;
    public Text Movement;
    public Text HexPath;
    public Text TurnCount;

    MouseController mouseController;
   // Unit unit;

    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
    }

    void Update()
    {
        if (mouseController.SelectedUnit != null)
        {
            Title.text = mouseController.SelectedUnit.Name;
            Movement.text = string.Format("Movement: {0}/{1}", mouseController.SelectedUnit.MovementRemaining, mouseController.SelectedUnit.Movement);
            Hex[] hexPath = mouseController.SelectedUnit.GetHexPath();
            HexPath.text  = hexPath == null ? "0" : hexPath.Length.ToString();
            TurnCount.text = ("Turn: " + mouseController.SelectedUnit.numOfTurns);
        }
    }
}
