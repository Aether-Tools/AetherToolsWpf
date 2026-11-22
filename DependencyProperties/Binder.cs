// © XIV-Tools.
// Licensed under the MIT license.

namespace XivToolsWpf.DependencyProperties;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

public class Binder
{
	public static DependencyProperty<TValue> Register<TValue, TOwner>(string propertyName, BindMode mode, bool inherits = false, TValue defaultValue = default!)
	{
		static void callback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
		return Register<TValue, TOwner>(propertyName, new PropertyChangedCallback((Action<DependencyObject, DependencyPropertyChangedEventArgs>)callback), mode, inherits, defaultValue);
	}

	public static DependencyProperty<TValue> Register<TValue, TOwner>(string propertyName, Action<TOwner, TValue>? changed = null, BindMode mode = BindMode.TwoWay, bool inherits = false, TValue defaultValue = default!)
	{
		void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TOwner owner)
			{
				changed?.Invoke(owner, (TValue)e.NewValue);
			}
		}

		return Register<TValue, TOwner>(propertyName, new PropertyChangedCallback((Action<DependencyObject, DependencyPropertyChangedEventArgs>)callback), mode, inherits, defaultValue);
	}

	public static DependencyProperty<TValue> Register<TValue, TOwner>(string propertyName, Action<TOwner, TValue, TValue> changed, BindMode mode = BindMode.TwoWay, bool inherits = false, TValue defaultValue = default!)
	{
		void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TOwner owner)
			{
				changed?.Invoke(owner, (TValue)e.OldValue, (TValue)e.NewValue);
			}
		}

		return Register<TValue, TOwner>(propertyName, new PropertyChangedCallback((Action<DependencyObject, DependencyPropertyChangedEventArgs>)callback), mode, inherits, defaultValue);
	}

	private static DependencyProperty<TValue> Register<TValue, TOwner>(string propertyName, PropertyChangedCallback callback, BindMode mode, bool inherits, TValue defaultValue)
	{
		PropertyInfo? property = typeof(TOwner).GetProperty(propertyName)
			?? throw new Exception("Failed to locate property: \"" + propertyName + "\" on type: \"" + typeof(TOwner) + "\" for binding.");

		var meta = new FrameworkPropertyMetadata(defaultValue, new PropertyChangedCallback(callback))
		{
			BindsTwoWayByDefault = mode == BindMode.TwoWay,
			DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
			Inherits = inherits,
		};

		DependencyProperty dp = DependencyProperty.Register(propertyName, typeof(TValue), typeof(TOwner), meta);
		var dpv = new DependencyProperty<TValue>(dp);
		return dpv;
	}
}

public enum BindMode
{
	OneWay,
	TwoWay,
}
