// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>Shows <c>RegisterMessageSource</c> feeding the bus from an existing stream, such as a weather station.</summary>
public static class RegisterMessageSourceExamples
{
    /// <summary>A weather station's own stream becomes bus messages, so every screen listening for a reading gets one without knowing where it came from.</summary>
    public static void RegisterTheWeatherStationAsASource()
    {
        MessageBus office = new MessageBus();
        List<double> readings = [];
        using IDisposable subscription = office.Listen<WeatherReading>().Subscribe(reading => readings.Add(reading.TemperatureCelsius));

        IObservable<WeatherReading> weatherStation = Signal.Emit(new WeatherReading(18.5));
        using IDisposable sourceRegistration = office.RegisterMessageSource(weatherStation);

        Console.WriteLine(readings.Count);
        Console.WriteLine(readings[0]);

        // Output:
        // 1
        // 18.5
    }

    /// <summary>The contract overload scopes a source to one channel, such as the weather station for a single building.</summary>
    public static void RegisterTheWeatherStationPerBuilding()
    {
        MessageBus office = new MessageBus();
        List<double> mainBuildingReadings = [];
        using IDisposable subscription = office.Listen<WeatherReading>("MainBuilding").Subscribe(reading => mainBuildingReadings.Add(reading.TemperatureCelsius));

        IObservable<WeatherReading> mainBuildingStation = Signal.Emit(new WeatherReading(19.2));
        using IDisposable sourceRegistration = office.RegisterMessageSource(mainBuildingStation, "MainBuilding");

        Console.WriteLine(mainBuildingReadings[0]);

        // Output:
        // 19.2
    }
}
