using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class RadialMenu : MonoBehaviour
{
    public enum AppearanceType
    {
        None,
        Fade,
        ScaleChange,
        FadeAndScaleChange,
        Spread,
        Progressive,
        ProgressiveFade,
        ProgressiveScaleChange,
        ProgressiveSpread,
    }
    private enum MainType
    {
        AlphaChange,
        ScaleChange,
        AlphaAndScaleChange
    }

    [SerializeField] private AppearanceType _appearanceType;
    [SerializeField] private EasingType _appearanceEasing;

    [SerializeField] private MainType _mainType;

    [SerializeField] private AppearanceType _disappearanceType;
    [SerializeField] private EasingType _disappearanceEasing;

    private Dictionary<AppearanceType, AppearanceState> _appStateDict =
        new Dictionary<AppearanceType, AppearanceState>();

    private Dictionary<MainType, MainState> _mainStateDict =
        new Dictionary<MainType, MainState>();

    private Dictionary<AppearanceType, DisappearanceState> _disStateDict =
        new Dictionary<AppearanceType, DisappearanceState>();

    private MenuState _currentState;
    private MenuState AState => _appStateDict[_appearanceType];
    private MenuState MState => _mainStateDict[_mainType];
    private MenuState DState => _disStateDict[_disappearanceType];

    private float _stateProgress = 0f;

    private void Update()
    {
        _currentState.Update();
    }

    private void InitStateDicts()
    {
        _currentState = NullState.Instance;

        // 1. Appearance
        _appStateDict.Add(AppearanceType.None, new DefaultAppearance(this));
        _appStateDict.Add(AppearanceType.Fade, new FadeIn(this));
        _appStateDict.Add(AppearanceType.ScaleChange, new ScaleUp(this));
        _appStateDict.Add(AppearanceType.FadeAndScaleChange, new FadeInAndScaleUp(this));
        _appStateDict.Add(AppearanceType.Spread, new Spread(this));
        _appStateDict.Add(AppearanceType.Progressive, new ProgressiveAppearance(this));
        _appStateDict.Add(AppearanceType.ProgressiveFade, new ProgressiveFadeIn(this));
        _appStateDict.Add(AppearanceType.ProgressiveScaleChange, new ProgressiveScaleUp(this));
        _appStateDict.Add(AppearanceType.ProgressiveSpread, new ProgressiveSpread(this));

        // 2. Main
        _mainStateDict.Add(MainType.AlphaChange, new MainAlphaChange(this));
        _mainStateDict.Add(MainType.ScaleChange, new MainScaleChange(this));
        _mainStateDict.Add(MainType.AlphaAndScaleChange, new MainAlphaAndScaleChange(this));

        // 3. Disappearance
        _disStateDict.Add(AppearanceType.None, new DefaultDisappearance(this));
        _disStateDict.Add(AppearanceType.Fade, new FadeOut(this));
        _disStateDict.Add(AppearanceType.ScaleChange, new ScaleDown(this));
        _disStateDict.Add(AppearanceType.FadeAndScaleChange, new FadeOutAndScaleDown(this));
        _disStateDict.Add(AppearanceType.Spread, new Gather(this));
        _disStateDict.Add(AppearanceType.Progressive, new ProgressiveDisappearance(this));
        _disStateDict.Add(AppearanceType.ProgressiveFade, new ProgressiveFadeOut(this));
        _disStateDict.Add(AppearanceType.ProgressiveScaleChange, new ProgressiveScaleDown(this));
        _disStateDict.Add(AppearanceType.ProgressiveSpread, new ProgressiveGather(this));
    }

    private void ChangeToNextState()
    {
        _currentState.OnExit();

        if (_currentState == NullState.Instance) _currentState = AState;
        else if (_currentState == AState) _currentState = MState;
        else if (_currentState == MState) _currentState = DState;
        else _currentState = NullState.Instance;

        _currentState.OnEnter();
    }

    private void ForceToEnterAppearanceState()
    {
        if (_stateProgress > 0f)
        {
            return;
        }
        _currentState = AState;
        _currentState.OnEnter();
    }

    private void ForceToEnterDisappearanceState()
    {
        if (_stateProgress < 1f)
        {
            _currentState = _disStateDict[AppearanceType.None];
            _currentState.OnEnter();
        }
        else
        {
            _currentState.OnExit();
            _currentState = DState;
            _currentState.OnEnter();
        }
    }

    private abstract class MenuState
    {
        protected readonly RadialMenu menu;

        public MenuState(RadialMenu menu)
            => this.menu = menu;

        public abstract void OnEnter();
        public abstract void Update();
        public abstract void OnExit();

        protected float GetMainStatePieceAlpha()
        {
            switch (menu._mainType)
            {
                case MainType.AlphaChange:
                case MainType.AlphaAndScaleChange:
                    return NotSelectedPieceAlpha;

                default:
                    return 1f;
            }
        }
    }

    private sealed class NullState : MenuState
    {
        public static NullState Instance => new NullState();

        public NullState() : base(null) { }

        public override void OnEnter() { }

        public override void OnExit() { }

        public override void Update() { }
    }

    // 1. 등장
    private abstract class AppearanceState : MenuState
    {
        public AppearanceState(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            menu._selectedIndex = -1;
            menu.ShowGameObject();
            menu.SetArrow(false);

            if (menu._stateProgress == 0f)
                OnEnterAtZeroProgress();
        }
        
        protected virtual void OnEnterAtZeroProgress() { }

        public override void Update()
        {
            Execute();
            menu._stateProgress += Time.deltaTime / menu._appearanceDuration;

            if (menu._stateProgress >= 1f)
            {
                menu._stateProgress = 1f;
                menu.ChangeToNextState();
            }
        }

        protected abstract void Execute();

        public override void OnExit() { }

        protected float GetEasedProgress(float progress)
        {
            return Easing.Get(progress, menu._appearanceEasing);
        }
    }

    // 2. 유지
    private abstract class MainState : MenuState
    {
        protected int prevSelectedIndex = -1;

        public MainState(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            prevSelectedIndex = -1;

            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(1f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(true);
        }

        public override void Update()
        {
            bool showArrow = false;

            var mViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            var mPos = new Vector2(mViewportPos.x - 0.5f, mViewportPos.y - 0.5f);

            var mDist = new Vector2(mPos.x * Screen.width / Screen.height, mPos.y).magnitude;

            if (mDist < menu._centerRange)
            {
                menu._selectedIndex = -1;
            }
            else
            {
                ClockwisePolarCoord mousePC = ClockwisePolarCoord.FromVector2(mPos);

                menu._arrowRotationZ = -mousePC.Angle;
                showArrow = true;

                if (menu._pieceCount <= 2)
                {
                    if (mousePC.Angle >= 270 || mousePC.Angle < 0)
                    {
                        menu._selectedIndex = 1;
                    }
                    else if (mousePC.Angle >= 0 && mousePC.Angle < 90)
                    {
                        menu._selectedIndex = 0;
                    }
                    else
                    {
                        menu._selectedIndex = -1;
                    }
                }
                else
                {
                    float fIndex = (mousePC.Angle / 360f) * menu._pieceCount;
                    menu._selectedIndex = Mathf.RoundToInt(fIndex) % menu._pieceCount;
                }
            }

            menu.SetArrow(showArrow);

            if (prevSelectedIndex != menu._selectedIndex)
                OnSelectedIndexChanged(prevSelectedIndex, menu._selectedIndex);

            prevSelectedIndex = menu._selectedIndex;
        }

        public abstract void OnSelectedIndexChanged(int prevIndex, int currentIndex);

        public override void OnExit() { }
    }

    // 3. 소멸
    private abstract class DisappearanceState : MenuState
    {
        public DisappearanceState(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
        }

        public override void Update()
        {
            Execute();
            menu._stateProgress -= Time.deltaTime / menu._disppearanceDuration;

            if (menu._stateProgress <= 0f)
            {
                menu._stateProgress = 0f;
                menu.ChangeToNextState();
            }
        }

        protected abstract void Execute();

        public override void OnExit()
        {
            menu.HideGameObject();
        }

        protected float GetEasedProgress(float progress)
        {
            return Easing.Get(progress, menu._disappearanceEasing);
        }
    }

    private sealed class DefaultAppearance : AppearanceState
    {
        public DefaultAppearance(RadialMenu menu) : base(menu) { }

        protected override void Execute()
        {
            menu.ChangeToNextState();
        }
    }

    private sealed class FadeIn : AppearanceState
    {
        private float alphaGoal;

        public FadeIn(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();

            alphaGoal = GetMainStatePieceAlpha();
        }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(1f);
            menu.SetAllPieceAlpha(0f);
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            menu.SetAllPieceAlpha(alphaGoal * GetEasedProgress(menu._stateProgress));
        }
    }

    private sealed class ScaleUp : AppearanceState
    {
        public ScaleUp(RadialMenu menu) : base(menu) { }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(0.001f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            menu.SetAllPieceScale(GetEasedProgress(menu._stateProgress));
        }
    }

    private sealed class FadeInAndScaleUp : AppearanceState
    {
        private float alphaGoal;
        public FadeInAndScaleUp(RadialMenu menu) : base(menu) { }

        protected override void OnEnterAtZeroProgress()
        {
            alphaGoal = GetMainStatePieceAlpha();

            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(0.001f);
            menu.SetAllPieceAlpha(alphaGoal);
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            float t = GetEasedProgress(menu._stateProgress);

            menu.SetAllPieceScale(t);
            menu.SetAllPieceAlpha(t * alphaGoal);
        }
    }

    private sealed class Spread : AppearanceState
    {
        public Spread(RadialMenu menu) : base(menu) { }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(0f);
            menu.SetAllPieceScale(0.001f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            float t = GetEasedProgress(menu._stateProgress);

            float dist = t * menu._pieceDist;

            menu.SetAllPieceDistance(dist);

            menu.SetAllPieceScale(t);
        }
    }

    private sealed class ProgressiveAppearance : AppearanceState
    {
        public ProgressiveAppearance(RadialMenu menu) : base(menu) { }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(1f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(false);
        }

        protected override void Execute()
        {
            int lastIndex = (int)(menu._stateProgress * menu._pieceCount);

            for (int i = 0; i < menu._pieceCount; i++)
            {
                menu._pieceImages[i].enabled = (i <= lastIndex);
            }
        }
    }

    private sealed class ProgressiveFadeIn : AppearanceState
    {
        private float alphaGoal;

        public ProgressiveFadeIn(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();
            alphaGoal = GetMainStatePieceAlpha();
        }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(1f);
            menu.SetAllPieceAlpha(0f);
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float a = i / n;
                float alpha = (t - a) / (1 - a);

                menu._pieceImages[i].color = new Color(1f, 1f, 1f, alpha * alphaGoal);
            }
        }
    }

    private sealed class ProgressiveScaleUp : AppearanceState
    {
        public ProgressiveScaleUp(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(menu._pieceDist);
            menu.SetAllPieceScale(0f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float a = i / n;
                float scale = (t - a) / (1 - a);
                if (scale < 0f) scale = 0f;

                menu._pieceRects[i].localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    private sealed class ProgressiveSpread : AppearanceState
    {
        public ProgressiveSpread(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnEnterAtZeroProgress()
        {
            menu.SetAllPieceDistance(0f);
            menu.SetAllPieceScale(0f);
            menu.SetAllPieceAlpha(GetMainStatePieceAlpha());
            menu.SetAllPieceImageEnabled(true);
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float value = n * t - i;
                value = Mathf.Clamp01(value);

                menu._pieceRects[i].localScale = new Vector3(value, value, 1f);
                menu._pieceRects[i].anchoredPosition = menu._pieceDirections[i] * value * menu._pieceDist;
            }
        }
    }

    private sealed class MainAlphaChange : MainState
    {
        public MainAlphaChange(RadialMenu menu) : base(menu) { }

        public override void OnSelectedIndexChanged(int prevIndex, int currentIndex)
        {
            if (prevIndex >= 0)
                menu.SetPieceAlpha(prevIndex, NotSelectedPieceAlpha);

            if (currentIndex >= 0)
                menu.SetPieceAlpha(currentIndex, 1f);
        }

        public override void OnExit()
        {
            if (menu._selectedIndex >= 0)
            {
                menu.SetPieceAlpha(menu._selectedIndex, NotSelectedPieceAlpha);
            }
        }
    }

    private sealed class MainScaleChange : MainState
    {
        public MainScaleChange(RadialMenu menu) : base(menu) { }

        public override void OnSelectedIndexChanged(int prevIndex, int currentIndex)
        {
            if (prevIndex >= 0)
                menu.SetPieceScale(prevIndex, 1f);

            if (currentIndex >= 0)
                menu.SetPieceScale(currentIndex, 1.2f);
        }

        public override void OnExit()
        {
            if (menu._selectedIndex >= 0)
            {
                menu.SetPieceScale(menu._selectedIndex, 1f);
            }
        }
    }

    private sealed class MainAlphaAndScaleChange : MainState
    {
        public MainAlphaAndScaleChange(RadialMenu menu) : base(menu) { }

        public override void OnSelectedIndexChanged(int prevIndex, int currentIndex)
        {
            if (prevIndex >= 0)
            {
                menu.SetPieceAlpha(prevIndex, NotSelectedPieceAlpha);
                menu.SetPieceScale(prevIndex, 1f);
            }

            if (currentIndex >= 0)
            {
                menu.SetPieceAlpha(currentIndex, 1f);
                menu.SetPieceScale(currentIndex, 1.2f);
            }
        }

        public override void OnExit()
        {
            if (menu._selectedIndex >= 0)
            {
                menu.SetPieceAlpha(menu._selectedIndex, NotSelectedPieceAlpha);
                menu.SetPieceScale(menu._selectedIndex, 1f);
            }
        }
    }

    private sealed class DefaultDisappearance : DisappearanceState
    {
        public DefaultDisappearance(RadialMenu menu) : base(menu) { }

        protected override void Execute()
        {
            menu._stateProgress = 0f;
            menu.ChangeToNextState();
        }
    }

    private sealed class FadeOut : DisappearanceState
    {
        private float alphaGoal;
        public FadeOut(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();

            alphaGoal = GetMainStatePieceAlpha();
        }

        protected override void Execute()
        {
            menu.SetAllPieceAlpha(alphaGoal * GetEasedProgress(menu._stateProgress));
        }
    }

    private sealed class ScaleDown : DisappearanceState
    {
        public ScaleDown(RadialMenu menu) : base(menu) { }

        protected override void Execute()
        {
            menu.SetAllPieceScale(GetEasedProgress(menu._stateProgress));
        }
    }
    
    private sealed class FadeOutAndScaleDown : DisappearanceState
    {
        private float alphaGoal;
        public FadeOutAndScaleDown(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();

            alphaGoal = GetMainStatePieceAlpha();
        }

        protected override void Execute()
        {
            float t = GetEasedProgress(menu._stateProgress);

            menu.SetAllPieceScale(t);
            menu.SetAllPieceAlpha(t * alphaGoal);
        }
    }

    private sealed class Gather : DisappearanceState
    {
        public Gather(RadialMenu menu) : base(menu) { }

        protected override void Execute()
        {
            float t = GetEasedProgress(menu._stateProgress);

            float dist = t * menu._pieceDist;

            menu.SetAllPieceDistance(dist);

            menu.SetAllPieceScale(t);
        }
    }

    private sealed class ProgressiveDisappearance : DisappearanceState
    {
        public ProgressiveDisappearance(RadialMenu menu) : base(menu) { }

        protected override void Execute()
        {
            int lastIndex = (int)(menu._stateProgress * menu._pieceCount);

            for (int i = 0; i < menu._pieceCount; i++)
            {
                menu._pieceImages[i].enabled = (i <= lastIndex);
            }
        }
    }

    private sealed class ProgressiveFadeOut : DisappearanceState
    {
        private float alphaGoal;

        public ProgressiveFadeOut(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();

            alphaGoal = GetMainStatePieceAlpha();
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float a = i / n;
                float alpha = (t - a) / (1 - a);

                menu._pieceImages[i].color = new Color(1f, 1f, 1f, alpha * alphaGoal);
            }
        }
    }

    private sealed class ProgressiveScaleDown : DisappearanceState
    {
        public ProgressiveScaleDown(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float a = i / n;
                float scale = (t - a) / (1 - a);
                if (scale < 0f) scale = 0f;

                //float scale = n * t - i;
                //scale = Mathf.Clamp01(scale);

                menu._pieceRects[i].localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    private sealed class ProgressiveGather : DisappearanceState
    {
        public ProgressiveGather(RadialMenu menu) : base(menu) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void Execute()
        {
            float t = menu._stateProgress;
            float n = menu._pieceCount;

            for (int i = 0; i < n; i++)
            {
                float value = n * t - i;
                value = Mathf.Clamp01(value);

                menu._pieceRects[i].localScale = new Vector3(value, value, 1f);
                menu._pieceRects[i].anchoredPosition = menu._pieceDirections[i] * value * menu._pieceDist;
            }
        }
    }

}
