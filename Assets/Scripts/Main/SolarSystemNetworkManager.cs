using Characters;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        private string _playerName;
        
        [SerializeField] private NetworkManagerHUD _networkManagerHUD;

        [Header("UI elements")]
        [SerializeField] private Button _OKButton;
        [SerializeField] private TMP_InputField _InputField;
        [SerializeField] private GameObject _loginPanel;

        [SerializeField] TMP_Text[] _panelsText;

        private int _index;

        private List<PlayerPanel> _playerPanels = new List<PlayerPanel> ();
        Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        private void Start()
        {
            _OKButton.onClick.AddListener(SetPlayerName);
            _networkManagerHUD.enabled = false;
        }

        private void SetPlayerName()
        {
            if (_InputField.text == "")
            {
                return;
            }

            _OKButton?.onClick.RemoveListener(SetPlayerName);
            _playerName = _InputField.text;
            
            _networkManagerHUD.enabled = true;
            _loginPanel.SetActive(false);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            
            var shipController = player.GetComponent<ShipController>();
            shipController.PlayerName = _playerName;
            _players.Add(conn.connectionId, shipController);
            _panelsText[_index].gameObject.SetActive(true);
            _playerPanels.Add(new PlayerPanel(shipController, _panelsText[_index]));
            _index++;

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            //if (NetworkServer.active)
            //{
                SpawnPrefabs();
            //}
            NetworkServer.RegisterHandler(100, RecievName);
        }

        private void SpawnPrefabs()
        {
            var spawnTransform = GetStartPosition();
            for (int i = 0; i < spawnPrefabs.Count; i++)
            {
                var randPosition = new Vector3(UnityEngine.Random.Range(-200f, 200f), 
                    0, UnityEngine.Random.Range(-200f, 200f));
                var pref = Instantiate(spawnPrefabs[i],
                    spawnTransform.position + randPosition, Quaternion.identity);
                NetworkServer.Spawn(pref);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            StringMessage login = new StringMessage(_playerName);
            conn.Send(100, login);
        }

        public void RecievName(NetworkMessage networkMessage)
        {
            _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
            _players[networkMessage.conn.connectionId].gameObject.name = _players[networkMessage.conn.connectionId].PlayerName;
            _playerPanels[_index - 1].name = _players[networkMessage.conn.connectionId].PlayerName;
            Debug.Log(_players[networkMessage.conn.connectionId]);
            
        }
    }
}