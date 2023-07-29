using Characters;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class PlayerPanel : IDisposable
    {
        public string name;
        public int points;

        private TMP_Text _text;
        private ShipController _shipController;


        public PlayerPanel(ShipController shipController, TMP_Text text)
        {
            _shipController = shipController;
            name = _shipController.PlayerName;
            points = 0;
            _text = text;
            _text.text = PanelText();
            _shipController.PointsChanged += UpdatePanel;
        }

        public void UpdatePanel()
        {
            points++;
            _text.text = PanelText();
        }

        public string PanelText()
        {
            return name + ": " + points.ToString();
        }

        public void Dispose()
        {
            _shipController.PointsChanged -= UpdatePanel;
        }
    }
}