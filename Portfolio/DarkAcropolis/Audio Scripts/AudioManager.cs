using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //Could private an [SerializeField]
    [Header("Player Sounds")]
    public audioAssets[] playerWalkingSound;
    public audioAssets[] playerBreathWalkSound;
    public audioAssets[] playerBreathSprintSound;
    public audioAssets[] playerOutOfBreathSound;
    public audioAssets[] playerDamageSound;
    [Header("Monster Sounds")]
    public audioAssets[] monsterFoundSound;
    public audioAssets[] monsterAttackSound;
    public audioAssets[] monsterWalkSound;
    public audioAssets[] monsterStaggerSound;
    public audioAssets[] monsterRoarSound;
    public audioAssets[] monsterIdleSound;
    [Header("Object Sounds")]
    public audioAssets[] TorchSound;
    public audioAssets[] SealBeatSound;
    public audioAssets[] SealBuildUpSound;
    public audioAssets[] SealDestructionSound;
    public audioAssets[] HeartBeatSound;
    public audioAssets[] HeartDestructionSound;
    public audioAssets[] FlashSound;


    [Header("Mixer")]
    [SerializeField] AudioMixerGroup playerMixer;
    [SerializeField] AudioMixerGroup monsterMixer;
    [SerializeField] AudioMixerGroup objectsMixer;

    private Monster monster;

    //On awake assigns the sound there new elements in the array
    private void Awake()
    {
        SetAudioAssestPlayer(playerWalkingSound);
        SetAudioAssestPlayer(playerBreathWalkSound);
        SetAudioAssestPlayer(playerBreathSprintSound);
        SetAudioAssestPlayer(playerOutOfBreathSound);
        SetAudioAssestPlayer(playerDamageSound);

        SetAudioAssestMonster(monsterFoundSound);
        SetAudioAssestMonster(monsterAttackSound);
        SetAudioAssestMonster(monsterWalkSound);
        SetAudioAssestMonster(monsterStaggerSound);
        SetAudioAssestMonster(monsterRoarSound);
        SetAudioAssestMonster(monsterIdleSound);

        SetAudioAssestObject(TorchSound);
        SetAudioAssestObject(SealBeatSound);
        SetAudioAssestObject(SealBuildUpSound);
        SetAudioAssestObject(SealDestructionSound);
        SetAudioAssestObject(HeartBeatSound);
        SetAudioAssestObject(HeartDestructionSound);
        SetAudioAssestObject(FlashSound);
    }

    private void Start()
    {
        monster = FindObjectOfType<Monster>();
    }

    //Instead of a string have it be audio assets names as it will shortent the code
    //all this means in other scripts they will have to a refercer to the audio manager
    //than make a new function to call when needing to stop the audio from playing
    public void PlaySound(int index, string name)
    {
        //Player Mixer
        if (name == "Player Walk") 
        {
            audioAssets playerWalk = Array.Find(playerWalkingSound, sound => sound.index == index);
            playerWalk.source.Play();
        }
        else if (name == "Breath Walk") 
        {
            audioAssets breathWalk = Array.Find(playerBreathWalkSound, sound => sound.index == index);
            breathWalk.source.Play();
        }
        else if (name == "Breath Sprint") 
        {
            audioAssets breathSprint = Array.Find(playerBreathSprintSound, sound => sound.index == index);
            breathSprint.source.Play();
        }
        else if (name == "Out of Breath") 
        {
            audioAssets outOfBreath = Array.Find(playerOutOfBreathSound, sound => sound.index == index);
            outOfBreath.source.Play();
        }
        else if (name == "Player Damage") 
        {
            audioAssets playerDamage = Array.Find(playerDamageSound, sound => sound.index == index);
            playerDamage.source.Play();
        }

        //Monster Mixer
        //else if (name == "Monster Found")
        //{
        //    audioAssets monsterFound = Array.Find(monsterFoundSound, sound => sound.index == index);
        //    monsterFound.source.Play();
        //}
        else if (name == "Monster Attack")
        {
            audioAssets monsterAttack = Array.Find(monsterAttackSound, sound => sound.index == index);
            monsterAttack.source.Play();
        }
        else if (name == "Monster Walk")
        {
            audioAssets monsterWalk = Array.Find(monsterWalkSound, sound => sound.index == index);
            //float PathDistance = 0.0f;
            //PathDistance = monster.DistanceToPlayer(PathDistance);
            //monsterWalk.source.volume = 1.0f - (PathDistance / 40.0f);
            //Debug.Log(PathDistance);
            monsterWalk.source.Play();
        }
        else if (name == "Monster Stagger")
        {
            audioAssets monsterStagger = Array.Find(monsterStaggerSound, sound => sound.index == index);
            monsterStagger.source.Play();
        }
        else if (name == "Monster Roar")
        {
            audioAssets monsterRoar = Array.Find(monsterRoarSound, sound => sound.index == index);
            monsterRoar.source.Play();
        }
        else if (name == "Monster Idle")
        {
            audioAssets monsterIdle = Array.Find(monsterIdleSound, sound => sound.index == index);
            monsterIdle.source.Play();
        }

        //Object Mixer 
        else if (name == "Torch")
        {
            audioAssets torch = Array.Find(TorchSound, sound => sound.index == index);
            torch.source.Play();
        }
        else if (name == "Seal Beat")
        {
            audioAssets sealBeat = Array.Find(SealBeatSound, sound => sound.index == index);
            sealBeat.source.Play();
        }
        else if (name == "Seal Build Up")
        {
            audioAssets sealBuildUp = Array.Find(SealBuildUpSound, sound => sound.index == index);
            sealBuildUp.source.Play();
        }
        else if (name == "Seal Destruction")
        {
            audioAssets sealDestruction = Array.Find(SealDestructionSound, sound => sound.index == index);
            sealDestruction.source.Play();
        }
        else if (name == "Heart Beat")
        {
            audioAssets heartBeat = Array.Find(HeartBeatSound, sound => sound.index == index);
            heartBeat.source.Play();
        }
        else if (name == "Heart Destruction")
        {
            audioAssets heartDestruction = Array.Find(HeartDestructionSound, sound => sound.index == index);
            heartDestruction.source.Play();
        }
        else if (name == "Flash")
        {
            audioAssets flash = Array.Find(FlashSound, sound => sound.index == index);
            flash.source.Play();
        }

    }

    //New way to play sound using this function
    //for distance sound have if statement that check if testaudio equals audiomanager audioassets name
   //public void soundplaytest(int index, audioAssets[] testaudio)
   //{
   //    audioAssets something = Array.Find(testaudio, sound => sound.index == index);
   //}

    void SetAudioAssestPlayer(audioAssets[] AudioList)
    {
        foreach (audioAssets s in AudioList)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = playerMixer;

            s.source.volume = s.volume;

            s.source.pitch = s.pitch;
        }
    }
    void SetAudioAssestMonster(audioAssets[] AudioList)
    {
        foreach (audioAssets s in AudioList)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = monsterMixer;

            s.source.volume = s.volume;

            s.source.pitch = s.pitch;
        }
    }
    void SetAudioAssestObject(audioAssets[] AudioList)
    {
        foreach (audioAssets s in AudioList)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = objectsMixer;

            s.source.volume = s.volume;

            s.source.pitch = s.pitch;
        }
    }

}
