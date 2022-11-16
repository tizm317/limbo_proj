using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public void Arrow_(GameObject _target, float _speed, Player _player)
    {
        StartCoroutine(_Arrow(_target,_speed,_player));
    }
    IEnumerator _Arrow(GameObject _target, float _speed, Player _player)
    {
        bool hit = false;
        float time = 0;
        Vector3 target_pos = _target.transform.position;
        while(!hit)
        {
            yield return new WaitForEndOfFrame();
            if(_target != null)
                target_pos = _target.transform.position;
            Vector3 dir = (target_pos - gameObject.transform.position).normalized;
            gameObject.transform.position += dir * Time.deltaTime * _speed;
            gameObject.transform.forward = dir;
            time += Time.deltaTime;

            if(Vector3.Distance(gameObject.transform.position, target_pos) < 0.5f)
            {
                Stat target_stat = _target.GetComponent<Stat>();
                Managers.Sound.Stop();
                if(_player.my_job == Define.Job.ARCHER)
                {
                    Managers.Sound.Play("Sound/Skill_Sound/Archer_Hit",Define.Sound.Effect, 1.0f, true);
                    target_stat.OnAttacked(_player.my_stat);
                }
                else if(_player.my_job == Define.Job.SORCERER)
                {
                    Managers.Sound.Play("Sound/Skill_Sound/Sorcerer_Hit",Define.Sound.Effect, 1.0f, true);
                    target_stat.OnAttacked(_player.my_stat.Attack * 2,_player.my_stat);
                }
                if(target_stat.Hp <= 0)
                {
                    _player.my_enemy = null;
                    Destroy(gameObject);
                }
                hit = true;
            }
            else if(time > 5f)
                hit = true;
        }
        Destroy(gameObject);
    }
}
