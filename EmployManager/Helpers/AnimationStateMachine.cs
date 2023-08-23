﻿using System;
namespace EmployManager.Helpers;

public enum AnimationType
{
    Scale,
    Opacity,
    TranslationX,
    TranslationY,
    Rotation,
    Layout,
    FillColor,
    IsEnable,
    IsVisible,
    HeightRequest
}

public class AnimationStateMachine
{
    readonly Dictionary<string, ViewTransition[]> _stateTransitions = new Dictionary<string, ViewTransition[]>();

    public void Add(object state, ViewTransition[] viewTransitions)
    {
        var stateStr = state?.ToString().ToUpperInvariant();

        if (string.IsNullOrEmpty(stateStr) || viewTransitions == null)
            throw new NullReferenceException("Value of 'state', 'viewTransitions' cannot be null");

        if (_stateTransitions.ContainsKey(stateStr))
            throw new ArgumentException($"State {state} already added");

        _stateTransitions.Add(stateStr, viewTransitions);
    }

    public object CurrentState { get; set; }

    public void Go(object newState, bool withAnimation = true)
    {
        var newStateStr = newState?.ToString().ToUpperInvariant();

        if (string.IsNullOrEmpty(newStateStr))
            throw new NullReferenceException("Value of newState cannot be null");

        if (!_stateTransitions.ContainsKey(newStateStr))
            throw new KeyNotFoundException($"There is no state {newState}");

        // Get all ViewTransitions 
        var viewTransitions = _stateTransitions[newStateStr];

        // Get transition tasks
        var tasks = viewTransitions.Select(viewTransition => viewTransition.GetTransition(withAnimation));

        // Run all transition tasks
        Task.WhenAll(tasks);

        // update the current state we are in
        CurrentState = newState;
    }
}

public class ViewTransition
{
    readonly AnimationType _animationType;
    readonly int _delay;
    readonly uint _length;
    readonly Easing _easing;
    readonly double _endValue;
    readonly Rect _rectangle;
    readonly bool _isEnable;
    readonly WeakReference<VisualElement> _targetElementReference;

    public ViewTransition(VisualElement targetElement, AnimationType animationType, double endValue, uint length = 250, Easing easing = null, int delay = 0)
    {
        _targetElementReference = new WeakReference<VisualElement>(targetElement);
        _animationType = animationType;
        _length = length;
        _endValue = endValue;
        _delay = delay;
        _easing = easing;
    }

    public ViewTransition(VisualElement targetElement, AnimationType animationType, Rect endLayout, uint length = 250, Easing easing = null, int delay = 0)
    {
        _targetElementReference = new WeakReference<VisualElement>(targetElement);
        _animationType = animationType;
        _length = length;
        _rectangle = endLayout;
        _delay = delay;
        _easing = easing;
    }

    public ViewTransition(VisualElement targetElement, AnimationType animationType, bool endState, uint length = 250, Easing easing = null, int delay = 0)
    {
        _targetElementReference = new WeakReference<VisualElement>(targetElement);
        _animationType = animationType;
        _length = length;
        _isEnable = endState;
        _delay = delay;
        _easing = easing;
    }


    public async Task GetTransition(bool withAnimation)
    {
        VisualElement targetElement;
        if (!_targetElementReference.TryGetTarget(out targetElement))
            throw new ObjectDisposedException("Target VisualElement was disposed");

        if (_delay > 0)
            await Task.Delay(_delay);

        withAnimation &= _length > 0;

        switch (_animationType)
        {
            case AnimationType.Layout:
                if (withAnimation)
                    await targetElement.LayoutTo(_rectangle, _length, _easing);
                else
                    await targetElement.LayoutTo(_rectangle, 0, null);

                AbsoluteLayout.SetLayoutBounds(targetElement, _rectangle);
                break;

            case AnimationType.Scale:
                if (withAnimation)
                    await targetElement.ScaleTo(_endValue, _length, _easing);
                else
                    targetElement.Scale = _endValue;
                break;

            case AnimationType.Opacity:
                if (withAnimation)
                {
                    if (!targetElement.IsVisible && _endValue <= 0)
                        break;

                    if (targetElement.IsVisible && _endValue < targetElement.Opacity)
                    {
                        await targetElement.FadeTo(_endValue, _length, _easing);
                        targetElement.IsVisible = _endValue > 0;
                    }
                    else if (targetElement.IsVisible && _endValue > targetElement.Opacity)
                    {
                        await targetElement.FadeTo(_endValue, _length, _easing);
                    }
                    else if (!targetElement.IsVisible && _endValue > targetElement.Opacity)
                    {
                        targetElement.Opacity = 0;
                        targetElement.IsVisible = true;
                        await targetElement.FadeTo(_endValue, _length, _easing);
                    }
                }
                else
                {
                    targetElement.Opacity = _endValue;
                    targetElement.IsVisible = _endValue > 0;
                }
                break;

            case AnimationType.TranslationX:
                if (withAnimation)
                    await targetElement.TranslateTo(_endValue, targetElement.TranslationY, _length, _easing);
                else
                    targetElement.TranslationX = _endValue;
                break;

            case AnimationType.TranslationY:
                if (withAnimation)
                    await targetElement.TranslateTo(targetElement.TranslationX, _endValue, _length, _easing);
                else
                    targetElement.TranslationY = _endValue;
                break;

            case AnimationType.Rotation:
                if (withAnimation)
                    await targetElement.RotateTo(_endValue, _length, _easing);
                else
                    targetElement.Rotation = _endValue;
                break;

            case AnimationType.IsEnable:
                if (withAnimation)
                    targetElement.IsEnabled = _isEnable;
                else
                    targetElement.IsEnabled = _isEnable;
                break;

            case AnimationType.IsVisible:
                if (withAnimation)
                    targetElement.IsVisible = _isEnable;
                else
                    targetElement.IsVisible = _isEnable;
                break;

            case AnimationType.HeightRequest:
                if (withAnimation)
                {
                    var anim = new Animation(v => targetElement.HeightRequest = v, targetElement.HeightRequest, _endValue, _easing);
                    anim.Commit(targetElement, nameof(targetElement), 16, _length, _easing);
                }

                else
                    targetElement.HeightRequest = _endValue;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public static class ViewExtensions
{
    public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing easing = null)
    {
        Func<double, Color> transform = (t) =>
          Color.FromRgba(fromColor.Red + t * (toColor.Red - fromColor.Red),
                         fromColor.Green + t * (toColor.Green - fromColor.Green),
                         fromColor.Blue + t * (toColor.Blue - fromColor.Blue),
                         fromColor.Alpha + t * (toColor.Alpha - fromColor.Alpha));
        return ColorAnimation(self, "ColorTo", transform, callback, length, easing);
    }

    public static void CancelAnimation(this VisualElement self)
    {
        self.AbortAnimation("ColorTo");
    }

    static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
    {
        easing = easing ?? Easing.Linear;
        var taskCompletionSource = new TaskCompletionSource<bool>();

        element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
        return taskCompletionSource.Task;
    }
}

