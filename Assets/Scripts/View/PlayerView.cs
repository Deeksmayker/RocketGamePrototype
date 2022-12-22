using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerController _playerController;
    private GameObject _playerMovingParticle;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerMovingParticle = GetComponentInChildren<ParticleSystem>().gameObject;
    }

    void Update()
    {
        /*if (_playerController.IsWalking() || _playerController.IsJumping || _playerController.IsWallGrab ||
            _playerController.IsWallJumping)
        {
            _playerMovingParticle.SetActive(true);
        }

        if (!_playerController.IsWalking() && !_playerController.IsJumping && !_playerController.IsWallGrab &&
            !_playerController.IsWallJumping)
        {
            _playerMovingParticle.SetActive(false);
        }*/

        if (_playerController.IsJumping)
        {
            _playerMovingParticle.SetActive(false);
        }
        else
        {
            _playerMovingParticle.SetActive(true);
        }
    }
}