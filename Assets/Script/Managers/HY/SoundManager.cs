using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init()
    {
        // AudioSource �� ������Ʈ -> ����� new �� �� ����
        // �� ������Ʈ(@Sound) ���� ��, �ű⿡ AudioSource ������Ʈ�� ���̴� �ʱ�ȭ �Լ�

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

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        // �ּҷ� ã�� ���� Play �Լ�

        // AudioClip ã�� �κ�
        AudioClip audioClip = GetOrAddAudioClip(path, type);

        // Play �κ� - �ٸ� ���� Play �Լ� ȣ��
        Play(audioClip, type, pitch); 
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
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
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // ������ ���, �߰�

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
}
