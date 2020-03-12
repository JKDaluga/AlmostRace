using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelectViewport : MonoBehaviour
{
    public MapSelectManager mapSelectManager;

    [Header("UI Values")]
    [Space(20)]
    public Image mineMapSelect;
    public Image stellarMapSelect;
    public Image coverBox;
    public Image mineSnapshot;
    public Image stellarSnapshot;
    public Image[] mineSnapshotBehind;
    public Image[] stellarSnapshotBehind;
    public TextMeshProUGUI _statusText;

    private int _mapSelected;
    private VehicleInput _playerInput;
    private bool _selected;
    private bool _isSelecting;
    private bool _occupied;

    private void Start()
    {
        coverBox.enabled = false;
        mineMapSelect.enabled = true;
        stellarMapSelect.enabled = false;
        mineSnapshot.enabled = true;
        stellarSnapshot.enabled = false;
        _mapSelected = 1;
        foreach (Image snapshot in mineSnapshotBehind)
        {
            snapshot.enabled = false;
        }
        foreach (Image snapshot in stellarSnapshotBehind)
        {
            snapshot.enabled = false;
        }
    }

    private void Update()
    {
        if (!mapSelectManager.GetReadyToStart())
        {
            if (!_selected)
            {
                MapScroll();
                if (Input.GetButtonDown(_playerInput.selectButton))
                {
                    MapSelect(true);
                }
                else if (Input.GetButtonDown(_playerInput.backButton))
                {
                    PlayerJoin(false, null);
                }
            }
            else
            {
                if (Input.GetButtonDown(_playerInput.backButton))
                {
                    MapSelect(false);
                }
            }
        }
    }

    private void MapScroll()
    {
        if (Input.GetAxis(_playerInput.horizontal) < -0.3f)
        {
            if(!_isSelecting)
            {
                HighlightMineMap();
            }
        }
        else if (Input.GetAxis(_playerInput.horizontal) > 0.3f)
        {
            if(!_isSelecting)
            {
                HighlightInterstellareMap();
            }
        }
    }

    private void HighlightMineMap()
    {
        _isSelecting = true;
        stellarMapSelect.enabled = false;
        mineMapSelect.enabled = true;
        mineSnapshot.enabled = true;
        stellarSnapshot.enabled = false;
        _mapSelected = 1;
        _isSelecting = false;
    }

    private void HighlightInterstellareMap()
    {
        _isSelecting = true;
        mineMapSelect.enabled = false;
        stellarMapSelect.enabled = true;   
        stellarSnapshot.enabled = true;
        mineSnapshot.enabled = false;
        _mapSelected = 2;
        _isSelecting = false;
    }
    
    public void PlayerJoin(bool status, VehicleInput controllerNumber)
    {
        if (status == true)
        {
            _occupied = true;
            _playerInput = controllerNumber;
            _statusText.text = "PRESS A TO CONFIRM MAP";
            coverBox.enabled = false;
            mineMapSelect.enabled = true;
            stellarMapSelect.enabled = false;
            mineSnapshot.enabled = true;
            stellarSnapshot.enabled = false;
            foreach (Image snapshot in mineSnapshotBehind)
            {
                snapshot.enabled = false;
            }
            foreach (Image snapshot in stellarSnapshotBehind)
            {
                snapshot.enabled = false;
            }

        }
        else if (status == false)
        {
            mapSelectManager.LoadVehicleSelect();
            _occupied = false;
        }
    }

    private void MapSelect(bool status)
    {
        if (status == true)
        {
            _selected = true;
            if (DataManager.instance.getNumActivePlayers() == 1)
            {
                coverBox.enabled = true;
            }
            _statusText.text = "PRESS B TO UNCONFIRM";
            if (_mapSelected == 1)
            {
                foreach (Image snapshot in mineSnapshotBehind)
                {
                    snapshot.enabled = true;
                }
                foreach (Image snapshot in stellarSnapshotBehind)
                {
                    snapshot.enabled = false;
                }
            }
            else if (_mapSelected == 2)
            {
                foreach (Image snapshot in mineSnapshotBehind)
                {
                    snapshot.enabled = false;
                }
                foreach (Image snapshot in stellarSnapshotBehind)
                {
                    snapshot.enabled = true;
                }
            }
            if (DataManager.instance.getNumActivePlayers() == 1)
            {
                mineSnapshot.enabled = false;
                stellarSnapshot.enabled = false;
            }
            mapSelectManager.UpdateData(_selected, _mapSelected);
        }
        else
        {
            _selected = false;
            coverBox.enabled = false;
            _statusText.text = "PRESS A TO CONFIRM MAP";
            foreach (Image snapshot in mineSnapshotBehind)
            {
                snapshot.enabled = false;
            }
            foreach (Image snapshot in stellarSnapshotBehind)
            {
                snapshot.enabled = false;
            }
            if (DataManager.instance.getNumActivePlayers() == 1)
            {
                if (_mapSelected == 1)
                {
                    mineSnapshot.enabled = true;
                }
                else if (_mapSelected == 2)
                {
                    stellarSnapshot.enabled = true;
                }
            }
            mapSelectManager.UpdateData(_selected, _mapSelected);
            
        }
    }

    public bool GetOccupied()
    {
        return _occupied;
    }

    public bool GetSelected()
    {
        return _selected;
    }

    public VehicleInput GetInput()
    {
        return _playerInput;
    }
}
