using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quantum2D.UnityView
{
    public enum CharacterAnimState
    {
        Idle = 0,
        Run = 1,
        Attack = 2,
        Hurt = 3,
        Dash = 4,
        Die = 5
    }

    [System.Serializable]
    public struct AnimationClip2D
    {
        public CharacterAnimState state;
        public Sprite[] frames;
        public float frameRate;
        public bool loop;
    }

    /// <summary>
    /// Frame-by-frame 2D Sprite Animator cho nhân vật Top-Down
    /// Tương thích trực tiếp với Sprite Sheets của Kiếm Sĩ & Nữ Pháp Sư
    /// Đồng bộ mượt mà với Quantum 3 Tick State
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimationController2D : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Animation Clips (Tách từ Sprite Sheet)")]
        [SerializeField] private List<AnimationClip2D> animationClips = new List<AnimationClip2D>();

        [Header("VFX Mount Points")]
        [SerializeField] private Transform attackVfxPoint;
        [SerializeField] private Transform dashVfxPoint;
        [SerializeField] private Transform hitVfxPoint;

        private CharacterAnimState _currentState = CharacterAnimState.Idle;
        private int _currentFrameIndex = 0;
        private float _timer = 0f;
        private AnimationClip2D _activeClip;
        private bool _isFacingLeft = false;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            SetState(CharacterAnimState.Idle);
        }

        private void Update()
        {
            PlayAnimationFrames(Time.deltaTime);
        }

        public void SetState(CharacterAnimState newState, bool forceRestart = false)
        {
            if (_currentState == newState && !forceRestart) return;

            _currentState = newState;
            _currentFrameIndex = 0;
            _timer = 0f;

            _activeClip = animationClips.Find(c => c.state == newState);
            if (_activeClip.frames != null && _activeClip.frames.Length > 0)
            {
                spriteRenderer.sprite = _activeClip.frames[0];
            }
        }

        public void SetFacingDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > 0.05f)
            {
                _isFacingLeft = direction.x < 0;
                spriteRenderer.flipX = _isFacingLeft;

                // Đồng bộ hướng của điểm sinh hiệu ứng VFX
                if (attackVfxPoint != null)
                {
                    Vector3 localPos = attackVfxPoint.localPosition;
                    localPos.x = Mathf.Abs(localPos.x) * (_isFacingLeft ? -1f : 1f);
                    attackVfxPoint.localPosition = localPos;
                }
            }
        }

        private void PlayAnimationFrames(float deltaTime)
        {
            if (_activeClip.frames == null || _activeClip.frames.Length == 0) return;

            float frameDuration = 1f / Mathf.Max(1f, _activeClip.frameRate);
            _timer += deltaTime;

            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentFrameIndex++;

                if (_currentFrameIndex >= _activeClip.frames.Length)
                {
                    if (_activeClip.loop)
                    {
                        _currentFrameIndex = 0;
                    }
                    else
                    {
                        _currentFrameIndex = _activeClip.frames.Length - 1;
                        // Sau khi hết đòn đánh / dash / hurt -> quay về Idle
                        if (_currentState == CharacterAnimState.Attack || _currentState == CharacterAnimState.Dash || _currentState == CharacterAnimState.Hurt)
                        {
                            SetState(CharacterAnimState.Idle);
                        }
                    }
                }

                if (_currentFrameIndex < _activeClip.frames.Length)
                {
                    spriteRenderer.sprite = _activeClip.frames[_currentFrameIndex];
                }
            }
        }

        public void SetAnimationClips(List<Sprite> idle, List<Sprite> run, List<Sprite> attack, List<Sprite> hurt, List<Sprite> dash, List<Sprite> die)
        {
            animationClips.Clear();
            if (idle != null && idle.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Idle, frames = idle.ToArray(), frameRate = 8f, loop = true });
            if (run != null && run.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Run, frames = run.ToArray(), frameRate = 12f, loop = true });
            if (attack != null && attack.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Attack, frames = attack.ToArray(), frameRate = 14f, loop = false });
            if (hurt != null && hurt.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Hurt, frames = hurt.ToArray(), frameRate = 10f, loop = false });
            if (dash != null && dash.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Dash, frames = dash.ToArray(), frameRate = 16f, loop = false });
            if (die != null && die.Count > 0)
                animationClips.Add(new AnimationClip2D { state = CharacterAnimState.Die, frames = die.ToArray(), frameRate = 8f, loop = false });
        }

        public Transform GetVfxPoint(string vfxType)
        {
            switch (vfxType.ToLower())
            {
                case "attack": return attackVfxPoint != null ? attackVfxPoint : transform;
                case "dash": return dashVfxPoint != null ? dashVfxPoint : transform;
                case "hit": return hitVfxPoint != null ? hitVfxPoint : transform;
                default: return transform;
            }
        }
    }
}
