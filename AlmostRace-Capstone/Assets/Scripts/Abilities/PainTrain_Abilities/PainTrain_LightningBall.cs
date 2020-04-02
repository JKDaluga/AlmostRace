using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_LightningBall : Projectile
{
    private float _lightningBallDamage;

    private float _lightningBallDuration;

    private float _lightningBallFrequency;

    private Collider objCollider;

    private List<CarHealthBehavior> _carsToDamage;

    public Transform targetCar;
    private bool hasStuckCar;

    public GameObject lightningBoom;

    // Start is called before the first frame update
    void Start()
    {
        objCollider = gameObject.GetComponent<Collider>();
        _carsToDamage = new List<CarHealthBehavior>();
        GiveSpeed();
    }

    public void GiveInfo(float lightningBallDamage, float lightningBallDuration, float lightningBallFrequency)
    {
        _lightningBallDamage = lightningBallDamage;
        _lightningBallDuration = lightningBallDuration;
        _lightningBallFrequency = lightningBallFrequency;
    }

    public IEnumerator DamageCars()
    {
        while (_carsToDamage.Count > 0)
        {
           

            foreach (CarHealthBehavior car in _carsToDamage)
            {
                if (!car.isDead) //Make sure car is alive.
                {
                    car.DamageCar(_lightningBallDamage, _immunePlayerScript.playerID); //Damage car.

                    if (car.healthCurrent <= 0) //See if car was killed by the damage.
                    {
                        _carsToDamage.Remove(car);// If a car is killed, remove it from the list of cars being damaged.
                        if(car.transform == targetCar)
                        {
                            gameObject.transform.SetParent(null);
                            StopCoroutine(FollowTargetCar());
                            StopCoroutine(TrackTargetCar());
                            targetCar = null;
                            GetComponent<Rigidbody>().useGravity = true;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(_lightningBallFrequency);
        }
        _carsToDamage.Clear();     
    }

    public void AddCarToDamage(CarHealthBehavior carToAdd)
    {
        if (carToAdd != _immunePlayer)
        {
            if (!_carsToDamage.Contains(carToAdd))
            {
                _carsToDamage.Add(carToAdd);
            }
            if (_carsToDamage.Count == 1)
            {
                StartCoroutine(DamageCars());
            }
        }
    }

    public void RemoveCarFromDamage(CarHealthBehavior carToRemove)
    {
        if(carToRemove != _immunePlayer)
        {
            if (_carsToDamage.Contains(carToRemove))
            {
                _carsToDamage.Remove(carToRemove);
            }

            if (_carsToDamage.Count > 1)
            {
                StopCoroutine(DamageCars());
            }
        } 
    }

    public IEnumerator TrackTargetCar()
    {
        while (!hasStuckCar)
        {
            _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed);
            gameObject.transform.LookAt(targetCar);
            yield return null;
        }
        yield return null;
    }

    public IEnumerator FollowTargetCar()
    {
        while(hasStuckCar)
        {
            if(gameObject.transform.parent != null)
            {
                gameObject.transform.position = gameObject.transform.parent.position;
            }         
            yield return null;
        }
    }

    public Transform GetTargetCar()
    {
        return targetCar;
    }

    public void SetTargetCar(Transform carToStick)
    {
        if (carToStick != _immunePlayer.transform)
        {
            targetCar = carToStick;
            StartCoroutine(TrackTargetCar());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        lightningBoom.SetActive(true);
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            objCollider.isTrigger = true;
            transform.SetParent(collision.gameObject.transform);
            hasStuckCar = true;
            StopCoroutine(TrackTargetCar());
            StartCoroutine(FollowTargetCar());

            _rigidBody.velocity = new Vector3(0, 0, 0);
            Destroy(gameObject, _lightningBallDuration);
        }

        if (collision.gameObject.CompareTag("Interactable"))
        {
            objCollider.isTrigger = true;
            targetCar = collision.gameObject.transform;
            transform.SetParent(collision.gameObject.transform);
            hasStuckCar = true;
            StopCoroutine(TrackTargetCar());
            StartCoroutine(FollowTargetCar());
            _rigidBody.velocity = new Vector3(0, 0, 0);
            Destroy(gameObject, _lightningBallDuration);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _rigidBody.velocity = new Vector3(0, 0, 0);
            targetCar = collision.gameObject.transform;
            Destroy(gameObject, _lightningBallDuration);
        }
    }

}
