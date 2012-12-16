using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

/* This file is substantially copied from http://rx.codeplex.com/SourceControl/changeset/view/ef6a42709f49#Rx.NET/System.Reactive.Core/Reactive/Internal/ImmutableList.cs
 * Check LICENSE in this folder for licensing information */

namespace ReactiveUI
{
    internal class ImmutableList<T>
    {
        T[] data;

        public ImmutableList()
        {
            data = new T[0];
        }

        public ImmutableList(T[] data)
        {
            this.data = data;
        }

        public ImmutableList<T> Add(T value)
        {
            var newData = new T[data.Length + 1];
            Array.Copy(data, newData, data.Length);
            newData[data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        public ImmutableList<T> Remove(T value)
        {
            var i = IndexOf(value);
            if (i < 0)
                return this;
            var newData = new T[data.Length - 1];
            Array.Copy(data, 0, newData, 0, i);
            Array.Copy(data, i + 1, newData, i, data.Length - i - 1);
            return new ImmutableList<T>(newData);
        }

        public int IndexOf(T value)
        {
            for (var i = 0; i < data.Length; ++i)
                if (data[i].Equals(value))
                    return i;
            return -1;
        }

        public T[] Data
        {
            get { return data; }
        }
    }
}

