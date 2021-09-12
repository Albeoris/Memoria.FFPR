using System;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.Core
{
    public sealed class GameSpeedControl
    {
        public GameSpeedControl()
        {
        }

        private Boolean _isDisabled;
        private Boolean _isToggled;
        private int _isHeld;
        private Single _speedFactor = Time.timeScale;

        public void Update()
        {
            try
            {
                if (_isDisabled)
                    return;

                ProcessSpeedUp();
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                ModComponent.Log.LogError($"[{nameof(GameSpeedControl)}].{nameof(Update)}(): {ex}");
            }
        }

        private void ProcessSpeedUp()
        {
            Single currentFactor = Time.timeScale;
            if (currentFactor == 0.0f) // Do not unpause the game 
                return;
            //ModComponent.Log.LogInfo("Current Game Speed: " + currentFactor.ToString());
            var config = ModComponent.Instance.Config;

            var toggleFactor = config.Speed.ToggleFactor.Value;
            var toggleKey = config.Speed.ToggleKey.Value;
            var toggleAction = config.Speed.ToggleAction.Value;

            var holdFactor = config.Speed.HoldFactor.Value;
            var holdKey = config.Speed.HoldKey.Value;
            var holdAction = config.Speed.HoldAction.Value;

            Boolean isToggled = InputManager.GetKeyUp(toggleKey) || InputManager.GetKeyUp(toggleAction);
            Boolean isHold = InputManager.GetKey(holdKey) || InputManager.GetKey(holdAction);
            Single speedFactor = 0.0f;
            Boolean toggleOff = false;
            if (isToggled)
            {
                if (!_isToggled)
                    speedFactor = Math.Max(speedFactor, toggleFactor);
                if (_isToggled)
                    toggleOff = true;
                _isToggled = !_isToggled;
            }

            if (isHold)
            {
                speedFactor = Math.Max(speedFactor, holdFactor);
                if (_isHeld != 1)
                {
                    _isHeld = 1;//key is being held
                }
            }
            else
            {
                if(_isHeld > 0)
                {
                    _isHeld = 2;//key is no longer being held
                }
            }

            if (speedFactor == 0.0f)
            {
                speedFactor = _isToggled ? _speedFactor : 1.0f;
            }
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (currentFactor != speedFactor)
            {
                if (toggleOff)
                    Time.timeScale = speedFactor;//set it to 1 so our multiplicative approach returns to normal
                else if (_isHeld > 1)
                {
                    Time.timeScale = speedFactor;
                    _isHeld = 0;//holding logic should be ignored until key is held again
                }
                else
                    Time.timeScale = currentFactor * speedFactor;
                _speedFactor = speedFactor;
            }
        }
    }
}