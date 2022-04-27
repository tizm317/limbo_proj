using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    // 사운드 매니저

    // 사운드에서 필요한 것?
    // 사운드 근원지   -> AudioSource
    // 음원           -> AudioClip
    // 관객(귀)       -> AudioListener

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount]; // BGM, SFX 2가지

    // 오디오클립 딕셔너리 - 캐슁 역할 : 효과음을 매번 리소스매니저 이용해서 찾아오면 너무 느림
    // SoundManager 영구적 -> 계속 추가만 되다가 메모리 부족할 수도
    // 클리어하는 것도 신경 써야함
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        // AudioSource 도 컴포넌트 -> 맘대로 new 할 수 없음
        // 빈 오브젝트(@Sound) 생성 후, 거기에 AudioSource 컴포넌트를 붙이는 초기화 함수

        GameObject sound = GameObject.Find("@Sound"); // Sound의 Root 오브젝트
        if (sound == null)
        {
            sound = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(sound); // 씬 변해도 안 사라지도록

            // 사운드 이름(Bgm, Effect ...) 추출
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound)); // Define.Sound 에서 Reflection 기능으로 추출
            for (int i = 0; i < soundNames.Length - 1; i++) // Length - 1 : MaxCount 빼고
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();

                go.transform.parent = sound.transform;  // UI 할 때는 setParent 였는데 그건 rectTransform 이여서 그랬고, 일반적으로는  parent 사용
            }

            // Bgm loop 설정
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        // 씬 이동 할 때 호출해서 캐쉬 날려서 메모리 관리
        // 사운드가 딕셔너리에 계속 보관되면 무거워짐 (사운드매니저는 DontDestroyOnLoad이기 때문에)
        // Clear로 관리

        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        // 주소로 찾는 버전 Play 함수

        // AudioClip 찾는 부분
        AudioClip audioClip = GetOrAddAudioClip(path, type);

        // Play 부분 - 다른 버전 Play 함수 호출
        Play(audioClip, type, pitch); 
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        // 오디오클립 직접 받는 버전 Play 함수

        // AudioClip 직접 받기 때문에 찾는 부분 X

        // Play 부분
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            // Bgm

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            // 이미 bgm 나오는 중이면, 꺼주고
            if (audioSource.isPlaying)
                audioSource.Stop();
            // 다음 bgm 실행
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            // Effect

            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip); // 1번 실행
        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        // AudioClip 가져오거나 없으면 추가하는 함수
        // Bgm, Effect 둘 다 사용 가능

        // 효과음 같은 경우, 자주 바뀌어서 , 매번 리소스 로드하면 오래걸림
        // 딕셔너리 찾아서 있으면 리턴, 없으면 그 때 리소스 로드

        // path는 반드시 Sounds 산하에 있어야함
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // 빼먹은 경우, 추가

        AudioClip audioClip = null;

        // loop ? -> BGM
        if (type == Define.Sound.Bgm)
        {
            // 오디오클립 선택하는 부분
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            // 효과음

            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                // 딕셔너리 찾아서 있는 경우 audioClip에 저장(true라서 여기로 안 들어옴)
                
                // false 일 경우 (없는 경우)
                // 리소스에서 찾아서 추가
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip missing : {path}");

        return audioClip;
    }
}
