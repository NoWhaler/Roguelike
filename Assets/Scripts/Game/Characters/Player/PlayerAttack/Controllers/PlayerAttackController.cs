using Core.ObjectPooling.Pools;
using Game.Characters.Player.PlayerMovement.Models;
using Game.Inputs.Common.Model;
using Zenject;

namespace Game.Characters.Player.PlayerAttack.Controllers
{
    public class PlayerAttackController: ITickable
    {
        private ArrowsPool _arrowsPool;
        private InputModel _inputModel;
        private PlayerModel _playerModel;
        
        [Inject]
        private void Constructor(ArrowsPool arrowsPool, InputModel inputModel, PlayerModel playerModel)
        {
            _arrowsPool = arrowsPool;
            _inputModel = inputModel;
            _playerModel = playerModel;
        }
        
        public void Tick()
        {
            if (_inputModel.LeftMouseButtonInputClick)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            SpawnArrow();
        }

        private void SpawnArrow()
        {
            var arrow = _arrowsPool.Get();

            arrow.transform.position = _playerModel.ArrowSpawnPosition.position;
            arrow.gameObject.SetActive(true);
        }
    }
}