using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VExtension
{
    public static T GetRandom<T>(this IEnumerable<T> input)
    {
        if (input == null || !input.Any()) return default(T);
        return input.ElementAt(UnityEngine.Random.Range(0, input.Count()));
    }

    public static T GetRandomEnumValue<T>() where T : System.Enum
    {
        var values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
    public static bool InRange(this float input, float min, float Max)
    {
        return input >= min && input <= Max;
    }

    public static float RandomBetweenXandY(this Vector2 randomRangeAsVector2) => UnityEngine.Random.Range(randomRangeAsVector2.x, randomRangeAsVector2.y);
    public static bool EqualsWithOffset(this Vector3 main, float offset)
    {
        return main.x.InRange(main.x - offset, main.x + offset)
            && main.y.InRange(main.y - offset, main.y + offset)
            && main.z.InRange(main.z - offset, main.z + offset);
    }
    public static bool EqualsWithOffset(this Vector3 main, Vector3 target, float offset)
    {
        return main.x.InRange(target.x - offset, target.x + offset)
            && main.y.InRange(target.y - offset, target.y + offset)
            && main.z.InRange(target.z - offset, target.z + offset);
    }
    public static bool DivisibleFor(this int num, int divisor) => num % divisor == 0;

    #region Transform Checking
    public static bool IsLookAtIgnoreY(Transform thisTrans, Transform target, float _dotValue = 0.95f)
    {
        Vector3 _temp = thisTrans.position;
        Vector3 _tempTarget = target.position;
        return Vector3.Dot(thisTrans.forward, (new Vector3(_tempTarget.x, _temp.y, _tempTarget.z) - _temp).normalized) >= _dotValue;
    }
    public static bool IsLookAtIgnoreY(Transform thisTrans, Vector3 target, float _dotValue = 0.95f)
    {
        var value = Vector3.Dot(thisTrans.forward, (new Vector3(target.x, thisTrans.position.y, target.z) - thisTrans.position).normalized);
        return value >= _dotValue;
    }
    public static bool IsLookAt(Transform thisTrans, Vector3 target, float _dotValue = 0.95f)
    {
        return Vector3.Dot(thisTrans.forward, (target - thisTrans.position).normalized) >= _dotValue;
    }
    public static bool CheckOppositeIgnoreY(Transform thisTrans, Vector3 target, float _dotValue = 0.95f)
    {
        return Vector3.Dot(thisTrans.forward, (new Vector3(target.x, thisTrans.position.y, target.z) - thisTrans.position).normalized) > _dotValue;
    }
    public static bool IsOnTheRightOfTransform(Vector3 point, Transform _rootTrans)
    {
        Vector3 referenceObjectRight = _rootTrans.right;

        Vector3 directionToTarget = point - _rootTrans.position;

        float projection = Vector3.Dot(directionToTarget, referenceObjectRight);

        return projection > 0f;
    }
    public static bool IsInFrontOf(Transform _root, Transform _checkTarget)
    {
        Vector3 directionToB = _checkTarget.position - _root.position;
        float dotProduct = Vector3.Dot(_root.forward, directionToB);

        return dotProduct > 0;
    }
    public static bool IsInFrontOf(Transform _root, Vector3 _checkTarget)
    {
        Vector3 directionToB = _checkTarget - _root.position;
        float dotProduct = Vector3.Dot(_root.forward, directionToB);

        return dotProduct > 0;
    }
    #endregion

    #region Rotate
    /// <summary>
    /// Look at target smoothly
    /// </summary>
    /// <param name="thisTransform"></param>
    /// <param name="target"></param>
    /// <param name="_speed"></param>
    public static void SmoothLookAt(Transform thisTransform, Transform target, float _speed = 1f)
    {
        Quaternion _dir = Quaternion.LookRotation(target.position - thisTransform.position);

        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, _dir, _speed * Time.deltaTime);
    }
    public static void SmoothLookAt(Transform thisTransform, Vector3 target, float _speed = 1f)
    {
        Quaternion _dir = Quaternion.LookRotation(target - thisTransform.position);

        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, _dir, _speed * Time.deltaTime);
    }
    public static void SmoothLookAt(Transform thisTransform, Vector3 target, Vector3 _multiOffset, float _speed = 1f)
    {
        target = new Vector3(target.x * _multiOffset.x, target.y * _multiOffset.y, target.z * _multiOffset.z);
        Quaternion _dir = Quaternion.LookRotation(target - thisTransform.position, Vector3.up);

        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, _dir, _speed * Time.deltaTime);
    }

    public static IEnumerator SmoothLookAtHuman(Transform thisTransform, Transform target, float time, float _speed = 1f)
    {
        float timer = 0;
        while (timer <= time)
        {
            SmoothLookAt(thisTransform, target, _speed);
            yield return null;
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Rotate only y instantly
    /// </summary>
    /// <param name="thisTransform"></param>
    /// <param name="target"></param>
    /// <param name="_speed"></param>
    public static void Instant_RotateOnlyY(Transform thisTransform, Transform target)
    {
        Vector3 dir = target.position - thisTransform.position;
        Quaternion _dir = Quaternion.LookRotation(dir);
        thisTransform.rotation = _dir;
    }
    public static void Instant_RotateOnlyY(Transform thisTransform, Vector3 target)
    {
        Vector3 dir = target - thisTransform.position;
        Quaternion _dir = Quaternion.LookRotation(dir);
        thisTransform.rotation = _dir;
    }
    public static void Instant_RotateIgnoreHeight(Transform thisTransform, Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - thisTransform.position);
        targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
        thisTransform.rotation = targetRotation;
    }
    public static void Instant_RotateIgnoreHeight(Transform thisTransform, Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - thisTransform.position);
        targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
        thisTransform.rotation = targetRotation;
    }
    public static void RotateToTargetIgnoreHeight(Transform _thisTrans, Transform target, float rotaSpeed)
    {
        Vector3 _target = new Vector3(target.position.x, _thisTrans.position.y, target.position.z);
        Vector3 _dirr = _target - _thisTrans.position;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, Vector3.up);
            _thisTrans.rotation = Quaternion.RotateTowards(_thisTrans.rotation, _rota, rotaSpeed * Time.deltaTime);
        }
    }
    public static void RotateToTargetIgnoreHeight(Transform _thisTrans, Vector3 target, float rotaSpeed)
    {
        Vector3 _target = new Vector3(target.x, _thisTrans.position.y, target.z);
        Vector3 _dirr = _target - _thisTrans.position;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, Vector3.up);
            _thisTrans.rotation = Quaternion.RotateTowards(_thisTrans.rotation, _rota, rotaSpeed * Time.deltaTime);
        }
    }
    public static void RotateToTargetIgnoreHeight_Local(Transform _thisTrans, Transform _parrent, Vector3 target, float rotaSpeed)
    {
        Vector3 _myPos = _thisTrans.position;
        Vector3 _target = new Vector3(target.x, _myPos.y, target.z);
        Vector3 _dirr = _target - _myPos;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, _thisTrans.up);
            Quaternion _localTargetRotation = Quaternion.Inverse(_parrent.rotation) * _rota;
            _thisTrans.localRotation = Quaternion.RotateTowards(_thisTrans.localRotation, _localTargetRotation, rotaSpeed * Time.deltaTime);
        }
    }
    public static void RotateToTargetOnlyY(Transform _thisTrans, Transform target, float rotaSpeed)
    {
        Vector3 _target = new Vector3(_thisTrans.position.x, target.position.y, _thisTrans.position.z);
        Vector3 _dirr = _target - _thisTrans.position;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, Vector3.up);
            _thisTrans.rotation = Quaternion.RotateTowards(_thisTrans.rotation, _rota, rotaSpeed * Time.deltaTime);
        }
    }
    public static void RotateToTargetOnlyY(Transform _thisTrans, Vector3 target, float rotaSpeed)
    {
        Vector3 currentPos = _thisTrans.position;
        //Vector3 _target = new Vector3(currentPos.x, target.y, currentPos.z);
        Vector3 _dirr = target - currentPos;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, Vector3.up);
            _rota.y = 0;
            _rota.z = 0;
            _thisTrans.rotation = Quaternion.RotateTowards(_thisTrans.rotation, _rota, rotaSpeed * Time.deltaTime);
        }
    }
    public static void RotateToTargetIgnoreXAxis(Transform _thisTrans, Vector3 target, float rotaSpeed)
    {
        Vector3 currentPos = _thisTrans.position;
        Vector3 _target = new Vector3(currentPos.x, target.y, target.z);
        Vector3 _dirr = _target - currentPos;
        if (_dirr.x != 0 || _dirr.y != 0 || _dirr.z != 0)
        {
            Quaternion _rota = Quaternion.LookRotation(_dirr, Vector3.up);
            _thisTrans.rotation = Quaternion.RotateTowards(_thisTrans.rotation, _rota, rotaSpeed * Time.deltaTime);
        }
    }

    public static void RotateToTargetIgnoreXAxis_Local(Transform _thisTrans, Vector3 target, float rotaSpeed)
    {
        Vector3 currentPos = _thisTrans.position;
        Vector3 direction = currentPos - target;
        float angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(angle, 0, 0);
        _thisTrans.localRotation = Quaternion.Slerp(_thisTrans.localRotation, targetRotation, rotaSpeed * Time.deltaTime);
    }

    public static void SetListTransformLookAtSomething(List<Transform> _objects, Vector3 _target)
    {
        foreach (Transform _object in _objects) _object.LookAt(_target);
    }

    public static void SmoothChangeLocalEulerAngle(Transform _trans, Vector3 _eulerAngleTarget, float _speed = 1)
    {
        _trans.localEulerAngles = Vector3.Lerp(_trans.localEulerAngles, _eulerAngleTarget, _speed * Time.deltaTime);
    }
    public static void SmoothChangeLocalPosition(Transform _trans, Vector3 _positionTarget, float _speed = 1)
    {
        _trans.localPosition = Vector3.Lerp(_trans.localPosition, _positionTarget, _speed * Time.deltaTime);
    }
    #endregion
}
