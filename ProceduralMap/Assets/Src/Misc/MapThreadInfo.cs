using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapThreadInfo<T>
{
	public readonly Action<T> callBack;
	public readonly T parameter;

	public MapThreadInfo(Action<T> _callBack, T _parameter)
	{
		this.callBack = _callBack;
		this.parameter = _parameter;
	}

}
