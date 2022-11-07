using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager
{
    // ���� �Ŵ���

    // ���忡�� �ʿ��� ��?
    // ���� �ٿ���   -> AudioSource
    // ����           -> AudioClip
    // ����(��)       -> AudioListener

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount]; // BGM, SFX 2����


    // �����Ŭ�� ��ųʸ� - ĳ�� ���� : ȿ������ �Ź� ���ҽ��Ŵ��� �̿��ؼ� ã�ƿ��� �ʹ� ����
    // SoundManager ������ -> ��� �߰��� �Ǵٰ� �޸� ������ ����
    // Ŭ�����ϴ� �͵� �Ű� �����
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    AudioMixer audioMixer;

    public void Init()
    {
        // AudioSource �� ������Ʈ -> ����� new �� �� ����
        // �� ������Ʈ(@Sound) ���� ��, �ű⿡ AudioSource ������Ʈ�� ���̴� �ʱ�ȭ �Լ�

        // 오디오 믹서
        // 호영 : null 레퍼런스 오류 떠서 찾아서 넣어줌
        audioMixer = Managers.Resource.Load<AudioMixer>("Sound/Master");

        GameObject sound = GameObject.Find("@Sound"); // Sound�� Root ������Ʈ
        if (sound == null)
        {
            sound = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(sound); // �� ���ص� �� ���������

            // ���� �̸�(Bgm, Effect ...) ����
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound)); // Define.Sound ���� Reflection ������� ����
            for (int i = 0; i < soundNames.Length - 1; i++) // Length - 1 : MaxCount ����
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();

                go.transform.parent = sound.transform;  // UI �� ���� setParent ���µ� �װ� rectTransform �̿��� �׷���, �Ϲ������δ�  parent ���
            }

            // Bgm loop ����
            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.Effect].loop = false;//영찬
            
            // 호영 : null 레퍼런스 오류 떠서 찾아서 넣어줌
            AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
            _audioSources[(int)Define.Sound.Bgm].outputAudioMixerGroup = audioMixerGroups[1];
            _audioSources[(int)Define.Sound.Effect].outputAudioMixerGroup = audioMixerGroups[2];

            // (호영) -> 각 씬 클래스 Init 함수에서 BGM 설정
            //Set_BGM();
        }
    }

    public void Clear()
    {
        // �� �̵� �� �� ȣ���ؼ� ĳ�� ������ �޸� ����
        // ���尡 ��ųʸ��� ��� �����Ǹ� ���ſ��� (����Ŵ����� DontDestroyOnLoad�̱� ������)
        // Clear�� ����

        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f, bool option = false)
    {
        // �ּҷ� ã�� ���� Play �Լ�

        // AudioClip ã�� �κ�
        AudioClip audioClip = GetOrAddAudioClip(path, type);

        // Play �κ� - �ٸ� ���� Play �Լ� ȣ��
        Play(audioClip, type, pitch, option); 
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f, bool option = false)
    {
        // �����Ŭ�� ���� �޴� ���� Play �Լ�

        // AudioClip ���� �ޱ� ������ ã�� �κ� X

        // Play �κ�
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            // Bgm

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            // �̹� bgm ������ ���̸�, ���ְ�
            if (audioSource.isPlaying)
                audioSource.Stop();
            // ���� bgm ����
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            // Effect

            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            if(!option)
                audioSource.PlayOneShot(audioClip);
            else
                if(!audioSource.isPlaying)//영찬
                    audioSource.PlayOneShot(audioClip); // 1�� ����
        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        // AudioClip �������ų� ������ �߰��ϴ� �Լ�
        // Bgm, Effect �� �� ��� ����

        // ȿ���� ���� ���, ���� �ٲ� , �Ź� ���ҽ� �ε��ϸ� �����ɸ�
        // ��ųʸ� ã�Ƽ� ������ ����, ������ �� �� ���ҽ� �ε�

        // path�� �ݵ�� Sounds ���Ͽ� �־����
        if (path.Contains("Sound/") == false)
            path = $"Sound/{path}"; // ������ ���, �߰�

        AudioClip audioClip = null;

        // loop ? -> BGM
        if (type == Define.Sound.Bgm)
        {
            // �����Ŭ�� �����ϴ� �κ�
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            // ȿ����

            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                // ��ųʸ� ã�Ƽ� �ִ� ��� audioClip�� ����(true�� ����� �� ����)
                
                // false �� ��� (���� ���)
                // ���ҽ����� ã�Ƽ� �߰�
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip missing : {path}");

        return audioClip;
    }



    // 사운드 조절
    public float GetVolume(string soundType)
    {
        float volume;
        audioMixer.GetFloat(soundType, out volume);
        return volume;
    }
    public void SetVolume(int idx, Slider[] AudioSlide, Toggle[] Toggle)
    {
        string name;
        switch (idx)
        {
            case 0:
                name = "BGM";
                break;
            case 1:
                name = "SFX";
                break;
            default:
                name = "BGM";
                break;
        }
        float volume = AudioSlide[idx].value;
        if (volume >= 0.01f)
        {
            audioMixer.SetFloat(name, volume);
            Toggle[idx].isOn = true;
        }
        else
        {
            AudioSlide[idx].value = 0.0f;
            audioMixer.SetFloat(name, -80);//음소거
            Toggle[idx].isOn = false;
        }
    }
    float[] temp = new float[10];
    public void Volume_Toggle(int idx, Slider[] AudioSlide, Toggle[] Toggle)
    {
        string name;
        switch (idx)
        {
            case 0:
                name = "BGM";
                break;
            case 1:
                name = "SFX";
                break;
            default:
                name = "BGM";
                break;
        }
        float volume = AudioSlide[idx].value;

        // 누르는 순간 이미 isOn 바뀌어있음 (f -> t)
        if (Toggle[idx].isOn)
        {
            volume = temp[idx];
        }
        else
        {
            temp[idx] = volume;
            volume = -80f;
        }
        AudioSlide[idx].value = volume;
        audioMixer.SetFloat(name, volume);
    }
}
